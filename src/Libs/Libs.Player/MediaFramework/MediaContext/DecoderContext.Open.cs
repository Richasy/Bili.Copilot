// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Player.Core;
using Bili.Copilot.Libs.Player.Enums;
using Bili.Copilot.Libs.Player.MediaFramework.MediaDemuxer;
using Bili.Copilot.Libs.Player.MediaFramework.MediaPlaylist;
using Bili.Copilot.Libs.Player.MediaFramework.MediaStream;
using Bili.Copilot.Libs.Player.Models;

using static Bili.Copilot.Libs.Player.Misc.Logger;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaContext;

/// <summary>
/// 解码器上下文类.
/// </summary>
public partial class DecoderContext
{
    /// <summary>
    /// 打开完成事件.
    /// </summary>
    public event EventHandler<OpenCompletedEventArgs> OpenCompleted;

    /// <summary>
    /// 打开会话完成事件.
    /// </summary>
    public event EventHandler<OpenSessionCompletedEventArgs> OpenSessionCompleted;

    /// <summary>
    /// 打开字幕完成事件.
    /// </summary>
    public event EventHandler<OpenSubtitlesCompletedEventArgs> OpenSubtitlesCompleted;

    /// <summary>
    /// 打开播放列表项完成事件.
    /// </summary>
    public event EventHandler<OpenPlaylistItemCompletedEventArgs> OpenPlaylistItemCompleted;

    /// <summary>
    /// 打开音频流完成事件.
    /// </summary>
    public event EventHandler<OpenAudioStreamCompletedEventArgs> OpenAudioStreamCompleted;

    /// <summary>
    /// 打开视频流完成事件.
    /// </summary>
    public event EventHandler<OpenVideoStreamCompletedEventArgs> OpenVideoStreamCompleted;

    /// <summary>
    /// 打开字幕流完成事件.
    /// </summary>
    public event EventHandler<OpenSubtitlesStreamCompletedEventArgs> OpenSubtitlesStreamCompleted;

    /// <summary>
    /// 打开外部音频流完成事件.
    /// </summary>
    public event EventHandler<OpenExternalAudioStreamCompletedEventArgs> OpenExternalAudioStreamCompleted;

    /// <summary>
    /// 打开外部视频流完成事件.
    /// </summary>
    public event EventHandler<OpenExternalVideoStreamCompletedEventArgs> OpenExternalVideoStreamCompleted;

    /// <summary>
    /// 打开外部字幕流完成事件.
    /// </summary>
    public event EventHandler<OpenExternalSubtitlesStreamCompletedEventArgs> OpenExternalSubtitlesStreamCompleted;

    /// <summary>
    /// 打开指定 URL 的媒体流.
    /// </summary>
    /// <param name="url">媒体流的 URL.</param>
    /// <param name="defaultPlaylistItem">是否使用默认的播放列表项.</param>
    /// <param name="defaultVideo">是否使用默认的视频流.</param>
    /// <param name="defaultAudio">是否使用默认的音频流.</param>
    /// <param name="defaultSubtitles">是否使用默认的字幕流.</param>
    /// <returns>打开媒体流的结果.</returns>
    public OpenCompletedEventArgs Open(string url, bool defaultPlaylistItem = true, bool defaultVideo = true, bool defaultAudio = true, bool defaultSubtitles = true)
        => Open((object)url, defaultPlaylistItem, defaultVideo, defaultAudio, defaultSubtitles);

    /// <summary>
    /// 打开指定流的媒体流.
    /// </summary>
    /// <param name="iostream">媒体流的输入流.</param>
    /// <param name="defaultPlaylistItem">是否使用默认的播放列表项.</param>
    /// <param name="defaultVideo">是否使用默认的视频流.</param>
    /// <param name="defaultAudio">是否使用默认的音频流.</param>
    /// <param name="defaultSubtitles">是否使用默认的字幕流.</param>
    /// <returns>打开媒体流的结果.</returns>
    public OpenCompletedEventArgs Open(Stream iostream, bool defaultPlaylistItem = true, bool defaultVideo = true, bool defaultAudio = true, bool defaultSubtitles = true)
        => Open((object)iostream, defaultPlaylistItem, defaultVideo, defaultAudio, defaultSubtitles);

    /// <summary>
    /// 打开指定 URL 的字幕流.
    /// </summary>
    /// <param name="url">字幕流的 URL.</param>
    /// <returns>打开字幕流的结果.</returns>
    public new OpenSubtitlesCompletedEventArgs OpenSubtitle(string url)
    {
        OpenSubtitlesCompletedEventArgs args = new();

        try
        {
            var res = base.OpenSubtitle(url);
            args.Error = res == null ? "未找到外部字幕流" : res.Error;

            if (args.Success)
            {
                args.Error = Open(res.ExternalSubtitleStream).Error;
            }

            return args;
        }
        catch (Exception e)
        {
            args.Error = !args.Success ? args.Error + "\r\n" + e.Message : e.Message;
            return args;
        }
        finally
        {
            OnOpenSubtitles(args);
        }
    }

