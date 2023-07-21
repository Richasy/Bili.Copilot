// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Libs.Player.MediaFramework.MediaStream;

namespace Bili.Copilot.Libs.Player.Models;

/// <summary>
/// OpenAsyncData 类用于异步打开流.
/// </summary>
internal class OpenAsyncData
{
    /// <summary>
    /// 使用指定的 url_iostream 初始化 OpenAsyncData 类的新实例.
    /// </summary>
    /// <param name="urlOrIoStream">要打开的流的 url_iostream.</param>
    /// <param name="defaultPlaylistItem">是否使用默认播放列表项.</param>
    /// <param name="defaultVideo">是否使用默认视频流.</param>
    /// <param name="defaultAudio">是否使用默认音频流.</param>
    /// <param name="defaultSubtitles">是否使用默认字幕流.</param>
    public OpenAsyncData(object urlOrIoStream, bool defaultPlaylistItem = true, bool defaultVideo = true, bool defaultAudio = true, bool defaultSubtitles = true)
    {
        UrlOrIoStream = urlOrIoStream;
        DefaultPlaylistItem = defaultPlaylistItem;
        DefaultVideo = defaultVideo;
        DefaultAudio = defaultAudio;
        DefaultSubtitles = defaultSubtitles;
    }

    /// <summary>
    /// 使用指定的 session 初始化 OpenAsyncData 类的新实例.
    /// </summary>
    /// <param name="session">要打开的流的 session.</param>
    public OpenAsyncData(Session session)
        => Session = session;

    /// <summary>
    /// 使用指定的 playlistItem 初始化 OpenAsyncData 类的新实例.
    /// </summary>
    /// <param name="playlistItem">要打开的流的 playlistItem.</param>
    /// <param name="defaultVideo">是否使用默认视频流.</param>
    /// <param name="defaultAudio">是否使用默认音频流.</param>
    /// <param name="defaultSubtitles">是否使用默认字幕流.</param>
    public OpenAsyncData(PlaylistItem playlistItem, bool defaultVideo = true, bool defaultAudio = true, bool defaultSubtitles = true)
    {
        PlaylistItem = playlistItem;
        DefaultVideo = defaultVideo;
        DefaultAudio = defaultAudio;
        DefaultSubtitles = defaultSubtitles;
    }

    /// <summary>
    /// 使用指定的 extStream 初始化 OpenAsyncData 类的新实例.
    /// </summary>
    /// <param name="extStream">要打开的流的 extStream.</param>
    /// <param name="resync">是否重新同步流.</param>
    /// <param name="defaultAudio">是否使用默认音频流.</param>
    /// <param name="streamIndex">要打开的流的索引.</param>
    public OpenAsyncData(ExternalStream extStream, bool resync = true, bool defaultAudio = true, int streamIndex = -1)
    {
        ExtStream = extStream;
        ReSync = resync;
        StreamIndex = streamIndex;
        DefaultAudio = defaultAudio;
    }

    /// <summary>
    /// 使用指定的 stream 初始化 OpenAsyncData 类的新实例.
    /// </summary>
    /// <param name="stream">要打开的流的 stream.</param>
    /// <param name="resync">是否重新同步流.</param>
    /// <param name="defaultAudio">是否使用默认音频流.</param>
    public OpenAsyncData(StreamBase stream, bool resync = true, bool defaultAudio = true)
    {
        Stream = stream;
        ReSync = resync;
        DefaultAudio = defaultAudio;
    }

    /// <summary>
    /// 是否使用默认播放列表项.
    /// </summary>
    public bool DefaultPlaylistItem { get; set; }

    /// <summary>
    /// 是否使用默认音频流.
    /// </summary>
    public bool DefaultAudio { get; set; }

    /// <summary>
    /// 是否使用默认视频流.
    /// </summary>
    public bool DefaultVideo { get; set; }

    /// <summary>
    /// 是否使用默认字幕流.
    /// </summary>
    public bool DefaultSubtitles { get; set; }

    /// <summary>
    /// 表示URL或IO流.
    /// </summary>
    public object UrlOrIoStream { get; set; }

    /// <summary>
    /// 表示流索引.
    /// </summary>
    public int StreamIndex { get; set; }

    /// <summary>
    /// 表示流基类.
    /// </summary>
    public StreamBase Stream { get; set; }

    /// <summary>
    /// 表示是否重新同步.
    /// </summary>
    public bool ReSync { get; set; }

    /// <summary>
    /// 表示会话.
    /// </summary>
    public Session Session { get; set; }

    /// <summary>
    /// 表示播放列表项.
    /// </summary>
    public PlaylistItem PlaylistItem { get; set; }

    /// <summary>
    /// 表示外部流.
    /// </summary>
    public ExternalStream ExtStream { get; set; }
}
