// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.Extensions.Logging;
using Mpv.Core.Args;
using Mpv.Core.Enums.Client;
using Mpv.Core.Enums.Player;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 播放器视图模型.
/// </summary>
public sealed partial class PlayerViewModel
{
    private void OnPositionChanged(object? sender, PlaybackPositionChangedEventArgs e)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            if (e.Duration == 0)
            {
                Player.ResetDuration();
            }

            Position = Convert.ToInt32(e.Position);
            Duration = Convert.ToInt32(e.Duration);
            _progressAction?.Invoke(Position, Duration);
        });
    }

    private void OnStateChanged(object? sender, PlaybackStateChangedEventArgs e)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            if (e.NewState == PlaybackState.Playing)
            {
                IsPaused = false;
                IsBuffering = false;
                ActiveDisplay();
            }
            else if (e.NewState == PlaybackState.Paused || e.NewState == PlaybackState.None)
            {
                IsPaused = true;
                IsBuffering = false;
                ReleaseDisplay();
            }
            else
            {
                IsPaused = true;
                IsBuffering = e.NewState is PlaybackState.Buffering or PlaybackState.Opening or PlaybackState.Decoding;
            }

            _stateAction?.Invoke(e.NewState);
        });
    }

    private void OnPlaybackStopped(object? sender, PlaybackStoppedEventArgs e)
    {
        Position = 0;
        _endAction?.Invoke();
    }

    private void OnLogMessageReceived(object? sender, LogMessageReceivedEventArgs e)
    {
        if (e.Level == MpvLogLevel.Error)
        {
            _logger.LogError(e.Message);
        }
        else
        {
            _logger.LogCritical(e.Message);
        }
    }
}