    /// <summary>
    /// 打开指定会话的媒体流.
    /// </summary>
    /// <param name="session">要打开的会话.</param>
    /// <returns>打开媒体流的结果.</returns>
    public OpenSessionCompletedEventArgs Open(Session session)
    {
        OpenSessionCompletedEventArgs args = new(session);

        try
        {
            // 打开
            if (session.Url != null && session.Url != Playlist.Url)
            {
                args.Error = Open(session.Url, false, false, false, false).Error;
                if (!args.Success)
                {
                    return args;
                }
            }

            // 打开播放列表项
            if (session.PlaylistItem != -1)
            {
                args.Error = Open(Playlist.Items[session.PlaylistItem], false, false, false).Error;
                if (!args.Success)
                {
                    return args;
                }
            }

            // 打开视频流
            if (session.ExternalVideoStream != -1)
            {
                args.Error = Open(Playlist.Selected.ExternalVideoStreams[session.ExternalVideoStream], false, session.VideoStream).Error;
                if (!args.Success)
                {
                    return args;
                }
            }
            else if (session.VideoStream != -1)
            {
                args.Error = Open(VideoDemuxer.AVStreamToStream[session.VideoStream], false).Error;
                if (!args.Success)
                {
                    return args;
                }
            }

            string tmpErr = null;

            // 打开音频流
            if (session.ExternalAudioStream != -1)
            {
                tmpErr = Open(Playlist.Selected.ExternalAudioStreams[session.ExternalAudioStream], false, session.AudioStream).Error;
            }
            else if (session.AudioStream != -1)
            {
                tmpErr = Open(VideoDemuxer.AVStreamToStream[session.AudioStream], false).Error;
            }

            if (tmpErr != null & VideoStream == null)
            {
                args.Error = tmpErr;
                return args;
            }

            // 打开字幕流
            if (session.ExternalSubtitlesUrl != null)
            {
                OpenSubtitle(session.ExternalSubtitlesUrl);
            }
            else if (session.SubtitlesStream != -1)
            {
                Open(VideoDemuxer.AVStreamToStream[session.SubtitlesStream]);
            }

            Config.Audio.SetDelay(session.AudioDelay);
            Config.Subtitles.SetDelay(session.SubtitlesDelay);

            if (session.CurTime > 1 * 1000L * 10000)
            {
                Seek(session.CurTime / 10000);
            }

            return args;
        }
        catch (Exception e)
        {
            args.Error = !args.Success ? args.Error + "\r\n" + e.Message : e.Message;
            return args;
        }
        finally
        {
            OnOpenSessionCompleted(args);
        }
    }

    /// <summary>
    /// 打开指定播放列表项的媒体流.
    /// </summary>
    /// <param name="item">要打开的播放列表项.</param>
    /// <param name="defaultVideo">是否使用默认的视频流.</param>
    /// <param name="defaultAudio">是否使用默认的音频流.</param>
    /// <param name="defaultSubtitles">是否使用默认的字幕流.</param>
    /// <returns>打开媒体流的结果.</returns>
    public OpenPlaylistItemCompletedEventArgs Open(PlaylistItem item, bool defaultVideo = true, bool defaultAudio = true, bool defaultSubtitles = true)
    {
        OpenPlaylistItemCompletedEventArgs args = new(item);

        try
        {
            var stoppedTime = GetCurTime();
            InitializeSwitch();

            // 禁用旧的播放列表项
            if (Playlist.Selected != null)
            {
                args.OldItem = Playlist.Selected;
                Playlist.Selected.Enabled = false;
            }

            if (item == null)
            {
                args.Error = "已取消";
                return args;
            }

            Playlist.Selected = item;
            Playlist.Selected.Enabled = true;

            // 重置当前项的外部流
            if (Playlist.Selected.ExternalAudioStream != null)
            {
                Playlist.Selected.ExternalAudioStream.Enabled = false;
                Playlist.Selected.ExternalAudioStream = null;
            }

            if (Playlist.Selected.ExternalVideoStream != null)
            {
                Playlist.Selected.ExternalVideoStream.Enabled = false;
                Playlist.Selected.ExternalVideoStream = null;
            }

            if (Playlist.Selected.ExternalSubtitlesStream != null)
            {
                Playlist.Selected.ExternalSubtitlesStream.Enabled = false;
                Playlist.Selected.ExternalSubtitlesStream = null;
            }

            args.Error = OpenItem().Error;

            if (!args.Success)
            {
                return args;
            }

            if (Playlist.Selected.Url != null || Playlist.Selected.IoStream != null)
            {
                args.Error = OpenDemuxerInput(VideoDemuxer, Playlist.Selected);
            }

            if (!args.Success)
            {
                return args;
            }

            if (defaultVideo && Config.Video.Enabled)
            {
                args.Error = OpenSuggestedVideo(defaultAudio);
            }
            else if (defaultAudio && Config.Audio.Enabled)
            {
                args.Error = OpenSuggestedAudio();
            }

            if ((defaultVideo || defaultAudio) && AudioStream == null && VideoStream == null)
            {
                args.Error ??= "未找到音频/视频流";

                return args;
            }

            if (defaultSubtitles && Config.Subtitles.Enabled)
            {
                if (Playlist.Selected.ExternalSubtitlesStream != null)
                {
                    Open(Playlist.Selected.ExternalSubtitlesStream);
                }
                else
                {
                    OpenSuggestedSubtitles();
                }
            }

            return args;
        }
        catch (Exception e)
        {
            args.Error = !args.Success ? args.Error + "\r\n" + e.Message : e.Message;
            return args;
        }
        finally
        {
            OnOpenPlaylistItemCompleted(args);
        }
    }

