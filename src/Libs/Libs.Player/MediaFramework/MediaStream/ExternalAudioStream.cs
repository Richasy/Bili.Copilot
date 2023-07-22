// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Libs.Player.Core;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaStream;

/// <summary>
/// 外部音频流类，继承自外部流类.
/// </summary>
public class ExternalAudioStream : ExternalStream
{
    /// <summary>
    /// 获取或设置音频采样率.
    /// </summary>
    public int SampleRate { get; set; }

    /// <summary>
    /// 获取或设置音频通道布局.
    /// </summary>
    public string ChannelLayout { get; set; }

    /// <summary>
    /// 获取或设置音频语言.
    /// </summary>
    public Language Language { get; set; }

    /// <summary>
    /// 获取或设置是否包含视频.
    /// </summary>
    public bool HasVideo { get; set; }
}
