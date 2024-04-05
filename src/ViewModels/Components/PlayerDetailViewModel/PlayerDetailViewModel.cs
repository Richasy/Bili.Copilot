// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Constants.Player;
using Bili.Copilot.Models.Data.Live;
using Bili.Copilot.Models.Data.Pgc;
using Bili.Copilot.Models.Data.Player;
using Bili.Copilot.Models.Data.Video;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Windows.System.Display;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 媒体播放器视图模型.
/// </summary>
public sealed partial class PlayerDetailViewModel : ViewModelBase, IDisposable
{
    private bool _disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerDetailViewModel"/> class.
    /// </summary>
    public PlayerDetailViewModel(Window attachedWindow)
    {
        AttachedWindow = attachedWindow;
        _dispatcherQueue = AttachedWindow.DispatcherQueue;
        SubtitleViewModel = new SubtitleModuleViewModel();
        DanmakuViewModel = new DanmakuModuleViewModel();
        InteractionViewModel = new InteractionModuleViewModel();
        DownloadViewModel = new DownloadModuleViewModel(attachedWindow);

        SubtitleViewModel.MetaChanged += OnSubtitleMetaChanged;
        InteractionViewModel.NoMoreChoices += OnInteractionModuleNoMoreChoices;
        PropertyChanged += OnPropertyChanged;

        Volume = SettingsToolkit.ReadLocalSetting(SettingNames.Volume, 100);
        PlaybackRate = SettingsToolkit.ReadLocalSetting(SettingNames.PlaybackRate, 1d);
        IsPlaybackRateSliderEnabled = SettingsToolkit.ReadLocalSetting(SettingNames.PlaybackRateSliderEnabled, false);

        Formats = new ObservableCollection<FormatInformation>();
        PlaybackRates = new ObservableCollection<PlaybackRateItemViewModel>();

        AttachIsRunningToAsyncCommand(p => IsReloading = p, ReloadCommand);
        AttachExceptionHandlerToAsyncCommand(DisplayException, ReloadCommand);
        AttachExceptionHandlerToAsyncCommand(LogException, ReportViewProgressCommand);

        InitializeTimers();
    }

    /// <summary>
    /// 设置视频播放数据.
    /// </summary>
    public void SetVideoData(VideoPlayerView data, bool isInPrivate = false)
    {
        _viewData = data;
        _isInPrivate = isInPrivate;
        _videoType = VideoType.Video;
        _ = ReloadCommand.ExecuteAsync(default);
    }

    /// <summary>
    /// 设置PGC播放数据.
    /// </summary>
    public void SetPgcData(PgcPlayerView view, EpisodeInformation episode)
    {
        _viewData = view;
        _currentEpisode = episode;
        _videoType = VideoType.Pgc;
        _ = ReloadCommand.ExecuteAsync(null);
    }

    /// <summary>
    /// 设置直播播放数据.
    /// </summary>
    public void SetLiveData(LivePlayerView data)
    {
        _viewData = data;
        _videoType = VideoType.Live;
        _ = ReloadCommand.ExecuteAsync(null);
    }

    /// <summary>
    /// 设置WebDav播放数据.
    /// </summary>
    /// <param name="video">视频.</param>
    public void SetWebDavData(WebDavVideoInformation video)
    {
        _webDavVideo = video;
        _videoType = VideoType.WebDav;
        _ = ReloadCommand.ExecuteAsync(null);
    }

    /// <summary>
    /// 激活显示（不会自动锁屏）.
    /// </summary>
    public void ActiveDisplay()
    {
        if (_displayRequest != null)
        {
            return;
        }

        _ = _dispatcherQueue.TryEnqueue(() =>
        {
            _displayRequest = new DisplayRequest();
            _displayRequest.RequestActive();
        });
    }

    /// <summary>
    /// 释放显示（会自动锁屏）.
    /// </summary>
    public void ReleaseDisplay()
    {
        if (_displayRequest == null)
        {
            return;
        }

        _ = _dispatcherQueue.TryEnqueue(() =>
        {
            _displayRequest?.RequestRelease();
            _displayRequest = null;
        });
    }

    /// <summary>
    /// 设置播放下一个内容的动作.
    /// </summary>
    /// <param name="action">动作.</param>
    public void SetPlayNextAction(Action action)
        => _playNextAction = action;

    /// <summary>
    /// 设置播放上一个内容的动作.
    /// </summary>
    /// <param name="action">动作.</param>
    public void SetPlayPreviousAction(Action action)
        => _playPreviousAction = action;