    /// <summary>
    /// 打开外部流.
    /// </summary>
    /// <param name="extStream">外部流对象.</param>
    /// <param name="defaultAudio">是否默认音频流.</param>
    /// <param name="streamIndex">流索引.</param>
    /// <returns>打开外部流的结果.</returns>
    public ExternalStreamOpenedEventArgs Open(ExternalStream extStream, bool defaultAudio = false, int streamIndex = -1)
    {
        ExternalStreamOpenedEventArgs args = null;

        try
        {
            Demuxer demuxer;

            if (extStream is ExternalVideoStream stream)
            {
                args = new OpenExternalVideoStreamCompletedEventArgs(stream, Playlist.Selected.ExternalVideoStream);

                if (args.OldExtStream != null)
                {
                    args.OldExtStream.Enabled = false;
                }

                Playlist.Selected.ExternalVideoStream = stream;

                foreach (var plugin in Plugins.Values)
                {
                    plugin.OnOpenExternalVideo();
                }

                demuxer = VideoDemuxer;
            }
            else if (extStream is ExternalAudioStream audioStream)
            {
                args = new OpenExternalAudioStreamCompletedEventArgs(audioStream, Playlist.Selected.ExternalAudioStream);

                if (args.OldExtStream != null)
                {
                    args.OldExtStream.Enabled = false;
                }

                Playlist.Selected.ExternalAudioStream = audioStream;

                foreach (var plugin in Plugins.Values)
                {
                    plugin.OnOpenExternalAudio();
                }

                demuxer = AudioDemuxer;
            }
            else
            {
                args = new OpenExternalSubtitlesStreamCompletedEventArgs((ExternalSubtitleStream)extStream, Playlist.Selected.ExternalSubtitlesStream);

                if (args.OldExtStream != null)
                {
                    args.OldExtStream.Enabled = false;
                }

                Playlist.Selected.ExternalSubtitlesStream = (ExternalSubtitleStream)extStream;

                if (!Playlist.Selected.ExternalSubtitlesStream.Downloaded)
                {
                    DownloadSubtitle(Playlist.Selected.ExternalSubtitlesStream);
                }

                foreach (var plugin in Plugins.Values)
                {
                    plugin.OnOpenExternalSubtitles();
                }

                demuxer = SubtitlesDemuxer;
            }

            // 打开外部流
            args.Error = OpenDemuxerInput(demuxer, extStream);

            if (!args.Success)
            {
                return args;
            }

            // 更新嵌入流的外部流指针
            foreach (var embStream in demuxer.VideoStreams)
            {
                embStream.ExternalStream = extStream;
            }

            foreach (var embStream in demuxer.AudioStreams)
            {
                embStream.ExternalStream = extStream;
            }

            foreach (var embStream in demuxer.SubtitlesStreams)
            {
                embStream.ExternalStream = extStream;
            }

            // 打开嵌入流
            if (streamIndex != -2)
            {
                StreamBase suggestedStream = null;
                if (streamIndex != -1 && (streamIndex >= demuxer.AVStreamToStream.Count || streamIndex < 0 || demuxer.AVStreamToStream[streamIndex].Type != extStream.Type))
                {
                    args.Error = $"无效的流索引 {streamIndex}";
                    demuxer.Dispose();
                    return args;
                }

                if (demuxer.Type == MediaType.Video)
                {
                    suggestedStream = streamIndex == -1 ? SuggestVideo(demuxer.VideoStreams) : demuxer.AVStreamToStream[streamIndex];
                }
                else if (demuxer.Type == MediaType.Audio)
                {
                    suggestedStream = streamIndex == -1 ? SuggestAudio(demuxer.AudioStreams) : demuxer.AVStreamToStream[streamIndex];
                }
                else
                {
                    var langs = Config.Subtitles.Languages.ToList();
                    langs.Add(Language.Unknown);
                    suggestedStream = streamIndex == -1 ? SuggestSubtitles(demuxer.SubtitlesStreams, langs) : demuxer.AVStreamToStream[streamIndex];
                }

                if (suggestedStream == null)
                {
                    demuxer.Dispose();
                    args.Error = "未找到嵌入流";
                    return args;
                }

                args.Error = Open(suggestedStream, defaultAudio).Error;
                if (!args.Success)
                {
                    return args;
                }
            }

            extStream.Enabled = true;

            return args;
        }
        catch (Exception e)
        {
            args.Error = !args.Success ? args.Error + "\r\n" + e.Message : e.Message;
            return args;
        }
        finally
        {
            if (extStream is ExternalVideoStream)
            {
                OnOpenExternalVideoStreamCompleted((OpenExternalVideoStreamCompletedEventArgs)args);
            }
            else if (extStream is ExternalAudioStream)
            {
                OnOpenExternalAudioStreamCompleted((OpenExternalAudioStreamCompletedEventArgs)args);
            }
            else
            {
                OnOpenExternalSubtitlesStreamCompleted((OpenExternalSubtitlesStreamCompletedEventArgs)args);
            }
        }
    }

