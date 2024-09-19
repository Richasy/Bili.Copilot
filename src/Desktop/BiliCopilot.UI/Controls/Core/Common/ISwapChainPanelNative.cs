﻿// Copyright (c) Bili Copilot. All rights reserved.

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace BiliCopilot.UI.Controls.Core.Common;

/// <summary>
/// 交换链面板本机.
/// </summary>
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("63aad0b8-7c24-40ff-85a8-640d944cc325")]
[GeneratedComInterface]
public partial interface ISwapChainPanelNative
{
    /// <summary>
    /// 设置交换链.
    /// </summary>
    /// <returns><see cref="int"/>.</returns>
    [PreserveSig]
    int SetSwapChain(IntPtr swapChain);

    /// <summary>
    /// 释放.
    /// </summary>
    /// <returns>结果.</returns>
    [PreserveSig]
    ulong Release();
}
