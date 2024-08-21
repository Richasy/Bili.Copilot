// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;

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
                IsBuffering = state is PlayerState.Buffering or PlayerState.Opening or PlayerState.Decoding;
                IsPaused = !IsBuffering;
            }

            _stateAction?.Invoke(state);
        });
    }

    /// <summary>
    /// 播放结束.
    /// </summary>
    protected virtual void ReachEnd()
        => _endAction?.Invoke();
}
