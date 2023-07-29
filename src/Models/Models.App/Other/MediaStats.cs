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

    /// <summary>
    /// 像素格式.
    /// </summary>
    public string PixelFormat { get; set; }

    /// <summary>
    /// 比特率.
    /// </summary>
    public double Bitrate { get; set; }

    /// <summary>
    /// 色域.
    /// </summary>
    public string ColorSpace { get; set; }

    /// <summary>
    /// 音频采样格式.
    /// </summary>
    public string AudioSampleFormat { get; set; }

    /// <summary>
    /// 音频通道数.
    /// </summary>
    public int AudioChannels { get; set; }

    /// <summary>
    /// 音频采样率.
    /// </summary>
    public int AudioSampleRate { get; set; }
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
        PixelFormat = info.PixelFormat;
        Bitrate = info.Bitrate;
        ColorSpace = info.ColorSpace;
        AudioSampleFormat = info.AudioSampleFormat;
        AudioChannels = info.AudioChannels;
        AudioSampleRate = info.AudioSampleRate;
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
        PixelFormat = info.PixelFormat;
        Bitrate = info.Bitrate;
        ColorSpace = info.ColorSpace;
        AudioSampleFormat = info.AudioSampleFormat;
        AudioChannels = info.AudioChannels;
        AudioSampleRate = info.AudioSampleRate;
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