    /// <summary>
    /// 打开视频流.
    /// </summary>
    /// <param name="stream">视频流对象.</param>
    /// <param name="defaultAudio">是否默认音频流.</param>
    /// <returns>打开视频流的结果.</returns>
    public StreamOpenedEventArgs OpenVideoStream(VideoStream stream, bool defaultAudio = true)
        => Open(stream, defaultAudio);

    /// <summary>
    /// 打开音频流.
    /// </summary>
    /// <param name="stream">音频流对象.</param>
    /// <returns>打开音频流的结果.</returns>
    public StreamOpenedEventArgs OpenAudioStream(AudioStream stream)
        => Open(stream);

    /// <summary>
    /// 打开字幕流.
    /// </summary>
    /// <param name="stream">字幕流对象.</param>
    /// <returns>打开字幕流的结果.</returns>
    public StreamOpenedEventArgs OpenSubtitlesStream(SubtitlesStream stream)
        => Open(stream);

    /// <summary>
    /// 打开建议的视频流.
    /// </summary>
    /// <param name="defaultAudio">是否默认音频.</param>
    /// <returns>错误信息.</returns>
    public string OpenSuggestedVideo(bool defaultAudio = false)
    {
        VideoStream stream;
        ExternalVideoStream extStream;
        string error = null;

        if (ClosedVideoStream != null)
        {
            Log.Debug("[Video] Found previously closed stream");

            extStream = ClosedVideoStream.Item1;
            if (extStream != null)
            {
                return Open(extStream, false, ClosedVideoStream.Item2 >= 0 ? ClosedVideoStream.Item2 : -1).Error;
            }

            stream = ClosedVideoStream.Item2 >= 0 ? (VideoStream)VideoDemuxer.AVStreamToStream[ClosedVideoStream.Item2] : null;
        }
        else
        {
            SuggestVideo(out stream, out extStream, VideoDemuxer.VideoStreams);
        }

        if (stream != null)
        {
            error = Open(stream, defaultAudio).Error;
        }
        else if (extStream != null)
        {
            error = Open(extStream, defaultAudio).Error;
        }
        else if (defaultAudio)
        {
            error = OpenSuggestedAudio(); // We still need audio if no video exists
        }

        return error;
    }

    /// <summary>
    /// 打开建议的音频流.
    /// </summary>
    /// <returns>错误信息.</returns>
    public string OpenSuggestedAudio()
    {
        AudioStream stream = null;
        ExternalAudioStream extStream = null;
        string error = null;

        if (ClosedAudioStream != null)
        {
            Log.Debug("[Audio] Found previously closed stream");

            extStream = ClosedAudioStream.Item1;
            if (extStream != null)
            {
                return Open(extStream, false, ClosedAudioStream.Item2 >= 0 ? ClosedAudioStream.Item2 : -1).Error;
            }

            stream = ClosedAudioStream.Item2 >= 0 ? (AudioStream)VideoDemuxer.AVStreamToStream[ClosedAudioStream.Item2] : null;
        }
        else
        {
            SuggestAudio(out stream, out extStream, VideoDemuxer.AudioStreams);
        }

        if (stream != null)
        {
            error = Open(stream).Error;
        }
        else if (extStream != null)
        {
            error = Open(extStream).Error;
        }

        return error;
    }

