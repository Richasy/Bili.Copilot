// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.View;
using Microsoft.Extensions.DependencyInjection;
using NLog.Extensions.Logging;
using Richasy.BiliKernel;
using Richasy.BiliKernel.Authorizers.TV;

namespace BiliCopilot.UI;

internal static class GlobalDependencies
{
    /// <summary>
    /// 获取服务提供程序.
    /// </summary>
    public static Kernel Kernel { get; private set; }

    public static void Initialize()
    {
        if (Kernel is not null)
        {
            return;
        }

        Kernel = Kernel.CreateBuilder()
            .AddNLog()
            .AddHttpClient()
            .AddBasicAuthenticator()
            .AddWinUICookiesResolver()
            .AddWinUITokenResolver()
            .AddWinUIQRCodeResolver(RenderQRCodeAsync)
            .AddTVAuthentication()
            .UseDefaultAuthenticationService<TVAuthenticationService>()
            .AddMyProfileService()
            .AddRelationshipService()
            .AddViewLaterService()
            .AddViewHistoryService()
            .AddVideoDiscoveryService()
            .AddLiveDiscoveryService()
            .AddEntertainmentDiscoveryService()
            .AddArticleDiscoveryService()
            .AddMomentDiscoveryService()
            .AddMomentOperationService()
            .AddMessageService()
            .AddFavoriteService()
            .AddSearchService()
            .AddSingleton<AppViewModel>()
            .AddSingleton<StartupPageViewModel>()
            .Build();
    }

    public static IKernelBuilder AddSingleton<T>(this IKernelBuilder kernelBuilder)
    {
        kernelBuilder.AddSingleton<T>();
        return kernelBuilder;
    }

    public static IKernelBuilder AddNLog(this IKernelBuilder kernelBuilder)
    {
        kernelBuilder.Services.AddLogging(builder => builder.AddNLog());
        return kernelBuilder;
    }

    public static T Get<T>(this Window window)
        where T : class
        => Kernel.GetRequiredService<T>();

    public static T Get<T>(this FrameworkElement element)
        where T : class
        => Kernel.GetRequiredService<T>();

    public static T Get<T>(this Page page)
        where T : class
        => Kernel.GetRequiredService<T>();

    private static async Task RenderQRCodeAsync(byte[] imageData)
    {
        var vm = Kernel.GetRequiredService<StartupPageViewModel>();
        await vm.RenderQRCodeCommand.ExecuteAsync(imageData).ConfigureAwait(true);
    }
}
