// Copyright (c) Bili Copilot. All rights reserved.

using System.Runtime.InteropServices;
using Windows.ApplicationModel.DataTransfer;
using WinRT;

namespace BiliCopilot.UI.Extensions;

/// <summary>
/// 系统数据转移管理器互操作接口.
/// </summary>
[ComImport]
[System.Runtime.InteropServices.Guid("3A3DCD6C-3EAB-43DC-BCDE-45671CE800C8")]
[System.Runtime.InteropServices.InterfaceType(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsIUnknown)]
internal interface IDataTransferManagerInterop
{
    /// <summary>
    /// 从窗口句柄获取数据转移管理器.
    /// </summary>
    /// <returns><see cref="DataTransferManager"/>.</returns>
    IntPtr GetForWindow(IntPtr appWindow, [System.Runtime.InteropServices.In] ref Guid riid);

    /// <summary>
    /// 显示共享 UI.
    /// </summary>
    void ShowShareUIForWindow(IntPtr appWindow);
}

/// <summary>
/// 数据转移管理器互操作.
/// </summary>
public static class DataTransferManagerInterop
{
    private static Guid _dtm_iid = new(0xa5caee9b, 0x8708, 0x49d1, 0x8d, 0x36, 0x67, 0xd2, 0x5a, 0x8d, 0xa0, 0x0c);

    /// <summary>
    /// 从窗口句柄获取数据转移管理器.
    /// </summary>
    /// <returns><see cref="DataTransferManager"/>.</returns>
    public static DataTransferManager GetForWindow(IntPtr hWnd)
    {
        var interop = DataTransferManager.As<IDataTransferManagerInterop>();
        return MarshalInterface<DataTransferManager>.FromAbi(interop.GetForWindow(hWnd, ref _dtm_iid));
    }

    /// <summary>
    /// 显示共享 UI.
    /// </summary>
    public static void ShowShareUIForWindow(IntPtr hWnd)
    {
        var interop = DataTransferManager.As<IDataTransferManagerInterop>();
        interop.ShowShareUIForWindow(hWnd);
    }
}
