// Copyright (c) Bili Copilot. All rights reserved.

using System.Reflection;
using Silk.NET.Core.Native;
using Silk.NET.Direct3D11;
using Silk.NET.DXGI;
using Silk.NET.OpenGL;
using Silk.NET.WGL.Extensions.NV;

namespace BiliCopilot.UI.Controls.Core.Common;

/// <summary>
/// 帧缓冲.
/// </summary>
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
#pragma warning disable SA1600 // 缺少对公共可见类型或成员的 XML 注释
public unsafe class FrameBuffer : FrameBufferBase
{
    public FrameBuffer(
        RenderContext context,
        int frameBufferWidth,
        int frameBufferHeight,
        double compositionScaleX,
        double compositionScaleY)
    {
        Context = context;
        BufferWidth = Convert.ToInt32(frameBufferWidth * compositionScaleX);
        BufferHeight = Convert.ToInt32(frameBufferHeight * compositionScaleY);

        IDXGISwapChain1* swapChain;

        var size = Toolkits.AppToolkit.GetScreenSize(GlobalDependencies.Kernel.GetRequiredService<ViewModels.Core.AppViewModel>().ActivatedWindow);

        // SwapChain
        {
            SwapChainDesc1 swapChainDesc = new()
            {
                Width = (uint)size.Width,
                Height = (uint)size.Height,
                Format = Format.FormatB8G8R8A8Unorm,
                Stereo = false,
                SampleDesc = new SampleDesc()
                {
                    Count = 1,
                    Quality = 0,
                },
                BufferUsage = DXGI.UsageRenderTargetOutput,
                BufferCount = 2,
                SwapEffect = SwapEffect.FlipSequential,
                Flags = 0,
                Scaling = Scaling.Stretch,
                AlphaMode = AlphaMode.Ignore,
            };

            ((IDXGIFactory2*)Context.DxDeviceFactory)->CreateSwapChainForComposition((IUnknown*)Context.DxDeviceHandle, &swapChainDesc, null, &swapChain);

            SwapChainHandle = (IntPtr)swapChain;
        }

        GLFrameBufferHandle = RenderContext.GL.GenFramebuffer();
    }

    public RenderContext Context { get; }

    public uint GLColorRenderBufferHandle { get; set; }

    public uint GLDepthRenderBufferHandle { get; set; }

    public uint GLFrameBufferHandle { get; set; }

    public IntPtr DxInteropColorHandle { get; set; }

    public override int BufferWidth { get; protected set; }
    public override int BufferHeight { get; protected set; }
    public override nint SwapChainHandle { get; protected set; }

    public void Begin()
    {
        ID3D11Texture2D* colorbuffer;

        RenderContext.GL.BindFramebuffer(FramebufferTarget.Framebuffer, GLFrameBufferHandle);

        // Texture2D
        {
            var guid = typeof(ID3D11Texture2D).GetTypeInfo().GUID;
            ((IDXGISwapChain1*)SwapChainHandle)->GetBuffer(0, &guid, (void**)&colorbuffer);
        }

        // GL
        {
            GLColorRenderBufferHandle = RenderContext.GL.GenRenderbuffer();
            DxInteropColorHandle = RenderContext.NVDXInterop.DxregisterObject(Context.GlDeviceHandle, colorbuffer, GLColorRenderBufferHandle, (NV)RenderbufferTarget.Renderbuffer, NV.AccessReadWriteNV);
            RenderContext.GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, RenderbufferTarget.Renderbuffer, (uint)GLColorRenderBufferHandle);
        }

        colorbuffer->Release();

        // NVDXInterop 在 AMD GPU 上会造成严重的内存泄露，可能是缺少相关实现，需要换成更为通用的解决方案（比如 ANGLE）
        RenderContext.NVDXInterop.DxlockObjects(Context.GlDeviceHandle, 1, [DxInteropColorHandle]);

        RenderContext.GL.BindFramebuffer(FramebufferTarget.Framebuffer, GLFrameBufferHandle);
        RenderContext.GL.Viewport(0, 0, (uint)BufferHeight, (uint)BufferHeight);
    }

    public void End()
    {
        RenderContext.GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        RenderContext.NVDXInterop.DxunlockObjects(Context.GlDeviceHandle, 1, [DxInteropColorHandle]);
        RenderContext.NVDXInterop.DxunregisterObject(Context.GlDeviceHandle, DxInteropColorHandle);
        RenderContext.GL.DeleteRenderbuffer(GLColorRenderBufferHandle);
        ((IDXGISwapChain1*)SwapChainHandle)->Present(1, 0);
    }

    public void UpdateSize(
        int framebufferWidth,
        int framebufferHeight,
        double compositionScaleX,
        double compositionScaleY)
    {
        BufferWidth = Convert.ToInt32(framebufferWidth * compositionScaleX);
        BufferHeight = Convert.ToInt32(framebufferHeight * compositionScaleY);

        ((IDXGISwapChain1*)SwapChainHandle)->ResizeBuffers(2, (uint)BufferWidth, (uint)BufferHeight, Format.FormatUnknown, 0);
        var matrix = new Matrix3X2F { DXGI11 = 1.0f / (float)compositionScaleX, DXGI22 = 1.0f / (float)compositionScaleY };
        ((IDXGISwapChain2*)SwapChainHandle)->SetMatrixTransform(ref matrix);
    }

    public override void Dispose()
    {
        RenderContext.GL.DeleteFramebuffer(GLFrameBufferHandle);

        RenderContext.NVDXInterop.DxunregisterObject(Context.GlDeviceHandle, DxInteropColorHandle);
        RenderContext.GL.DeleteRenderbuffer(GLColorRenderBufferHandle);

        GC.SuppressFinalize(this);
    }
}
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
