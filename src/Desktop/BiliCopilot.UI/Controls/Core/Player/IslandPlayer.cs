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
    private double _lastOffset;

    /// <summary>
    /// Initializes a new instance of the <see cref="IslandPlayer"/> class.
    /// </summary>
    public IslandPlayer()
    {
        DefaultStyleKey = typeof(IslandPlayer);
        _parentWindow = this.Get<AppViewModel>().ActivatedWindow;
    }

    /// <summary>
    /// 剪裁顶部区域.
    /// </summary>
    /// <param name="offset">裁切高度.</param>
    public void CutTopRgn(double offset)
    {
        if (_playerWindow is not null)
        {
            var handle = _playerWindow.GetHandle();
            _lastOffset = offset * _scale;
            var rgn = PInvoke.CreateRectRgn(0, (int)_lastOffset, (int)_lastWidth, (int)_lastHeight);
            PInvoke.SetWindowRgn(new(handle), rgn, true);
            OnRootGridSizeChanged(default, default);
        }
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        _rootGrid = (Grid)GetTemplateChild("RootGrid");
        _rootGrid.SizeChanged += OnRootGridSizeChanged;
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
            _rootGrid.SizeChanged -= OnRootGridSizeChanged;
        }

        _playerWindow = default;
        _overlayWindow = default;
    }

    private void OnRootGridSizeChanged(object sender, SizeChangedEventArgs e)
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, async () =>
        {
            await Task.Delay(200);
            InitializeLayoutPoints();
            if (_playerWindow is not null && _playerWindow.GetHandle() != IntPtr.Zero)
            {
                PInvoke.SetWindowPos(new(_playerWindow.GetHandle()), HWND.Null, (int)_windowLeftTopPoint.X, (int)_windowLeftTopPoint.Y, (int)_lastWidth, (int)_lastHeight, SET_WINDOW_POS_FLAGS.SWP_SHOWWINDOW);
                _overlayWindow.MoveAndResize((int)_lastWidth, (int)_lastHeight);
            }
        });
    }

    private void InitializeLayoutPoints()
    {
        var windowRoot = this.Get<AppViewModel>().ActivatedWindow.Content;
        var transform = this.TransformToVisual(windowRoot);
        _scale = XamlRoot.RasterizationScale;
        _windowLeftTopPoint = transform.TransformPoint(new Point(0, 0));
        _windowLeftTopPoint.X *= _scale;
        _windowLeftTopPoint.Y *= _scale;
        _lastWidth = (ActualWidth - 6) * _scale;
        _lastHeight = ActualHeight * _scale;
    }
}
