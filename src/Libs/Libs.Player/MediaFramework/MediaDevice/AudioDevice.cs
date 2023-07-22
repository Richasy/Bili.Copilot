// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Libs.Player.Core;
using Vortice.MediaFoundation;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaDevice;

/// <summary>
/// 音频设备类.
/// </summary>
public class AudioDevice : DeviceBase<AudioDeviceStream>
{
    /// <summary>
    /// 构造函数，用于创建音频设备对象.
    /// </summary>
    /// <param name="friendlyName">友好名称.</param>
    /// <param name="symbolicLink">符号链接.</param>
    public AudioDevice(string friendlyName, string symbolicLink)
        : base(friendlyName, symbolicLink)
        => Url = $"fmt://dshow?audio={FriendlyName}";

    /// <summary>
    /// 刷新音频设备列表.
    /// </summary>
    public static void RefreshDevices()
    {
        Utils.UI(() =>
        {
            Engine.Audio.CapDevices.Clear();

            var devices = MediaFactory.MFEnumAudioDeviceSources();
            foreach (var device in devices)
            {
                try
                {
                    Engine.Audio.CapDevices.Add(new AudioDevice(device.FriendlyName, device.SymbolicLink));
                }
                catch (Exception)
                {
                }
            }
        });
    }
}
