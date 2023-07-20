// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using Bili.Copilot.Libs.Player.Enums;
using Bili.Copilot.Libs.Player.Models;
using Windows.UI;

namespace Bili.Copilot.Libs.Player.Core.Configs;

/// <summary>
/// 视频配置类.
/// </summary>
public sealed class VideoConfig : ObservableObject
{
    private MediaPlayer.Player _player;
    private AspectRatio _aspectRatio = AspectRatio.Keep;
    private AspectRatio _customAspectRatio = new(16, 9);
    private bool _enabled = true;
    private float _hdrToSdrTone = 1.4f;
    private VideoProcessor _videoProcessor = VideoProcessor.Auto;
    private bool _deinterlace;
    private bool _deinterlaceBottomFirst;
    private HdrToSdrMethod _hdrToSdrMethod = HdrToSdrMethod.Hable;
    private Vortice.Mathematics.Color _backgroundColor = (Vortice.Mathematics.Color)Vortice.Mathematics.Colors.Black;

    /// <summary>
    /// 强制使用特定的 GPU 适配器来渲染器.
    /// GPUAdapter 必须与适配器的描述匹配，例如 rx 580（可在 Engine.Video.GPUAdapters 中找到可用的适配器）.
    /// </summary>
    public string GPUAdapter { get; set; }

    /// <summary>
    /// 视频的宽高比.
    /// </summary>
    public AspectRatio AspectRatio
    {
        get => _aspectRatio;
        set
        {
            if (Set(ref _aspectRatio, value) && _player != null && _player.Renderer != null && !_player.Renderer.SCDisposed)
            {
                lock (_player.Renderer.lockDevice)
                {
                    _player.Renderer.SetViewport();
                    _player.Renderer.child?.SetViewport();
                }
            }
        }
    }

    /// <summary>
    /// 自定义的宽高比（AspectRatio 必须设置为 Custom 才会生效）.
    /// </summary>
    public AspectRatio CustomAspectRatio
    {
        get => _customAspectRatio;
        set
        {
            if (Set(ref _customAspectRatio, value))
            {
                AspectRatio = AspectRatio.Custom;
            }
        }
    }

    /// <summary>
    /// 播放器控件的背景颜色.
    /// </summary>
    public Color BackgroundColor
    {
        get => Utils.VorticeToWinUIColor(_backgroundColor);
        set
        {
            Set(ref _backgroundColor, Utils.WinUIToVorticeColor(value));
            _player?.Renderer?.UpdateBackgroundColor();
        }
    }

    /// <summary>
    /// 在打开新的输入/流之前延迟清除上一帧的屏幕.
    /// </summary>
    public bool ClearScreenOnOpen { get; set; }

    /// <summary>
    /// 是否允许播放视频.
    /// </summary>
    public bool Enabled
    {
        get => _enabled;
        set
        {
            if (Set(ref _enabled, value))
            {
                if (value)
                {
                    _player?.Video.Enable();
                }
                else
                {
                    _player?.Video.Disable();
                }
            }
        }
    }

    /// <summary>
    /// 当前系统可以实现的最大分辨率，将从输入/流建议插件中使用.
    /// </summary>
    public int MaxVerticalResolutionAuto { get; internal set; }

    /// <summary>
    /// 自定义的最大垂直分辨率，将从输入/流建议插件中使用.
    /// </summary>
    public int MaxVerticalResolutionCustom { get; set; }

    /// <summary>
    /// 当前使用的最大分辨率（基于 Auto/Custom）.
    /// </summary>
    public int MaxVerticalResolution => MaxVerticalResolutionCustom == 0
        ? (MaxVerticalResolutionAuto != 0 ? MaxVerticalResolutionAuto : 1080)
        : MaxVerticalResolutionCustom;

    /// <summary>
    /// 在没有硬件加速或后处理加速的像素格式时使用 FFmpeg 的 SwsScale.
    /// </summary>
    public bool SwsHighQuality { get; set; } = false;

    /// <summary>
    /// 对于非硬件解码的帧，强制使用 SwsScale 而不是 FlyleafVP.
    /// </summary>
    public bool SwsForce { get; set; } = false;

    /// <summary>
    /// 激活 Direct3D 视频加速（解码）.
    /// </summary>
    public bool VideoAcceleration { get; set; } = true;

