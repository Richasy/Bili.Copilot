// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Constants.Player;
using Bili.Copilot.Models.Data.Pgc;
using Bili.Copilot.Models.Data.Video;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Windows.Media;
using Windows.UI.ViewManagement;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 媒体播放器视图模型.
/// </summary>
public sealed partial class PlayerDetailViewModel
{
    private static string GetVideoPreferCodecId()
    {
        var preferCodec = SettingsToolkit.ReadLocalSetting(SettingNames.PreferCodec, PreferCodec.H264);
        var id = preferCodec switch
        {
            PreferCodec.H265 => "hev",
            PreferCodec.Av1 => "av01",
            _ => "avc",
        };

        return id;
    }

    private static string GetLivePreferCodecId()
    {
        var preferCodec = SettingsToolkit.ReadLocalSetting(SettingNames.PreferCodec, PreferCodec.H264);
        var id = preferCodec switch
        {
            PreferCodec.H265 => "hevc",
            PreferCodec.Av1 => "av1",
            _ => "avc",
        };

        return id;
    }

    private void ResetMediaData()
    {
        TryClear(Formats);
        TryClear(PlaybackRates);
        IsShowProgressTip = false;
        IsShowMediaTransport = false;
        ProgressTip = default;
        _video = null;
        _audio = null;
        CurrentFormat = null;
        IsLoop = false;
        DurationSeconds = 0;
        DurationText = "--";
        ProgressSeconds = 0;
        ProgressText = "--";
        Volume = SettingsToolkit.ReadLocalSetting(SettingNames.Volume, 100);
        _lastReportProgress = TimeSpan.Zero;
        _initializeProgress = TimeSpan.Zero;
        _originalPlayRate = 0;
        IsInteractionEnd = false;
        IsInteractionVideo = false;
        IsShowInteractionChoices = false;
        DanmakuViewModel.ResetCommand.Execute(null);
    }

    private void InitializePlaybackRates()
    {
        var isEnhancement = SettingsToolkit.ReadLocalSetting(SettingNames.PlaybackRateEnhancement, false);
        MaxPlaybackRate = isEnhancement ? 6d : 3d;
        PlaybackRateStep = isEnhancement ? 0.2 : 0.1;

        TryClear(PlaybackRates);
        var defaultList = !isEnhancement
            ? new List<double> { 0.5, 0.75, 1.0, 1.25, 1.5, 2.0 }
            : new List<double> { 0.5, 1.0, 1.5, 2.0, 3.0, 4.0 };

        defaultList.ForEach(p =>
        {
            var vm = new PlaybackRateItemViewModel(p, ChangePlayRateCommand.Execute)
            {
                IsSelected = p == PlaybackRate,
            };
            PlaybackRates.Add(vm);
        });

        var isGlobal = SettingsToolkit.ReadLocalSetting(SettingNames.GlobalPlaybackRate, false);
        if (!isGlobal)
        {
            PlaybackRate = 1d;
        }
    }

    /// <summary>
    /// 清理播放数据.
    /// </summary>
    private void ResetPlayer()
    {
        ReleaseDisplay();

        if (Player != null)
        {
            Player.MediaOpened -= OnMediaOpened;
            Player.MediaEnded -= OnMediaEnded;
            Player.PositionChanged -= OnMediaPositionChanged;
            Player.PropertyChanged -= OnPropertyChanged;
            Player.StateChanged -= OnMediaStateChanged;
            Player?.Dispose();
            Player = default;
        }

        _lastReportProgress = TimeSpan.Zero;
        _progressTimer?.Stop();
        _unitTimer?.Stop();

        Status = PlayerStatus.NotLoad;
    }

    private void InitializeTimers()
    {
        if (_unitTimer == null)
        {
            _unitTimer = new DispatcherTimer();
            _unitTimer.Interval = TimeSpan.FromMilliseconds(100);
            _unitTimer.Tick += OnUnitTimerTick;
        }

        if (_progressTimer == null)
        {
            _progressTimer = new DispatcherTimer();
            _progressTimer.Interval = TimeSpan.FromSeconds(5);
            _progressTimer.Tick += OnProgressTimerTick;
        }
    }

    private void StartTimers()
    {
        _progressTimer?.Start();
        _unitTimer?.Start();
    }

