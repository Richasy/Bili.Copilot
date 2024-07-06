// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using Richasy.BiliKernel.Models.Article;

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
        IList<LiveInformation>? lives = default,
        IList<ArticleInformation>? articles = default,
        long? offset = default)
    {
        Tab = tabType;

        if (videos?.Count > 0)
        {
            Videos = videos;
        }

        if (episodes?.Count > 0)
        {
            Episodes = episodes;
        }

        if (lives?.Count > 0)
        {
            Lives = lives;
        }

        if (articles?.Count > 0)
        {
            Articles = articles;
        }

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
    /// 直播间列表.
    /// </summary>
    public IList<LiveInformation>? Lives { get; }

    /// <summary>
    /// 文章列表.
    /// </summary>
    public IList<ArticleInformation>? Articles { get; }

    /// <summary>
    /// 偏移量.
    /// </summary>
    public long? Offset { get; }
}
