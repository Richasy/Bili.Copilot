// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.IO;
using Bili.Copilot.Libs.Player.MediaFramework.MediaPlaylist;
using Bili.Copilot.Libs.Player.MediaFramework.MediaStream;

namespace Bili.Copilot.Libs.Player.Models;

/// <summary>
/// 表示打开完成事件参数.
/// </summary>
public sealed class OpenCompletedEventArgs : EventArgs
{
    /// <summary>
    /// 初始化 <see cref="OpenCompletedEventArgs"/> 类的新实例.
    /// </summary>
    /// <param name="url">URL.</param>
    /// <param name="ioStream">IO流.</param>
    /// <param name="error">错误信息.</param>
    /// <param name="isSubtitles">是否为字幕.</param>
    public OpenCompletedEventArgs(string url = null, Stream ioStream = null, string error = null, bool isSubtitles = false)
    {
        Url = url;
        IoStream = ioStream;
        Error = error;
        IsSubtitles = isSubtitles;
    }

    /// <summary>
    /// 获取或设置URL.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// 获取或设置IO流.
    /// </summary>
    public Stream IoStream { get; set; }

    /// <summary>
    /// 获取或设置错误信息.
    /// </summary>
    public string Error { get; set; }

    /// <summary>
    /// 获取是否成功.
    /// </summary>
    public bool Success => Error == null;

    /// <summary>
    /// 获取或设置是否为字幕.
    /// </summary>
    public bool IsSubtitles { get; set; }
}

/// <summary>
/// 表示打开字幕完成的事件参数.
/// </summary>
public class OpenSubtitlesCompletedEventArgs : EventArgs
{
    /// <summary>
    /// 初始化 OpenSubtitlesCompletedEventArgs 类的新实例.
    /// </summary>
    /// <param name="url">字幕的 URL.</param>
    /// <param name="error">错误信息.</param>
    public OpenSubtitlesCompletedEventArgs(string url = null, string error = null)
    {
        Url = url;
        Error = error;
    }

    /// <summary>
    /// 获取或设置字幕的 URL.
    /// </summary>
    public string Url { get; }

    /// <summary>
    /// 获取或设置错误信息.
    /// </summary>
    public string Error { get; set; }

    /// <summary>
    /// 获取一个值，指示是否成功.
    /// </summary>
    public bool Success => Error == null;
}

/// <summary>
/// 表示打开会话完成的事件参数.
/// </summary>
public class OpenSessionCompletedEventArgs : EventArgs
{
    /// <summary>
    /// 初始化 OpenSessionCompletedEventArgs 类的新实例.
    /// </summary>
    /// <param name="session">会话.</param>
    /// <param name="error">错误信息.</param>
    public OpenSessionCompletedEventArgs(Session session = null, string error = null)
    {
        Session = session;
        Error = error;
    }

    /// <summary>
    /// 获取或设置会话.
    /// </summary>
    public Session Session { get; set; }

    /// <summary>
    /// 获取或设置错误信息.
    /// </summary>
    public string Error { get; set; }

    /// <summary>
    /// 获取一个值，指示是否成功.
    /// </summary>
    public bool Success => Error == null;
}

/// <summary>
/// 表示打开播放列表项完成的事件参数.
/// </summary>
public class OpenPlaylistItemCompletedEventArgs : EventArgs
{
    /// <summary>
    /// 初始化 OpenPlaylistItemCompletedEventArgs 类的新实例.
    /// </summary>
    /// <param name="item">播放列表项.</param>
    /// <param name="oldItem">旧的播放列表项.</param>
    /// <param name="error">错误信息.</param>
    public OpenPlaylistItemCompletedEventArgs(PlaylistItem item = null, PlaylistItem oldItem = null, string error = null)
    {
        Item = item;
        OldItem = oldItem;
        Error = error;
    }

    /// <summary>
    /// 获取或设置播放列表项.
    /// </summary>
    public PlaylistItem Item { get; set; }

    /// <summary>
    /// 获取或设置旧的播放列表项.
    /// </summary>
    public PlaylistItem OldItem { get; set; }

    /// <summary>
    /// 获取或设置错误信息.
    /// </summary>
    public string Error { get; set; }

