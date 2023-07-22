// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaPlaylist;

/// <summary>
/// 表示一个会话.
/// </summary>
public class Session
{
    /// <summary>
    /// 获取或设置会话的 URL.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// 获取或设置播放列表项.
    /// </summary>
    public int PlaylistItem { get; set; } = -1;

    /// <summary>
    /// 获取或设置外部音频流.
    /// </summary>
    public int ExternalAudioStream { get; set; } = -1;

    /// <summary>
    /// 获取或设置外部视频流.
    /// </summary>
    public int ExternalVideoStream { get; set; } = -1;

    /// <summary>
    /// 获取或设置外部字幕的 URL.
    /// </summary>
    public string ExternalSubtitlesUrl { get; set; }

    /// <summary>
    /// 获取或设置音频流.
    /// </summary>
    public int AudioStream { get; set; } = -1;

    /// <summary>
    /// 获取或设置视频流.
    /// </summary>
    public int VideoStream { get; set; } = -1;

    /// <summary>
    /// 获取或设置字幕流.
    /// </summary>
    public int SubtitlesStream { get; set; } = -1;

    /// <summary>
    /// 获取或设置当前时间.
    /// </summary>
    public long CurTime { get; set; }

    /// <summary>
    /// 获取或设置音频延迟.
    /// </summary>
    public long AudioDelay { get; set; }

    /// <summary>
    /// 获取或设置字幕延迟.
    /// </summary>
    public long SubtitlesDelay { get; set; }
}
