// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.IO;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Player.Enums;
using Bili.Copilot.Libs.Player.MediaFramework.MediaDecoder;
using Bili.Copilot.Libs.Player.MediaFramework.MediaPlaylist;
using Bili.Copilot.Libs.Player.MediaFramework.MediaStream;
using Bili.Copilot.Libs.Player.Models;

using static Bili.Copilot.Libs.Player.Misc.Logger;

namespace Bili.Copilot.Libs.Player.MediaPlayer;

/// <summary>
/// 播放器.
/// </summary>
public unsafe partial class Player
{
    /// <summary>
    /// 当打开完成时发生.也用于字幕.
    /// </summary>
    public event EventHandler<OpenCompletedEventArgs> OpenCompleted;

    /// <summary>
    /// 当打开播放列表项完成时发生.
    /// </summary>
    public event EventHandler<OpenPlaylistItemCompletedEventArgs> OpenPlaylistItemCompleted;

    /// <summary>
    /// 当打开会话完成时发生.
    /// </summary>
    public event EventHandler<OpenSessionCompletedEventArgs> OpenSessionCompleted;

    /// <summary>
    /// 当打开音频流完成时发生.
    /// </summary>
    public event EventHandler<OpenAudioStreamCompletedEventArgs> OpenAudioStreamCompleted;

    /// <summary>
    /// 当打开视频流完成时发生.
    /// </summary>
    public event EventHandler<OpenVideoStreamCompletedEventArgs> OpenVideoStreamCompleted;

    /// <summary>
    /// 当打开字幕流完成时发生.
    /// </summary>
    public event EventHandler<OpenSubtitlesStreamCompletedEventArgs> OpenSubtitlesStreamCompleted;

    /// <summary>
    /// 当打开外部音频流完成时发生.
    /// </summary>
    public event EventHandler<OpenExternalAudioStreamCompletedEventArgs> OpenExternalAudioStreamCompleted;

    /// <summary>
    /// 当打开外部视频流完成时发生.
    /// </summary>
    public event EventHandler<OpenExternalVideoStreamCompletedEventArgs> OpenExternalVideoStreamCompleted;

    /// <summary>
    /// 当打开外部字幕流完成时发生.
    /// </summary>
    public event EventHandler<OpenExternalSubtitlesStreamCompletedEventArgs> OpenExternalSubtitlesStreamCompleted;

    /// <summary>
    /// 打开一个新的媒体文件（音频/字幕/视频）.
    /// </summary>
    /// <param name="url">媒体文件的 URL.</param>
    /// <param name="defaultPlaylistItem">是否在播放列表中打开第一个/默认项.</param>
    /// <param name="defaultVideo">是否打开插件建议的默认视频流.</param>
    /// <param name="defaultAudio">是否打开插件建议的默认音频流.</param>
    /// <param name="defaultSubtitles">是否打开插件建议的默认字幕流.</param>
    /// <returns><see cref="OpenCompletedEventArgs"/>.</returns>
    public OpenCompletedEventArgs Open(string url, bool defaultPlaylistItem = true, bool defaultVideo = true, bool defaultAudio = true, bool defaultSubtitles = true)
        => Utils.ExtensionsSubtitles.Contains(Utils.GetUrlExtension(url))
        ? OpenSubtitles(url)
        : OpenInternal(url, defaultPlaylistItem, defaultVideo, defaultAudio, defaultSubtitles);

    /// <summary>
    /// 异步打开一个新的媒体文件（音频/字幕/视频），不会阻塞.
    /// 可以通过 <see cref="OpenCompleted"/> 获取结果.
    /// </summary>
    /// <param name="url">媒体文件的 URL.</param>
    /// <param name="defaultPlaylistItem">是否在播放列表中打开第一个/默认项.</param>
    /// <param name="defaultVideo">是否打开插件建议的默认视频流.</param>
    /// <param name="defaultAudio">是否打开插件建议的默认音频流.</param>
    /// <param name="defaultSubtitles">是否打开插件建议的默认字幕流.</param>
    public void OpenAsync(string url, bool defaultPlaylistItem = true, bool defaultVideo = true, bool defaultAudio = true, bool defaultSubtitles = true)
        => OpenAsyncPush(url, defaultPlaylistItem, defaultVideo, defaultAudio, defaultSubtitles);