    /// <summary>
    /// 打开建议的字幕流.
    /// </summary>
    public void OpenSuggestedSubtitles()
    {
        var sessionId = OpenItemCounter;

        try
        {
            // 1. Closed / History / Remember last user selection? Probably application must handle this
            if (ClosedSubtitlesStream != null)
            {
                Log.Debug("[Subtitles] Found previously closed stream");

                var extStream = ClosedSubtitlesStream.Item1;
                if (extStream != null)
                {
                    Open(extStream, false, ClosedSubtitlesStream.Item2 >= 0 ? ClosedSubtitlesStream.Item2 : -1);
                    return;
                }

                var stream = ClosedSubtitlesStream.Item2 >= 0 ? (SubtitlesStream)VideoDemuxer.AVStreamToStream[ClosedSubtitlesStream.Item2] : null;

                if (stream != null)
                {
                    Open(stream);
                    return;
                }
                else if (extStream != null)
                {
                    Open(extStream);
                    return;
                }
                else
                {
                    ClosedSubtitlesStream = null;
                }
            }

            // High Suggest (first lang priority + high rating + already converted/downloaded)
            // 2. Check embedded steams for high suggest
            if (Config.Subtitles.Languages.Count > 0)
            {
                foreach (var stream in VideoDemuxer.SubtitlesStreams)
                {
                    if (stream.Language == Config.Subtitles.Languages[0])
                    {
                        Log.Debug("[Subtitles] Found high suggested embedded stream");
                        Open(stream);
                        return;
                    }
                }
            }

            // 3. Check external streams for high suggest
            if (Playlist.Selected.ExternalSubtitlesStreams.Count > 0)
            {
                var extStream = SuggestBestExternalSubtitle();
                if (extStream != null)
                {
                    Log.Debug("[Subtitles] Found high suggested external stream");
                    Open(extStream);
                    return;
                }
            }
        }
        catch (Exception e)
        {
            Log.Debug($"OpenSuggestedSubtitles canceled? [{e.Message}]");
            return;
        }

        Task.Run(() =>
        {
            try
            {
                if (sessionId != OpenItemCounter)
                {
                    Log.Debug("OpenSuggestedSubtitles canceled");
                    return;
                }

                ExternalSubtitleStream extStream;

                // 4. Search offline if allowed (not async)
                if (!Playlist.Selected.SearchedLocal && Config.Subtitles.SearchLocal && (Config.Subtitles.SearchLocalOnInputType == null || Config.Subtitles.SearchLocalOnInputType.Count == 0 || Config.Subtitles.SearchLocalOnInputType.Contains(Playlist.InputType)))
                {
                    Log.Debug("[Subtitles] Searching Local");
                    SearchLocalSubtitle();

                    // 4.1 Check external streams for high suggest (again for the new additions if any)
                    extStream = SuggestBestExternalSubtitle();
                    if (extStream != null)
                    {
                        Log.Debug("[Subtitles] Found high suggested external stream");
                        Open(extStream);
                        return;
                    }
                }

                if (sessionId != OpenItemCounter)
                {
                    Log.Debug("OpenSuggestedSubtitles canceled");
                    return;
                }

                // 5. Search online if allowed (not async)
                if (!Playlist.Selected.SearchedOnline && Config.Subtitles.SearchOnline && (Config.Subtitles.SearchOnlineOnInputType == null || Config.Subtitles.SearchOnlineOnInputType.Count == 0 || Config.Subtitles.SearchOnlineOnInputType.Contains(Playlist.InputType)))
                {
                    Log.Debug("[Subtitles] Searching Online");
                    SearchOnlineSubtitle();
                }

                if (sessionId != OpenItemCounter)
                {
                    Log.Debug("OpenSuggestedSubtitles canceled");
                    return;
                }

                // 6. (Any) Check embedded/external streams for config languages (including 'undefined')
                SuggestSubtitle(out var stream, out extStream);

                if (stream != null)
                {
                    Open(stream);
                }
                else if (extStream != null)
                {
                    Open(extStream);
                }
            }
            catch (Exception e)
            {
                Log.Debug($"OpenSuggestedSubtitles canceled? [{e.Message}]");
            }
        });
    }

    /// <summary>
    /// 打开解复用器输入.
    /// </summary>
    /// <param name="demuxer">解复用器.</param>
    /// <param name="demuxerInput">解复用器输入.</param>
    /// <returns>错误信息.</returns>
    public string OpenDemuxerInput(Demuxer demuxer, DemuxerInput demuxerInput)
    {
        OpenedPlugin?.OnBuffering();

        string error = null;

        Dictionary<string, string> formatOpt = null;
        Dictionary<string, string> copied = null;

        try
        {
            // Set HTTP Config
            if (Playlist.InputType == InputType.Web)
            {
                formatOpt = Config.Demuxer.GetFormatOptPtr(demuxer.Type);
                copied = new Dictionary<string, string>();

                foreach (var opt in formatOpt)
                {
                    copied.Add(opt.Key, opt.Value);
                }

                if (demuxerInput.UserAgent != null)
                {
                    formatOpt["user_agent"] = demuxerInput.UserAgent;
                }

                if (demuxerInput.Referrer != null)
                {
                    formatOpt["referer"] = demuxerInput.Referrer;
                }
                else if (!formatOpt.ContainsKey("referer") && Playlist.Url != null)
                {
                    formatOpt["referer"] = Playlist.Url;
                }

                if (demuxerInput.HttpHeaders != null)
                {
                    formatOpt["headers"] = string.Empty;
                    foreach (var header in demuxerInput.HttpHeaders)
                    {
                        formatOpt["headers"] += header.Key + ": " + header.Value + "\r\n";
                    }
                }
            }

            // Open Demuxer Input
            if (demuxerInput.Url != null)
            {
                error = demuxer.Open(demuxerInput.Url);
                if (error != null)
                {
                    return error;
                }

                if (!string.IsNullOrEmpty(demuxerInput.UrlFallback))
                {
                    Log.Warn($"Fallback to {demuxerInput.UrlFallback}");
                    error = demuxer.Open(demuxerInput.UrlFallback);
                }
            }
            else if (demuxerInput.IoStream != null)
            {
                error = demuxer.Open(demuxerInput.IoStream);
            }

            return error;
        }
        finally
        {
            // Restore HTTP Config
            if (Playlist.InputType == InputType.Web)
            {
                formatOpt.Clear();
                foreach (var opt in copied)
                {
                    formatOpt.Add(opt.Key, opt.Value);
                }
            }

            OpenedPlugin?.OnBufferingCompleted();
        }
    }

