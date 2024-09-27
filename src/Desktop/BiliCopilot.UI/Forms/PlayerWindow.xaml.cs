// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Controls.Components;
using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Input;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUI.Share.Base;
using WebDav;
using Windows.Graphics;
using Windows.System;
using WinUIEx;

namespace BiliCopilot.UI.Forms;

/// <summary>
/// 播放器窗口.
/// </summary>
public sealed partial class PlayerWindow : WindowBase, IPlayerHostWindow, ITipWindow
{
    private const int WindowMinWidth = 640;
    private const int WindowMinHeight = 480;
    private readonly InputActivationListener _inputActivationListener;
    private bool _isFirstActivated = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerWindow"/> class.
    /// </summary>
    public PlayerWindow()
    {
        InitializeComponent();
        SetTitleBar(TitleBar);
        Title = ResourceToolkit.GetLocalizedString(StringNames.AppName);
        this.SetIcon("Assets/logo.ico");
        AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Standard;
        MinWidth = WindowMinWidth;
        MinHeight = WindowMinHeight;
        this.Get<AppViewModel>().Windows.Add(this);
        _inputActivationListener = InputActivationListener.GetForWindowId(AppWindow.Id);
        _inputActivationListener.InputActivationChanged += OnInputActivationChanged;
        Activated += OnActivated;
        Closed += OnClosed;
    }

    /// <summary>
    /// 打开视频.
    /// </summary>
    public void OpenVideo(VideoSnapshot snapshot)
    {
        Activate();
        var preferPlayer = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerType, PlayerType.Native);
        if (preferPlayer == PlayerType.Web)
        {
            MainFrame.Navigate(typeof(WebPlayerPage), $"https://www.bilibili.com/video/av{snapshot.Video.Identifier.Id}");
            return;
        }

