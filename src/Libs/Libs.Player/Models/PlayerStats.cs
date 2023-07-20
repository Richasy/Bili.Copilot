// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Libs.Player.Models;

/// <summary>
/// 播放器统计信息.
/// </summary>
public sealed class PlayerStats
{
    /// <summary>
    /// 总字节数.
    /// </summary>
    public long TotalBytes { get; set; }

    /// <summary>
    /// 视频字节数.
    /// </summary>
    public long VideoBytes { get; set; }

    /// <summary>
    /// 音频字节数.
    /// </summary>
    public long AudioBytes { get; set; }

    /// <summary>
    /// 显示的帧数.
    /// </summary>
    public long FramesDisplayed { get; set; }
}
