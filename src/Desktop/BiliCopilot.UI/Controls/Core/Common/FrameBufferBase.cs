// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Controls.Core.Common;

/// <summary>
/// 帧缓冲基类.
/// </summary>
public abstract class FrameBufferBase : IDisposable
{
    /// <summary>
    /// 缓冲宽度.
    /// </summary>
    public abstract int BufferWidth { get; protected set; }

    /// <summary>
    /// 缓冲高度.
    /// </summary>
    public abstract int BufferHeight { get; protected set; }

    /// <summary>
    /// 交换链句柄.
    /// </summary>
    public abstract IntPtr SwapChainHandle { get; protected set; }

    /// <inheritdoc/>
    public abstract void Dispose();
}
