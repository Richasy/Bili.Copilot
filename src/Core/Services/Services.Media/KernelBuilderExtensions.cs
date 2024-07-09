// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.DependencyInjection;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Services.Media;

namespace Richasy.BiliKernel;

/// <summary>
/// 内核构建器扩展.
/// </summary>
public static class KernelBuilderExtensions
{
    /// <summary>
    /// 添加稍后再看服务.
    /// </summary>
    public static IKernelBuilder AddViewLaterService(this IKernelBuilder builder)
    {
        builder.Services.AddSingleton<IViewLaterService, ViewLaterService>();
        return builder;
    }

    /// <summary>
    /// 添加观看历史服务.
    /// </summary>
    public static IKernelBuilder AddViewHistoryService(this IKernelBuilder builder)
    {
        builder.Services.AddSingleton<IViewHistoryService, ViewHistoryService>();
        return builder;
    }

    /// <summary>
    /// 添加流行视频服务.
    /// </summary>
    public static IKernelBuilder AddPopularVideoService(this IKernelBuilder builder)
    {
        builder.Services.AddSingleton<IPopularVideoService, PopularVideoService>();
        return builder;
    }

    /// <summary>
    /// 添加视频分区服务.
    /// </summary>
    public static IKernelBuilder AddVideoPartitionService(this IKernelBuilder builder)
    {
        builder.Services.AddSingleton<IVideoPartitionService, VideoPartitionService>();
        return builder;
    }

    /// <summary>
    /// 添加热门直播服务.
    /// </summary>
    public static IKernelBuilder AddPopularLiveService(this IKernelBuilder builder)
    {
        builder.Services.AddSingleton<IPopularLiveService, PopularLiveService>();
        return builder;
    }

    /// <summary>
    /// 添加直播分区服务.
    /// </summary>
    public static IKernelBuilder AddLivePartitionService(this IKernelBuilder builder)
    {
        builder.Services.AddSingleton<ILivePartitionService, LivePartitionService>();
        return builder;
    }

    /// <summary>
    /// 添加动漫服务.
    /// </summary>
    public static IKernelBuilder AddAnimeService(this IKernelBuilder builder)
    {
        builder.Services.AddSingleton<IAnimeService, AnimeService>();
        return builder;
    }

    /// <summary>
    /// 添加电影/电视剧/纪录片服务.
    /// </summary>
    public static IKernelBuilder AddEntertainmentService(this IKernelBuilder builder)
    {
        builder.Services.AddSingleton<IEntertainmentService, EntertainmentService>();
        return builder;
    }
}