    /// <summary>
    /// 关闭音频流.
    /// </summary>
    public void CloseAudio()
    {
        ClosedAudioStream = new Tuple<ExternalAudioStream, int>(Playlist.Selected.ExternalAudioStream, AudioStream != null ? AudioStream.StreamIndex : -1);

        if (Playlist.Selected.ExternalAudioStream != null)
        {
            Playlist.Selected.ExternalAudioStream.Enabled = false;
            Playlist.Selected.ExternalAudioStream = null;
        }

        AudioDecoder.Dispose(true);
    }

    /// <summary>
    /// 关闭视频流.
    /// </summary>
    public void CloseVideo()
    {
        ClosedVideoStream = new Tuple<ExternalVideoStream, int>(Playlist.Selected.ExternalVideoStream, VideoStream != null ? VideoStream.StreamIndex : -1);

        if (Playlist.Selected.ExternalVideoStream != null)
        {
            Playlist.Selected.ExternalVideoStream.Enabled = false;
            Playlist.Selected.ExternalVideoStream = null;
        }

        VideoDecoder.Dispose(true);
        VideoDecoder.Renderer?.ClearScreen();
    }

    /// <summary>
    /// 关闭字幕流.
    /// </summary>
    public void CloseSubtitles()
    {
        ClosedSubtitlesStream = new Tuple<ExternalSubtitleStream, int>(Playlist.Selected.ExternalSubtitlesStream, SubtitlesStream != null ? SubtitlesStream.StreamIndex : -1);

        if (Playlist.Selected.ExternalSubtitlesStream != null)
        {
            Playlist.Selected.ExternalSubtitlesStream.Enabled = false;
            Playlist.Selected.ExternalSubtitlesStream = null;
        }

        SubtitlesDecoder.Dispose(true);
    }

    /// <summary>
    /// 打开.
    /// </summary>
    /// <param name="input">输入对象.</param>
    /// <param name="defaultPlaylistItem">是否默认播放列表项.</param>
    /// <param name="defaultVideo">是否默认视频.</param>
    /// <param name="defaultAudio">是否默认音频.</param>
    /// <param name="defaultSubtitles">是否默认字幕.</param>
    /// <returns>打开的结果.</returns>
    internal OpenCompletedEventArgs Open(object input, bool defaultPlaylistItem = true, bool defaultVideo = true, bool defaultAudio = true, bool defaultSubtitles = true)
    {
        OpenCompletedEventArgs args = new();

        try
        {
            Initialize();

            if (input is Stream stream)
            {
                Playlist.IOStream = stream;
            }
            else
            {
                Playlist.Url = input.ToString(); // TBR: check UI update
            }

            args.Url = Playlist.Url;
            args.IoStream = Playlist.IOStream;
            args.Error = Open().Error;

            if (Playlist.Items.Count == 0 && args.Success)
            {
                args.Error = "未找到播放列表项";
            }

            if (!args.Success)
            {
                return args;
            }

            if (!defaultPlaylistItem)
            {
                return args;
            }

            args.Error = Open(SuggestItem(), defaultVideo, defaultAudio, defaultSubtitles).Error;

            return args;
        }
        catch (Exception e)
        {
            args.Error = !args.Success ? args.Error + "\r\n" + e.Message : e.Message;
            return args;
        }
        finally
        {
            OnOpenCompleted(args);
        }
    }

