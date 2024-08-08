// Copyright (c) Bili Copilot. All rights reserved.

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
}
