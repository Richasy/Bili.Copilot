﻿// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Bili.Copilot.App.Pages;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Player;
using Bili.Copilot.Models.Data.Local;
using Bili.Copilot.Models.Data.Video;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.Graphics;
using Windows.System;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using WinUIEx;

namespace Bili.Copilot.App.Forms;

/// <summary>
/// 播放器窗口.
/// </summary>
public sealed partial class PlayerWindow : WindowBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerWindow"/> class.
    /// </summary>
    public PlayerWindow()
    {
        InitializeComponent();
        Activated += OnActivated;
        Closed += OnClosed;
        RestoreWindowState();
        MinWidth = 560;
        MinHeight = 320;
        AppWindow.Changed += OnAppWindowChanged;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerWindow"/> class.
    /// </summary>
    public PlayerWindow(PlaySnapshot snapshot)
        : this()
    {
        Title = snapshot.Title;
        var navArgs = new PlayerPageNavigateEventArgs
        {
            Snapshot = snapshot,
            AttachedWindow = this,
        };
        if (snapshot.VideoType == Models.Constants.Bili.VideoType.Video)
        {
            _ = MainFrame.Navigate(typeof(VideoPlayerPage), navArgs);
        }
        else if (snapshot.VideoType == Models.Constants.Bili.VideoType.Live)
        {
            _ = MainFrame.Navigate(typeof(LivePlayerPage), navArgs);
        }
        else if (snapshot.VideoType == Models.Constants.Bili.VideoType.Pgc)
        {
            _ = MainFrame.Navigate(typeof(PgcPlayerPage), navArgs);
        }

        TraceLogger.LogPlayerOpen(
            snapshot.VideoType.ToString(),
            SettingsToolkit.ReadLocalSetting(SettingNames.PreferCodec, PreferCodec.H264).ToString(),
            SettingsToolkit.ReadLocalSetting(SettingNames.PreferQuality, PreferQuality.HDFirst).ToString(),
            SettingsToolkit.ReadLocalSetting(SettingNames.DecodeType, DecodeType.HardwareDecode).ToString());
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerWindow"/> class.
    /// </summary>
    public PlayerWindow(List<VideoInformation> snapshots)
        : this()
    {
        Title = ResourceToolkit.GetLocalizedString(StringNames.Playlist);
        var navArgs = new PlayerPageNavigateEventArgs
        {
            Playlist = snapshots,
            AttachedWindow = this,
        };

        _ = MainFrame.Navigate(typeof(VideoPlayerPage), navArgs);
    }

    private void OnClosed(object sender, WindowEventArgs args)
    {
        // 保存当前窗口大小和位置.
        if (AppWindow.Presenter.Kind == AppWindowPresenterKind.Overlapped)
        {
            var left = AppWindow.Position.X;
            var top = AppWindow.Position.Y;
            SettingsToolkit.WriteLocalSetting(SettingNames.PlayerWindowWidth, Width);
            SettingsToolkit.WriteLocalSetting(SettingNames.PlayerWindowHeight, Height);
            SettingsToolkit.WriteLocalSetting(SettingNames.PlayerWindowLeft, left);
            SettingsToolkit.WriteLocalSetting(SettingNames.PlayerWindowTop, top);
        }

        MainFrame.Navigate(typeof(Page));
        MainWindow.Instance.Activate();
        GC.Collect();
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
        }
    }

    private void RestoreWindowState()
    {
        var width = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerWindowWidth, 1280d);
        var height = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerWindowHeight, 720d);

        var left = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerWindowLeft, 0);
        var top = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerWindowTop, 0);
        var area = DisplayArea.GetFromPoint(new PointInt32(Convert.ToInt32(left), Convert.ToInt32(top)), DisplayAreaFallback.Nearest);
        var isValidPosition = left > 0 && top > 0 && left + Width < area.WorkArea.Width && top + height < area.WorkArea.Height;
        if (!isValidPosition)
        {
            Width = width;
            Height = height;
            this.CenterOnScreen();
        }
        else
        {
            this.MoveAndResize(left, top, width, height);
        }
    }

    internal class KeyboardHook
    {
        private const int WM_KEYDOWN = 0x0100;

        private static readonly HOOKPROC _proc = HookCallback;
        private static UnhookWindowsHookExSafeHandle _hookID = new UnhookWindowsHookExSafeHandle();

        public static event EventHandler<PlayerKeyboardEventArgs> KeyDown;

        public static void Start() => _hookID = SetHook(_proc);

        public static void Stop() => PInvoke.UnhookWindowsHookEx(new HHOOK(_hookID.DangerousGetHandle()));

        private static UnhookWindowsHookExSafeHandle SetHook(HOOKPROC proc)
        {
            using (var curProcess = Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                return PInvoke.SetWindowsHookEx(WINDOWS_HOOK_ID.WH_KEYBOARD_LL, proc, Windows.Win32.PInvoke.GetModuleHandle(curModule.ModuleName), 0);
            }
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
