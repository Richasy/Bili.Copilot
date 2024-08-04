// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.View;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
using NLog.Extensions.Logging;
using Richasy.BiliKernel;
using Richasy.BiliKernel.Authorizers.TV;
using Richasy.WinUI.Share.ViewModels;

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
            .AddDispatcherQueue()
            .AddSingleton<AppViewModel>()
            .AddSingleton<NavigationViewModel>()
            .AddSingleton<StartupPageViewModel>()
            .AddSingleton<AccountViewModel>()
            .AddSingleton<SearchViewModel>()
            .AddSingleton<PopularPageViewModel>()
            .AddSingleton<VideoPartitionPageViewModel>()
            .AddSingleton<LivePartitionPageViewModel>()
            .AddSingleton<ArticlePartitionPageViewModel>()
            .AddSingleton<AnimePageViewModel>()
            .AddSingleton<CinemaPageViewModel>()
            .AddSingleton<FansPageViewModel>()
            .Build();
    }

    public static IKernelBuilder AddDispatcherQueue(this IKernelBuilder kernelBuilder)
    {
        kernelBuilder.Services.AddSingleton<DispatcherQueue>(_ => DispatcherQueue.GetForCurrentThread());
        return kernelBuilder;
    }

    public static IKernelBuilder AddSingleton<T>(this IKernelBuilder kernelBuilder)
        where T : class
    {
        kernelBuilder.Services.AddSingleton<T>();
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

    public static T Get<T>(this ViewModelBase vm)
        where T : class
        => Kernel.GetRequiredService<T>();

    private static Task RenderQRCodeAsync(byte[] imageData)
    {
        var vm = Kernel.GetRequiredService<StartupPageViewModel>();
        vm.RenderQRCodeCommand.Execute(imageData);
        return Task.CompletedTask;
    }
}
