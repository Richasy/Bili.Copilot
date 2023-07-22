// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Bili.Copilot.Libs.Player.Core;
using Bili.Copilot.Libs.Player.Enums;
using Bili.Copilot.Libs.Player.MediaFramework.MediaDecoder;
using Bili.Copilot.Libs.Player.MediaFramework.MediaFrame;
using Bili.Copilot.Libs.Player.MediaFramework.MediaStream;
using Bili.Copilot.Libs.Player.Misc;
using Bili.Copilot.Libs.Player.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Vortice;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Mathematics;
using Windows.Foundation;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaRenderer;

/// <summary>
/// 渲染器类，继承自ObservableObject和IDisposable接口.
/// </summary>
public partial class Renderer : ObservableObject, IDisposable
{
    private readonly Renderer _parent; // 父渲染器
    private int _panXOffset; // X轴偏移量
    private int _panYOffset; // Y轴偏移量
    private double _zoom = 1; // 缩放比例
    private uint _rotationAngle; // 旋转角度
    private Point _zoomCenter = ZoomCenterPoint; // 缩放中心点
    private CornerRadius _cornerRadius = new(0); // 圆角半径
    private CornerRadius _zeroCornerRadius = new(0); // 零圆角半径
    private VideoProcessorRotation _d3d11vpRotation = VideoProcessorRotation.Identity; // D3D11VP旋转角度

    [ObservableProperty]
    private bool _isHDR; // 是否为HDR

    [ObservableProperty]
    private VideoProcessor _videoProcessor = VideoProcessor.Flyleaf; // 视频处理器

    /// <summary>
    /// 构造函数，创建一个渲染器对象.
    /// </summary>
    /// <param name="videoDecoder">视频解码器.</param>
    /// <param name="handle">控件句柄.</param>
    /// <param name="uniqueId">唯一标识符.</param>
    public Renderer(VideoDecoder videoDecoder, IntPtr handle = default, int uniqueId = -1)
    {
        UniqueId = uniqueId == -1 ? Utils.GetUniqueId() : uniqueId;
        VideoDecoder = videoDecoder;
        Config = videoDecoder.Config;
        Log = new LogHandler(("[#" + UniqueId + "]").PadRight(8, ' ') + " [Renderer      ] ");

        singleStageDesc = new Texture2DDescription()
        {
            Usage = ResourceUsage.Staging,
            Format = Format.B8G8R8A8_UNorm,
            ArraySize = 1,
            MipLevels = 1,
            BindFlags = BindFlags.None,
            CPUAccessFlags = CpuAccessFlags.Read,
            SampleDescription = new SampleDescription(1, 0),

            Width = -1,
            Height = -1,
        };

        singleGpuDesc = new Texture2DDescription()
        {
            Usage = ResourceUsage.Default,
            Format = Format.B8G8R8A8_UNorm,
            ArraySize = 1,
            MipLevels = 1,
            BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
            SampleDescription = new SampleDescription(1, 0),
        };

        wndProcDelegate = new(WndProc);
        wndProcDelegatePtr = Marshal.GetFunctionPointerForDelegate(wndProcDelegate);
        ControlHandle = handle;
        Initialize();
    }

    /// <summary>
    /// 构造函数，创建一个渲染器对象.
    /// </summary>
    /// <param name="renderer">父渲染器.</param>
    /// <param name="handle">控件句柄.</param>
    /// <param name="uniqueId">唯一标识符.</param>
    public Renderer(Renderer renderer, IntPtr handle, int uniqueId = -1)
    {
        UniqueId = uniqueId == -1 ? Utils.GetUniqueId() : uniqueId;
        Log = new LogHandler(("[#" + UniqueId + "]").PadRight(8, ' ') + " [Renderer  Repl] ");

        renderer.Child = this;
        _parent = renderer;
        Config = renderer.Config;
        wndProcDelegate = new(WndProc);
        wndProcDelegatePtr = Marshal.GetFunctionPointerForDelegate(wndProcDelegate);
        ControlHandle = handle;
    }

    /// <summary>
    /// 缩放中心点.
    /// </summary>
    public static Point ZoomCenterPoint { get; } = new(0.5, 0.5);

    /// <summary>
    /// 配置对象.
    /// </summary>
    public Config Config { get; private set; }

    /// <summary>
    /// 控件宽度.
    /// </summary>
    public int ControlWidth { get; private set; }

    /// <summary>
    /// 控件高度.
    /// </summary>
    public int ControlHeight { get; private set; }

    /// <summary>
    /// D3D11设备.
    /// </summary>
    public ID3D11Device Device { get; private set; }

    /// <summary>
    /// D3D11VP是否失败.
    /// </summary>
    public bool D3D11VPFailed => vc == null;

    /// <summary>
    /// GPU适配器.
    /// </summary>
    public GpuAdapter GPUAdapter { get; private set; }

    /// <summary>
    /// 是否已释放.
    /// </summary>
    public bool Disposed { get; private set; } = true;

    /// <summary>
    /// 交换链是否已释放.
    /// </summary>
    public bool SCDisposed { get; private set; } = true;

    /// <summary>
    /// 最大离屏纹理数量.
    /// </summary>
    public int MaxOffScreenTextures { get; set; } = 20;

    /// <summary>
    /// 视频解码器.
    /// </summary>
    public VideoDecoder VideoDecoder { get; internal set; }

    /// <summary>
    /// 视频流.
    /// </summary>
    public VideoStream VideoStream => VideoDecoder.VideoStream;

    /// <summary>
    /// 视口.
    /// </summary>
    public Viewport GetViewport { get; private set; }

