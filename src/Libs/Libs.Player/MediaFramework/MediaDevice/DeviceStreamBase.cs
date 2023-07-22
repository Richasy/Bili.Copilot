// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaDevice;

/// <summary>
/// 设备流基类.
/// </summary>
public class DeviceStreamBase
{
    /// <summary>
    /// 构造函数，用于初始化设备友好名称.
    /// </summary>
    /// <param name="deviceFriendlyName">设备友好名称.</param>
    public DeviceStreamBase(string deviceFriendlyName)
        => DeviceFriendlyName = deviceFriendlyName;

    /// <summary>
    /// 获取设备友好名称.
    /// </summary>
    public string DeviceFriendlyName { get; }

    /// <summary>
    /// 获取或设置设备流的 URL.
    /// </summary>
    public string Url { get; protected set; }
}
