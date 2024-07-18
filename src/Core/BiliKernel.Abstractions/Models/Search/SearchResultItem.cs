// Copyright (c) Richasy. All rights reserved.

using Richasy.BiliKernel.Models.Article;
using Richasy.BiliKernel.Models.Media;
using Richasy.BiliKernel.Models.User;

namespace Richasy.BiliKernel.Models.Search;

/// <summary>
/// 搜索结果条目.
/// </summary>
public sealed class SearchResultItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchResultItem"/> class.
    /// </summary>
    public SearchResultItem(
        VideoInformation? video = default,
        SeasonInformation? season = default,
        LiveInformation? live = default,
        ArticleInformation? article = default,
        UserCard? user = default)
    {
        Video = video;
        Season = season;
        Live = live;
        Article = article;
        User = user;
    }

    /// <summary>
    /// 视频信息.
    /// </summary>
    public VideoInformation? Video { get; }

    /// <summary>
    /// 影视信息.
    /// </summary>
    public SeasonInformation? Season { get; }

    /// <summary>
    /// 直播信息.
    /// </summary>
    public LiveInformation? Live { get; }

    /// <summary>
    /// 专栏信息.
    /// </summary>
    public ArticleInformation? Article { get; }

    /// <summary>
    /// 用户信息.
    /// </summary>
    public UserCard? User { get; }

    /// <summary>
    /// 是否无效.
    /// </summary>
    public bool IsInvalid()
        => Video is null && Season is null && Live is null && Article is null && User is null;
}
