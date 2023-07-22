// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Linq;
using Bili.Copilot.Libs.Player.Core;
using Vortice.MediaFoundation;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaDevice;

/// <summary>
/// 视频设备类.
/// </summary>
public class VideoDevice : DeviceBase<VideoDeviceStream>
{
    /// <summary>
    /// 构造函数，用于初始化 VideoDevice 类的新实例.
    /// </summary>
    /// <param name="friendlyName">设备的友好名称.</param>
    /// <param name="symbolicLink">设备的符号链接.</param>
    public VideoDevice(string friendlyName, string symbolicLink)
        : base(friendlyName, symbolicLink)
    {
        Streams = VideoDeviceStream.GetVideoFormatsForVideoDevice(friendlyName, symbolicLink);
        Url = Streams.Where(f => f.SubType.Contains("MJPG") && f.FrameRate >= 30).OrderByDescending(f => f.FrameSizeHeight).FirstOrDefault()?.Url;
    }

    /// <summary>
    /// 刷新设备列表.
    /// </summary>
    public static void RefreshDevices()
    {
        Utils.UI(() =>
        {
            Engine.Video.CapDevices.Clear();

            var devices = MediaFactory.MFEnumVideoDeviceSources();
            foreach (var device in devices)
            {
                try
                {
                    Engine.Video.CapDevices.Add(new VideoDevice(device.FriendlyName, device.SymbolicLink));
                }
                catch (Exception)
                {
                }
            }
        });
    }
}
