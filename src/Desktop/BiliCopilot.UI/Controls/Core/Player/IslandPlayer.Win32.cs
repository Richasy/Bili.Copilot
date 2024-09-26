// Copyright (c) Bili Copilot. All rights reserved.

using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.Win32.UI.WindowsAndMessaging;

namespace BiliCopilot.UI.Controls.Core;

/// <summary>
/// 岛播放器.
/// </summary>
public sealed partial class IslandPlayer
{
}

/// <summary>
/// MPV 播放器窗口.
/// </summary>
public class MpvPlayerWindow : IDisposable
{
    private const string PlayerWindowClassName = "BiliPlayerWindow";
    private static WNDPROC _wndProc;
    private static bool _isWndProcCreated;
    private static bool _isWindowClassRegistered;
    private HWND _hwnd;

    /// <summary>
    /// Initializes a new instance of the <see cref="MpvPlayerWindow"/> class.
    /// </summary>
    public MpvPlayerWindow()
    {
        if (!_isWndProcCreated)
        {
            _wndProc = new WNDPROC(WindowProc);
            _isWndProcCreated = true;
        }

        if (!_isWindowClassRegistered)
        {
            RegisterWindowClass();
        }
    }

    /// <summary>
    /// 获取句柄.
    /// </summary>
    /// <returns>Window handle.</returns>
    public IntPtr GetHandle()
        => _hwnd.Value;

    /// <summary>
    /// 创建窗口.
    /// </summary>
    /// <returns>窗口句柄.</returns>
    public unsafe IntPtr Create(
        IntPtr parentWindow,
        Point leftTopPoint,
        int width,
        int height)
    {
        fixed (char* classNamePtr = PlayerWindowClassName)
        {
            fixed (char* windowNamePtr = "BiliPlayer")
            {
                var hwnd = PInvoke.CreateWindowEx(
                    WINDOW_EX_STYLE.WS_EX_LAYERED | WINDOW_EX_STYLE.WS_EX_NOACTIVATE | WINDOW_EX_STYLE.WS_EX_TRANSPARENT | WINDOW_EX_STYLE.WS_EX_TOPMOST,
                    classNamePtr,
                    windowNamePtr,
                    WINDOW_STYLE.WS_CHILDWINDOW,
                    (int)leftTopPoint.X,
                    (int)leftTopPoint.Y,
                    width,
                    height,
                    new(parentWindow),
                    HMENU.Null,
                    HINSTANCE.Null);
                if (hwnd.Value == IntPtr.Zero)
                {
                    var error = Marshal.GetLastWin32Error();
                    Debug.WriteLine($"CreateWindowEx failed with error code: {error}");
                }

                PInvoke.ShowWindow(hwnd, SHOW_WINDOW_CMD.SW_SHOW);

                PInvoke.UpdateWindow(hwnd);
                PInvoke.SetParent(hwnd, new(parentWindow));
                _hwnd = hwnd;
                return hwnd.Value;
            }
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (!_hwnd.IsNull)
        {
            PInvoke.DestroyWindow(_hwnd);
        }
    }

    private static LRESULT WindowProc(HWND hWnd, uint msg, WPARAM wParam, LPARAM lParam)
    {
        Debug.WriteLine($"[IslandPlayer] Message: {msg}");
        return PInvoke.DefWindowProc(hWnd, msg, wParam, lParam);
    }

    private unsafe void RegisterWindowClass()
    {
        fixed (char* classNamePtr = PlayerWindowClassName)
        {
            var wndClass = new WNDCLASSEXW
            {
                cbSize = (uint)Marshal.SizeOf<WNDCLASSEXW>(),
                hInstance = PInvoke.GetModuleHandle((PCWSTR)null),
                lpfnWndProc = _wndProc,
                lpszClassName = classNamePtr,
            };

            var result = PInvoke.RegisterClassEx(wndClass);
            if (result == 0)
            {
                Debug.WriteLine("Window reg failed");
                return;
            }

            _isWindowClassRegistered = true;
        }
    }
}
