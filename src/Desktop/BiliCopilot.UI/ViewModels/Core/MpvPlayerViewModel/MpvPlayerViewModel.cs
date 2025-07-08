// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Forms;
using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Input;
using Richasy.WinUIKernel.Share.ViewModels;
using WinUIEx;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// Mpv播放器视图模型.
/// </summary>
public sealed partial class MpvPlayerViewModel(DispatcherQueue queue, ILogger<MpvPlayerViewModel> logger) : ViewModelBase, IAsyncDisposable
{
    public async ValueTask DisposeAsync()
    {
        if (Client?.IsDisposed == false)
        {
            Client.ErrorOccurred -= OnClientErrorOccurred;
            Client.CacheStateChanged -= OnClientCacheStateChanged;
            await Client.DisposeAsync();
        }

        if (Player is not null)
        {
            Player.PropertyChanged -= OnPlayerPropertyChanged;
            await Player.DisposeAsync();
        }

        if (Window?.IsDisposed == false)
        {
            Window.GetWindow().Destroying -= OnWindowDestroying;
            await Window.DisposeAsync();
        }

        this.Get<AppViewModel>().Players.Remove(this);
    }

    [RelayCommand]
    public async Task InitializeAsync(MediaSnapshot snapshot)
    {
        _snapshot = snapshot;
        ErrorMessage = default;
        CacheSpeedText = default;
        ShowPlayerWindow();
        IsConnecting = true;
        IsFormatSelectable = false;
        IsAudioSelectable = false;
        IsSubtitleEmpty = true;
        _isFirstLoadTrackUpdated = true;
        UseIntegrationOperation = SettingsToolkit.ReadLocalSetting(SettingNames.HideMainWindowOnPlay, false)
            && SettingsToolkit.ReadLocalSetting(SettingNames.UseIntegrationWhenSinglePlayWindow, true);
        BackwardTip = string.Format(ResourceToolkit.GetLocalizedString(StringNames.BackwardTemplate), SettingsToolkit.ReadLocalSetting(SettingNames.StepBackwardSecond, 10d));
        ForwardTip = string.Format(ResourceToolkit.GetLocalizedString(StringNames.ForwardTemplate), SettingsToolkit.ReadLocalSetting(SettingNames.StepForwardSecond, 30d));
        await InitializeInternalAsync();
        _sourceResolver = snapshot.Type switch
        {
            _ => throw new NotSupportedException("Unsupported media type for MpvPlayerViewModel."),
        };
        _sourceResolver.WindowHandle = Window!.Handle;

        if (_historyResolver != null && Player != null)
        {
            await _historyResolver.SaveHistoryAsync(Player.Position, Player.Duration, true);
        }

        _historyResolver = snapshot.Type switch
        {
            _ => null
        };

        if (Window.IsClosed)
        {
            return;
        }

        if (Player is null)
        {
            Player = new Richasy.MpvKernel.Player.MpvPlayer(Client!, _sourceResolver, _historyResolver, logger: logger);
            Player.PropertyChanged += OnPlayerPropertyChanged;
        }
        else
        {
            Player.UpdateResolvers(_sourceResolver, _historyResolver);
        }

        await Player.InitializeAsync();
        if (Window.IsClosed)
        {
            return;
        }

        await ResetPlayerPresenterModeAsync();
        if (Window.IsClosed)
        {
            return;
        }

        CheckFullScreen();
        CheckCompactOverlay();
        ProgressChanged?.Invoke(this, 0);
        await LoadOnlineInformationAsync();
        if (Window.IsClosed)
        {
            return;
        }

        //if (Connector != null)
        //{
        //    await Connector.InitializeAsync(_snapshot, _sourceResolver);
        //    if (Window.IsClosed)
        //    {
        //        return;
        //    }
        //}

        IsConnecting = false;
    }

    public void UpdateTheme(ElementTheme theme)
        => Window?.SetTheme(theme);

    /// <summary>
    /// 关闭播放器.
    /// </summary>
    public void Close()
        => Window?.Close();

    private void ShowPlayerWindow()
    {
        if (Window?.IsDisposed != false)
        {
            Window = new PlayerWindow();
            Window.GetWindow().Destroying += OnWindowDestroying;
            var pointerSource = InputPointerSource.GetForIsland(Window.XamlRoot?.ContentIsland);
            pointerSource.PointerReleased += OnWindowPointerReleased;
            var keyboardSource = InputKeyboardSource.GetForIsland(Window.XamlRoot?.ContentIsland);
            keyboardSource.KeyUp += OnWindowKeyUp;
        }

        var hideMainWindowOnPlay = SettingsToolkit.ReadLocalSetting(SettingNames.HideMainWindowOnPlay, false);
        if (hideMainWindowOnPlay)
        {
            var mainWnd = this.Get<Core.AppViewModel>().Windows.First(p => p is MainWindow);
            if (mainWnd.AppWindow.IsVisible)
            {
                var currentPos = mainWnd.AppWindow.Position;
                var currentSize = mainWnd.AppWindow.Size;
                Window.GetWindow().MoveAndResize(new Windows.Graphics.RectInt32(currentPos.X, currentPos.Y, currentSize.Width, currentSize.Height));
                mainWnd.Hide();
            }
        }

        if (!Window.GetWindow().IsVisible)
        {
            Window!.Show(Player == null && !hideMainWindowOnPlay);
        }

        if (!_uiElementSetted)
        {
            var overlay = new PlayerOverlay { ViewModel = this };
            Window!.SetUIElement(overlay);
            _uiElementSetted = true;

            var currentTheme = SettingsToolkit.ReadLocalSetting(SettingNames.AppTheme, ElementTheme.Default);
            Window.SetTheme(currentTheme);
            var defaultPresenter = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerPresenterMode, PlayerPresenterMode.Default);
            if (defaultPresenter == PlayerPresenterMode.FullScreen)
            {
                Window.EnterFullScreen();
            }
            else if (defaultPresenter == PlayerPresenterMode.CompactOverlay)
            {
                Window.EnterCompactOverlay();
            }
        }

        CheckBlackBackgroundVisible();
    }
}
