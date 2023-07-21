// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Libs.Player.Models;

/// <summary>
/// 媒体部分.
/// </summary>
public class MediaPart
{
    /// <summary>
    /// 季度.
    /// </summary>
    public int Season { get; set; }

    /// <summary>
    /// 集数.
    /// </summary>
    public int Episode { get; set; }

    /// <summary>
    /// 标题.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 年份.
    /// </summary>
    public int Year { get; set; }
}
