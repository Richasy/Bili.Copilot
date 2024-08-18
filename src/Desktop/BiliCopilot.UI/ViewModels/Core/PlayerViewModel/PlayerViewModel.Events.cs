// Copyright (c) Bili Copilot. All rights reserved.

using Microsoft.Extensions.Logging;
using Mpv.Core.Args;
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
            if (e.Duration == 0 && !IsLive)
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
                IsBuffering = e.NewState is PlaybackState.Buffering or PlaybackState.Opening or PlaybackState.Decoding;
                IsPaused = !IsBuffering;
            }

            _stateAction?.Invoke(e.NewState);
        });
    }

    private void OnPlaybackStopped(object? sender, PlaybackStoppedEventArgs e)
    {
        if (!IsLive)
        {
            Position = 0;
        }

        _endAction?.Invoke();
    }

    private void OnLogMessageReceived(object? sender, LogMessageReceivedEventArgs e)
    {
        _logger.LogError($"MPV: {e.Message}");
    }
}