    /// <summary>
    /// 是否使用内置的视频处理器和自定义像素着色器或 D3D11.
    /// （当前 D3D11 仅在视频加速/硬件表面上工作）
    /// * FLVP 支持 HDR 到 SDR，D3D11 不支持
    /// * FLVP 支持平移/缩放，D3D11 不支持
    /// * D3D11 在颜色转换和滤镜方面可能性能更好，FLVP 仅支持亮度/对比度滤镜
    /// * D3D11 支持去隔行（bob）.
    /// </summary>
    public VideoProcessor VideoProcessor
    {
        get => _videoProcessor;
        set
        {
            if (Set(ref _videoProcessor, value))
            {
                _player?.Renderer?.UpdateVideoProcessor();
            }
        }
    }

    /// <summary>
    /// 是否启用垂直同步（0：禁用，1：启用）.
    /// </summary>
    public short VSync { get; set; }

    /// <summary>
    /// 启用视频处理器执行后处理去隔行.
    /// （必须启用 D3D11 视频处理器并支持 bob 去隔行方法）.
    /// </summary>
    public bool Deinterlace
    {
        get => _deinterlace;
        set
        {
            if (Set(ref _deinterlace, value))
            {
                _player?.Renderer?.UpdateDeinterlace();
            }
        }
    }

    /// <summary>
    /// 启用底部优先的去隔行.
    /// </summary>
    public bool DeinterlaceBottomFirst
    {
        get => _deinterlaceBottomFirst;
        set
        {
            if (Set(ref _deinterlaceBottomFirst, value))
            {
                _player?.Renderer?.UpdateDeinterlace();
            }
        }
    }

    /// <summary>
    /// 将由像素着色器使用的 HDR 到 SDR 方法.
    /// </summary>
    public unsafe HdrToSdrMethod HdrToSdrMethod
    {
        get => _hdrToSdrMethod;
        set
        {
            if (Set(ref _hdrToSdrMethod, value)
                && _player != null
                && _player.VideoDecoder.VideoStream != null
                && _player.VideoDecoder.VideoStream.ColorSpace == ColorSpace.BT2020)
            {
                _player.Renderer.UpdateHDRtoSDR();
            }
        }
    }

    /// <summary>
    /// HDR 到 SDR 的色调浮点修正（Reinhard 不使用）.
    /// </summary>
    public unsafe float HdrToSdrTone
    {
        get => _hdrToSdrTone;
        set
        {
            if (Set(ref _hdrToSdrTone, value)
                && _player != null
                && _player.VideoDecoder.VideoStream != null
                && _player.VideoDecoder.VideoStream.ColorSpace == ColorSpace.BT2020)
            {
                _player.Renderer.UpdateHDRtoSDR();
            }
        }
    }

    /// <summary>
    /// 渲染器是否使用 10 位交换链或 8 位输出.
    /// </summary>
    public bool Swap10Bit { get; set; }

    /// <summary>
    /// 渲染器交换链使用的缓冲区数量.
    /// </summary>
    public int SwapBuffers { get; set; } = 2;

    /// <summary>
    /// 是否强制使用 R8G8B8A8_UNorm 格式而不是 B8G8R8A8_UNorm 格式的交换链（实验性）.
    /// （TBR：与 D3D11VP 会导致略微不同的颜色）.
    /// </summary>
    public bool SwapForceR8G8B8A8 { get; set; }

    /// <summary>
    /// 视频滤镜集合，用于存储视频滤镜和其对应的配置.
    /// </summary>
    public Dictionary<VideoFilters, VideoFilter> Filters { get; set; } = DefaultFilters();

    /// <summary>
    /// 获取默认的视频滤镜集合.
    /// </summary>
    /// <returns>滤镜集合.</returns>
    public static Dictionary<VideoFilters, VideoFilter> DefaultFilters()
    {
        Dictionary<VideoFilters, VideoFilter> filters = new();

        var available = Enum.GetValues(typeof(VideoFilters));

        foreach (var filter in available)
        {
            filters.Add((VideoFilters)filter, new VideoFilter((VideoFilters)filter));
        }

        return filters;
    }

    /// <summary>
    /// 克隆当前的 VideoConfig 对象.
    /// </summary>
    /// <returns><see cref="VideoConfig"/>.</returns>
    public VideoConfig Clone()
    {
        var video = (VideoConfig)MemberwiseClone();
        video._player = null;

        return video;
    }

    internal void SetEnabled(bool enabled)
        => Set(ref _enabled, enabled, true, nameof(Enabled));

    internal void SetPlayer(MediaPlayer.Player player)
        => _player = player;
}