    /// <summary>
    /// 打开一个新的媒体 I/O 流（音频/视频），不会阻塞.
    /// </summary>
    /// <param name="iostream">媒体流.</param>
    /// <param name="defaultPlaylistItem">是否在播放列表中打开第一个/默认项.</param>
    /// <param name="defaultVideo">是否打开插件建议的默认视频流.</param>
    /// <param name="defaultAudio">是否打开插件建议的默认音频流.</param>
    /// <param name="defaultSubtitles">是否打开插件建议的默认字幕流.</param>
    /// <returns><see cref="OpenCompletedEventArgs"/>.</returns>
    public OpenCompletedEventArgs Open(Stream iostream, bool defaultPlaylistItem = true, bool defaultVideo = true, bool defaultAudio = true, bool defaultSubtitles = true)
        => OpenInternal(iostream, defaultPlaylistItem, defaultVideo, defaultAudio, defaultSubtitles);

    /// <summary>
    /// 异步打开一个新的媒体 I/O 流（音频/视频），不会阻塞.
    /// 可以通过 <see cref="OpenCompleted"/> 获取结果.
    /// </summary>
    /// <param name="iostream">媒体流.</param>
    /// <param name="defaultPlaylistItem">是否在播放列表中打开第一个/默认项.</param>
    /// <param name="defaultVideo">是否打开插件建议的默认视频流.</param>
    /// <param name="defaultAudio">是否打开插件建议的默认音频流.</param>
    /// <param name="defaultSubtitles">是否打开插件建议的默认字幕流.</param>
    public void OpenAsync(Stream iostream, bool defaultPlaylistItem = true, bool defaultVideo = true, bool defaultAudio = true, bool defaultSubtitles = true)
        => OpenAsyncPush(iostream, defaultPlaylistItem, defaultVideo, defaultAudio, defaultSubtitles);