    private StreamOpenedEventArgs Open(StreamBase stream, bool defaultAudio = false)
    {
        StreamOpenedEventArgs args = null;

        try
        {
            lock (stream.Demuxer.LockActions)
                lock (stream.Demuxer.lockFmtCtx)
                {
                    var oldStream = stream.Type == MediaType.Video ? VideoStream : (stream.Type == MediaType.Audio ? AudioStream : (StreamBase)SubtitlesStream);

                    // Close external demuxers when opening embedded
                    if (stream.Demuxer.Type == MediaType.Video)
                    {
                        // TBR: if (stream.Type == MediaType.Video) | We consider that we can't have Embedded and External Video Streams at the same time
                        if (stream.Type == MediaType.Audio) // TBR: && VideoStream != null)
                        {
                            if (!EnableDecoding)
                            {
                                AudioDemuxer.Dispose();
                            }

                            if (Playlist.Selected.ExternalAudioStream != null)
                            {
                                Playlist.Selected.ExternalAudioStream.Enabled = false;
                                Playlist.Selected.ExternalAudioStream = null;
                            }
                        }
                        else if (stream.Type == MediaType.Subtitle)
                        {
                            if (!EnableDecoding)
                            {
                                SubtitlesDemuxer.Dispose();
                            }

                            if (Playlist.Selected.ExternalSubtitlesStream != null)
                            {
                                Playlist.Selected.ExternalSubtitlesStream.Enabled = false;
                                Playlist.Selected.ExternalSubtitlesStream = null;
                            }
                        }
                    }
                    else if (!EnableDecoding)
                    {
                        // Disable embeded audio when enabling external audio (TBR)
                        if (stream.Demuxer.Type == MediaType.Audio && stream.Type == MediaType.Audio && AudioStream != null && AudioStream.Demuxer.Type == MediaType.Video)
                        {
                            foreach (var aStream in VideoDemuxer.AudioStreams)
                            {
                                VideoDemuxer.DisableStream(aStream);
                            }
                        }
                    }

                    // Open Codec / Enable on demuxer
                    if (EnableDecoding)
                    {
                        var ret = GetDecoderPtr(stream.Type).Open(stream);

                        if (ret != null)
                        {
                            return stream.Type == MediaType.Video
                            ? (args = new OpenVideoStreamCompletedEventArgs((VideoStream)stream, (VideoStream)oldStream, $"Failed to open video stream #{stream.StreamIndex}\r\n{ret}"))
                            : stream.Type == MediaType.Audio
                            ? (args = new OpenAudioStreamCompletedEventArgs((AudioStream)stream, (AudioStream)oldStream, $"Failed to open audio stream #{stream.StreamIndex}\r\n{ret}"))
                            : (args = new OpenSubtitlesStreamCompletedEventArgs((SubtitlesStream)stream, (SubtitlesStream)oldStream, $"Failed to open subtitles stream #{stream.StreamIndex}\r\n{ret}"));
                        }
                    }
                    else
                    {
                        stream.Demuxer.EnableStream(stream);
                    }

                    // Open Audio based on new Video Stream (if not the same suggestion)
                    if (defaultAudio && stream.Type == MediaType.Video && Config.Audio.Enabled)
                    {
                        var requiresChange = true;
                        SuggestAudio(out var aStream, out var aExtStream, VideoDemuxer.AudioStreams);

                        if (AudioStream != null)
                        {
                            // External audio streams comparison
                            if (Playlist.Selected.ExternalAudioStream != null && aExtStream != null && aExtStream.Index == Playlist.Selected.ExternalAudioStream.Index)
                            {
                                requiresChange = false;
                            }

                            // Embedded audio streams comparison
                            else if (Playlist.Selected.ExternalAudioStream == null && aStream != null && aStream.StreamIndex == AudioStream.StreamIndex)
                            {
                                requiresChange = false;
                            }
                        }

                        if (!requiresChange)
                        {
                            if (CanDebug)
                            {
                                Log.Debug($"Audio no need to follow video");
                            }
                        }
                        else
                        {
                            if (aStream != null)
                            {
                                Open(aStream);
                            }
                            else if (aExtStream != null)
                            {
                                Open(aExtStream);
                            }
                        }
                    }

                    return stream.Type == MediaType.Video
                    ? (args = new OpenVideoStreamCompletedEventArgs((VideoStream)stream, (VideoStream)oldStream))
                    : stream.Type == MediaType.Audio
                    ? (args = new OpenAudioStreamCompletedEventArgs((AudioStream)stream, (AudioStream)oldStream))
                    : (args = new OpenSubtitlesStreamCompletedEventArgs((SubtitlesStream)stream, (SubtitlesStream)oldStream));
                }
        }
        catch (Exception e)
        {
            return args = new StreamOpenedEventArgs(null, null, e.Message);
        }
        finally
        {
            if (stream.Type == MediaType.Video)
            {
                OnOpenVideoStreamCompleted((OpenVideoStreamCompletedEventArgs)args);
            }
            else if (stream.Type == MediaType.Audio)
            {
                OnOpenAudioStreamCompleted((OpenAudioStreamCompletedEventArgs)args);
            }
            else
            {
                OnOpenSubtitlesStreamCompleted((OpenSubtitlesStreamCompletedEventArgs)args);
            }
        }
    }

    private void OnOpenCompleted(OpenCompletedEventArgs args = null)
    {
        if (_shouldDispose)
        {
            Dispose();
            return;
        }

        VideoDecoder.Renderer?.ClearScreen();
        if (CanInfo)
        {
            Log.Info($"[Open] {args.Url ?? "None"} {(!args.Success ? " [Error: " + args.Error + "]" : string.Empty)}");
        }

        OpenCompleted?.Invoke(this, args);
    }

    private void OnOpenSessionCompleted(OpenSessionCompletedEventArgs args = null)
    {
        if (_shouldDispose)
        {
            Dispose();
            return;
        }

        VideoDecoder.Renderer?.ClearScreen();
        if (CanInfo)
        {
            Log.Info($"[OpenSession] {args.Session.Url ?? "None"} - Item: {args.Session.PlaylistItem} {(!args.Success ? " [Error: " + args.Error + "]" : string.Empty)}");
        }

        OpenSessionCompleted?.Invoke(this, args);
    }

    private void OnOpenSubtitles(OpenSubtitlesCompletedEventArgs args = null)
    {
        if (_shouldDispose)
        {
            Dispose();
            return;
        }

        if (CanInfo)
        {
            Log.Info($"[OpenSubtitles] {args.Url ?? "None"} {(!args.Success ? " [Error: " + args.Error + "]" : string.Empty)}");
        }

        OpenSubtitlesCompleted?.Invoke(this, args);
    }

