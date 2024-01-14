// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.Pgc;
using Bili.Copilot.Models.Data.Video;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 媒体播放器视图模型.
/// </summary>
public sealed partial class PlayerDetailViewModel
{
    [RelayCommand]
    private void PlayPause()
    {
        EnsureMediaPlayerExist();

        _ = _dispatcherQueue.TryEnqueue(() =>
        {
            if (Status == PlayerStatus.Playing)
            {
                Player.Pause();
            }
            else if (Status == PlayerStatus.Pause)
            {
                Player.Play();
            }
            else if (Status == PlayerStatus.End)
            {
                Player.SeekTo(TimeSpan.Zero);
                RefreshCommand.Execute(default);
            }
        });
    }

    [RelayCommand]
    private void ForwardSkip()
    {
        EnsureMediaPlayerExist();

        var seconds = SettingsToolkit.ReadLocalSetting(SettingNames.SingleFastForwardAndRewindSpan, 30d);
        if (seconds <= 0
            || Status == PlayerStatus.NotLoad
            || Status == PlayerStatus.Buffering)
        {
            return;
        }

        _ = _dispatcherQueue.TryEnqueue(() =>
        {
            var duration = Player.Duration;
            var currentPos = Player.Position;
            if ((duration - currentPos).TotalSeconds > seconds)
            {
                currentPos += TimeSpan.FromSeconds(seconds);
            }
            else
            {
                currentPos = duration;
            }

            Player.SeekTo(currentPos);
        });
    }

    [RelayCommand]
    private void BackwardSkip()
    {
        EnsureMediaPlayerExist();

        var seconds = SettingsToolkit.ReadLocalSetting(SettingNames.SingleFastForwardAndRewindSpan, 30d);
        if (seconds <= 0
            || Status == PlayerStatus.NotLoad
            || Status == PlayerStatus.Buffering)
        {
            return;
        }

        _ = _dispatcherQueue.TryEnqueue(() =>
        {
            var duration = Player.Duration;
            var currentPos = Player.Position;
            if (currentPos.TotalSeconds > seconds)
            {
                currentPos -= TimeSpan.FromSeconds(seconds);
            }
            else
            {
                currentPos = TimeSpan.Zero;
            }

            Player.SeekTo(currentPos);
        });
    }

    [RelayCommand]
    private void ChangePlayRate(double rate)
    {
        try
        {
            EnsureMediaPlayerExist();
        }
        catch (Exception)
        {
            return;
        }

        _ = _dispatcherQueue.TryEnqueue(() =>
        {
            if (rate > MaxPlaybackRate)
            {
                return;
            }

            if (PlaybackRate != rate)
            {
                PlaybackRate = rate;
                SettingsToolkit.WriteLocalSetting(SettingNames.PlaybackRate, rate);
            }

            Player.SetPlayRate(rate);

            foreach (var r in PlaybackRates)
            {
                r.IsSelected = r.Data == PlaybackRate;
            }
        });
    }

    [RelayCommand]
    private void ChangeVolume(int volume)
    {
        try
        {
            EnsureMediaPlayerExist();
        }
        catch (Exception)
        {
            return;
        }

        if (volume > 100)
        {
            volume = 100;
        }
        else if (volume < 0)
        {
            volume = 0;
        }

        if (Volume != volume)
        {
            Volume = volume;
        }

        SettingsToolkit.WriteLocalSetting(SettingNames.Volume, Volume);
    }

    [RelayCommand]
    private void ToggleFullScreenMode()
    {
        DisplayMode = DisplayMode != PlayerDisplayMode.FullScreen
            ? PlayerDisplayMode.FullScreen
            : PlayerDisplayMode.Default;
    }

    [RelayCommand]
    private void ToggleCompactOverlayMode()
    {
        DisplayMode = DisplayMode != PlayerDisplayMode.CompactOverlay
            ? PlayerDisplayMode.CompactOverlay
            : PlayerDisplayMode.Default;
    }

    [RelayCommand]
    private void ExitFullPlayer()
        => DisplayMode = PlayerDisplayMode.Default;

    private void EnsureMediaPlayerExist()
    {
        if (Player?.IsPlayerReady ?? false)
        {
            return;
        }

        throw new InvalidOperationException("此时媒体播放器尚未就绪");
    }