    /// <summary>
    /// 在切换片源时记录当前已播放的进度，以便在切换后重新定位.
    /// </summary>
    private void MarkProgressBreakpoint()
    {
        var progress = TimeSpan.FromSeconds(ProgressSeconds);

        if (progress > TimeSpan.FromSeconds(1))
        {
            _initializeProgress = progress;
        }
    }

    private void InitializeDisplayModeText()
    {
        FullScreenText = DisplayMode == PlayerDisplayMode.FullScreen
            ? ResourceToolkit.GetLocalizedString(StringNames.ExitFullScreen)
            : ResourceToolkit.GetLocalizedString(StringNames.EnterFullScreen);

        FullWindowText = DisplayMode == PlayerDisplayMode.FullWindow
            ? ResourceToolkit.GetLocalizedString(StringNames.ExitFullWindow)
            : ResourceToolkit.GetLocalizedString(StringNames.EnterFullWindow);

        CompactOverlayText = DisplayMode == PlayerDisplayMode.CompactOverlay
            ? ResourceToolkit.GetLocalizedString(StringNames.ExitCompactOverlay)
            : ResourceToolkit.GetLocalizedString(StringNames.EnterCompactOverlay);
    }

    [RelayCommand]
    private async Task ReportViewProgressAsync()
    {
        if (AuthorizeProvider.Instance.State == Models.Constants.Authorize.AuthorizeState.SignedIn)
        {
            return;
        }

        var progress = Player.Position;
        if (progress != _lastReportProgress && progress > TimeSpan.Zero)
        {
            if (_videoType == VideoType.Video && !_isInPrivate)
            {
                var view = _viewData as VideoPlayerView;
                var aid = view.Information.Identifier.Id;
                var cid = _currentPart.Id;
                await PlayerProvider.ReportProgressAsync(aid, cid, progress.TotalSeconds);
            }
            else if (_videoType == VideoType.Pgc && _currentEpisode != null)
            {
                var view = _viewData as PgcPlayerView;
                var aid = _currentEpisode.VideoId;
                var cid = _currentEpisode.PartId;
                var epid = _currentEpisode.Identifier.Id;
                var sid = view.Information.Identifier.Id;
                await PlayerProvider.ReportProgressAsync(aid, cid, epid, sid, progress.TotalSeconds);
            }

            _lastReportProgress = progress;
        }
    }

    [RelayCommand]
    private void ShowNextVideoTip()
        => IsShowNextVideoTip = true;

    [RelayCommand]
    private void PlayNextVideo()
        => _playNextAction?.Invoke();

    [RelayCommand]
    private void ClearSourceProgress()
    {
        if (_viewData is VideoPlayerView videoPlayer)
        {
            videoPlayer.Progress = default;
        }
        else if (_viewData is PgcPlayerView pgcPlayer)
        {
            pgcPlayer.Progress = default;
        }
    }

    private void CheckExitFullPlayerButtonVisibility()
    {
        var isFullPlayer = DisplayMode != PlayerDisplayMode.Default;
        IsShowExitFullPlayerButton = isFullPlayer && (IsError || IsShowMediaTransport);
    }

    private void OnMediaStateChanged(object sender, MediaStateChangedEventArgs e)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            IsError = e.Status == PlayerStatus.Failed || !string.IsNullOrEmpty(Player.Player.LastError);
            Status = e.Status;
            IsMediaPause = e.Status != PlayerStatus.Playing;
            IsBuffering = e.Status == PlayerStatus.Buffering;

            if (e.Status == PlayerStatus.Failed)
            {
                ErrorText = e.Message;
            }
            else if (e.Status == PlayerStatus.Playing)
            {
                if (Player.Position < _initializeProgress)
                {
                    Player.SeekTo(_initializeProgress);
                    _initializeProgress = TimeSpan.Zero;
                }
            }

