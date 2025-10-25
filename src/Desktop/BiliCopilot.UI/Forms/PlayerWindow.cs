using BiliCopilot.UI.Controls.Core;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Hosting;
using Richasy.WinUIKernel.Share.Base;
using Windows.Graphics;
using Windows.Win32.UI.WindowsAndMessaging;
using WinUIEx;

namespace BiliCopilot.UI.Forms;

public sealed partial class PlayerWindow : IAsyncDisposable
{
    private const int WindowMinWidth = 480;
    private const int WindowMinHeight = 270;
    private readonly AppWindow _rootWindow;
    private readonly DesktopWindowXamlSource _xamlSource;
    private readonly CursorGrid _rootGrid;
    private InputActivationListener _inputActivationListener;
    private bool _isFirstActivated = true;
    private bool _enteringFullScreen;
    private bool _enteringCompactOverlay;
    private bool _isPresenterChanging;
    private Action<bool> _activeChangedAction;

    public PlayerWindow()
    {
        _rootWindow = AppWindow.Create();
        _rootWindow.AssociateWithDispatcherQueue(DispatcherQueue.GetForCurrentThread());
        _rootWindow.Destroying += OnWindowDestroying;

        _rootWindow.SetIcon("Assets/logo.ico");
        _rootWindow.TitleBar.ExtendsContentIntoTitleBar = true;
        _rootWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        _rootWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        _rootWindow.TitleBar.ButtonHoverBackgroundColor = "#222222".ToColor();
        _rootWindow.TitleBar.ButtonPressedBackgroundColor = "#111111".ToColor();
        _rootWindow.TitleBar.ButtonForegroundColor = Colors.White;
        _rootWindow.TitleBar.ButtonHoverForegroundColor = Colors.White;
        _rootWindow.TitleBar.ButtonPressedForegroundColor = Colors.White;
        _rootWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;

        _rootGrid = new CursorGrid();
        _xamlSource = new DesktopWindowXamlSource();
        _xamlSource.Initialize(_rootWindow.Id);
        _xamlSource.Content = _rootGrid;

        _inputActivationListener = InputActivationListener.GetForWindowId(_rootWindow.Id);
        _inputActivationListener.InputActivationChanged += OnInputActivationChanged;

        ResetWindowMinSize();
    }

    /// <summary>
    /// 对象是否已经被释放.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// 窗口是否已关闭.
    /// </summary>
    public bool IsClosed { get; private set; }

    /// <summary>
    /// 窗口展示模式是否正在改变.
    /// </summary>
    public bool IsPresenterChanging => _isPresenterChanging;

    /// <summary>
    /// 窗口大小改变事件.
    /// </summary>
    public event EventHandler<Size> SizeChanged;

    /// <summary>
    /// 窗口句柄.
    /// </summary>
    public IntPtr Handle => Win32Interop.GetWindowFromWindowId(_rootWindow.Id);

    /// <summary>
    /// XAML 根元素.
    /// </summary>
    public XamlRoot? XamlRoot => _xamlSource?.Content?.XamlRoot;

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        if (IsDisposed)
        {
            return ValueTask.CompletedTask;
        }

        IsDisposed = true;

        if (_rootWindow != null)
        {
            _rootWindow.Changed -= OnWindowChanged;
            _rootWindow.Destroy();
        }

        _xamlSource?.Dispose();
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// 显示窗口.
    /// </summary>
    public void Show(bool restorePosition = true)
    {
        _rootWindow.Show();
        PInvoke.SetForegroundWindow(new(Handle));
        PInvoke.SetFocus(new(Handle));
        if (restorePosition)
        {
            MoveAndResize();
        }

        if (_isFirstActivated)
        {
            _rootWindow.Changed += OnWindowChanged;
            UpdateXamlSourcePosition();
            if (PInvoke.GetForegroundWindow() == Handle)
            {
                PInvoke.SetFocus(new(Win32Interop.GetWindowFromWindowId(_xamlSource.SiteBridge.WindowId)));
                ResetFocus();
            }

            _isFirstActivated = false;
        }
    }

    public void ShowTip(string message, InfoType type)
    {
        if (_rootGrid?.Children.FirstOrDefault() is PlayerOverlay overlay)
        {
            overlay.ShowTip(message, type);
        }
    }

