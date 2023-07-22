// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaStream;

/// <summary>
/// 外部视频流类.
/// </summary>
public class ExternalVideoStream : ExternalStream
{
    /// <summary>
    /// 帧率.
    /// </summary>
    public double FPS { get; set; }

    /// <summary>
    /// 视频高度.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// 视频宽度.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// 是否包含音频.
    /// </summary>
    public bool HasAudio { get; set; }
}
