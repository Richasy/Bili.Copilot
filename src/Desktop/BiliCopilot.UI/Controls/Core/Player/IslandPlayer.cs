// Copyright (c) Bili Copilot. All rights reserved.

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
    private readonly Window _parentWindow;
    private MpvPlayerWindow _playerWindow;
    private MpvPlayerOverlayWindow _overlayWindow;
    private double _scale;
    private Grid _rootGrid;
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
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        _rootGrid = (Grid)GetTemplateChild("RootGrid");
        _rootGrid.SizeChanged += OnRootGridSizeChangedAsync;
    }

    /// <inheritdoc/>
    protected override async void OnControlLoaded()
    {
        InitializeLayoutPoints();
        if (_playerWindow is null)
        {
            _playerWindow = new MpvPlayerWindow();
            _playerWindow.Create(_parentWindow.GetWindowHandle(), _windowLeftTopPoint, (int)_lastWidth, (int)_lastHeight);
        }

        if (_overlayWindow is null && _playerWindow.GetHandle() != IntPtr.Zero)
        {
            _overlayWindow = new MpvPlayerOverlayWindow();
            _overlayWindow.Create(_playerWindow.GetHandle(), (int)_lastWidth, (int)_lastHeight);
        }

        if (_playerWindow.GetHandle() != IntPtr.Zero)
        {
            await ViewModel.InitializeAsync(_playerWindow, _overlayWindow);
        }
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (_rootGrid is not null)
        {
            _rootGrid.SizeChanged -= OnRootGridSizeChangedAsync;
        }

        _playerWindow = default;
        _overlayWindow = default;
    }

    private async void OnRootGridSizeChangedAsync(object sender, SizeChangedEventArgs e)
    {
        await Task.Delay(100);
        InitializeLayoutPoints();
        if (_playerWindow is not null && _playerWindow.GetHandle() != IntPtr.Zero)
        {
            PInvoke.SetWindowPos(new(_playerWindow.GetHandle()), HWND.Null, (int)_windowLeftTopPoint.X, (int)_windowLeftTopPoint.Y, (int)_lastWidth, (int)_lastHeight, SET_WINDOW_POS_FLAGS.SWP_SHOWWINDOW);
            _overlayWindow.MoveAndResize((int)_lastWidth, (int)_lastHeight);
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
