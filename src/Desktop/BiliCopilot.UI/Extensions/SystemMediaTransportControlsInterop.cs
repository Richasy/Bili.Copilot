// Copyright (c) Bili Copilot. All rights reserved.

using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Media;

namespace BiliCopilot.UI.Extensions;

/// <summary>
/// 系统媒体传输控件互操作接口.
/// </summary>
[System.Runtime.InteropServices.Guid("ddb0472d-c911-4a1f-86d9-dc3d71a95f5a")]
[System.Runtime.InteropServices.InterfaceType(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsIInspectable)]
internal interface ISystemMediaTransportControlsInterop
{
    /// <summary>
    /// 从窗口句柄获取系统媒体传输控件.
    /// </summary>
    /// <returns><see cref="SystemMediaTransportControls"/>.</returns>
    SystemMediaTransportControls GetForWindow(IntPtr appWindow, [System.Runtime.InteropServices.In] ref Guid riid);
}

/// <summary>
/// 系统媒体传输控件互操作.
/// </summary>
public static class SystemMediaTransportControlsInterop
{
    /// <summary>
    /// 从窗口句柄获取系统媒体传输控件.
    /// </summary>
    /// <returns><see cref="SystemMediaTransportControls"/>.</returns>
    public static SystemMediaTransportControls GetForWindow(IntPtr hWnd)
    {
        var systemMediaTransportControlsInterop = (ISystemMediaTransportControlsInterop)WindowsRuntimeMarshal.GetActivationFactory(typeof(SystemMediaTransportControls));
        var guid = typeof(SystemMediaTransportControls).GUID;

        return systemMediaTransportControlsInterop.GetForWindow(hWnd, ref guid);
    }
}