    /// <summary>
    /// 圆角半径.
    /// </summary>
    public CornerRadius CornerRadius
    {
        get => _cornerRadius; set
        {
            if (_cornerRadius == value)
            {
                return;
            }

            _cornerRadius = value;
            UpdateCornerRadius();
        }
    }

    /// <summary>
    /// X轴边缘像素数.
    /// </summary>
    public int SideXPixels { get; private set; }

    /// <summary>
    /// Y轴边缘像素数.
    /// </summary>
    public int SideYPixels { get; private set; }

    /// <summary>
    /// X轴偏移量.
    /// </summary>
    public int PanXOffset { get => _panXOffset; set => SetPanX(value); }

    /// <summary>
    /// Y轴偏移量.
    /// </summary>
    public int PanYOffset { get => _panYOffset; set => SetPanY(value); }

    /// <summary>
    /// 旋转角度.
    /// </summary>
    public uint Rotation
    {
        get => _rotationAngle; set
        {
            lock (lockDevice)
            {
                UpdateRotation(value);
            }
        }
    }

    /// <summary>
    /// 缩放比例（百分比，100%等于1.0）.
    /// </summary>
    public double Zoom { get => _zoom; set => SetZoom(value); }

    /// <summary>
    /// 唯一标识符.
    /// </summary>
    public int UniqueId { get; private set; }

    /// <summary>
    /// 视频滤镜字典.
    /// </summary>
    public Dictionary<VideoFilters, VideoFilter> Filters { get; set; }

    /// <summary>
    /// 最后一帧视频帧.
    /// </summary>
    public VideoFrame LastFrame { get; set; }

    /// <summary>
    /// 视频矩形.
    /// </summary>
    public RawRect VideoRect { get; set; }

    /// <summary>
    /// 日志处理器.
    /// </summary>
    internal LogHandler Log { get; }

    /// <summary>
    /// 子渲染器.
    /// </summary>
    internal Renderer Child { get; set; }

    /// <summary>
    /// 控件句柄.
    /// </summary>
    internal IntPtr ControlHandle { get; set; }

    /// <summary>
    /// 交换链回调函数.
    /// </summary>
    internal Action<IDXGISwapChain2> SwapChainWinUIClbk { get; set; }

    /// <summary>
    /// 设置X轴偏移量.
    /// </summary>
    /// <param name="panX">X轴偏移量.</param>
    /// <param name="refresh">是否刷新.</param>
    public void SetPanX(int panX, bool refresh = true)
    {
        lock (lockDevice)
        {
            _panXOffset = panX;

            if (Disposed)
            {
                return;
            }

            SetViewport(refresh);
        }
    }

    /// <summary>
    /// 设置Y轴偏移量.
    /// </summary>
    /// <param name="panY">Y轴偏移量.</param>
    /// <param name="refresh">是否刷新.</param>
    public void SetPanY(int panY, bool refresh = true)
    {
        lock (lockDevice)
        {
            _panYOffset = panY;

            if (Disposed)
            {
                return;
            }

            SetViewport(refresh);
        }
    }

    /// <summary>
    /// 设置缩放比例.
    /// </summary>
    /// <param name="zoom">缩放比例.</param>
    /// <param name="refresh">是否刷新.</param>
    public void SetZoom(double zoom, bool refresh = true)
    {
        lock (lockDevice)
        {
            _zoom = zoom;

            if (Disposed)
            {
                return;
            }

            SetViewport(refresh);
        }
    }

    /// <summary>
    /// 设置缩放中心点.
    /// </summary>
    /// <param name="p">缩放中心点.</param>
    /// <param name="refresh">是否刷新.</param>
    public void SetZoomCenter(Point p, bool refresh = true)
    {
        lock (lockDevice)
        {
            _zoomCenter = p;

            if (Disposed)
            {
                return;
            }

            if (refresh)
            {
                SetViewport();
            }
        }
    }

    /// <summary>
    /// 设置缩放比例和缩放中心点.
    /// </summary>
    /// <param name="zoom">缩放比例.</param>
    /// <param name="p">缩放中心点.</param>
    /// <param name="refresh">是否刷新.</param>
    public void SetZoomAndCenter(double zoom, Point p, bool refresh = true)
    {
        lock (lockDevice)
        {
            _zoom = zoom;
            _zoomCenter = p;

            if (Disposed)
            {
                return;
            }

            if (refresh)
            {
                SetViewport();
            }
        }
    }

    /// <summary>
    /// 设置全部参数（X轴偏移量、Y轴偏移量、旋转角度、缩放比例、缩放中心点）.
    /// </summary>
    /// <param name="panX">X轴偏移量.</param>
    /// <param name="panY">Y轴偏移量.</param>
    /// <param name="rotation">旋转角度.</param>
    /// <param name="zoom">缩放比例.</param>
    /// <param name="p">缩放中心点.</param>
    /// <param name="refresh">是否刷新.</param>
    public void SetPanAll(int panX, int panY, uint rotation, double zoom, Point p, bool refresh = true)
    {
        lock (lockDevice)
        {
            _panXOffset = panX;
            _panYOffset = panY;
            _zoom = zoom;
            _zoomCenter = p;
            UpdateRotation(rotation, false);

            if (Disposed)
            {
                return;
            }

            if (refresh)
            {
                SetViewport();
            }
        }
    }

    /// <summary>
    /// 设置子渲染器的句柄.
    /// </summary>
    /// <param name="handle">子渲染器的句柄.</param>
    public void SetChildHandle(IntPtr handle)
    {
        lock (lockDevice)
        {
            if (Child != null)
            {
                DisposeChild();
            }

            if (handle == IntPtr.Zero)
            {
                return;
            }

            Child = new(this, handle, UniqueId);
            InitializeChildSwapChain();
        }
    }
}
