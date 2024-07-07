// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;

namespace Richasy.BiliKernel.Models.Media;

/// <summary>
/// 视频溢出菜单.
/// </summary>
public sealed class VideoOverflowFlyout
{
    /// <summary>
    /// 菜单项.
    /// </summary>
    public IList<VideoOverflowFlyoutItem>? Items { get; set; }
}

/// <summary>
/// 视频溢出菜单项.
/// </summary>
public sealed class VideoOverflowFlyoutItem
{
    /// <summary>
    /// 标题.
    /// </summary>
    public string? Title { get; set; }
    
    /// <summary>
    /// 标识.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// 选项.
    /// </summary>
    public Dictionary<string, string>? Options { get; set; }
}
