// Copyright (c) Bili Copilot. All rights reserved.

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
        var duration = TimeSpan.FromSeconds(e.Duration);
        var position = TimeSpan.FromSeconds(e.Position);
        _dispatcherQueue.TryEnqueue(() =>
        {
            // 更新播放进度.
        });
    }

    private void OnStateChanged(object? sender, PlaybackStateChangedEventArgs e)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            if (e.NewState == PlaybackState.Playing)
            {
                // ToDo: 播放状态.
                IsPlayerDataLoading = false;
            }
            else if (e.NewState == PlaybackState.Paused || e.NewState == PlaybackState.None)
            {
                // ToDo: 暂停状态.
                IsPlayerDataLoading = false;
            }
            else
            {
                // ToDo: 加载状态.
                IsPlayerDataLoading = true;
            }
        });
    }

    private void OnLogMessageReceived(object? sender, LogMessageReceivedEventArgs e)
    {
        Debug.WriteLine($"[{e.Level}]\t{e.Prefix}: {e.Message}");
    }
}
