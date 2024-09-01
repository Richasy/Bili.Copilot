// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using Windows.Media;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 播放器视图模型基类.
/// </summary>
public abstract partial class PlayerViewModelBase
{
    /// <summary>
    /// 更新进度.
    /// </summary>
    protected virtual void UpdatePosition(int position, int? duration = default)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            if (IsPaused)
            {
                return;
            }

            Position = position;
            if (duration >= 0)
            {
                Duration = duration.Value;
            }

            _progressAction?.Invoke(Position, Duration);
        });
    }

    /// <summary>
    /// 更新状态.
    /// </summary>
    protected virtual void UpdateState(PlayerState state)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            if (state == PlayerState.Playing)
            {
                IsPaused = false;
                IsBuffering = false;
                ActiveDisplay();
            }
            else if (state == PlayerState.Paused || state == PlayerState.None)
            {
                IsPaused = true;
                IsBuffering = false;
                ReleaseDisplay();
            }
            else
            {
                IsBuffering = state is PlayerState.Buffering;
                IsPaused = !IsBuffering;
            }

            if (_smtc is not null)
            {
                _smtc.PlaybackStatus = IsFailed ? MediaPlaybackStatus.Stopped : IsPaused ? MediaPlaybackStatus.Paused : MediaPlaybackStatus.Playing;
            }

            IsFailed = state == PlayerState.Failed;
            _stateAction?.Invoke(state);
        });
    }

    /// <summary>
    /// 播放结束.
    /// </summary>
    protected virtual void ReachEnd()
        => _endAction?.Invoke();

    private void OnSystemControlsButtonPressedAsync(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
    {
        switch (args.Button)
        {
            case SystemMediaTransportControlsButton.Play:
            case SystemMediaTransportControlsButton.Pause:
                _dispatcherQueue.TryEnqueue(() =>
                {
                    TogglePlayPauseCommand.Execute(default);
                });
                break;
            default:
                break;
        }
    }
}
