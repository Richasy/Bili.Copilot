// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Bili.Copilot.Libs.Player.Models;
using Vortice.DXGI;

namespace Bili.Copilot.Libs.Player.Core.Engines;

/// <summary>
/// 视频引擎类，用于处理视频相关操作.
/// </summary>
public sealed class VideoEngine
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoEngine"/> class.
    /// </summary>
    internal VideoEngine()
    {
        if (DXGI.CreateDXGIFactory1(out Factory).Failure)
        {
            throw new InvalidOperationException("无法创建 IDXGIFactory1");
        }

        GpuAdapters = GetAdapters();
    }

    /// <summary>
    /// 视频捕获设备列表.
    /// </summary>
    public ObservableCollection<VideoDevice> CapDevices { get; set; } = new();

    /// <summary>
    /// GPU适配器列表 <see cref="Config.VideoConfig.GpuAdapter"/>.
    /// </summary>
    public Dictionary<long, GpuAdapter> GpuAdapters { get; private set; }

    /// <summary>
    /// 默认GPU适配器的屏幕列表（注意：在屏幕连接/断开时不会更新）.
    /// </summary>
    public List<GpuOutput> Screens { get; private set; } = new();

    internal IDXGIFactory2 Factory { get; }

    /// <summary>
    /// 刷新视频捕获设备列表.
    /// </summary>
    public void RefreshCapDevices() => VideoDevice.RefreshDevices();

    /// <summary>
    /// 根据位置获取屏幕.
    /// </summary>
    /// <param name="top">位置的纵坐标.</param>
    /// <param name="left">位置的横坐标.</param>
    /// <returns>屏幕对象.</returns>
    public GpuOutput GetScreenFromPosition(int top, int left)
    {
        foreach (var screen in Screens)
        {
            if (top >= screen.Top && top <= screen.Bottom && left >= screen.Left && left <= screen.Right)
            {
                return screen;
            }
        }

        return null;
    }

    /// <summary>
    /// 根据厂商ID获取厂商名称.
    /// </summary>
    /// <param name="vendorId">厂商ID.</param>
    /// <returns>厂商名称.</returns>
    private static string VendorIdStr(int vendorId)
    {
        return vendorId switch
        {
            0x1002 => "ATI",
            0x10DE => "NVIDIA",
            0x1106 => "VIA",
            0x8086 => "Intel",
            0x5333 => "S3 Graphics",
            0x4D4F4351 => "Qualcomm",
            _ => "Unknown",
        };
    }

    /// <summary>
    /// 获取GPU适配器列表.
    /// </summary>
    /// <returns>GPU适配器字典.</returns>
    private Dictionary<long, GpuAdapter> GetAdapters()
    {
        Dictionary<long, GpuAdapter> adapters = new();

        var dump = string.Empty;

        for (var i = 0; Factory.EnumAdapters1(i, out var adapter).Success; i++)
        {
            var hasOutput = false;

            List<GpuOutput> outputs = new();

            var maxHeight = 0;
            for (var o = 0; adapter.EnumOutputs(o, out var output).Success; o++)
            {
                GpuOutput gpuOut = new()
                {
                    Id = GpuOutput.GpuOutputIdGenerator++,
                    DeviceName = output.Description.DeviceName,
                    Left = output.Description.DesktopCoordinates.Left,
                    Top = output.Description.DesktopCoordinates.Top,
                    Right = output.Description.DesktopCoordinates.Right,
                    Bottom = output.Description.DesktopCoordinates.Bottom,
                    IsAttached = output.Description.AttachedToDesktop,
                    Rotation = (int)output.Description.Rotation,
                };

                if (maxHeight < gpuOut.Height)
                {
                    maxHeight = gpuOut.Height;
                }

                outputs.Add(gpuOut);

                if (gpuOut.IsAttached)
                {
                    hasOutput = true;
                }

                output.Dispose();
            }

            if (Screens.Count == 0 && outputs.Count > 0)
            {
                Screens = outputs;
            }

            adapters[adapter.Description1.Luid] = new GpuAdapter()
            {
                SystemMemory = adapter.Description1.DedicatedSystemMemory,
                VideoMemory = adapter.Description1.DedicatedVideoMemory,
                SharedMemory = adapter.Description1.SharedSystemMemory,
                Vendor = VendorIdStr(adapter.Description1.VendorId),
                Description = adapter.Description1.Description,
                Id = adapter.Description1.DeviceId,
                Luid = adapter.Description1.Luid,
                MaxHeight = maxHeight,
                HasOutput = hasOutput,
                Outputs = outputs,
            };

            dump += $"[#{i + 1}] {adapters[adapter.Description1.Luid]}\r\n";

            adapter.Dispose();
        }

        Engine.Log.Info($"GPU适配器\r\n{dump}");

        return adapters;
    }
}
