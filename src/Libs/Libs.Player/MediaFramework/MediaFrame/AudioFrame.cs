// Copyright (c) Bili Copilot. All rights reserved.

using System;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaFrame;

/// <summary>
/// 音频帧类.
/// </summary>
public class AudioFrame : FrameBase
{
    /// <summary>
    /// 获取或设置音频数据指针.
    /// </summary>
    public IntPtr DataPtr { get; set; }

    /// <summary>
    /// 获取或设置音频数据长度.
    /// </summary>
    public int DataLen { get; set; }
}