            if (e.Status == PlayerStatus.Playing)
            {
                ActiveDisplay();
            }
            else
            {
                ReleaseDisplay();
            }
        });
    }

    private void OnMediaPositionChanged(object sender, MediaPositionChangedEventArgs e)
    {
        DurationSeconds = e.Duration.TotalSeconds;
        ProgressSeconds = e.Position.TotalSeconds;

        DurationText = NumberToolkit.FormatDurationText(e.Duration, e.Duration.Hours > 0);
        ProgressText = NumberToolkit.FormatDurationText(e.Position, e.Duration.Hours > 0);

        if (SubtitleViewModel.HasSubtitles)
        {
            SubtitleViewModel.SeekCommand.Execute(ProgressSeconds);
        }

        var segmentIndex = Convert.ToInt32(Math.Ceiling(ProgressSeconds / 360d));
        if (segmentIndex < 1)
        {
            segmentIndex = 1;
        }

        DanmakuViewModel.LoadSegmentDanmakuCommand.ExecuteAsync(segmentIndex);
        DanmakuViewModel.SeekCommand.Execute(ProgressSeconds);
    }

    private void OnMediaOpened(object sender, EventArgs e)
    {
        ChangePlayRateCommand.Execute(PlaybackRate);
        ChangeVolumeCommand.Execute(Volume);

        var autoPlay = SettingsToolkit.ReadLocalSetting(SettingNames.IsAutoPlayWhenLoaded, true);
        if (autoPlay)
        {
            Player.Play();
        }
    }

    private void OnMediaEnded(object sender, EventArgs e)
    {
        if (IsInteractionVideo)
        {
            if (InteractionViewModel.Choices.Count == 1 && string.IsNullOrEmpty(InteractionViewModel.Choices.First().Text))
            {
                // 这是默认选项，直接切换.
                SelectInteractionChoiceCommand.Execute(InteractionViewModel.Choices.First());
            }
            else
            {
                IsShowInteractionChoices = true;
            }
        }

        if (!IsLoop)
        {
            MediaEnded?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnProgressTimerTick(object sender, object e)
        => ReportViewProgressCommand.ExecuteAsync(null);

    private void OnUnitTimerTick(object sender, object e)
    {
        _presetVolumeHoldTime += 100;
        if (_presetVolumeHoldTime > 300)
        {
            _presetVolumeHoldTime = 0;
            if (Player != null && Volume != Player.Volume)
            {
                _dispatcherQueue.TryEnqueue(() =>
                {
                    var msg = Volume > 0
                        ? $"{ResourceToolkit.GetLocalizedString(StringNames.CurrentVolume)}: {Volume}"
                        : ResourceToolkit.GetLocalizedString(StringNames.Muted);
                    RequestShowTempMessage?.Invoke(this, msg);
                    Player.SetVolume(Volume);
                });
            }
        }
    }

    private void OnViewVisibleBoundsChanged(ApplicationView sender, object args)
    {
        // 如果用户通过窗口按钮手动退出全屏状态，则播放器调整为默认模式.
        if (!sender.IsFullScreenMode && DisplayMode == PlayerDisplayMode.FullScreen)
        {
            DisplayMode = PlayerDisplayMode.Default;
        }
    }

    private void OnInteractionModuleNoMoreChoices(object sender, EventArgs e)
        => IsInteractionEnd = true;

    private void OnSystemControlsButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
    {
        switch (args.Button)
        {
            case SystemMediaTransportControlsButton.Play:
                _dispatcherQueue.TryEnqueue(() =>
                {
                    Player?.Play();
                });
                break;
            case SystemMediaTransportControlsButton.Pause:
                _dispatcherQueue.TryEnqueue(() =>
                {
                    Player?.Pause();
                });
                break;
            default:
                break;
        }
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ProgressSeconds))
        {
            ChangeProgressCommand.Execute(ProgressSeconds);
        }
    }

    private int GetFormatId(bool isLive = false)
    {
        var defaultPreferQuality = PreferQuality.Auto;
        var preferQuality = SettingsToolkit.ReadLocalSetting(SettingNames.PreferQuality, defaultPreferQuality);
        var formatId = 0;
        if (preferQuality == PreferQuality.HDFirst)
        {
            var hdQuality = isLive ? 10000 : 116;
            formatId = Formats.Where(p => !p.IsLimited && p.Quality <= hdQuality).Max(p => p.Quality);
        }
        else if (preferQuality == PreferQuality.HighQuality)
        {
            formatId = Formats.Where(p => !p.IsLimited).Max(p => p.Quality);
        }

        if (formatId == 0)
        {
            var formatSetting = isLive ? SettingNames.DefaultLiveFormat : SettingNames.DefaultVideoFormat;
            var defaultFormat = isLive ? 400 : 64;
            formatId = SettingsToolkit.ReadLocalSetting(formatSetting, defaultFormat);
        }

        if (!Formats.Any(p => p.Quality == formatId))
        {
            formatId = Formats.Where(p => !p.IsLimited).Max(p => p.Quality);
        }

        return formatId;
    }
}
