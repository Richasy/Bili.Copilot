// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.View;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Input;
using Richasy.WinUIKernel.Share.Base;
using Richasy.WinUIKernel.Share.Toolkits;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.Graphics;
using Windows.System;
using Windows.Win32.UI.WindowsAndMessaging;
using WinUIEx;

namespace BiliCopilot.UI.Forms;

/// <summary>
/// 主窗口.
/// </summary>
public sealed partial class MainWindow : WindowBase, IPlayerHostWindow, ITipWindow
{
    private const int WindowMinWidth = 640;
    private const int WindowMinHeight = 480;
    private readonly InputActivationListener _inputActivationListener;
    private bool _isFirstActivated = true;
    private bool _shouldExit;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        SetTitleBar(RootLayout.GetMainTitleBar());
        Title = ResourceToolkit.GetLocalizedString(StringNames.AppName);
        this.SetIcon("Assets/logo.ico");
        AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
        MinWidth = WindowMinWidth;
        MinHeight = WindowMinHeight;
        this.Get<AppViewModel>().Windows.Add(this);
        _inputActivationListener = InputActivationListener.GetForWindowId(AppWindow.Id);
        _inputActivationListener.InputActivationChanged += OnInputActivationChanged;
        Activated += OnActivated;
        Closed += OnClosed;
    }

    /// <inheritdoc/>
    public void EnterPlayerHostMode(PlayerDisplayMode mode)
        => RootLayout.PrepareFullPlayerPresenter(mode);

    /// <inheritdoc/>
    public void ExitPlayerHostMode()
        => RootLayout.ExitFullPlayerPresenter();

    /// <inheritdoc/>
    public async Task ShowTipAsync(string text, InfoType type = InfoType.Error)
    {
        var popup = new TipPopup() { Text = text };
        TipContainer.Visibility = Visibility.Visible;
        TipContainer.Children.Add(popup);
        await popup.ShowAsync(type);
        TipContainer.Children.Remove(popup);
        TipContainer.Visibility = Visibility.Collapsed;
    }

    private static PointInt32 GetSavedWindowPosition()
    {
        var left = SettingsToolkit.ReadLocalSetting(SettingNames.MainWindowPositionLeft, 0);
        var top = SettingsToolkit.ReadLocalSetting(SettingNames.MainWindowPositionTop, 0);
        return new PointInt32(left, top);
    }

    private void OnInputActivationChanged(InputActivationListener sender, InputActivationListenerActivationChangedEventArgs args)
    {
        var isDeactivated = sender.State == InputActivationState.Deactivated;
        if (isDeactivated)
        {
            GlobalHook.KeyDown -= OnWindowKeyDown;
            GlobalHook.KeyUp -= OnWindowKeyUp;
            GlobalHook.Stop();
        }
        else
        {
            GlobalHook.Start();
            GlobalHook.KeyDown += OnWindowKeyDown;
            GlobalHook.KeyUp += OnWindowKeyUp;
        }
    }

    private void OnActivated(object sender, WindowActivatedEventArgs args)
    {
        if (args.WindowActivationState != WindowActivationState.Deactivated)
        {
            this.Get<AppViewModel>().ActivatedWindow = this;
        }

        if (!_isFirstActivated)
        {
            return;
        }

        MoveAndResize();
        this.Get<ILogger<App>>().LogInformation($"App version: {this.Get<IAppToolkit>().GetPackageVersion()}");
        var localTheme = SettingsToolkit.ReadLocalSetting(SettingNames.AppTheme, ElementTheme.Default);
        this.Get<AppViewModel>().ChangeThemeCommand.Execute(localTheme);
        AppWindow.Changed += OnWindowChanged;
        _isFirstActivated = false;
    }

    private async void OnClosed(object sender, WindowEventArgs e)
    {
        if (!_shouldExit)
        {
            e.Handled = true;
            this.Get<AppViewModel>().IsClosed = true;
            RootLayout.ViewModel.Back();
            foreach (var item in this.Get<AppViewModel>().Windows)
            {
                if (item is not MainWindow)
                {
                    item.Close();
                }
            }

            _shouldExit = true;
            this.Hide();

            var hideWhenClose = SettingsToolkit.ReadLocalSetting(SettingNames.HideWhenCloseWindow, false);
            if (!hideWhenClose)
            {
                Activated -= OnActivated;
                Closed -= OnClosed;

                GlobalDependencies.Kernel.GetRequiredService<AppViewModel>().Windows.Remove(this);
                await this.Get<SettingsPageViewModel>().CheckSaveServicesAsync();
            }

            GlobalHook.Stop();
            SaveCurrentWindowStats();
            App.Current?.Exit();
            Environment.Exit(0);
        }
    }

    private void OnWindowKeyDown(object? sender, PlayerKeyboardEventArgs e)
    {
        if (e.Key == VirtualKey.Space || e.Key == VirtualKey.Pause)
        {
            if (e.Key == VirtualKey.Space)
            {
                var focusEle = FocusManager.GetFocusedElement(RootLayout.XamlRoot);
                if (focusEle is TextBox)
                {
                    return;
                }
            }

            e.Handled = RootLayout.TryTogglePlayPauseIfInPlayer();
        }
        else if (e.Key == VirtualKey.Right)
        {
            var focusEle = FocusManager.GetFocusedElement(RootLayout.XamlRoot);
            if (focusEle is TextBox)
            {
                return;
            }

            e.Handled = RootLayout.TryMarkRightArrowPressedTime();
        }
    }

    private void OnWindowKeyUp(object? sender, PlayerKeyboardEventArgs e)
    {
        if (e.Key == VirtualKey.Right)
        {
            var focusEle = FocusManager.GetFocusedElement(RootLayout.XamlRoot);
            if (focusEle is TextBox)
            {
                return;
            }

            e.Handled = RootLayout.TryCancelRightArrow();
        }
    }

    private RectInt32 GetRenderRect(DisplayArea area)
    {
        var workArea = area.WorkArea;
        var scaleFactor = HwndExtensions.GetDpiForWindow(this.GetWindowHandle()) / 96d;
        var previousWidth = SettingsToolkit.ReadLocalSetting(SettingNames.MainWindowWidth, 1120d);
        var previousHeight = SettingsToolkit.ReadLocalSetting(SettingNames.MainWindowHeight, 740d);
        var width = Convert.ToInt32(previousWidth * scaleFactor);
        var height = Convert.ToInt32(previousHeight * scaleFactor);

        // 如果宽高比为 16 : 9，则计算出窗口实际宽度。
        double maxPageWidth;
        if (area.OuterBounds.Width * 9 == area.OuterBounds.Height * 16)
        {
            var maxWindowWidth = area.OuterBounds.Width / scaleFactor;
            maxPageWidth = maxWindowWidth - 74;
        }
        else
        {
            // 取当前高度的 16 : 9 比例宽度。
            var maxWindowWidth = area.OuterBounds.Height * 16 / 9 / scaleFactor;
            maxPageWidth = maxWindowWidth - 74;
        }

        App.Current.Resources["MaxPageWidth"] = maxPageWidth;

        // Ensure the window is not larger than the work area.
        if (height > workArea.Height - 20)
        {
            height = workArea.Height - 20;
        }

        var lastPoint = GetSavedWindowPosition();
        var isZeroPoint = lastPoint.X == 0 && lastPoint.Y == 0;
        var isValidPosition = lastPoint.X >= workArea.X && lastPoint.Y >= workArea.Y && lastPoint.X + width <= workArea.X + workArea.Width && lastPoint.Y + height <= workArea.Y + workArea.Height;
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
        AppWindow.MoveAndResize(rect);
        var isMaximized = SettingsToolkit.ReadLocalSetting(SettingNames.IsMainWindowMaximized, false);
        if (isMaximized)
        {
            (AppWindow.Presenter as OverlappedPresenter)?.Maximize();
        }
    }

    private void SaveCurrentWindowStats()
    {
        var left = AppWindow.Position.X;
        var top = AppWindow.Position.Y;
        var isMaximized = PInvoke.IsZoomed(new HWND(this.GetWindowHandle()));
        SettingsToolkit.WriteLocalSetting(SettingNames.IsMainWindowMaximized, (bool)isMaximized);

        if (!isMaximized)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.MainWindowPositionLeft, left);
            SettingsToolkit.WriteLocalSetting(SettingNames.MainWindowPositionTop, top);

            if (Height >= WindowMinHeight && Width >= WindowMinWidth)
            {
                SettingsToolkit.WriteLocalSetting(SettingNames.MainWindowHeight, Height);
                SettingsToolkit.WriteLocalSetting(SettingNames.MainWindowWidth, Width);
            }
        }
    }

    private void OnWindowChanged(AppWindow sender, AppWindowChangedEventArgs args)
    {
        if (args.DidSizeChange || args.DidPositionChange)
        {
            SaveCurrentWindowStats();
        }

        if (args.DidVisibilityChange && !sender.IsVisible)
        {
            // this.Get<AppViewModel>().RestoreOriginalWheelScrollCommand.Execute(default);
        }
    }

    private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        var point = e.GetCurrentPoint((UIElement)sender);
        if (point.Properties.IsXButton1Pressed || point.Properties.IsXButton2Pressed)
        {
            e.Handled = true;
            if (RootLayout.TryBackToDefaultIfPlayerHostMode())
            {
                return;
            }

            if (RootLayout.ViewModel.IsOverlayOpen)
            {
                RootLayout.ViewModel.Back();
            }
        }
    }
}