    [RelayCommand]
    private void ChangeProgress(double seconds)
    {
        var ts = TimeSpan.FromSeconds(seconds);
        if (Player == null
            || Math.Abs(ts.TotalSeconds - Player.Position.TotalSeconds) < 3)
        {
            return;
        }

        Player.SeekTo(ts);
        var msg = $"{ResourceToolkit.GetLocalizedString(StringNames.CurrentProgress)}: {TimeSpan.FromSeconds(seconds):g}";
        RequestShowTempMessage?.Invoke(this, msg);

        if (_videoType != VideoType.WebDav)
        {
            DanmakuViewModel.ResetTimePositionCommand.Execute(ts);
        }
    }

    [RelayCommand]
    private void StartTempQuickPlay()
    {
        EnsureMediaPlayerExist();
        if (Status != PlayerStatus.Playing || PlaybackRate >= 3)
        {
            return;
        }

        _originalPlayRate = PlaybackRate;
        _originalDanmakuSpeed = DanmakuViewModel.DanmakuSpeed;
        ChangePlayRate(3);
        DanmakuViewModel.DanmakuSpeed = 2;
        var msg = ResourceToolkit.GetLocalizedString(StringNames.StartQuickPlay);
        RequestShowTempMessage?.Invoke(this, msg);
    }

    [RelayCommand]
    private void StopTempQuickPlay()
    {
        if (_originalPlayRate <= 0)
        {
            return;
        }

        DanmakuViewModel.DanmakuSpeed = _originalDanmakuSpeed;
        ChangePlayRate(_originalPlayRate);
        _originalPlayRate = 0;
        _originalDanmakuSpeed = 0;
        RequestShowTempMessage?.Invoke(this, default);
    }

    [RelayCommand]
    private void JumpToLastProgress()
    {
        if (_videoType == VideoType.Video)
        {
            var view = _viewData as VideoPlayerView;
            if (view.Progress != null)
            {
                if (view.Progress.Identifier.Id != _currentPart.Id)
                {
                    // 切换分P.
                    InternalPartChanged?.Invoke(this, view.Progress.Identifier);
                }
                else
                {
                    ChangeProgress(view.Progress.Progress);
                    ResetProgressHistory();
                }
            }

            IsShowProgressTip = false;
        }
        else if (_videoType == VideoType.Pgc)
        {
            var view = _viewData as PgcPlayerView;
            if (view.Progress != null)
            {
                var episode = view.Progress.Identifier;
                if (_currentEpisode.Identifier.Id != episode.Id)
                {
                    // 切换分集.
                    InternalPartChanged?.Invoke(this, view.Progress.Identifier);
                }
                else
                {
                    ChangeProgress(view.Progress.Progress);
                    ResetProgressHistory();
                }
            }
        }
    }

    [RelayCommand]
    private void IncreasePlayRate()
    {
        if (_videoType == VideoType.Live)
        {
            return;
        }

        var rate = PlaybackRate + PlaybackRateStep;
        if (rate > MaxPlaybackRate)
        {
            rate = MaxPlaybackRate;
        }

        ChangePlayRateCommand.Execute(rate);
        RequestShowTempMessage?.Invoke(this, $"{ResourceToolkit.GetLocalizedString(StringNames.CurrentPlaybackRate)}: {rate}x");
    }

    [RelayCommand]
    private void DecreasePlayRate()
    {
        if (_videoType == VideoType.Live)
        {
            return;
        }

        var rate = PlaybackRate - PlaybackRateStep;
        if (rate < 0.5)
        {
            rate = 0.5;
        }

        ChangePlayRateCommand.Execute(rate);
        RequestShowTempMessage?.Invoke(this, $"{ResourceToolkit.GetLocalizedString(StringNames.CurrentPlaybackRate)}: {rate}x");
    }

    [RelayCommand]
    private void IncreaseVolume()
    {
        var volume = Volume + 5;
        if (volume > 100)
        {
            volume = 100;
        }

        ChangeVolumeCommand.Execute(volume);
    }

    [RelayCommand]
    private void DecreaseVolume()
    {
        var volume = Volume - 5;
        if (volume < 0)
        {
            volume = 0;
        }

        ChangeVolumeCommand.Execute(volume);
    }

    [RelayCommand]
    private void BackToDefaultMode()
    {
        if (DisplayMode == PlayerDisplayMode.FullScreen)
        {
            ToggleFullScreenModeCommand.Execute(default);
        }
        else if (DisplayMode == PlayerDisplayMode.CompactOverlay)
        {
            ToggleCompactOverlayModeCommand.Execute(default);
        }
    }
}
