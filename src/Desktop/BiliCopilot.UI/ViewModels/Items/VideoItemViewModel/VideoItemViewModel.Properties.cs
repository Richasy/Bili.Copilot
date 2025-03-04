// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 视频项视图模型.
/// </summary>
public sealed partial class VideoItemViewModel
{
    /// <summary>
    /// 样式.
    /// </summary>
    public VideoCardStyle Style { get; }

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
    /// 视频时长.
    /// </summary>
    public string? Duration { get; init; }

    /// <summary>
    /// 发布时间.
    /// </summary>
    public string? PublishRelativeTime { get; init; }

    /// <summary>
    /// 作者头像.
    /// </summary>
    public Uri? Avatar { get; init; }

    /// <summary>
    /// 播放数.
    /// </summary>
    public double? PlayCount { get; init; }

    /// <summary>
    /// 弹幕数.
    /// </summary>
    public double? DanmakuCount { get; set; }

    /// <summary>
    /// 点赞数.
    /// </summary>
    public double? LikeCount { get; set; }

    /// <summary>
    /// 标签名称.
    /// </summary>
    public string? TagName { get; init; }

    /// <summary>
    /// 推荐理由.
    /// </summary>
    public string? RecommendReason { get; init; }

    /// <summary>
    /// 收集时间.
    /// </summary>
    public string? CollectTime { get; init; }

    /// <summary>
    /// 进度文本.
    /// </summary>
    public string? ProgressText { get; init; }

    /// <summary>
    /// 用户信息是否有效.
    /// </summary>
    public bool IsUserValid { get; init; }
}
