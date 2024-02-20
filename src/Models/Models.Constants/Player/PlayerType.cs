// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.Constants.Player;

/// <summary>
/// 播放器类型.
/// </summary>
public enum PlayerType
{
    /// <summary>
    /// 原生播放解码，使用 AdaptiveMediaSource.
    /// </summary>
    Native,

    /// <summary>
    /// FFmpeg解码，借助 Flyleaf.
    /// </summary>
    FFmpeg,

    /// <summary>
    /// VLC解码，借助 LibVLCSharp.
    /// </summary>
    Vlc,

    /// <summary>
    /// MPV 播放器.
    /// </summary>
    Mpv,
}
