// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 直播条目视图模型.
/// </summary>
public sealed partial class LiveItemViewModel
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
    /// 观看者人数.
    /// </summary>
    public double? ViewerCount { get; init; }

    /// <summary>
    /// 作者.
    /// </summary>
    public string? Author { get; init; }

    /// <summary>
    /// 作者头像.
    /// </summary>
    public Uri? Avatar { get; init; }
}
