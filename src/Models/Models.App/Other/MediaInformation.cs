// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.App.Other;

/// <summary>
/// 媒体信息.
/// </summary>
public class MediaStats
{
    /// <summary>
    /// 视频比特率.
    /// </summary>
    public double VideoBitrate { get; set; }

    /// <summary>
    /// 音频比特率.
    /// </summary>
    public double AudioBitrate { get; set; }

    /// <summary>
    /// 视频宽度.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// 视频高度.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// 视频编解码器.
    /// </summary>
    public string VideoCodec { get; set; }

    /// <summary>
    /// 音频编解码器.
    /// </summary>
    public string AudioCodec { get; set; }
}

/// <summary>
/// 直播播放信息.
/// </summary>
public sealed class LiveMediaStats : MediaStats
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LiveMediaStats"/> class.
    /// </summary>
    /// <param name="info">播放信息.</param>
    public LiveMediaStats(MediaStats info)
    {
        VideoBitrate = info.VideoBitrate;
        AudioBitrate = info.AudioBitrate;
        Width = info.Width;
        Height = info.Height;
        VideoCodec = info.VideoCodec;
        AudioCodec = info.AudioCodec;
    }

    /// <summary>
    /// 播放地址.
    /// </summary>
    public string PlayUrl { get; set; }
}
