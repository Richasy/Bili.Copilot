﻿// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 文章项视图模型.
/// </summary>
public sealed partial class ArticleItemViewModel
{
    /// <summary>
    /// 标题.
    /// </summary>
    public string Title { get; init; }

    /// <summary>
    /// 副标题.
    /// </summary>
    public string Subtitle { get; init; }

    /// <summary>
    /// 封面.
    /// </summary>
    public Uri Cover { get; init; }

    /// <summary>
    /// 作者.
    /// </summary>
    public string Author { get; init; }

    /// <summary>
    /// 作者头像.
    /// </summary>
    public Uri? Avatar { get; init; }

    /// <summary>
    /// 发布时间.
    /// </summary>
    public string? PublishRelativeTime { get; init; }

    /// <summary>
    /// 点赞数.
    /// </summary>
    public double? LikeCount { get; init; }

    /// <summary>
    /// 收集的相对时间.
    /// </summary>
    public string? CollectRelativeTime { get; init; }
}