    /// <summary>
    /// 显示错误.
    /// </summary>
    public void DisplayException(Exception exception)
    {
        _ = _dispatcherQueue.TryEnqueue(() =>
        {
            IsError = true;
            var msg = GetErrorMessage(exception);
            ErrorText = $"{ResourceToolkit.GetLocalizedString(StringNames.RequestVideoFailed)}\n{msg}";
        });

        LogException(exception);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Clear()
    {
        if (IsError)
        {
            // 说明解析出现了错误，可能是解码失败，此时应尝试重置清晰度.
            CurrentFormat = null;
        }

        ResetPlayer();
        ResetMediaData();
        ResetVideoData();
        ResetLiveData();
        InitializePlaybackRates();
    }

    [RelayCommand]
    private async Task ReloadAsync()
    {
        Clear();
        IsBackButtonShown = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerWindowBehaviorType, PlayerWindowBehavior.Main) == PlayerWindowBehavior.Main;

        Player?.Stop();
        IsBottomProgressEnabled = SettingsToolkit.ReadLocalSetting(SettingNames.BottomProgressVisible, true) && _videoType != VideoType.Live;
        if (_videoType == VideoType.Video)
        {
            await LoadVideoAsync();
        }
        else if (_videoType == VideoType.Pgc)
        {
            await LoadEpisodeAsync();
        }
        else if (_videoType == VideoType.Live)
        {
            await LoadLiveAsync();
        }
        else if (_videoType == VideoType.WebDav)
        {
            await LoadWebDavAsync();
        }
    }

    /// <summary>
    /// 重新载入当前的流.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    [RelayCommand]
    private async Task RefreshAsync()
    {
        if (CurrentFormat != null)
        {
            await ChangeFormatAsync(CurrentFormat);
        }
        else if (_videoType == VideoType.WebDav)
        {
            await RefreshWebDavAsync();
        }
    }

    [RelayCommand]
    private async Task ChangePartAsync(VideoIdentifier part)
    {
        if (_videoType == VideoType.Video)
        {
            await ChangeVideoPartAsync(part);
        }
        else if (_videoType == VideoType.Pgc)
        {
            await ChangeEpisodeAsync(part);
        }
    }

    [RelayCommand]
    private async Task ChangeFormatAsync(FormatInformation information)
    {
        var needResume = Status == PlayerStatus.Playing;
        if (_videoType != VideoType.Live)
        {
            _initializeProgress = Player.Position;
        }

        Player.Stop();
        Player.IsStatsUpdated = false;
        if (_videoType is VideoType.Video
            or VideoType.Pgc)
        {
            await SelectVideoFormatAsync(information);
        }
        else if (_videoType == VideoType.Live)
        {
            await SelectLiveFormatAsync(information);
        }

        if (needResume)
        {
            Player.Play();
        }
    }

    [RelayCommand]
    private void ResetProgressHistory()
    {
        if (_videoType == VideoType.Video && _viewData is VideoPlayerView videoView)
        {
            videoView.Progress = null;
        }
        else if (_videoType == VideoType.Pgc && _viewData is PgcPlayerView pgcView)
        {
            pgcView.Progress = null;
        }
    }

    [RelayCommand]
    private void OpenInBrowser()
        => RequestOpenInBrowser?.Invoke(this, EventArgs.Empty);

    [RelayCommand]
    private void Back()
    {
        if (DisplayMode == PlayerDisplayMode.CompactOverlay)
        {
            ToggleCompactOverlayModeCommand.Execute(default);
        }
        else if (DisplayMode == PlayerDisplayMode.FullScreen)
        {
            ToggleFullScreenModeCommand.Execute(default);
        }

        AppViewModel.Instance.BackCommand.Execute(default);
    }

    private void InitializePlayer()
    {
        InitializeDisplayModeText();

        if (Player == null)
        {
            if (_videoType == VideoType.WebDav)
            {
                var preferPlayer = SettingsToolkit.ReadLocalSetting(SettingNames.WebDavPlayerType, PlayerType.FFmpeg);
                Player = preferPlayer switch
                {
                    PlayerType.FFmpeg => new FlyleafPlayerViewModel(),
                    _ => new NativePlayerViewModel(),
                };

                Player.WebDavSubtitleListChanged += OnPlayerWebDavSubtitleListChanged;
                Player.WebDavSubtitleChanged += OnPlayerWebDavSubtitleChanged;
            }
            else
            {
                var preferPlayer = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerType, PlayerType.Native);
                Player = preferPlayer switch
                {
                    PlayerType.FFmpeg => new FlyleafPlayerViewModel(),

                    // PlayerType.Vlc => new VlcPlayerViewModel(),
                    _ => new NativePlayerViewModel(),
                };
            }

            Player.Initialize();
            Player.MediaOpened += OnMediaOpened;
            Player.MediaEnded += OnMediaEnded;
            Player.PositionChanged += OnMediaPositionChanged;
            Player.StateChanged += OnMediaStateChanged;
        }
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                if (_unitTimer != null)
                {
                    _unitTimer.Tick -= OnUnitTimerTick;
                    _unitTimer = null;
                }

                Clear();
                Formats.Clear();
                PlaybackRates.Clear();
            }

            _disposedValue = true;
        }
    }

    partial void OnPlaybackRateChanged(double value)
        => ChangePlayRateCommand?.Execute(value);

    partial void OnDisplayModeChanged(PlayerDisplayMode value)
    {
        InitializeDisplayModeText();
        CheckCurrentPlayerDisplayMode();
    }

    partial void OnIsLoopChanged(bool value)
    {
        if (Player != null)
        {
            Player.IsLoop = value;
        }
    }
}
