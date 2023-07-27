// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.App.Other;

/// <summary>
/// 媒体信息.
/// </summary>
public class MediaStats
{
    /// <summary>
    /// 视频帧率.
    /// </summary>
    public double Fps { get; set; }

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

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is MediaStats stats && Fps == stats.Fps && Width == stats.Width && Height == stats.Height && VideoCodec == stats.VideoCodec && AudioCodec == stats.AudioCodec;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Fps, Width, Height, VideoCodec, AudioCodec);
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
        Fps = info.Fps;
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

/// <summary>
/// 视频播放信息.
/// </summary>
public sealed class VideoMediaStats : MediaStats
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LiveMediaStats"/> class.
    /// </summary>
    /// <param name="info">播放信息.</param>
    public VideoMediaStats(MediaStats info)
    {
        Fps = info.Fps;
        Width = info.Width;
        Height = info.Height;
        VideoCodec = info.VideoCodec;
        AudioCodec = info.AudioCodec;
    }

    /// <summary>
    /// 视频地址.
    /// </summary>
    public string VideoUrl { get; set; }

    /// <summary>
    /// 音频地址.
    /// </summary>
    public string AudioUrl { get; set; }
}
