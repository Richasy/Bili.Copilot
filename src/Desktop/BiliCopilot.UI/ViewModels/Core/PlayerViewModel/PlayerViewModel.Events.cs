﻿// Copyright (c) Bili Copilot. All rights reserved.

using System.Diagnostics;
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
                // ToDo: 播放状态.
                IsPaused = false;
                ActiveDisplay();
            }
            else if (e.NewState == PlaybackState.Paused || e.NewState == PlaybackState.None)
            {
                // ToDo: 暂停状态.
                IsPaused = true;
                ReleaseDisplay();
            }
            else
            {
                IsPaused = true;
            }

            _stateAction?.Invoke(e.NewState);
        });
    }

    private void OnLogMessageReceived(object? sender, LogMessageReceivedEventArgs e)
    {
        Debug.WriteLine($"[{e.Level}]\t{e.Prefix}: {e.Message}");
    }
}