    /// <summary>
    /// 设置标题栏按钮的可见性.
    /// </summary>
    /// <param name="isVisible"></param>
    public void SetCaptionButtonVisibility(bool isVisible)
    {
        _rootWindow.TitleBar?.ButtonForegroundColor = isVisible ? Colors.White : "#111111".ToColor();
    }

    public void SetActiveAction(Action<bool> activeChangedAction)
        => _activeChangedAction = activeChangedAction;

    /// <summary>
    /// 关闭窗口.
    /// </summary>
    public void Close() => _rootWindow.Destroy();

    /// <summary>
    /// 隱藏光标.
    /// </summary>
    public void HideCursor()
        => _rootGrid?.HideCursor();

    /// <summary>
    /// 显示光标.
    /// </summary>
    public void ShowCursor()
        => _rootGrid?.ShowCursor();

    public void EnterFullScreen()
    {
        if (_rootWindow.Presenter.Kind != AppWindowPresenterKind.FullScreen)
        {
            if (IsTopMost())
            {
                SetTopMost(false);
            }

            _enteringFullScreen = true;
            _enteringCompactOverlay = false;
            _isPresenterChanging = true;
            _rootWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
            _ = Task.Run(() =>
            {
                Task.Delay(500).Wait();
                _isPresenterChanging = false;
            });
        }
    }

    public void ExitFullScreen()
    {
        if (_rootWindow.Presenter.Kind == AppWindowPresenterKind.FullScreen)
        {
            _enteringFullScreen = false;
            _isPresenterChanging = true;
            _rootWindow.SetPresenter(AppWindowPresenterKind.Overlapped);
            _ = Task.Run(() =>
            {
                Task.Delay(500).Wait();
                _isPresenterChanging = false;
            });
        }
    }

    public void EnterCompactOverlay()
    {
        if (_rootWindow.Presenter.Kind != AppWindowPresenterKind.CompactOverlay)
        {
            if (IsTopMost())
            {
                SetTopMost(false);
            }

            _enteringCompactOverlay = true;
            _enteringFullScreen = false;
            _isPresenterChanging = true;
            _rootWindow.SetPresenter(AppWindowPresenterKind.CompactOverlay);
            _ = Task.Run(() =>
            {
                Task.Delay(500).Wait();
                _isPresenterChanging = false;
            });
        }
    }

    public void ExitCompactOverlay()
    {
        if (_rootWindow.Presenter.Kind == AppWindowPresenterKind.CompactOverlay)
        {
            _enteringCompactOverlay = false;
            _isPresenterChanging = true;
            _rootWindow.SetPresenter(AppWindowPresenterKind.Overlapped);
            _ = Task.Run(() =>
            {
                Task.Delay(500).Wait();
                _isPresenterChanging = false;
            });
        }
    }

    /// <summary>
    /// 设置主题.
    /// </summary>
    /// <param name="theme">元素主题.</param>
    public void SetTheme(ElementTheme theme)
    {
        _rootGrid.RequestedTheme = theme;
    }

    /// <summary>
    /// 设置UI元素.
    /// </summary>
    /// <param name="element">UI元素.</param>
    public void SetUIElement(UIElement? element)
    {
        if (_rootGrid.Children.Count > 0)
        {
            _rootGrid.Children.Clear();
        }

        if (element is not null)
        {
            _rootGrid.Children.Add(element);
        }
    }

    /// <summary>
    /// 获取窗口对象.
    /// </summary>
    /// <returns><see cref="AppWindow"/>.</returns>
    public AppWindow GetWindow()
        => _rootWindow;

    public void DelayUpdateTitleBarPadding(bool delaySec = false)
    {
        if (_rootGrid?.Children.FirstOrDefault() is PlayerOverlay overlay)
        {
            this.Get<DispatcherQueue>().TryEnqueue(async () =>
            {
                if (delaySec)
                {
                    overlay?.HeaderBar.HorizontalAlignment = HorizontalAlignment.Left;
                    await Task.Delay(800);
                    overlay?.HeaderBar.HorizontalAlignment = HorizontalAlignment.Stretch;
                    await Task.Delay(800);
                }

                if (overlay?.HeaderBar?.ActualWidth > 0 && _rootWindow.IsVisible && !PInvoke.IsIconic(new(Handle)))
                {
                    overlay.HeaderBar.UpdatePadding();
                }
            });
        }
    }

