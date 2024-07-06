﻿using Bili.Console.Models;
using Microsoft.Extensions.Hosting;
using Richasy.BiliKernel;
using Richasy.BiliKernel.Authorizers.TV;
using Spectre.Console;

namespace Bili.Console;

internal sealed class BiliService : IHostedService
{
    private readonly Kernel _kernel;
    private readonly Dictionary<FeatureType, IFeatureModule> _features = new();
    private CancellationToken _cancellationToken;

    public BiliService()
    {
        _kernel = Kernel.CreateBuilder()
            .AddNativeTokenResolver()
            .AddNativeQRCodeResolver()
            .AddNativeCookiesResolver()
            .AddHttpClient()
            .AddBasicAuthenticator()
            .AddTVAuthentication()
            .AddDefaultAuthenticationService<TVAuthenticationService>()
            .AddMyProfileService()
            .AddRelationshipService()
            .AddViewLaterService()
            .Build();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        await InitializeAsync().ConfigureAwait(false);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var item in _features)
        {
            item.Value?.Exit();
        }

        return Task.CompletedTask;
    }

    private async Task InitializeAsync()
    {
        var feature = AskFeature();
        AnsiConsole.Clear();
        await feature.RunAsync().ConfigureAwait(false);
    }

    private IFeatureModule AskFeature()
    {
        var features = Enum.GetValues<FeatureType>();
        var featureType = AnsiConsole.Prompt(
            new SelectionPrompt<FeatureType>()
                .Title("请选择功能")
                .PageSize(10)
                .MoreChoicesText("更多")
                .AddChoices(features)
                .UseConverter(FeatureToString));

        return GetOrCreateFeature(featureType);

        static string FeatureToString(FeatureType feature)
        {
            return feature switch
            {
                FeatureType.Authorize => "登录授权",
                FeatureType.My => "个人信息",
                _ => throw new NotSupportedException(),
            };
        }
    }

    private async Task BackToFeatureSelectionAsync(string? lastMessage)
    {
        AnsiConsole.Clear();
        if (!string.IsNullOrEmpty(lastMessage))
        {
            AnsiConsole.MarkupLine(lastMessage);
        }

        await InitializeAsync().ConfigureAwait(false);
    }

    private IFeatureModule GetOrCreateFeature(FeatureType type)
    {
        if (!_features.TryGetValue(type, out var feature))
        {
            feature = type switch
            {
                FeatureType.Authorize => new AuthorizeModule(_kernel, _cancellationToken, BackToFeatureSelectionAsync),
                FeatureType.My => new MyProfileModule(_kernel, _cancellationToken, BackToFeatureSelectionAsync),
                _ => throw new NotSupportedException(),
            };

            _features[type] = feature;
        }

        return feature;
    }
}