internal static class GlobalHook
{
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;

    private static readonly HOOKPROC _procKeyboard = HookKeyboardCallback;
    private static UnhookWindowsHookExSafeHandle _keyboardHookID = new();
    public static event EventHandler<PlayerKeyboardEventArgs> KeyDown;
    public static event EventHandler<PlayerKeyboardEventArgs> KeyUp;

    public static void Start()
    {
        _keyboardHookID = SetHook(_procKeyboard, WINDOWS_HOOK_ID.WH_KEYBOARD_LL);
    }

    public static void Stop()
    {
        PInvoke.UnhookWindowsHookEx(new HHOOK(_keyboardHookID.DangerousGetHandle()));
    }

    private static UnhookWindowsHookExSafeHandle SetHook(HOOKPROC proc, WINDOWS_HOOK_ID hookId)
    {
        using var curProcess = Process.GetCurrentProcess();
        using var curModule = curProcess.MainModule;
        return PInvoke.SetWindowsHookEx(hookId, proc, PInvoke.GetModuleHandle(curModule.ModuleName), 0);
    }

    private static LRESULT HookKeyboardCallback(int nCode, WPARAM wParam, LPARAM lParam)
    {
        if (nCode >= 0 && wParam.Value == WM_KEYDOWN)
        {
            var vkCode = Marshal.ReadInt32(lParam);
            var isCtrlPressed = (PInvoke.GetKeyState((int)VirtualKey.Control) & 0x8000) != 0;
            var args = new PlayerKeyboardEventArgs(vkCode, isCtrlPressed);
            KeyDown?.Invoke(null, args);
            if (args.Handled)
            {
                return new LRESULT(1);
            }
        }
        else if (nCode >= 0 && wParam.Value == WM_KEYUP)
        {
            var vkCode = Marshal.ReadInt32(lParam);
            var isCtrlPressed = (PInvoke.GetKeyState((int)VirtualKey.Control) & 0x8000) != 0;
            var args = new PlayerKeyboardEventArgs(vkCode, isCtrlPressed);
            KeyUp?.Invoke(null, args);
            if (args.Handled)
            {
                return new LRESULT(1);
            }
        }

        return PInvoke.CallNextHookEx(HHOOK.Null, nCode, wParam, lParam);
    }
}

internal sealed class PlayerKeyboardEventArgs
{
    public PlayerKeyboardEventArgs(int keyCode, bool isControlPressed)
    {
        Key = (VirtualKey)keyCode;
        IsControlPressed = isControlPressed;
    }

    internal VirtualKey Key { get; }

    internal bool IsControlPressed { get; }

    internal bool Handled { get; set; }
}
