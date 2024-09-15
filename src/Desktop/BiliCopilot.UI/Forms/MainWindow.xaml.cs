// Copyright (c) Bili Copilot. All rights reserved.

using System.Diagnostics;
using System.Runtime.InteropServices;
using BiliCopilot.UI.Controls.Components;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Input;
using Richasy.WinUI.Share.Base;
using Windows.Graphics;
using Windows.System;
using Windows.UI.WindowManagement;
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
    public void EnterPlayerHostMode()
        => RootLayout.PrepareFullPlayerPresenter();

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
            GlobalHook.MouseSideButtonDown -= OnMouseSideButtonDown;
            GlobalHook.Stop();
        }
        else
        {
            GlobalHook.Start();
            GlobalHook.KeyDown += OnWindowKeyDown;
            GlobalHook.MouseSideButtonDown += OnMouseSideButtonDown;
        }
    }

    private void OnMouseSideButtonDown(object? sender, EventArgs e)
    {
        if (RootLayout.TryBackToDefaultIfPlayerHostMode())
        {
            return;
        }

        if (RootLayout.ViewModel.IsOverlayOpen)
        {
            RootLayout.ViewModel.Back();
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
        var isMaximized = SettingsToolkit.ReadLocalSetting(SettingNames.IsMainWindowMaximized, false);
        if (isMaximized)
        {
            (AppWindow.Presenter as OverlappedPresenter).Maximize();
        }

        var localTheme = SettingsToolkit.ReadLocalSetting(SettingNames.AppTheme, ElementTheme.Default);
        this.Get<AppViewModel>().ChangeThemeCommand.Execute(localTheme);
        _isFirstActivated = false;
    }

    private void OnClosed(object sender, WindowEventArgs e)
    {
        RootLayout.ViewModel.Back();

        foreach (var item in this.Get<AppViewModel>().Windows)
        {
            if (item is not MainWindow)
            {
                item.Close();
            }
        }

        var hideWhenClose = SettingsToolkit.ReadLocalSetting(SettingNames.HideWhenCloseWindow, false);
        if (!hideWhenClose)
        {
            Activated -= OnActivated;
            Closed -= OnClosed;

            GlobalDependencies.Kernel.GetRequiredService<AppViewModel>().Windows.Remove(this);
        }

        GlobalHook.Stop();
        SaveCurrentWindowStats();
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
    }

    private RectInt32 GetRenderRect(RectInt32 workArea)
    {
        var scaleFactor = this.GetDpiForWindow() / 96d;
        var previousWidth = SettingsToolkit.ReadLocalSetting(SettingNames.MainWindowWidth, 1120d);
        var previousHeight = SettingsToolkit.ReadLocalSetting(SettingNames.MainWindowHeight, 740d);
        var width = Convert.ToInt32(previousWidth * scaleFactor);
        var height = Convert.ToInt32(previousHeight * scaleFactor);

        // Ensure the window is not larger than the work area.
        if (height > workArea.Height - 20)
        {
            height = workArea.Height - 20;
        }

        var lastPoint = GetSavedWindowPosition();
        var isZeroPoint = lastPoint.X == 0 && lastPoint.Y == 0;
        var isValidPosition = lastPoint.X >= workArea.X && lastPoint.Y >= workArea.Y;
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
        var rect = GetRenderRect(displayArea.WorkArea);
        AppWindow.MoveAndResize(rect);
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
}

internal static class GlobalHook
{
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_XBUTTONCLK = 0x020B;

    private static readonly HOOKPROC _procKeyboard = HookKeyboardCallback;
    private static readonly HOOKPROC _procMouse = MouseHookCallback;
    private static UnhookWindowsHookExSafeHandle _keyboardHookID = new();
    private static UnhookWindowsHookExSafeHandle _mouseHookID = new();

    public static event EventHandler<PlayerKeyboardEventArgs> KeyDown;
    public static event EventHandler MouseSideButtonDown;

    public static void Start()
    {
        _keyboardHookID = SetHook(_procKeyboard, WINDOWS_HOOK_ID.WH_KEYBOARD_LL);
        _mouseHookID = SetHook(_procMouse, WINDOWS_HOOK_ID.WH_MOUSE_LL);
    }

    public static void Stop()
    {
        PInvoke.UnhookWindowsHookEx(new HHOOK(_keyboardHookID.DangerousGetHandle()));
        PInvoke.UnhookWindowsHookEx(new HHOOK(_mouseHookID.DangerousGetHandle()));
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

        return PInvoke.CallNextHookEx(new HHOOK(_keyboardHookID.DangerousGetHandle()), nCode, new WPARAM(unchecked((nuint)wParam)), lParam);
    }

    private static unsafe LRESULT MouseHookCallback(int nCode, WPARAM wParam, LPARAM lParam)
    {
        if (nCode >= 0 && wParam.Value == WM_XBUTTONCLK)
        {
            var mhookstruct = (MOUSEHOOKSTRUCT*)lParam.Value;
            MouseSideButtonDown?.Invoke(null, EventArgs.Empty);
            return new LRESULT(1);
        }

        return PInvoke.CallNextHookEx(new HHOOK(_mouseHookID.DangerousGetHandle()), nCode, new WPARAM(unchecked((nuint)wParam)), lParam);
    }
}

internal class PlayerKeyboardEventArgs
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
