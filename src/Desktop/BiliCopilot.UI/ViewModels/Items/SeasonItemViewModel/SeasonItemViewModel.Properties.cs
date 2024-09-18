// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 剧集条目视图模型.
/// </summary>
public sealed partial class SeasonItemViewModel
{
    /// <summary>
    /// 标题.
    /// </summary>
    public string Title { get; init; }

    /// <summary>
    /// 副标题.
    /// </summary>
    public string? Subtitle { get; init; }

    /// <summary>
    /// 高亮内容.
    /// </summary>
    public string? Highlight { get; init; }

    /// <summary>
    /// 封面.
    /// </summary>
    public Uri Cover { get; init; }

    /// <summary>
    /// 评分.
    /// </summary>
    public double? Score { get; init; }

    /// <summary>
    /// 处于想看状态.
    /// </summary>
    public bool? InWantWatch { get; init; }

    /// <summary>
    /// 处于在看状态.
    /// </summary>
    public bool? InWatching { get; init; }

    /// <summary>
    /// 处于已看状态.
    /// </summary>
    public bool? InWatched { get; init; }

    /// <summary>
    /// 样式.
    /// </summary>
    public SeasonCardStyle Style { get; init; }
}