    /// <summary>
    /// 获取一个值，指示是否成功.
    /// </summary>
    public bool Success => Error == null;
}

/// <summary>
/// 表示流打开完成的事件参数.
/// </summary>
public class StreamOpenedEventArgs : EventArgs
{
    /// <summary>
    /// 初始化 StreamOpenedEventArgs 类的新实例.
    /// </summary>
    /// <param name="stream">流.</param>
    /// <param name="oldStream">旧的流.</param>
    /// <param name="error">错误信息.</param>
    public StreamOpenedEventArgs(StreamBase stream = null, StreamBase oldStream = null, string error = null)
    {
        Stream = stream;
        OldStream = oldStream;
        Error = error;
    }

    /// <summary>
    /// 获取或设置流.
    /// </summary>
    public StreamBase Stream { get; set; }

    /// <summary>
    /// 获取或设置旧的流.
    /// </summary>
    public StreamBase OldStream { get; set; }

    /// <summary>
    /// 获取或设置错误信息.
    /// </summary>
    public string Error { get; set; }

    /// <summary>
    /// 获取一个值，指示是否成功.
    /// </summary>
    public bool Success => Error == null;
}

/// <summary>
/// 表示打开音频流完成的事件参数.
/// </summary>
public class OpenAudioStreamCompletedEventArgs : StreamOpenedEventArgs
{
    /// <summary>
    /// 初始化 OpenAudioStreamCompletedEventArgs 类的新实例.
    /// </summary>
    /// <param name="stream">音频流.</param>
    /// <param name="oldStream">旧的音频流.</param>
    /// <param name="error">错误信息.</param>
    public OpenAudioStreamCompletedEventArgs(AudioStream stream = null, AudioStream oldStream = null, string error = null)
        : base(stream, oldStream, error)
    {
    }

    /// <summary>
    /// 获取音频流.
    /// </summary>
    public new AudioStream Stream => (AudioStream)base.Stream;

    /// <summary>
    /// 获取旧的音频流.
    /// </summary>
    public new AudioStream OldStream => (AudioStream)base.OldStream;
}

/// <summary>
/// 表示打开视频流完成的事件参数.
/// </summary>
public class OpenVideoStreamCompletedEventArgs : StreamOpenedEventArgs
{
    /// <summary>
    /// 初始化 OpenVideoStreamCompletedEventArgs 类的新实例.
    /// </summary>
    /// <param name="stream">视频流.</param>
    /// <param name="oldStream">旧的视频流.</param>
    /// <param name="error">错误信息.</param>
    public OpenVideoStreamCompletedEventArgs(VideoStream stream = null, VideoStream oldStream = null, string error = null)
        : base(stream, oldStream, error)
    {
    }

    /// <summary>
    /// 获取视频流.
    /// </summary>
    public new VideoStream Stream => (VideoStream)base.Stream;

    /// <summary>
    /// 获取旧的视频流.
    /// </summary>
    public new VideoStream OldStream => (VideoStream)base.OldStream;
}

/// <summary>
/// 表示打开字幕流完成的事件参数.
/// </summary>
public class OpenSubtitlesStreamCompletedEventArgs : StreamOpenedEventArgs
{
    /// <summary>
    /// 初始化 OpenSubtitlesStreamCompletedEventArgs 类的新实例.
    /// </summary>
    /// <param name="stream">字幕流.</param>
    /// <param name="oldStream">旧的字幕流.</param>
    /// <param name="error">错误信息.</param>
    public OpenSubtitlesStreamCompletedEventArgs(SubtitlesStream stream = null, SubtitlesStream oldStream = null, string error = null)
        : base(stream, oldStream, error)
    {
    }

    /// <summary>
    /// 获取字幕流.
    /// </summary>
    public new SubtitlesStream Stream => (SubtitlesStream)base.Stream;

    /// <summary>
    /// 获取旧的字幕流.
    /// </summary>
    public new SubtitlesStream OldStream => (SubtitlesStream)base.OldStream;
}