    /// <summary>
    /// 打开一个新的媒体会话.
    /// </summary>
    /// <param name="session">媒体会话.</param>
    /// <returns><see cref="OpenSessionCompletedEventArgs"/>.</returns>
    public OpenSessionCompletedEventArgs Open(Session session)
    {
        OpenSessionCompletedEventArgs args = new(session);

        try
        {
            Playlist.Selected?.AddTag(GetCurrentSession(), _playerSessionTag);

            Initialize(PlayerStatus.Opening);
            args.Error = Decoder.Open(session).Error;

            if (!args.Success || !CanPlay)
            {
                Status = PlayerStatus.Failed;
                LastError = args.Error;
            }
            else
            {
                Status = PlayerStatus.Paused;

                if (Config.Player.AutoPlay)
                {
                    Play();
                }
            }

            UIAdd(() =>
            {
                LastError = LastError;
                Status = Status;
            });

            UIAll();

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
    /// 异步打开一个新的媒体会话.
    /// </summary>
    /// <param name="session">媒体会话.</param>
    public void OpenAsync(Session session) => OpenAsyncPush(session);

    /// <summary>
    /// 打开一个播放列表项 <see cref="Playlist.Items"/>.
    /// </summary>
    /// <param name="item">要打开的播放列表项.</param>
    /// <param name="defaultVideo">是否打开插件建议的默认视频流.</param>
    /// <param name="defaultAudio">是否打开插件建议的默认音频流.</param>
    /// <param name="defaultSubtitles">是否打开插件建议的默认字幕流.</param>
    /// <returns><see cref="OpenPlaylistItemCompletedEventArgs"/>.</returns>
    public OpenPlaylistItemCompletedEventArgs Open(PlaylistItem item, bool defaultVideo = true, bool defaultAudio = true, bool defaultSubtitles = true)
    {
        OpenPlaylistItemCompletedEventArgs args = new(item, Playlist.Selected);

        try
        {
            Playlist.Selected?.AddTag(GetCurrentSession(), _playerSessionTag);

            Initialize(PlayerStatus.Opening, true, true);

            // TODO: Config.Player.Reopen? to reopen session if (item.OpenedCounter > 0)
            args = Decoder.Open(item, defaultVideo, defaultAudio, defaultSubtitles);

            if (!args.Success)
            {
                UIAdd(() =>
                {
                    Status = PlayerStatus.Failed;
                    LastError = args.Error;
                });
            }
            else if (CanPlay)
            {
                Status = PlayerStatus.Paused;

                if (Config.Player.AutoPlay)
                {
                    Play();
                }
            }
            else if (!defaultVideo && !defaultAudio)
            {
                UIAdd(() =>
                {
                    IsLive = MainDemuxer.IsLive;
                    Duration = MainDemuxer.Duration;
                });
            }

            UIAll();

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
    /// 异步打开一个播放列表项 <see cref="Playlist.Items"/>，不会阻塞.
    /// 可以通过 <see cref="OpenPlaylistItemCompleted"/> 获取结果.
    /// </summary>
    /// <param name="item">要打开的播放列表项.</param>
    /// <param name="defaultVideo">是否打开插件建议的默认视频流.</param>
    /// <param name="defaultAudio">是否打开插件建议的默认音频流.</param>
    /// <param name="defaultSubtitles">是否打开插件建议的默认字幕流.</param>
    public void OpenAsync(PlaylistItem item, bool defaultVideo = true, bool defaultAudio = true, bool defaultSubtitles = true)
        => OpenAsyncPush(item, defaultVideo, defaultAudio, defaultSubtitles);

    /// <summary>
    /// 打开一个外部流（音频/字幕/视频）.
    /// </summary>
    /// <param name="extStream">要打开的外部流.</param>
    /// <param name="resync">是否强制与其他流进行重新同步.</param>
    /// <param name="defaultAudio">是否打开插件建议的默认音频流.</param>
    /// <param name="streamIndex">-2: 无，-1: 建议/默认，X: 指定的嵌入流索引.</param>
    /// <returns><see cref="ExternalStreamOpenedEventArgs"/>.</returns>
    public ExternalStreamOpenedEventArgs Open(ExternalStream extStream, bool resync = true, bool defaultAudio = true, int streamIndex = -1)
    {
        ExternalStreamOpenedEventArgs args = null;

        try
        {
            var syncMs = Decoder.GetCurTimeMs();

            if (LastError != null)
            {
                LastError = null;
            }

            if (extStream is ExternalAudioStream)
            {
                if (Decoder.VideoStream == null)
                {
                    RequiresBuffering = true;
                }

                _isAudioSwitch = true;
                Config.Audio.SetEnabled(true);
                args = Decoder.Open(extStream, false, streamIndex);

                if (!args.Success)
                {
                    _isAudioSwitch = false;
                    return args;
                }

                if (resync)
                {
                    ReSync(Decoder.AudioStream, syncMs);
                }

                if (VideoDemuxer.VideoStream == null)
                {
                    IsLive = MainDemuxer.IsLive;
                    Duration = MainDemuxer.Duration;
                }

                _isAudioSwitch = false;
            }
            else if (extStream is ExternalVideoStream)
            {
                var shouldPlay = false;
                if (IsPlaying)
                {
                    shouldPlay = true;
                    Pause();
                }

                Initialize(PlayerStatus.Opening, false, true);
                args = Decoder.Open(extStream, defaultAudio, streamIndex);

                if (!args.Success || !CanPlay)
                {
                    return args;
                }

                Decoder.Seek(syncMs, false, false);
                Decoder.GetVideoFrame(syncMs * 10000L);
                VideoDemuxer.Start();
                AudioDemuxer.Start();
                SubtitlesDemuxer.Start();
                Decoder.PauseOnQueueFull();

                // Initialize will Reset those and is posible that Codec Changed will not be called (as they are not chaning necessary)
                DecoderOpenAudioStreamCompleted(null, null);
                DecoderOpenSubtitlesStreamCompleted(null, null);

                if (shouldPlay)
                {
                    Play();
                }
                else
                {
                    ShowOneFrame();
                }
            }
            else // ExternalSubtitlesStream
            {
                if (!Video.IsOpened)
                {
                    args.Error = "Subtitles require opened video stream";
                    return args;
                }

                Config.Subtitles.SetEnabled(true);
                args = Decoder.Open(extStream, false, streamIndex);
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
    /// 异步打开一个外部流（音频/字幕/视频），不会阻塞.
    /// 可以通过 <see cref="OpenExternalAudioStreamCompleted"/>、<see cref="OpenExternalVideoStreamCompleted"/>、<see cref="OpenExternalSubtitlesStreamCompleted"/> 获取结果.
    /// </summary>
    /// <param name="extStream">要打开的外部流.</param>
    /// <param name="resync">是否强制与其他流进行重新同步.</param>
    /// <param name="defaultAudio">是否打开插件建议的默认音频流.</param>
    public void OpenAsync(ExternalStream extStream, bool resync = true, bool defaultAudio = true)
        => OpenAsyncPush(extStream, resync, defaultAudio);

    /// <summary>
    /// 打开一个嵌入流（音频/字幕/视频）.
    /// </summary>
    /// <param name="stream">现有的播放器媒体流.</param>
    /// <param name="resync">是否强制与其他流进行重新同步.</param>
    /// <param name="defaultAudio">是否根据新的视频流重新建议音频（仅对 VideoStream 有效）.</param>
    /// <returns><see cref="StreamOpenedEventArgs"/>.</returns>
    public StreamOpenedEventArgs Open(StreamBase stream, bool resync = true, bool defaultAudio = true)
    {
        StreamOpenedEventArgs args = new();

        try
        {
            var delay = DateTime.UtcNow.Ticks;
            var fromEnd = Duration - CurTime;

            if (stream.Demuxer.Type == MediaType.Video)
            {
                _isVideoSwitch = true;
                RequiresBuffering = true;
            }

            if (stream is AudioStream astream)
            {
                Config.Audio.SetEnabled(true);
                args = Decoder.OpenAudioStream(astream);
            }
            else if (stream is VideoStream vstream)
            {
                args = Decoder.OpenVideoStream(vstream, defaultAudio);
            }
            else if (stream is SubtitlesStream sstream)
            {
                Config.Subtitles.SetEnabled(true);
                args = Decoder.OpenSubtitlesStream(sstream);
            }

            if (resync)
            {
                // Wait for at least on package before seek to update the HLS context first_time
                if (stream.Demuxer.IsHLSLive)
                {
                    while (stream.Demuxer.IsRunning && stream.Demuxer.GetPacketsPtr(stream.Type).Count < 3)
                    {
                        System.Threading.Thread.Sleep(20);
                    }

                    ReSync(stream, (int)((Duration - fromEnd - (DateTime.UtcNow.Ticks - delay)) / 10000));
                }
                else
                {
                    ReSync(stream, (int)(CurTime / 10000), true);
                }
            }
            else
            {
                _isVideoSwitch = false;
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
            if (stream is VideoStream)
            {
                OnOpenVideoStreamCompleted((OpenVideoStreamCompletedEventArgs)args);
            }
            else if (stream is AudioStream)
            {
                OnOpenAudioStreamCompleted((OpenAudioStreamCompletedEventArgs)args);
            }
            else
            {
                OnOpenSubtitlesStreamCompleted((OpenSubtitlesStreamCompletedEventArgs)args);
            }
        }
    }

    /// <summary>
    /// 异步打开一个嵌入流（音频/字幕/视频），不会阻塞.
    /// 可以通过 <see cref="OpenAudioStreamCompleted"/>、<see cref="OpenVideoStreamCompleted"/>、<see cref="OpenSubtitlesStreamCompleted"/> 获取结果.
    /// </summary>
    /// <param name="stream">现有的播放器媒体流.</param>
    /// <param name="resync">是否强制与其他流进行重新同步.</param>
    /// <param name="defaultAudio">是否根据新的视频流重新建议音频（仅对 VideoStream 有效）.</param>
    public void OpenAsync(StreamBase stream, bool resync = true, bool defaultAudio = true)
        => OpenAsyncPush(stream, resync, defaultAudio);

    /// <summary>
    /// 获取可以稍后重新打开的会话 <see cref="Open(Session)"/>.
    /// </summary>
    /// <param name="item">当前选定的播放列表项，默认为 null.</param>
    /// <returns><see cref="Session"/>.</returns>
    public Session GetSession(PlaylistItem item = null)
        => Playlist.Selected != null && (item == null || item.Index == Playlist.Selected.Index)
        ? GetCurrentSession()
        : item != null && item.GetTag(_playerSessionTag) != null ? (Session)item.GetTag(_playerSessionTag) : null;

    internal void ReSync(StreamBase stream, int syncMs = -1, bool accurate = false)
    {
        if (stream == null)
        {
            return;
        }

        if (stream.Demuxer.Type == MediaType.Video)
        {
            _isVideoSwitch = true;
            _isAudioSwitch = true;
            _isSubsSwitch = true;
            RequiresBuffering = true;

            if (accurate && Video.IsOpened)
            {
                Decoder.PauseDecoders();
                Decoder.Seek(syncMs, false, false);
                Decoder.GetVideoFrame(syncMs * 10000L); // TBR: syncMs should not be -1 here
            }
            else
            {
                Decoder.Seek(syncMs, false, false);
            }

            AudioFrame = null;
            _isAudioSwitch = false;
            _isVideoSwitch = false;
            SubtitleFrame = null;
            _isSubsSwitch = false;

            if (!IsPlaying)
            {
                Decoder.PauseDecoders();
                Decoder.GetVideoFrame();
                ShowOneFrame();
            }
            else
            {
                Utils.UI(() => Subtitles.SubtitleText = string.Empty);
            }
        }
        else
        {
            if (stream.Demuxer.Type == MediaType.Audio)
            {
                _isAudioSwitch = true;
                Decoder.SeekAudio();
                AudioFrame = null;
                _isAudioSwitch = false;
            }
            else
            {
                _isSubsSwitch = true;
                Decoder.SeekSubtitles();
                SubtitleFrame = null;
                Utils.UI(() => Subtitles.SubtitleText = string.Empty);
                _isSubsSwitch = false;
            }

            if (IsPlaying)
            {
                stream.Demuxer.Start();
                Decoder.GetDecoderPtr(stream.Type).Start();
            }
        }
    }

    private void OnOpenCompleted(OpenCompletedEventArgs args = null)
        => OpenCompleted?.Invoke(this, args);

    private void OnOpenSessionCompleted(OpenSessionCompletedEventArgs args = null)
        => OpenSessionCompleted?.Invoke(this, args);

    private void OnOpenPlaylistItemCompleted(OpenPlaylistItemCompletedEventArgs args = null)
        => OpenPlaylistItemCompleted?.Invoke(this, args);

    private void OnOpenAudioStreamCompleted(OpenAudioStreamCompletedEventArgs args = null)
        => OpenAudioStreamCompleted?.Invoke(this, args);

    private void OnOpenVideoStreamCompleted(OpenVideoStreamCompletedEventArgs args = null)
        => OpenVideoStreamCompleted?.Invoke(this, args);

    private void OnOpenSubtitlesStreamCompleted(OpenSubtitlesStreamCompletedEventArgs args = null)
        => OpenSubtitlesStreamCompleted?.Invoke(this, args);

    private void OnOpenExternalAudioStreamCompleted(OpenExternalAudioStreamCompletedEventArgs args = null)
        => OpenExternalAudioStreamCompleted?.Invoke(this, args);

    private void OnOpenExternalVideoStreamCompleted(OpenExternalVideoStreamCompletedEventArgs args = null)
        => OpenExternalVideoStreamCompleted?.Invoke(this, args);

    private void OnOpenExternalSubtitlesStreamCompleted(OpenExternalSubtitlesStreamCompletedEventArgs args = null)
        => OpenExternalSubtitlesStreamCompleted?.Invoke(this, args);

    private void DecoderAudioCodecChanged(DecoderBase x)
    {
        Audio.Refresh();
        UIAll();
    }

    private void DecoderVideoCodecChanged(DecoderBase x)
    {
        Video.Refresh();
        UIAll();
    }

    private void DecoderOpenAudioStreamCompleted(object sender, OpenAudioStreamCompletedEventArgs e)
    {
        Config.Audio.SetDelay(0);
        Audio.Refresh();

        UIAdd(() =>
        {
            CanPlay = Video.IsOpened || Audio.IsOpened;
            IsLive = MainDemuxer.IsLive;
            Duration = MainDemuxer.Duration;
            if (Video.IsOpened)
            {
                Duration -= VideoDemuxer.VideoStream.FrameDuration;
            }
        });
        UIAll();
    }

    private void DecoderOpenVideoStreamCompleted(object sender, OpenVideoStreamCompletedEventArgs e)
    {
        Video.Refresh();

        UIAdd(() =>
        {
            CanPlay = Video.IsOpened || Audio.IsOpened;
            IsLive = MainDemuxer.IsLive;
            Duration = MainDemuxer.Duration;
            if (Video.IsOpened)
            {
                Duration -= VideoDemuxer.VideoStream.FrameDuration;
            }
        });
        UIAll();
    }

    private void DecoderOpenSubtitlesStreamCompleted(object sender, OpenSubtitlesStreamCompletedEventArgs e)
    {
        Config.Subtitles.SetDelay(0);
        Subtitles.Refresh();
        UIAll();
    }

    private void DecoderOpenExternalAudioStreamCompleted(object sender, OpenExternalAudioStreamCompletedEventArgs e)
    {
        if (!e.Success)
        {
            UIAdd(() => CanPlay = Video.IsOpened || Audio.IsOpened);
            UIAll();
        }
    }

    private void DecoderOpenExternalVideoStreamCompleted(object sender, OpenExternalVideoStreamCompletedEventArgs e)
    {
        if (!e.Success)
        {
            UIAdd(() => CanPlay = Video.IsOpened || Audio.IsOpened);
            UIAll();
        }
    }

    private void DecoderOpenExternalSubtitlesStreamCompleted(object sender, OpenExternalSubtitlesStreamCompletedEventArgs e)
    {
        if (e.Success)
        {
            lock (_lockSubtitles)
            {
                ReSync(Decoder.SubtitlesStream, Decoder.GetCurTimeMs());
            }
        }
    }

    private OpenCompletedEventArgs OpenInternal(object url_iostream, bool defaultPlaylistItem = true, bool defaultVideo = true, bool defaultAudio = true, bool defaultSubtitles = true)
    {
        OpenCompletedEventArgs args = new();

        try
        {
            if (url_iostream == null)
            {
                args.Error = "Invalid empty/null input";
                return args;
            }

            if (CanInfo)
            {
                Log.Info($"Opening {url_iostream}");
            }

            Initialize(PlayerStatus.Opening);
            var args2 = Decoder.Open(url_iostream, defaultPlaylistItem, defaultVideo, defaultAudio, defaultSubtitles);

            args.Url = args2.Url;
            args.IoStream = args2.IoStream;
            args.Error = args2.Error;

            if (!args.Success)
            {
                UIAdd(() =>
                {
                    Status = PlayerStatus.Failed;
                    LastError = args.Error;
                });
            }
            else if (CanPlay)
            {
                Status = PlayerStatus.Paused;

                if (Config.Player.AutoPlay)
                {
                    Play();
                }
            }
            else if (!defaultVideo && !defaultAudio)
            {
                UIAdd(() =>
                {
                    IsLive = MainDemuxer.IsLive;
                    Duration = MainDemuxer.Duration;
                });
            }

            UIAll();

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

    private OpenCompletedEventArgs OpenSubtitles(string url)
    {
        OpenCompletedEventArgs args = new(url, null, null, true);

        try
        {
            if (CanInfo)
            {
                Log.Info($"Opening subtitles {url}");
            }

            if (!Video.IsOpened)
            {
                args.Error = "Cannot open subtitles without video";
                return args;
            }

            Config.Subtitles.SetEnabled(true);
            args.Error = Decoder.OpenSubtitle(url).Error;

            if (args.Success)
            {
                ReSync(Decoder.SubtitlesStream);
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
            OnOpenCompleted(args);
        }
    }

    private Session GetCurrentSession()
    {
        Session session = new();
        var item = Playlist.Selected;

        session.Url = Playlist.Url;
        session.PlaylistItem = item.Index;

        if (item.ExternalAudioStream != null)
        {
            session.ExternalAudioStream = item.ExternalAudioStream.Index;
        }

        if (item.ExternalVideoStream != null)
        {
            session.ExternalVideoStream = item.ExternalVideoStream.Index;
        }

        if (item.ExternalSubtitlesStream != null)
        {
            session.ExternalSubtitlesUrl = item.ExternalSubtitlesStream.Url;
        }
        else if (Decoder.SubtitlesStream != null)
        {
            session.SubtitlesStream = Decoder.SubtitlesStream.StreamIndex;
        }

        if (Decoder.AudioStream != null)
        {
            session.AudioStream = Decoder.AudioStream.StreamIndex;
        }

        if (Decoder.VideoStream != null)
        {
            session.VideoStream = Decoder.VideoStream.StreamIndex;
        }

        session.CurTime = CurTime;
        session.AudioDelay = Config.Audio.Delay;
        session.SubtitlesDelay = Config.Subtitles.Delay;

        return session;
    }

    private void OpenAsync()
    {
        lock (_lockActions)
        {
            if (_taskOpenAsyncRuns)
            {
                return;
            }
        }

        _taskOpenAsyncRuns = true;

        Task.Run(() =>
        {
            if (IsDisposed)
            {
                return;
            }

            while (true)
            {
                if (_openInputs.TryPop(out var data))
                {
                    _openInputs.Clear();
                    Decoder.Interrupt = true;
                    OpenInternal(data.UrlOrIoStream, data.DefaultPlaylistItem, data.DefaultVideo, data.DefaultAudio, data.DefaultSubtitles);
                }
                else if (_openSessions.TryPop(out data))
                {
                    _openSessions.Clear();
                    Decoder.Interrupt = true;
                    Open(data.Session);
                }
                else if (_openItems.TryPop(out data))
                {
                    _openItems.Clear();
                    Decoder.Interrupt = true;
                    Open(data.PlaylistItem, data.DefaultVideo, data.DefaultAudio, data.DefaultSubtitles);
                }
                else if (_openVideo.TryPop(out data))
                {
                    _openVideo.Clear();
                    if (data.ExtStream != null)
                    {
                        Open(data.ExtStream, data.ReSync, data.DefaultAudio);
                    }
                    else
                    {
                        Open(data.Stream, data.ReSync, data.DefaultAudio);
                    }
                }
                else if (_openAudio.TryPop(out data))
                {
                    _openAudio.Clear();
                    if (data.ExtStream != null)
                    {
                        Open(data.ExtStream, data.ReSync);
                    }
                    else
                    {
                        Open(data.Stream, data.ReSync);
                    }
                }
                else if (_openSubtitles.TryPop(out data))
                {
                    _openSubtitles.Clear();
                    if (data.UrlOrIoStream != null)
                    {
                        OpenSubtitles(data.UrlOrIoStream.ToString());
                    }
                    else if (data.ExtStream != null)
                    {
                        Open(data.ExtStream, data.ReSync);
                    }
                    else if (data.Stream != null)
                    {
                        Open(data.Stream, data.ReSync);
                    }
                }
                else
                {
                    lock (_lockActions)
                    {
                        if (_openInputs.IsEmpty && _openSessions.IsEmpty && _openItems.IsEmpty && _openVideo.IsEmpty && _openAudio.IsEmpty && _openSubtitles.IsEmpty)
                        {
                            _taskOpenAsyncRuns = false;
                            break;
                        }
                    }
                }
            }
        });
    }

    private void OpenAsyncPush(object url_iostream, bool defaultPlaylistItem = true, bool defaultVideo = true, bool defaultAudio = true, bool defaultSubtitles = true)
    {
        lock (_lockActions)
        {
            if ((url_iostream is string) && Utils.ExtensionsSubtitles.Contains(Utils.GetUrlExtension(url_iostream.ToString())))
            {
                _openSubtitles.Push(new OpenAsyncData(url_iostream));
            }
            else
            {
                Decoder.Interrupt = true;
                _openInputs.Push(new OpenAsyncData(url_iostream, defaultPlaylistItem, defaultVideo, defaultAudio, defaultSubtitles));
            }

            OpenAsync();
        }
    }

    private void OpenAsyncPush(Session session)
    {
        lock (_lockActions)
        {
            if (!_openInputs.IsEmpty)
            {
                return;
            }

            Decoder.Interrupt = true;
            _openSessions.Clear();
            _openSessions.Push(new OpenAsyncData(session));

            OpenAsync();
        }
    }

    private void OpenAsyncPush(PlaylistItem playlistItem, bool defaultVideo = true, bool defaultAudio = true, bool defaultSubtitles = true)
    {
        lock (_lockActions)
        {
            if (!_openInputs.IsEmpty || !_openSessions.IsEmpty)
            {
                return;
            }

            Decoder.Interrupt = true;
            _openItems.Push(new OpenAsyncData(playlistItem, defaultVideo, defaultAudio, defaultSubtitles));

            OpenAsync();
        }
    }

    private void OpenAsyncPush(ExternalStream extStream, bool resync = true, bool defaultAudio = true, int streamIndex = -1)
    {
        lock (_lockActions)
        {
            if (!_openInputs.IsEmpty || !_openItems.IsEmpty || !_openSessions.IsEmpty)
            {
                return;
            }

            if (extStream is ExternalAudioStream)
            {
                _openAudio.Clear();
                _openAudio.Push(new OpenAsyncData(extStream, resync, false, streamIndex));
            }
            else if (extStream is ExternalVideoStream)
            {
                _openVideo.Clear();
                _openVideo.Push(new OpenAsyncData(extStream, resync, defaultAudio, streamIndex));
            }
            else
            {
                _openSubtitles.Clear();
                _openSubtitles.Push(new OpenAsyncData(extStream, resync, false, streamIndex));
            }

            OpenAsync();
        }
    }

    private void OpenAsyncPush(StreamBase stream, bool resync = true, bool defaultAudio = true)
    {
        lock (_lockActions)
        {
            if (!_openInputs.IsEmpty || !_openItems.IsEmpty || !_openSessions.IsEmpty)
            {
                return;
            }

            if (stream is AudioStream)
            {
                _openAudio.Clear();
                _openAudio.Push(new OpenAsyncData(stream, resync));
            }
            else if (stream is VideoStream)
            {
                _openVideo.Clear();
                _openVideo.Push(new OpenAsyncData(stream, resync, defaultAudio));
            }
            else
            {
                _openSubtitles.Clear();
                _openSubtitles.Push(new OpenAsyncData(stream, resync));
            }

            OpenAsync();
        }
    }
}
