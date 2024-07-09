// Copyright (c) Richasy. All rights reserved.

namespace Richasy.BiliKernel.Models;

/// <summary>
/// 文章排序.
/// </summary>
public enum ArticleSortType
{
    /// <summary>
    /// 默认排序.
    /// </summary>
    Default = 0,

    /// <summary>
    /// 最新发布优先.
    /// </summary>
    Newest = 1,

    /// <summary>
    /// 最多阅读优先.
    /// </summary>
    Read = 5,

    /// <summary>
    /// 最多回复优先.
    /// </summary>
    Reply = 3,

    /// <summary>
    /// 最多点赞优先.
    /// </summary>
    Like = 2,

    /// <summary>
    /// 最多收藏优先.
    /// </summary>
    Favorite = 4,
}