    private async void OnInputActivationChanged(InputActivationListener sender, InputActivationListenerActivationChangedEventArgs args)
    {
        // 延迟处理，避免打断正常的用户操作.
        await Task.Delay(400);
        if (IsClosed)
        {
            return;
        }

        if (sender.State != InputActivationState.Deactivated)
        {
            ResetFocus();
        }

        _activeChangedAction?.Invoke(sender.State == InputActivationState.Activated);

        if (_rootWindow.Position.X < -10000 || _rootWindow.Position.Y < -10000)
        {
            MoveAndResize();
        }
    }

    private void OnWindowChanged(AppWindow sender, AppWindowChangedEventArgs args)
    {
        if (args.DidSizeChange)
        {
            var size = _rootWindow.Presenter.Kind is AppWindowPresenterKind.FullScreen or AppWindowPresenterKind.CompactOverlay
                ? _rootWindow.Size
                : _rootWindow.ClientSize;
            SizeChanged?.Invoke(this, new Size(size.Width, size.Height));
        }

        if (args.DidSizeChange || args.DidVisibilityChange || args.DidPresenterChange)
        {
            UpdateXamlSourcePosition();
            SaveCurrentWindowStats();
        }

        if ((args.DidVisibilityChange || args.DidPresenterChange) && sender.IsVisible)
        {
            ResetFocus();
        }

        if (args.DidPresenterChange && sender.Presenter.Kind == AppWindowPresenterKind.Overlapped)
        {
            ResetWindowMinSize();
            UpdateXamlSourcePosition();
            DelayUpdateTitleBarPadding(true);
        }

        if (args.DidPositionChange)
        {
            SaveCurrentWindowStats();
        }
    }

    private void ResetWindowMinSize()
    {
        if (_rootWindow.Presenter.Kind == AppWindowPresenterKind.Overlapped)
        {
            var scale = XamlRoot?.RasterizationScale ?? 1d;
            (_rootWindow.Presenter as OverlappedPresenter)?.PreferredMinimumHeight = Convert.ToInt32(WindowMinHeight * scale);
            (_rootWindow.Presenter as OverlappedPresenter)?.PreferredMinimumWidth = Convert.ToInt32(WindowMinWidth * scale);
        }
        else if (_rootWindow.Presenter.Kind == AppWindowPresenterKind.CompactOverlay)
        {
            (_rootWindow.Presenter as CompactOverlayPresenter)?.InitialSize = CompactOverlaySize.Medium;
        }
    }

    private void OnWindowDestroying(AppWindow sender, object args)
    {
        IsClosed = true;
        _inputActivationListener.InputActivationChanged -= OnInputActivationChanged;
        _rootWindow.Destroying -= OnWindowDestroying;
        _rootWindow.Changed -= OnWindowChanged;
        SaveCurrentWindowStats();
        _inputActivationListener = default;
    }

    internal void UpdateXamlSourcePosition()
    {
        var size = _rootWindow.Presenter.Kind == AppWindowPresenterKind.FullScreen
            ? _rootWindow.Size
            : _rootWindow.ClientSize;
        _xamlSource?.SiteBridge?.MoveAndResize(new Windows.Graphics.RectInt32(
            0,
            0,
            size.Width,
            size.Height));
    }

    internal void SetTopMost(bool isTopMost)
    {
        // 全屏和小窗模式下不处理置顶.
        if (_rootWindow.Presenter.Kind is AppWindowPresenterKind.FullScreen or AppWindowPresenterKind.CompactOverlay)
        {
            return;
        }

        PInvoke.SetWindowPos(new(Handle), isTopMost ? new(-1) : new(-2), 0, 0, 0, 0, SET_WINDOW_POS_FLAGS.SWP_NOMOVE | SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_SHOWWINDOW);
    }

    internal bool IsTopMost()
    {
        var style = PInvoke.GetWindowLong(new(Handle), WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);
        return (style & 0x00000008) != 0; // WS_EX_TOPMOST
    }

