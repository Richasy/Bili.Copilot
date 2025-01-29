// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Core;
using BiliAgent.Interfaces;
using BiliAgent.Models;
using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.View;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
using Richasy.AgentKernel;
using Richasy.BiliKernel;
using Richasy.WinUI.Share.ViewModels;
using RichasyKernel;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using Windows.Storage;

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
            .AddSerilog()
            .AddBiliClient()
            .AddBiliAuthenticator()
            .AddWinUICookiesResolver()
            .AddWinUITokenResolver()
            .AddWinUIQRCodeResolver(RenderQRCodeAsync)
            .AddTVAuthenticationService()
            .AddMyProfileService()
            .AddUserService()
            .AddRelationshipService()
            .AddViewLaterService()
            .AddViewHistoryService()
            .AddCommentService()
            .AddVideoDiscoveryService()
            .AddLiveDiscoveryService()
            .AddEntertainmentDiscoveryService()
            .AddArticleDiscoveryService()
            .AddArticleOperationService()
            .AddMomentDiscoveryService()
            .AddMomentOperationService()
            .AddMessageService()
            .AddFavoriteService()
            .AddSearchService()
            .AddDanmakuService()
            .AddSubtitleService()
            .AddPlayerService()

            .AddDispatcherQueue()
            .AddAgentProvider()

            .AddOpenAIChatService()
            .AddAzureOpenAIChatService()
            .AddXAIChatService()
            .AddZhiPuChatService()
            .AddLingYiChatService()
            .AddAnthropicChatService()
            .AddMoonshotChatService()
            .AddGeminiChatService()
            .AddDeepSeekChatService()
            .AddQwenChatService()
            .AddErnieChatService()
            .AddHunyuanChatService()
            .AddSparkChatService()
            .AddDoubaoChatService()
            .AddSiliconFlowChatService()
            .AddOpenRouterChatService()
            .AddTogetherAIChatService()
            .AddGroqChatService()
            .AddOllamaChatService()
            .AddMistralChatService()
            .AddPerplexityChatService()

            .AddQwenChatModelProvider()
            .AddAzureOpenAIChatModelProvider()
            .AddAnthropicChatModelProvider()
            .AddErnieChatModelProvider()
            .AddDeepSeekChatModelProvider()
            .AddGeminiChatModelProvider()
            .AddGroqChatModelProvider()
            .AddSparkChatModelProvider()
            .AddLingYiChatModelProvider()
            .AddMoonshotChatModelProvider()
            .AddOllamaChatModelProvider()
            .AddOpenAIChatModelProvider()
            .AddOpenRouterChatModelProvider()
            .AddSiliconFlowChatModelProvider()
            .AddHunyuanChatModelProvider()
            .AddTogetherAIChatModelProvider()
            .AddDoubaoChatModelProvider()
            .AddXAIChatModelProvider()
            .AddDoubaoChatModelProvider()
            .AddMistralChatModelProvider()
            .AddZhiPuChatModelProvider()
            .AddPerplexityChatModelProvider()

            .AddTransient<CommentMainViewModel>()
            .AddSingleton<PinnerViewModel>()
            .AddSingleton<AppViewModel>()
            .AddSingleton<EmoteModuleViewModel>()
            .AddSingleton<NavigationViewModel>()
            .AddSingleton<StartupPageViewModel>()
            .AddSingleton<AccountViewModel>()
            .AddSingleton<SearchBoxViewModel>()
            .AddSingleton<PopularPageViewModel>()
            .AddSingleton<MomentPageViewModel>()
            .AddSingleton<VideoPartitionPageViewModel>()
            .AddSingleton<LivePartitionPageViewModel>()
            .AddSingleton<ArticlePartitionPageViewModel>()
            .AddSingleton<AnimePageViewModel>()
            .AddSingleton<CinemaPageViewModel>()
            .AddSingleton<FansPageViewModel>()
            .AddSingleton<FollowsPageViewModel>()
            .AddSingleton<SearchPageViewModel>()
            .AddSingleton<ArticleReaderPageViewModel>()
            .AddSingleton<ViewLaterPageViewModel>()
            .AddSingleton<HistoryPageViewModel>()
            .AddSingleton<FavoritesPageViewModel>()
            .AddSingleton<MessagePageViewModel>()
            .AddSingleton<SettingsPageViewModel>()
            .AddSingleton<UserSpacePageViewModel>()
            .AddSingleton<WebDavPageViewModel>()
            .AddSingleton<NotificationViewModel>()
            .AddTransient<DanmakuViewModel>()
            .AddTransient<SubtitleViewModel>()
            .AddTransient<AIViewModel>()
            .AddTransient<VideoPlayerPageViewModel>()
            .AddTransient<LivePlayerPageViewModel>()
            .AddTransient<PgcPlayerPageViewModel>()
            .AddTransient<WebDavPlayerPageViewModel>()
            .AddTransient<LiveChatSectionDetailViewModel>()
            .AddTransient<UserMomentDetailViewModel>()
            .AddTransient<DownloadViewModel>()
            .Build();

        AgentStatics.GlobalKernel = Kernel;
    }

    public static IKernelBuilder AddDispatcherQueue(this IKernelBuilder kernelBuilder)
    {
        kernelBuilder.Services.AddSingleton<DispatcherQueue>(_ => DispatcherQueue.GetForCurrentThread());
        return kernelBuilder;
    }

    public static IKernelBuilder AddAgentProvider(this IKernelBuilder kernelBuilder)
    {
        var chatProviderFactory = new AgentProviderFactory(new ChatClientConfiguration());
        kernelBuilder.Services.AddSingleton<IAgentProviderFactory>(chatProviderFactory)
            .AddSingleton<IAgentClient, AgentClient>();
        return kernelBuilder;
    }

    public static IKernelBuilder AddSingleton<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(this IKernelBuilder kernelBuilder)
        where T : class
    {
        kernelBuilder.Services.AddSingleton<T>();
        return kernelBuilder;
    }

    public static IKernelBuilder AddTransient<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(this IKernelBuilder kernelBuilder)
        where T : class
    {
        kernelBuilder.Services.AddTransient<T>();
        return kernelBuilder;
    }

    public static RichasyKernel.IKernelBuilder AddSerilog(this RichasyKernel.IKernelBuilder builder)
    {
        var loggerPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Logger");
        if (!Directory.Exists(loggerPath))
        {
            Directory.CreateDirectory(loggerPath);
        }

        // Create a logger with current date.
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(Path.Combine(loggerPath, $"log-{DateTimeOffset.Now:yyyy-MM-dd}.txt"))
            .CreateLogger();

        builder.Services.AddLogging(b => b.AddSerilog(dispose: true));
        return builder;
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
