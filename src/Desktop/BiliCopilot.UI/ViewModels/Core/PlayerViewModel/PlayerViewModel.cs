// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Controls.Core;
using BiliCopilot.UI.Forms;
using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Resolvers;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Richasy.WinUIKernel.Share.ViewModels;
using WinUIEx;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 播放器视图模型.
/// </summary>
public sealed partial class PlayerViewModel(DispatcherQueue queue, ILogger<PlayerViewModel> logger) : ViewModelBase, IAsyncDisposable
{
    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        Danmaku.ClearAll();
        _nativeMp?.Dispose();
        _nativeMp = null;

        if (_rightKeyLongPressTimer != null)
        {
            _rightKeyLongPressTimer.Tick -= OnRightKeyLongPressTimerTick;
            _rightKeyLongPressTimer.Stop();
            _rightKeyLongPressTimer = null;
        }

        if (_tripleTimer != null)
        {
            _tripleTimer.Tick -= OnTripleTimerTick;
            _tripleTimer.Stop();
            _tripleTimer = null;
        }

        if (_smtc != null)
        {
            _smtc.ButtonPressed -= OnSmtcButtonPressed;
            _smtc = null;
        }

        if (Connector != null)
        {
            Connector.PlaylistInitialized -= OnConnectorPlaylistInitialized;
            Connector.NewMediaRequest -= OnConnectorNewMediaRequest;
            Connector.RequestOpenExtraPanel -= OnRequestOpenExtraPanel;
            Connector.PropertiesUpdated -= OnConnectorPropertiesUpdated;
        }

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
            Window.SizeChanged -= OnWindowSizeChanged;
            Window.GetWindow().Destroying -= OnWindowDestroying;
            await Window.DisposeAsync();
        }

        this.Get<AppViewModel>().Players.Remove(this);
    }

    [RelayCommand]
    internal async Task InitializeAsync(MediaSnapshot snapshot)
    {
        _snapshot = snapshot;
        ErrorMessage = default;
        CacheSpeedText = default;
        ShowPlayerWindow();
        IsConnecting = true;
        IsSourceSelectable = false;
        IsDanmakuEnabled = false;
        IsDanmakuLoading = false;
        IsStatsOverlayShown = false;
        IsEnd = false;
        _isTlsFailed = false;
        _prevPosition = 0;
        IsSubtitleEmpty = true;
        _isDanmakuInitialized = false;
        IsNextTipShownInThisMedia = false;
        Danmaku.ClearAll();
        Chapters.Clear();
        IsChapterVisible = SettingsToolkit.ReadLocalSetting(SettingNames.IsChapterVisible, true);
        UseIntegrationOperation = SettingsToolkit.ReadLocalSetting(SettingNames.HideMainWindowOnPlay, true)
            && SettingsToolkit.ReadLocalSetting(SettingNames.UseIntegrationWhenSinglePlayWindow, true);
        BackwardTip = string.Format(ResourceToolkit.GetLocalizedString(StringNames.BackwardTemplate), SettingsToolkit.ReadLocalSetting(SettingNames.StepBackwardSecond, 10d));
        ForwardTip = string.Format(ResourceToolkit.GetLocalizedString(StringNames.ForwardTemplate), SettingsToolkit.ReadLocalSetting(SettingNames.StepForwardSecond, 30d));
        RequestHideNextTip?.Invoke(this, EventArgs.Empty);
        if (Client != null)
        {
            try
            {
                await Client.PauseAsync();
                await Client.StopAsync();
            }
            catch
            {
            }
        }

        await InitializeInternalAsync();
        if (Connector != null)
        {
            System.Diagnostics.Debug.WriteLine($"Player initialized with connector: {Connector.GetType().Name}");
            await Connector.InitializeAsync(_snapshot, default);
            if (Window.IsClosed)
            {
                return;
            }
        }

        Title = Connector switch
        {
            VideoConnectorViewModel videoVM => videoVM.Title,
            PgcConnectorViewModel pgcVM => pgcVM.Title,
            _ => string.Empty
        };

        // TODO: 创建源处理器
        switch (_snapshot.Type)
        {
            case BiliMediaType.Video:
                {
                    var resolver = this.Get<VideoMediaSourceResolver>();
                    var connector = Connector as VideoConnectorViewModel;
                    resolver.Initialize(_snapshot, connector._part);
                    _sourceResolver = resolver;
                }
                break;
            case BiliMediaType.Pgc:
                {
                    var resolver = this.Get<PgcMediaSourceResolver>();
                    var connector = Connector as PgcConnectorViewModel;
                    resolver.Initialize(_snapshot, connector._episode);
                    _sourceResolver = resolver;
                }
                break;
            case BiliMediaType.Live:
                break;
            default:
                break;
        }

        _sourceResolver.WindowHandle = Window!.Handle;

        if (_historyResolver != null && Player != null)
        {
            await _historyResolver.SaveHistoryAsync(Player.Position, Player.Duration, Richasy.MpvKernel.Core.Enums.MpvPlayerState.End, true);
        }

        // TODO: 创建历史记录处理器
        switch (_snapshot.Type)
        {
            case BiliMediaType.Video:
                {
                    var resolver = this.Get<VideoMediaHistoryResolver>();
                    var connector = Connector as VideoConnectorViewModel;
                    resolver.Initialize(connector._view, connector._part, _snapshot);
                    _historyResolver = resolver;
                }
                break;
            case BiliMediaType.Pgc:
                {
                    var resolver = this.Get<PgcMediaHistoryResolver>();
                    var connector = Connector as PgcConnectorViewModel;
                    resolver.Initialize(connector._view, connector._episode, _snapshot);
                    _historyResolver = resolver;
                }
                break;
            case BiliMediaType.Live:
                break;
            default:
                break;
        }

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

        try
        {
            await Player.InitializeAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return;
        }

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
        await LoadSourcesAsync();
        await LoadSubtitlesAsync();
        if (Window.IsClosed)
        {
            return;
        }

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
            Window.SetActiveAction(isActive => queue.TryEnqueue(DispatcherQueuePriority.Low, () => IsWindowDeactivated = !isActive));
            Window.GetWindow().Destroying += OnWindowDestroying;
            Window.SizeChanged += OnWindowSizeChanged;
            var pointerSource = InputPointerSource.GetForIsland(Window.XamlRoot?.ContentIsland);
            pointerSource.PointerReleased += OnWindowPointerReleased;
            var keyboardSource = InputKeyboardSource.GetForIsland(Window.XamlRoot?.ContentIsland);
            keyboardSource.KeyDown += OnWindowKeyDown;
            keyboardSource.KeyUp += OnWindowKeyUp;
        }

        var hideMainWindowOnPlay = SettingsToolkit.ReadLocalSetting(SettingNames.HideMainWindowOnPlay, true);

        if (!Window.GetWindow().IsVisible)
        {
            Window!.Show(Player == null && !hideMainWindowOnPlay);
        }

        var isFirstLoad = !_uiElementSetted;
        if (!_uiElementSetted)
        {
            var overlay = new PlayerOverlay { ViewModel = this };
            Window!.SetUIElement(overlay);
            _uiElementSetted = true;

            var currentTheme = SettingsToolkit.ReadLocalSetting(SettingNames.AppTheme, ElementTheme.Default);
            Window.SetTheme(currentTheme);
        }

        if (hideMainWindowOnPlay && isFirstLoad)
        {
            var mainWnd = this.Get<Core.AppViewModel>().Windows.First(p => p is MainWindow);
            if (mainWnd.AppWindow.IsVisible)
            {
                var scaleFactor = HwndExtensions.GetDpiForWindow(mainWnd.GetWindowHandle()) / 96d;
                var windowLeft = SettingsToolkit.ReadLocalSetting(SettingNames.MainWindowPositionLeft, 0);
                var windowTop = SettingsToolkit.ReadLocalSetting(SettingNames.MainWindowPositionTop, 0);
                var windowWidth = SettingsToolkit.ReadLocalSetting(SettingNames.MainWindowWidth, 1120d);
                var windowHeight = SettingsToolkit.ReadLocalSetting(SettingNames.MainWindowHeight, 740d);
                Window.GetWindow().MoveAndResize(new Windows.Graphics.RectInt32(windowLeft, windowTop, Convert.ToInt32(windowWidth * scaleFactor), Convert.ToInt32(windowHeight * scaleFactor)));
                if (SettingsToolkit.ReadLocalSetting(SettingNames.IsMainWindowMaximized, false))
                {
                    (Window.GetWindow().Presenter as OverlappedPresenter)?.Maximize();
                }

                mainWnd.Hide();
            }
        }

        if (isFirstLoad)
        {
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

        IsVerticalScreen = Window!.GetWindow().Presenter.Kind is Microsoft.UI.Windowing.AppWindowPresenterKind.FullScreen or Microsoft.UI.Windowing.AppWindowPresenterKind.CompactOverlay
            ? Window.GetWindow().Size.Height > Window.GetWindow().Size.Width
            : Window.GetWindow().Size.Height >= Window.GetWindow().Size.Width;
        CheckBlackBackgroundVisible();
    }

    private async Task SelectSourceAsync(SourceItemViewModel source)
    {
        if (source is null)
        {
            return;
        }

        foreach (var item in Sources)
        {
            item.IsSelected = item.Equals(source);
        }

        var selected = Sources.FirstOrDefault(s => s.IsSelected);
        SelectedSource = selected;
        _continuePosition = Player?.Position;
        if (_sourceResolver is VideoMediaSourceResolver videoResolver)
        {
            _snapshot.PreferQuality = selected.Data.Quality;
            videoResolver.ResetSnapshot(_snapshot);
            await Player.ReplayAsync(Player.Position);
        }

        await Task.CompletedTask;
    }

    private void CheckBottomProgressBarVisible()
        => IsBottomProgressBarVisible = !IsControlsVisible && SettingsToolkit.ReadLocalSetting(SettingNames.IsBottomProgressBarVisible, true);

    partial void OnIsControlsVisibleChanged(bool value)
        => Window?.SetCaptionButtonVisibility(value);

    partial void OnIsControlsVisibleChanged(bool oldValue, bool newValue)
    {
        IsTopMost = Window?.IsTopMost() ?? false;
        CheckBottomProgressBarVisible();
    }

    partial void OnIsSubtitleEnabledChanged(bool value)
    {
        ToggleSubtitleEnabledCommand.Execute(value);
    }
}
