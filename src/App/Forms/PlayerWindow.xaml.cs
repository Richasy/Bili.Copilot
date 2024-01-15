// Copyright (c) Bili Copilot. All rights reserved.

using System.Diagnostics;
using System.Runtime.InteropServices;
using Bili.Copilot.App.Pages;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Local;
using Bili.Copilot.Models.Data.User;
using Bili.Copilot.Models.Data.Video;
using Bili.Copilot.ViewModels;
using Bili.Copilot.ViewModels.Items;
using Microsoft.UI.Windowing;
using Windows.Graphics;
using Windows.System;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Bili.Copilot.App.Forms;

/// <summary>
/// 播放器窗口.
/// </summary>
public sealed partial class PlayerWindow : WindowBase, ITipWindow, IUserSpaceWindow
{
    private bool _isInitialized;
    private bool _isClosed;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerWindow"/> class.
    /// </summary>
    public PlayerWindow()
    {
        InitializeComponent();
        AppWindow.Changed += OnAppWindowChanged;
        MinWidth = 560;
        MinHeight = 320;
        AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Standard;

        Activated += OnActivated;
        Closed += OnClosedAsync;

        MoveAndResize();
    }

    /// <inheritdoc/>
    public async Task ShowTipAsync(UIElement element, double delaySeconds)
    {
        TipContainer.Visibility = Visibility.Visible;
        TipContainer.Children.Add(element);
        element.Visibility = Visibility.Visible;
        await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
        element.Visibility = Visibility.Collapsed;
        _ = TipContainer.Children.Remove(element);
        if (TipContainer.Children.Count == 0)
        {
            TipContainer.Visibility = Visibility.Collapsed;
        }
    }

    /// <inheritdoc/>
    public void ShowUserSpace(UserProfile profile)
    {
        Activate();
        MainSplitView.IsPaneOpen = true;
        var userVM = new UserSpaceViewModel();
        userVM.SetUserProfile(profile);
        _ = SplitFrame.Navigate(typeof(UserSpacePage), userVM);
    }

    /// <summary>
    /// 设置播放数据.
    /// </summary>
    /// <param name="snapshot">播放快照.</param>
    public void SetData(PlaySnapshot snapshot)
    {
        Title = snapshot.Title;
        PlayerUtils.InitializePlayer(snapshot, MainFrame, this);
    }

    /// <summary>
    /// 设置播放列表.
    /// </summary>
    /// <param name="snapshots">播放列表.</param>
    public void SetData(List<VideoInformation> snapshots)
    {
        Title = ResourceToolkit.GetLocalizedString(StringNames.Playlist);
        PlayerUtils.InitializePlayer(snapshots, MainFrame, this);
    }

    /// <summary>
    /// 设置播放列表.
    /// </summary>
    /// <param name="items">WebDAV 条目列表.</param>
    public void SetData(List<WebDavStorageItemViewModel> items, string title)
    {
        Title = title;
        PlayerUtils.InitializePlayer(items, MainFrame, this);
    }

