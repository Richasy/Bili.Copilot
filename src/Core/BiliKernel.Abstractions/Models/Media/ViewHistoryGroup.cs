// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;

namespace Richasy.BiliKernel.Models.Media;

/// <summary>
/// 观看历史分组.
/// </summary>
public sealed class ViewHistoryGroup
{
    /// <summary>
    /// 初始化.
    /// </summary>
    public ViewHistoryGroup(
        ViewHistoryTabType tabType,
        IList<VideoInformation>? videos = default,
        IList<EpisodeInformation>? episodes = default,
        long? offset = default)
    {
        Tab = tabType;
        Videos = videos;
        Offset = offset;
    }

    /// <summary>
    /// 标签页.
    /// </summary>
    public ViewHistoryTabType Tab { get; }

    /// <summary>
    /// 视频列表.
    /// </summary>
    public IList<VideoInformation>? Videos { get; }

    /// <summary>
    /// 剧集列表.
    /// </summary>
    public IList<EpisodeInformation>? Episodes { get; }

    /// <summary>
    /// 偏移量.
    /// </summary>
    public long? Offset { get; }
}