    public void ResetFocus()
    {
        DispatcherQueue.GetForCurrentThread().TryEnqueue(DispatcherQueuePriority.Low, () =>
        {
            if (_xamlSource is null || _xamlSource.SiteBridge is null || _rootGrid is null)
            {
                return;
            }

            PInvoke.SetFocus(new(Handle));
            PInvoke.SetFocus(new(Win32Interop.GetWindowFromWindowId(_xamlSource.SiteBridge.WindowId)));
            if (_rootGrid.Children.Count > 0)
            {
                var element = _rootGrid.Children[0] as FrameworkElement;
                element?.Focus(FocusState.Programmatic);
            }
        });
    }

    private static PointInt32 GetSavedWindowPosition()
    {
        var left = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerWindowPositionLeft, 0);
        var top = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerWindowPositionTop, 0);
        return new PointInt32(left, top);
    }

    private RectInt32 GetRenderRect(DisplayArea area)
    {
        var workArea = area.WorkArea;
        var scaleFactor = HwndExtensions.GetDpiForWindow(Handle) / 96d;
        var previousWidth = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerWindowWidth, 1120d);
        var previousHeight = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerWindowHeight, 740d);
        var width = Convert.ToInt32(previousWidth * scaleFactor);
        var height = Convert.ToInt32(previousHeight * scaleFactor);
        var workAreaHeight = Convert.ToInt32(workArea.Height * scaleFactor);
        // Ensure the window is not larger than the work area.
        if (height > workAreaHeight - 20)
        {
            height = workAreaHeight - 20;
        }

        var lastPoint = GetSavedWindowPosition();
        var isZeroPoint = lastPoint.X == 0 && lastPoint.Y == 0;
        var isValidPosition = lastPoint.X >= workArea.X && lastPoint.Y >= workArea.Y && lastPoint.X + width <= workArea.X + workArea.Width && lastPoint.Y + height <= workArea.Y + workArea.Height;
        // 下面的 workArea.Width 和 workArea.Height 别动，不然初始窗口位置会出问题.
        var left = isZeroPoint || !isValidPosition
            ? (workArea.Width - width) / 2d
            : lastPoint.X;
        var top = isZeroPoint || !isValidPosition
            ? (workArea.Height - height) / 2d
            : lastPoint.Y;
        return new RectInt32(Convert.ToInt32(left), Convert.ToInt32(top), width, height);
    }

    private void MoveAndResize()
    {
        var lastPoint = GetSavedWindowPosition();
        var displayArea = DisplayArea.GetFromPoint(lastPoint, DisplayAreaFallback.Primary)
            ?? DisplayArea.Primary;
        var rect = GetRenderRect(displayArea);
        _rootWindow.MoveAndResize(rect);
        var isMaximized = SettingsToolkit.ReadLocalSetting(SettingNames.IsPlayerWindowMaximized, false);
        if (isMaximized)
        {
            (_rootWindow.Presenter as OverlappedPresenter)?.Maximize();
        }
    }

    private void SaveCurrentWindowStats()
    {
        var isMaximized = PInvoke.IsZoomed(new HWND(Handle));
        SettingsToolkit.WriteLocalSetting(SettingNames.IsPlayerWindowMaximized, (bool)isMaximized);

        if (!isMaximized && !_enteringFullScreen && !_enteringCompactOverlay && _rootWindow.Presenter.Kind == AppWindowPresenterKind.Overlapped)
        {
            var left = _rootWindow.Position.X;
            var top = _rootWindow.Position.Y;
            SettingsToolkit.WriteLocalSetting(SettingNames.PlayerWindowPositionLeft, left);
            SettingsToolkit.WriteLocalSetting(SettingNames.PlayerWindowPositionTop, top);
            var scaleFactor = _xamlSource.Content.XamlRoot.RasterizationScale;
            var actualWidth = _rootWindow.Size.Width / scaleFactor;
            var actualHeight = _rootWindow.Size.Height / scaleFactor;
            if (actualHeight >= WindowMinHeight && actualWidth >= WindowMinWidth)
            {
                SettingsToolkit.WriteLocalSetting(SettingNames.PlayerWindowHeight, actualHeight);
                SettingsToolkit.WriteLocalSetting(SettingNames.PlayerWindowWidth, actualWidth);
            }
        }
    }
}

