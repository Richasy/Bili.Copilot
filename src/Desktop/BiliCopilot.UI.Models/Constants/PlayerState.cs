// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Models.Constants;

/// <summary>
/// The state of the player.
/// </summary>
public enum PlayerState
{
    /// <summary>
    /// No current state.
    /// </summary>
    None = 0,

    /// <summary>
    /// A media item is opening.
    /// </summary>
    Opening,

    /// <summary>
    /// A media item is decoding.
    /// </summary>
    Decoding,

    /// <summary>
    /// A media item is buffering.
    /// </summary>
    Buffering,

    /// <summary>
    /// A media item is playing.
    /// </summary>
    Playing,

    /// <summary>
    /// A media item is paused.
    /// </summary>
    Paused,

    /// <summary>
    /// 播放失败.
    /// </summary>
    Failed,
}
