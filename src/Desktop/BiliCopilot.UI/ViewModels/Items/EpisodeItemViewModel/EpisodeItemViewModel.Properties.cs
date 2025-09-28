// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 单集条目视图模型.
/// </summary>
public sealed partial class EpisodeItemViewModel
{
    /// <summary>
    /// 标题.
    /// </summary>
    public string Title { get; init; }

    /// <summary>
    /// 封面.
    /// </summary>
    public Uri? Cover { get; init; }

    /// <summary>
    /// 视频时长.
    /// </summary>
    public string? Duration { get; init; }

    /// <summary>
    /// 是否为预告片.
    /// </summary>
    public bool IsPreview { get; init; }

    /// <summary>
    /// 弹幕数.
    /// </summary>
    public double? DanmakuCount { get; init; }

    /// <summary>
    /// 播放数.
    /// </summary>
    public double? PlayCount { get; init; }

    /// <summary>
    /// 评论数.
    /// </summary>
    public double? CommentCount { get; init; }

    /// <summary>
    /// 硬币数.
    /// </summary>
    public double? CoinCount { get; init; }

    /// <summary>
    /// 点赞数.
    /// </summary>
    public double? LikeCount { get; init; }

    /// <summary>
    /// 索引.
    /// </summary>
    public int? Index { get; init; }

    /// <summary>
    /// 高亮文本.
    /// </summary>
    public string? HighlightText { get; init; }

    /// <summary>
    /// 卡片样式.
    /// </summary>
    public EpisodeCardStyle Style { get; init; }

    [ObservableProperty]
    public partial bool IsSelected { get; set; }
}