    private static PointInt32 GetSavedWindowPosition()
    {
        var left = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerWindowPositionLeft, 0);
        var top = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerWindowPositionTop, 0);
        return new PointInt32(left, top);
    }

    private async void OnClosedAsync(object sender, WindowEventArgs args)
    {
        if (!_isClosed)
        {
            args.Handled = true;
            SaveCurrentWindowStats();
            this.Hide();
            _ = MainFrame.Navigate(typeof(Page));
            AppViewModel.Instance.ActivateMainWindow();
            _isClosed = true;
            await Task.Delay(1000);
            Close();
        }
    }

    private void OnAppWindowChanged(AppWindow sender, AppWindowChangedEventArgs args)
    {
        if (args.DidPresenterChange)
        {
            var kind = sender.Presenter.Kind;
            TraceLogger.LogPlayerDisplayModeChanged(kind.ToString());
        }
    }

    private void OnActivated(object sender, WindowActivatedEventArgs args)
    {
        KeyboardHook.KeyDown -= OnWindowKeyDown;
        KeyboardHook.Stop();
        if (args.WindowActivationState != WindowActivationState.Deactivated)
        {
            KeyboardHook.Start();
            KeyboardHook.KeyDown += OnWindowKeyDown;
        }

        if (!_isInitialized)
        {
            var isMaximized = SettingsToolkit.ReadLocalSetting(SettingNames.IsPlayerWindowMaximized, false);
            if (isMaximized)
            {
                (AppWindow.Presenter as OverlappedPresenter).Maximize();
            }

            var localTheme = SettingsToolkit.ReadLocalSetting(SettingNames.AppTheme, ElementTheme.Default);
            AppViewModel.Instance.ChangeTheme(localTheme);
            _isInitialized = true;
        }
    }

    private void OnWindowKeyDown(object sender, PlayerKeyboardEventArgs e)
    {
        if (e.Key == VirtualKey.Space)
        {
            var focusEle = FocusManager.GetFocusedElement(MainFrame.XamlRoot);
            if (focusEle is TextBox)
            {
                return;
            }

            e.Handled = true;
            if (MainFrame.Content is VideoPlayerPageBase page)
            {
                page.ViewModel.PlayerDetail.PlayPauseCommand.Execute(default);
            }
            else if (MainFrame.Content is LivePlayerPage livePage)
            {
                livePage.ViewModel.PlayerDetail.PlayPauseCommand.Execute(default);
            }
            else if (MainFrame.Content is PgcPlayerPage pgcPage)
            {
                pgcPage.ViewModel.PlayerDetail.PlayPauseCommand.Execute(default);
            }
            else if (MainFrame.Content is WebDavPlayerPage webDavPage)
            {
                webDavPage.ViewModel.PlayerDetail.PlayPauseCommand.Execute(default);
            }
        }
    }

    private RectInt32 GetRenderRect(RectInt32 workArea)
    {
        var scaleFactor = this.GetDpiForWindow() / 96d;
        var previousWidth = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerWindowWidth, 1000d);
        var previousHeight = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerWindowHeight, 700d);
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
        var areas = DisplayArea.FindAll();
        var workArea = default(RectInt32);
        for (var i = 0; i < areas.Count; i++)
        {
            var area = areas[i];
            if (area.WorkArea.X < lastPoint.X && area.WorkArea.X + area.WorkArea.Width > lastPoint.X)
            {
                workArea = area.WorkArea;
                break;
            }
        }

        if (workArea == default)
        {
            workArea = DisplayArea.Primary.WorkArea;
        }

        var rect = GetRenderRect(workArea);
        AppWindow.MoveAndResize(rect);
    }

    private void SaveCurrentWindowStats()
    {
        var left = AppWindow.Position.X;
        var top = AppWindow.Position.Y;
        var isMaximized = Windows.Win32.PInvoke.IsZoomed(new HWND(this.GetWindowHandle()));
        SettingsToolkit.WriteLocalSetting(SettingNames.IsPlayerWindowMaximized, (bool)isMaximized);

        if (!isMaximized)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.PlayerWindowHeight, Height);
            SettingsToolkit.WriteLocalSetting(SettingNames.PlayerWindowWidth, Width);
        }

        SettingsToolkit.WriteLocalSetting(SettingNames.PlayerWindowPositionLeft, left);
        SettingsToolkit.WriteLocalSetting(SettingNames.PlayerWindowPositionTop, top);
    }

    internal class KeyboardHook
    {
        private const int WM_KEYDOWN = 0x0100;

        private static readonly HOOKPROC _proc = HookCallback;
        private static UnhookWindowsHookExSafeHandle _hookID = new();

        public static event EventHandler<PlayerKeyboardEventArgs> KeyDown;

        public static void Start() => _hookID = SetHook(_proc);

        public static void Stop() => PInvoke.UnhookWindowsHookEx(new HHOOK(_hookID.DangerousGetHandle()));

        private static UnhookWindowsHookExSafeHandle SetHook(HOOKPROC proc)
        {
            using var curProcess = Process.GetCurrentProcess();
            using var curModule = curProcess.MainModule;
            return PInvoke.SetWindowsHookEx(WINDOWS_HOOK_ID.WH_KEYBOARD_LL, proc, Windows.Win32.PInvoke.GetModuleHandle(curModule.ModuleName), 0);
        }

        private static LRESULT HookCallback(int nCode, WPARAM wParam, LPARAM lParam)
        {
            if (nCode >= 0 && wParam.Value == WM_KEYDOWN)
            {
                var vkCode = Marshal.ReadInt32(lParam);
                var args = new PlayerKeyboardEventArgs(vkCode);
                KeyDown?.Invoke(null, args);
                if (args.Handled)
                {
                    return new LRESULT(1);
                }
            }

            return PInvoke.CallNextHookEx(new HHOOK(_hookID.DangerousGetHandle()), nCode, new WPARAM(unchecked((nuint)wParam)), lParam);
        }
    }

    internal class PlayerKeyboardEventArgs
    {
        public PlayerKeyboardEventArgs(int keyCode) => Key = (VirtualKey)keyCode;

        internal VirtualKey Key { get; }

        internal bool Handled { get; set; }
    }
}