    private void OnOpenPlaylistItemCompleted(OpenPlaylistItemCompletedEventArgs args = null)
    {
        if (_shouldDispose)
        {
            Dispose();
            return;
        }

        VideoDecoder.Renderer?.ClearScreen();
        if (CanInfo)
        {
            Log.Info($"[OpenPlaylistItem] {(args.OldItem != null ? args.OldItem.Title : "None")} => {(args.Item != null ? args.Item.Title : "None")}{(!args.Success ? " [Error: " + args.Error + "]" : string.Empty)}");
        }

        OpenPlaylistItemCompleted?.Invoke(this, args);
    }

    private void OnOpenAudioStreamCompleted(OpenAudioStreamCompletedEventArgs args = null)
    {
        if (_shouldDispose)
        {
            Dispose();
            return;
        }

        ClosedAudioStream = null;
        MainDemuxer = !VideoDemuxer.Disposed ? VideoDemuxer : AudioDemuxer;

        if (CanInfo)
        {
            Log.Info($"[OpenAudioStream] #{(args.OldStream != null ? args.OldStream.StreamIndex.ToString() : "_")} => #{(args.Stream != null ? args.Stream.StreamIndex.ToString() : "_")}{(!args.Success ? " [Error: " + args.Error + "]" : string.Empty)}");
        }

        OpenAudioStreamCompleted?.Invoke(this, args);
    }

    private void OnOpenVideoStreamCompleted(OpenVideoStreamCompletedEventArgs args = null)
    {
        if (_shouldDispose)
        {
            Dispose();
            return;
        }

        ClosedVideoStream = null;
        MainDemuxer = !VideoDemuxer.Disposed ? VideoDemuxer : AudioDemuxer;

        if (CanInfo)
        {
            Log.Info($"[OpenVideoStream] #{(args.OldStream != null ? args.OldStream.StreamIndex.ToString() : "_")} => #{(args.Stream != null ? args.Stream.StreamIndex.ToString() : "_")}{(!args.Success ? " [Error: " + args.Error + "]" : string.Empty)}");
        }

        OpenVideoStreamCompleted?.Invoke(this, args);
    }

    private void OnOpenSubtitlesStreamCompleted(OpenSubtitlesStreamCompletedEventArgs args = null)
    {
        if (_shouldDispose)
        {
            Dispose();
            return;
        }

        ClosedSubtitlesStream = null;

        if (CanInfo)
        {
            Log.Info($"[OpenSubtitlesStream] #{(args.OldStream != null ? args.OldStream.StreamIndex.ToString() : "_")} => #{(args.Stream != null ? args.Stream.StreamIndex.ToString() : "_")}{(!args.Success ? " [Error: " + args.Error + "]" : string.Empty)}");
        }

        OpenSubtitlesStreamCompleted?.Invoke(this, args);
    }

    private void OnOpenExternalAudioStreamCompleted(OpenExternalAudioStreamCompletedEventArgs args = null)
    {
        if (_shouldDispose)
        {
            Dispose();
            return;
        }

        ClosedAudioStream = null;
        MainDemuxer = !VideoDemuxer.Disposed ? VideoDemuxer : AudioDemuxer;

        if (CanInfo)
        {
            Log.Info($"[OpenExternalAudioStream] {(args.OldExtStream != null ? args.OldExtStream.Url : "None")} => {(args.ExtStream != null ? args.ExtStream.Url : "None")}{(!args.Success ? " [Error: " + args.Error + "]" : string.Empty)}");
        }

        OpenExternalAudioStreamCompleted?.Invoke(this, args);
    }

    private void OnOpenExternalVideoStreamCompleted(OpenExternalVideoStreamCompletedEventArgs args = null)
    {
        if (_shouldDispose)
        {
            Dispose();
            return;
        }

        ClosedVideoStream = null;
        MainDemuxer = !VideoDemuxer.Disposed ? VideoDemuxer : AudioDemuxer;

        if (CanInfo)
        {
            Log.Info($"[OpenExternalVideoStream] {(args.OldExtStream != null ? args.OldExtStream.Url : "None")} => {(args.ExtStream != null ? args.ExtStream.Url : "None")}{(!args.Success ? " [Error: " + args.Error + "]" : string.Empty)}");
        }

        OpenExternalVideoStreamCompleted?.Invoke(this, args);
    }

    private void OnOpenExternalSubtitlesStreamCompleted(OpenExternalSubtitlesStreamCompletedEventArgs args = null)
    {
        if (_shouldDispose)
        {
            Dispose();
            return;
        }

        ClosedSubtitlesStream = null;

        if (CanInfo)
        {
            Log.Info($"[OpenExternalSubtitlesStream] {(args.OldExtStream != null ? args.OldExtStream.Url : "None")} => {(args.ExtStream != null ? args.ExtStream.Url : "None")}{(!args.Success ? " [Error: " + args.Error + "]" : string.Empty)}");
        }

        OpenExternalSubtitlesStreamCompleted?.Invoke(this, args);
    }
}