        MainFrame.Navigate(typeof(VideoPlayerPage), snapshot);
    }

    /// <summary>
    /// 打开PGC内容.
    /// </summary>
    public void OpenPgc(MediaIdentifier pgc)
    {
        Activate();
        var preferPlayer = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerType, PlayerType.Native);
        if (preferPlayer == PlayerType.Web)
        {
            var url = $"https://www.bilibili.com/bangumi/play/{pgc.Id.Replace("_", string.Empty)}";
            MainFrame.Navigate(typeof(WebPlayerPage), url);
            return;
        }

        MainFrame.Navigate(typeof(PgcPlayerPage), pgc);
    }

    /// <summary>
    /// 打开直播内容.
    /// </summary>
    public void OpenLive(MediaIdentifier room)
    {
        Activate();
        var preferPlayer = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerType, PlayerType.Native);
        if (preferPlayer == PlayerType.Web)
        {
            MainFrame.Navigate(typeof(WebPlayerPage), $"https://live.bilibili.com/{room.Id}");
            return;
        }

        MainFrame.Navigate(typeof(LivePlayerPage), room);
    }

    /// <summary>
    /// 打开 WebDAV 内容.
    /// </summary>
    public void OpenWebDav(WebDavResource resource, IList<WebDavResource> playlist)
    {
        Activate();
        MainFrame.Navigate(typeof(WebDavPlayerPage), (playlist, resource));
    }

    /// <inheritdoc/>
    public void EnterPlayerHostMode(PlayerDisplayMode mode)
    {
        TitleBar.Visibility = mode == PlayerDisplayMode.FullScreen ? Visibility.Collapsed : Visibility.Visible;
        if (MainFrame.Content is VideoPlayerPage vPage)
        {
            TitleBar.Title = vPage.ViewModel.Title;
            vPage.EnterPlayerHostMode();
        }
        else if (MainFrame.Content is PgcPlayerPage pPage)
        {
            TitleBar.Title = pPage.ViewModel.EpisodeTitle;
            pPage.EnterPlayerHostMode();
        }
        else if (MainFrame.Content is LivePlayerPage lPage)
        {
            TitleBar.Title = lPage.ViewModel.Title;
            lPage.EnterPlayerHostMode();
        }
        else if (MainFrame.Content is WebDavPlayerPage wPage)
        {
            TitleBar.Title = wPage.ViewModel.Title;
            wPage.EnterPlayerHostMode();
        }
    }

    /// <inheritdoc/>
    public void ExitPlayerHostMode()
    {
        TitleBar.Visibility = Visibility.Visible;
        TitleBar.Title = ResourceToolkit.GetLocalizedString(StringNames.AppName);
        if (MainFrame.Content is VideoPlayerPage vPage)
        {
            vPage.ExitPlayerHostMode();
        }
        else if (MainFrame.Content is PgcPlayerPage pPage)
        {
            pPage.ExitPlayerHostMode();
        }
        else if (MainFrame.Content is LivePlayerPage lPage)
        {
            lPage.ExitPlayerHostMode();
        }
        else if (MainFrame.Content is WebDavPlayerPage wPage)
        {
            wPage.ExitPlayerHostMode();
        }
    }

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
        var left = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerWindowPositionLeft, 0);
        var top = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerWindowPositionTop, 0);
        return new PointInt32(left, top);
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
        var isMaximized = SettingsToolkit.ReadLocalSetting(SettingNames.IsPlayerWindowMaximized, false);
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
        ClosePlayer();

        Activated -= OnActivated;
        Closed -= OnClosed;

        GlobalDependencies.Kernel.GetRequiredService<AppViewModel>().Windows.Remove(this);
        GlobalHook.Stop();
        SaveCurrentWindowStats();
    }

    private void OnInputActivationChanged(InputActivationListener sender, InputActivationListenerActivationChangedEventArgs args)
    {
        var isDeactivated = sender.State == InputActivationState.Deactivated;
        if (isDeactivated)
        {
            GlobalHook.KeyDown -= OnWindowKeyDown;
            GlobalHook.Stop();
        }
        else
        {
            GlobalHook.Start();
            GlobalHook.KeyDown += OnWindowKeyDown;
        }
    }

    private bool TryBackToDefaultMode()
    {
        if (MainFrame.Content is VideoPlayerPage vPage)
        {
            vPage.ViewModel.Player.BackToDefaultModeCommand.Execute(default);
            return true;
        }
        else if (MainFrame.Content is PgcPlayerPage pPage)
        {
            pPage.ViewModel.Player.BackToDefaultModeCommand.Execute(default);
            return true;
        }
        else if (MainFrame.Content is LivePlayerPage lPage)
        {
            lPage.ViewModel.Player.BackToDefaultModeCommand.Execute(default);
            return true;
        }
        else if (MainFrame.Content is WebDavPlayerPage wPage)
        {
            wPage.ViewModel.Player.BackToDefaultModeCommand.Execute(default);
            return true;
        }

        return false;
    }

    private bool TryTogglePlayPauseIfInPlayer()
    {
        if (MainFrame.Content is VideoPlayerPage vPage)
        {
            vPage.ViewModel.Player.TogglePlayPauseCommand.Execute(default);
            return true;
        }
        else if (MainFrame.Content is PgcPlayerPage pPage)
        {
            pPage.ViewModel.Player.TogglePlayPauseCommand.Execute(default);
            return true;
        }
        else if (MainFrame.Content is LivePlayerPage lPage)
        {
            lPage.ViewModel.Player.TogglePlayPauseCommand.Execute(default);
            return true;
        }
        else if (MainFrame.Content is WebDavPlayerPage wPage)
        {
            wPage.ViewModel.Player.TogglePlayPauseCommand.Execute(default);
            return true;
        }

        return false;
    }

    private void ClosePlayer()
    {
        if (MainFrame.Content is VideoPlayerPage vPage)
        {
            vPage.ViewModel.CleanCommand.Execute(default);
        }
        else if (MainFrame.Content is PgcPlayerPage pPage)
        {
            pPage.ViewModel.CleanCommand.Execute(default);
        }
        else if (MainFrame.Content is LivePlayerPage lPage)
        {
            lPage.ViewModel.CleanCommand.Execute(default);
        }
        else if (MainFrame.Content is WebDavPlayerPage wPage)
        {
            wPage.ViewModel.CleanCommand.Execute(default);
        }
    }

    private void OnWindowKeyDown(object? sender, PlayerKeyboardEventArgs e)
    {
        if (e.Key == VirtualKey.Space || e.Key == VirtualKey.Pause)
        {
            if (e.Key == VirtualKey.Space)
            {
                var focusEle = FocusManager.GetFocusedElement(MainFrame.XamlRoot);
                if (focusEle is TextBox)
                {
                    return;
                }
            }

            e.Handled = TryTogglePlayPauseIfInPlayer();
        }
    }

    private RectInt32 GetRenderRect(RectInt32 workArea)
    {
        var scaleFactor = this.GetDpiForWindow() / 96d;
        var previousWidth = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerWindowWidth, 1120d);
        var previousHeight = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerWindowHeight, 740d);
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
        SettingsToolkit.WriteLocalSetting(SettingNames.IsPlayerWindowMaximized, (bool)isMaximized);

        if (!isMaximized)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.PlayerWindowPositionLeft, left);
            SettingsToolkit.WriteLocalSetting(SettingNames.PlayerWindowPositionTop, top);

            if (Height >= WindowMinHeight && Width >= WindowMinWidth)
            {
                SettingsToolkit.WriteLocalSetting(SettingNames.PlayerWindowHeight, Height);
                SettingsToolkit.WriteLocalSetting(SettingNames.PlayerWindowWidth, Width);
            }
        }
    }

    private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        var point = e.GetCurrentPoint((UIElement)sender);
        if (point.Properties.IsXButton1Pressed || point.Properties.IsXButton2Pressed)
        {
            e.Handled = true;
            TryBackToDefaultMode();
        }
    }
}
