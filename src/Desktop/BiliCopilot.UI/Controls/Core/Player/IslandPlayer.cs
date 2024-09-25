// Copyright (c) Bili Copilot. All rights reserved.

using System.Diagnostics;
using System.Runtime.InteropServices;
using BiliCopilot.UI.ViewModels.Core;
using Richasy.WinUI.Share.Base;
using Windows.Win32.UI.WindowsAndMessaging;
using WinUIEx;

namespace BiliCopilot.UI.Controls.Core;

/// <summary>
/// 岛播放器.
/// </summary>
public sealed partial class IslandPlayer : LayoutControlBase<IslandPlayerViewModel>
{
    private const string PlayerWindowClassName = "BiliPlayerWindow";
    private readonly HWND _parentWindowHandle;
    private readonly Window _parentWindow;
    private double _scale;
    private Grid _rootGrid;
    private HWND? _playerWindowHandle;
    private WNDPROC _windowProc;
    private Point _windowLeftTopPoint;
    private double _lastWidth;
    private double _lastHeight;

    /// <summary>
    /// Initializes a new instance of the <see cref="IslandPlayer"/> class.
    /// </summary>
    public IslandPlayer()
    {
        DefaultStyleKey = typeof(IslandPlayer);
        _parentWindow = this.Get<AppViewModel>().ActivatedWindow;
        _parentWindowHandle = new(_parentWindow.GetWindowHandle());
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        _rootGrid = (Grid)GetTemplateChild("RootGrid");
        _rootGrid.SizeChanged += OnRootGridSizeChanged;
    }

    /// <inheritdoc/>
    protected override unsafe void OnControlLoaded()
    {
        InitializeLayoutPoints();
        _windowProc = new WNDPROC(WindowProc);
        fixed (char* classNamePtr = "BiliPlayerWindow")
        {
            fixed (char* windowNamePtr = "BiliPlayer")
            {
                var wndClass = new WNDCLASSEXW
                {
                    cbSize = (uint)Marshal.SizeOf<WNDCLASSEXW>(),
                    hInstance = PInvoke.GetModuleHandle((PCWSTR)null),
                    lpfnWndProc = _windowProc,
                    lpszClassName = classNamePtr,
                };

                PInvoke.RegisterClassEx(wndClass);
                _playerWindowHandle = PInvoke.CreateWindowEx(
                    WINDOW_EX_STYLE.WS_EX_LAYERED | WINDOW_EX_STYLE.WS_EX_NOACTIVATE | WINDOW_EX_STYLE.WS_EX_TRANSPARENT | WINDOW_EX_STYLE.WS_EX_TOPMOST,
                    classNamePtr,
                    windowNamePtr,
                    WINDOW_STYLE.WS_CHILDWINDOW,
                    (int)_windowLeftTopPoint.X,
                    (int)_windowLeftTopPoint.Y,
                    (int)_lastWidth,
                    (int)_lastHeight,
                    _parentWindowHandle,
                    HMENU.Null,
                    HINSTANCE.Null);
                if (_playerWindowHandle?.Value == IntPtr.Zero)
                {
                    var error = Marshal.GetLastWin32Error();
                    Debug.WriteLine($"CreateWindowEx failed with error code: {error}");
                }

                PInvoke.ShowWindow(_playerWindowHandle!.Value, SHOW_WINDOW_CMD.SW_SHOW);

                PInvoke.UpdateWindow(_playerWindowHandle!.Value);
                PInvoke.SetParent(_playerWindowHandle.Value, _parentWindowHandle);
                ViewModel.SetWindow(_playerWindowHandle!.Value);
            }
        }
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (_rootGrid is not null)
        {
            _rootGrid.SizeChanged -= OnRootGridSizeChanged;
        }

        if (_playerWindowHandle is not null)
        {
            PInvoke.DestroyWindow(_playerWindowHandle.Value);
            _playerWindowHandle = null;
        }
    }

    private void OnRootGridSizeChanged(object sender, SizeChangedEventArgs e)
    {
        InitializeLayoutPoints();
        if (_playerWindowHandle is not null)
        {
            PInvoke.SetWindowPos(_playerWindowHandle.Value, HWND.Null, (int)_windowLeftTopPoint.X, (int)_windowLeftTopPoint.Y, (int)_lastWidth, (int)_lastHeight, SET_WINDOW_POS_FLAGS.SWP_SHOWWINDOW);
        }
    }

    private void InitializeLayoutPoints()
    {
        var windowRoot = this.Get<AppViewModel>().ActivatedWindow.Content;
        var transform = this.TransformToVisual(windowRoot);
        _scale = XamlRoot.RasterizationScale;
        _windowLeftTopPoint = transform.TransformPoint(new Point(0, 0));
        _windowLeftTopPoint.X *= _scale;
        _windowLeftTopPoint.Y *= _scale;
        _lastWidth = ActualWidth * _scale;
        _lastHeight = ActualHeight * _scale;
    }
}
