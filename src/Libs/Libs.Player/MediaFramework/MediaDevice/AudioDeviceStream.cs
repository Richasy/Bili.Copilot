// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaDevice;

/// <summary>
/// 音频设备流.
/// </summary>
public class AudioDeviceStream : DeviceStreamBase
{
    /// <summary>
    /// 构造函数.
    /// </summary>
    /// <param name="deviceName">设备名称.</param>
    public AudioDeviceStream(string deviceName)
        : base(deviceName)
    {
    }
}