/// <summary>
/// 表示打开外部流完成的事件参数.
/// </summary>
public class ExternalStreamOpenedEventArgs : EventArgs
{
    /// <summary>
    /// 初始化 ExternalStreamOpenedEventArgs 类的新实例.
    /// </summary>
    /// <param name="extStream">外部流.</param>
    /// <param name="oldExtStream">旧的外部流.</param>
    /// <param name="error">错误信息.</param>
    public ExternalStreamOpenedEventArgs(ExternalStream extStream = null, ExternalStream oldExtStream = null, string error = null)
    {
        ExtStream = extStream;
        OldExtStream = oldExtStream;
        Error = error;
    }

    /// <summary>
    /// 获取或设置外部流.
    /// </summary>
    public ExternalStream ExtStream { get; set; }

    /// <summary>
    /// 获取或设置旧的外部流.
    /// </summary>
    public ExternalStream OldExtStream { get; set; }

    /// <summary>
    /// 获取或设置错误信息.
    /// </summary>
    public string Error { get; set; }

    /// <summary>
    /// 获取一个值，指示是否成功.
    /// </summary>
    public bool Success => Error == null;
}

/// <summary>
/// 表示打开外部音频流完成的事件参数.
/// </summary>
public class OpenExternalAudioStreamCompletedEventArgs : ExternalStreamOpenedEventArgs
{
    /// <summary>
    /// 初始化 OpenExternalAudioStreamCompletedEventArgs 类的新实例.
    /// </summary>
    /// <param name="extStream">外部音频流.</param>
    /// <param name="oldExtStream">旧的外部音频流.</param>
    /// <param name="error">错误信息.</param>
    public OpenExternalAudioStreamCompletedEventArgs(ExternalAudioStream extStream = null, ExternalAudioStream oldExtStream = null, string error = null)
        : base(extStream, oldExtStream, error)
    {
    }

    /// <summary>
    /// 获取外部音频流.
    /// </summary>
    public new ExternalAudioStream ExtStream => (ExternalAudioStream)base.ExtStream;

    /// <summary>
    /// 获取旧的外部音频流.
    /// </summary>
    public new ExternalAudioStream OldExtStream => (ExternalAudioStream)base.OldExtStream;
}

/// <summary>
/// 表示打开外部视频流完成的事件参数.
/// </summary>
public class OpenExternalVideoStreamCompletedEventArgs : ExternalStreamOpenedEventArgs
{
    /// <summary>
    /// 初始化 OpenExternalVideoStreamCompletedEventArgs 类的新实例.
    /// </summary>
    /// <param name="extStream">外部视频流.</param>
    /// <param name="oldExtStream">旧的外部视频流.</param>
    /// <param name="error">错误信息.</param>
    public OpenExternalVideoStreamCompletedEventArgs(ExternalVideoStream extStream = null, ExternalVideoStream oldExtStream = null, string error = null)
        : base(extStream, oldExtStream, error)
    {
    }

    /// <summary>
    /// 获取外部视频流.
    /// </summary>
    public new ExternalVideoStream ExtStream => (ExternalVideoStream)base.ExtStream;

    /// <summary>
    /// 获取旧的外部视频流.
    /// </summary>
    public new ExternalVideoStream OldExtStream => (ExternalVideoStream)base.OldExtStream;
}

/// <summary>
/// 表示打开外部字幕流完成的事件参数.
/// </summary>
public class OpenExternalSubtitlesStreamCompletedEventArgs : ExternalStreamOpenedEventArgs
{
    /// <summary>
    /// 初始化 OpenExternalSubtitlesStreamCompletedEventArgs 类的新实例.
    /// </summary>
    /// <param name="extStream">外部字幕流.</param>
    /// <param name="oldExtStream">旧的外部字幕流.</param>
    /// <param name="error">错误信息.</param>
    public OpenExternalSubtitlesStreamCompletedEventArgs(ExternalSubtitleStream extStream = null, ExternalSubtitleStream oldExtStream = null, string error = null)
        : base(extStream, oldExtStream, error)
    {
    }

    /// <summary>
    /// 获取外部字幕流.
    /// </summary>
    public new ExternalSubtitleStream ExtStream => (ExternalSubtitleStream)base.ExtStream;

    /// <summary>
    /// 获取旧的外部字幕流.
    /// </summary>
    public new ExternalSubtitleStream OldExtStream => (ExternalSubtitleStream)base.OldExtStream;
}
