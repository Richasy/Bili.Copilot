// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Concurrent;
using Bili.Copilot.Libs.Player.MediaFramework.MediaDecoder;
using Bili.Copilot.Libs.Player.MediaFramework.MediaStream;
using Bili.Copilot.Libs.Player.Models;

namespace Bili.Copilot.Libs.Player.MediaPlayer;

/// <summary>
/// 播放器.
/// </summary>
public unsafe sealed partial class Player
{
    /// <summary>
    /// 当打开完成时发生.也用于字幕.
    /// </summary>
    public event EventHandler<OpenCompletedEventArgs> OpenCompleted;

    /// <summary>
    /// 当打开播放列表项完成时发生.
    /// </summary>
    public event EventHandler<OpenPlaylistItemCompletedArgs> OpenPlaylistItemCompleted;

    /// <summary>
    /// 当打开会话完成时发生.
    /// </summary>
    public event EventHandler<OpenSessionCompletedArgs> OpenSessionCompleted;

    /// <summary>
    /// 当打开音频流完成时发生.
    /// </summary>
    public event EventHandler<OpenAudioStreamCompletedArgs> OpenAudioStreamCompleted;

    /// <summary>
    /// 当打开视频流完成时发生.
    /// </summary>
    public event EventHandler<OpenVideoStreamCompletedArgs> OpenVideoStreamCompleted;

    /// <summary>
    /// 当打开字幕流完成时发生.
    /// </summary>
    public event EventHandler<OpenSubtitlesStreamCompletedArgs> OpenSubtitlesStreamCompleted;

    /// <summary>
    /// 当打开外部音频流完成时发生.
    /// </summary>
    public event EventHandler<OpenExternalAudioStreamCompletedArgs> OpenExternalAudioStreamCompleted;

    /// <summary>
    /// 当打开外部视频流完成时发生.
    /// </summary>
    public event EventHandler<OpenExternalVideoStreamCompletedArgs> OpenExternalVideoStreamCompleted;

    /// <summary>
    /// 当打开外部字幕流完成时发生.
    /// </summary>
    public event EventHandler<OpenExternalSubtitlesStreamCompletedArgs> OpenExternalSubtitlesStreamCompleted;

    private void OnOpenCompleted(OpenCompletedEventArgs args = null)
        => OpenCompleted?.Invoke(this, args);

    private void OnOpenSessionCompleted(OpenSessionCompletedArgs args = null)
        => OpenSessionCompleted?.Invoke(this, args);

    private void OnOpenPlaylistItemCompleted(OpenPlaylistItemCompletedArgs args = null)
        => OpenPlaylistItemCompleted?.Invoke(this, args);

    private void OnOpenAudioStreamCompleted(OpenAudioStreamCompletedArgs args = null)
        => OpenAudioStreamCompleted?.Invoke(this, args);

    private void OnOpenVideoStreamCompleted(OpenVideoStreamCompletedArgs args = null)
        => OpenVideoStreamCompleted?.Invoke(this, args);

    private void OnOpenSubtitlesStreamCompleted(OpenSubtitlesStreamCompletedArgs args = null)
        => OpenSubtitlesStreamCompleted?.Invoke(this, args);

    private void OnOpenExternalAudioStreamCompleted(OpenExternalAudioStreamCompletedArgs args = null)
        => OpenExternalAudioStreamCompleted?.Invoke(this, args);

    private void OnOpenExternalVideoStreamCompleted(OpenExternalVideoStreamCompletedArgs args = null)
        => OpenExternalVideoStreamCompleted?.Invoke(this, args);

    private void OnOpenExternalSubtitlesStreamCompleted(OpenExternalSubtitlesStreamCompletedArgs args = null)
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

    private void DecoderOpenAudioStreamCompleted(object sender, OpenAudioStreamCompletedArgs e)
    {
        Config.Audio.SetDelay(0);
        Audio.Refresh();
        canPlay = Video.IsOpened || Audio.IsOpened;
        isLive = MainDemuxer.IsLive;
        duration = MainDemuxer.Duration;
        if (Video.isOpened)
            duration -= VideoDemuxer.VideoStream.FrameDuration;

        UIAdd(() =>
        {
            IsLive = IsLive;
            CanPlay = CanPlay;
            Duration = Duration;
        });
        UIAll();
    }

    private void DecoderOpenVideoStreamCompleted(object sender, OpenVideoStreamCompletedArgs e)
    {
        Video.Refresh();
        canPlay = Video.IsOpened || Audio.IsOpened;
        isLive = MainDemuxer.IsLive;
        duration = MainDemuxer.Duration;
        if (Video.isOpened)
            duration -= VideoDemuxer.VideoStream.FrameDuration;

        UIAdd(() =>
        {
            IsLive = IsLive;
            CanPlay = CanPlay;
            Duration = Duration;
        });
        UIAll();
    }

    private void DecoderOpenSubtitlesStreamCompleted(object sender, OpenSubtitlesStreamCompletedArgs e)
    {
        Config.Subtitles.SetDelay(0);
        Subtitles.Refresh();
        UIAll();
    }

    private void DecoderOpenExternalAudioStreamCompleted(object sender, OpenExternalAudioStreamCompletedArgs e)
    {
        if (!e.Success)
        {
            canPlay = Video.IsOpened || Audio.IsOpened;
            UIAdd(() => CanPlay = CanPlay);
            UIAll();
        }
    }

    private void DecoderOpenExternalVideoStreamCompleted(object sender, OpenExternalVideoStreamCompletedArgs e)
    {
        if (!e.Success)
        {
            canPlay = Video.IsOpened || Audio.IsOpened;
            UIAdd(() => CanPlay = CanPlay);
            UIAll();
        }
    }

    private void DecoderOpenExternalSubtitlesStreamCompleted(object sender, OpenExternalSubtitlesStreamCompletedArgs e)
    {
        if (e.Success)
        {
            lock (_lockSubtitles)
            {
                ReSync(decoder.SubtitlesStream, decoder.GetCurTimeMs());
            }
        }
    }
    #endregion

    #region Open Implementation
    private OpenCompletedArgs OpenInternal(object url_iostream, bool defaultPlaylistItem = true, bool defaultVideo = true, bool defaultAudio = true, bool defaultSubtitles = true)
    {
        OpenCompletedArgs args = new();

        try
        {
            if (url_iostream == null)
            {
                args.Error = "Invalid empty/null input";
                return args;
            }

            if (CanInfo)
                Log.Info($"Opening {url_iostream}");

            Initialize(Status.Opening);
            var args2 = decoder.Open(url_iostream, defaultPlaylistItem, defaultVideo, defaultAudio, defaultSubtitles);

            args.Url = args2.Url;
            args.IOStream = args2.IOStream;
            args.Error = args2.Error;

            if (!args.Success)
            {
                status = Status.Failed;
                lastError = args.Error;
            }
            else if (CanPlay)
            {
                status = Status.Paused;

                if (Config.Player.AutoPlay)
                    Play();
            }
            else if (!defaultVideo && !defaultAudio)
            {
                isLive = MainDemuxer.IsLive;
                duration = MainDemuxer.Duration;
                UIAdd(() =>
                {
                    IsLive = IsLive;
                    Duration = Duration;
                });
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
            OnOpenCompleted(args);
        }
    }
    private OpenCompletedArgs OpenSubtitles(string url)
    {
        OpenCompletedArgs args = new(url, null, null, true);

        try
        {
            if (CanInfo)
                Log.Info($"Opening subtitles {url}");

            if (!Video.IsOpened)
            {
                args.Error = "Cannot open subtitles without video";
                return args;
            }

            Config.Subtitles.SetEnabled(true);
            args.Error = decoder.OpenSubtitles(url).Error;

            if (args.Success)
                ReSync(decoder.SubtitlesStream);

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


    /// <summary>
    /// Opens a new media file (audio/subtitles/video)
    /// </summary>
    /// <param name="url">Media file's url</param>
    /// <param name="defaultPlaylistItem">Whether to open the first/default item in case of playlist</param>
    /// <param name="defaultVideo">Whether to open the default video stream from plugin suggestions</param>
    /// <param name="defaultAudio">Whether to open the default audio stream from plugin suggestions</param>
    /// <param name="defaultSubtitles">Whether to open the default subtitles stream from plugin suggestions</param>
    /// <returns></returns>
    public OpenCompletedArgs Open(string url, bool defaultPlaylistItem = true, bool defaultVideo = true, bool defaultAudio = true, bool defaultSubtitles = true)
        => ExtensionsSubtitles.Contains(GetUrlExtention(url))
        ? OpenSubtitles(url)
        : OpenInternal(url, defaultPlaylistItem, defaultVideo, defaultAudio, defaultSubtitles);

    /// <summary>
    /// Opens a new media file (audio/subtitles/video) without blocking
    /// You can get the results from <see cref="OpenCompleted"/>
    /// </summary>
    /// <param name="url">Media file's url</param>
    /// <param name="defaultPlaylistItem">Whether to open the first/default item in case of playlist</param>
    /// <param name="defaultVideo">Whether to open the default video stream from plugin suggestions</param>
    /// <param name="defaultAudio">Whether to open the default audio stream from plugin suggestions</param>
    /// <param name="defaultSubtitles">Whether to open the default subtitles stream from plugin suggestions</param>
    public void OpenAsync(string url, bool defaultPlaylistItem = true, bool defaultVideo = true, bool defaultAudio = true, bool defaultSubtitles = true)
        => OpenAsyncPush(url, defaultPlaylistItem, defaultVideo, defaultAudio, defaultSubtitles);

    /// <summary>
    /// Opens a new media I/O stream (audio/video) without blocking
    /// </summary>
    /// <param name="iostream">Media stream</param>
    /// <param name="defaultPlaylistItem">Whether to open the first/default item in case of playlist</param>
    /// <param name="defaultVideo">Whether to open the default video stream from plugin suggestions</param>
    /// <param name="defaultAudio">Whether to open the default audio stream from plugin suggestions</param>
    /// <param name="defaultSubtitles">Whether to open the default subtitles stream from plugin suggestions</param>
    /// <returns></returns>
    public OpenCompletedArgs Open(Stream iostream, bool defaultPlaylistItem = true, bool defaultVideo = true, bool defaultAudio = true, bool defaultSubtitles = true)
        => OpenInternal(iostream, defaultPlaylistItem, defaultVideo, defaultAudio, defaultSubtitles);

    /// <summary>
    /// Opens a new media I/O stream (audio/video) without blocking
    /// You can get the results from <see cref="OpenCompleted"/>
    /// </summary>
    /// <param name="iostream">Media stream</param>
    /// <param name="defaultPlaylistItem">Whether to open the first/default item in case of playlist</param>
    /// <param name="defaultVideo">Whether to open the default video stream from plugin suggestions</param>
    /// <param name="defaultAudio">Whether to open the default audio stream from plugin suggestions</param>
    /// <param name="defaultSubtitles">Whether to open the default subtitles stream from plugin suggestions</param>
    public void OpenAsync(Stream iostream, bool defaultPlaylistItem = true, bool defaultVideo = true, bool defaultAudio = true, bool defaultSubtitles = true)
        => OpenAsyncPush(iostream, defaultPlaylistItem, defaultVideo, defaultAudio, defaultSubtitles);

    /// <summary>
    /// Opens a new media session
    /// </summary>
    /// <param name="session">Media session</param>
    /// <returns></returns>
    public OpenSessionCompletedArgs Open(Session session)
    {
        OpenSessionCompletedArgs args = new(session);

        try
        {
            Playlist.Selected?.AddTag(GetCurrentSession(), playerSessionTag);

            Initialize(Status.Opening);
            args.Error = decoder.Open(session).Error;

            if (!args.Success || !CanPlay)
            {
                status = Status.Failed;
                lastError = args.Error;
            }
            else
            {
                status = Status.Paused;

                if (Config.Player.AutoPlay)
                    Play();
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
    /// Opens a new media session without blocking
    /// </summary>
    /// <param name="session">Media session</param>
    public void OpenAsync(Session session) => OpenAsyncPush(session);

    /// <summary>
    /// Opens a playlist item <see cref="Playlist.Items"/>
    /// </summary>
    /// <param name="item">The playlist item to open</param>
    /// <param name="defaultVideo">Whether to open the default video stream from plugin suggestions</param>
    /// <param name="defaultAudio">Whether to open the default audio stream from plugin suggestions</param>
    /// <param name="defaultSubtitles">Whether to open the default subtitles stream from plugin suggestions</param>
    /// <returns></returns>
    public OpenPlaylistItemCompletedArgs Open(PlaylistItem item, bool defaultVideo = true, bool defaultAudio = true, bool defaultSubtitles = true)
    {
        OpenPlaylistItemCompletedArgs args = new(item, Playlist.Selected);

        try
        {
            Playlist.Selected?.AddTag(GetCurrentSession(), playerSessionTag);

            Initialize(Status.Opening, true, true);

            // TODO: Config.Player.Reopen? to reopen session if (item.OpenedCounter > 0)
            args = decoder.Open(item, defaultVideo, defaultAudio, defaultSubtitles);

            if (!args.Success)
            {
                status = Status.Failed;
                lastError = args.Error;
            }
            else if (CanPlay)
            {
                status = Status.Paused;

                if (Config.Player.AutoPlay)
                    Play();
                // TODO: else Show on frame?
            }
            else if (!defaultVideo && !defaultAudio)
            {
                isLive = MainDemuxer.IsLive;
                duration = MainDemuxer.Duration;
                UIAdd(() =>
                {
                    IsLive = IsLive;
                    Duration = Duration;
                });
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
            OnOpenPlaylistItemCompleted(args);
        }
    }

    /// <summary>
    /// Opens a playlist item <see cref="Playlist.Items"/> without blocking
    /// You can get the results from <see cref="OpenPlaylistItemCompleted"/>
    /// </summary>
    /// <param name="item">The playlist item to open</param>
    /// <param name="defaultVideo">Whether to open the default video stream from plugin suggestions</param>
    /// <param name="defaultAudio">Whether to open the default audio stream from plugin suggestions</param>
    /// <param name="defaultSubtitles">Whether to open the default subtitles stream from plugin suggestions</param>
    /// <returns></returns>
    public void OpenAsync(PlaylistItem item, bool defaultVideo = true, bool defaultAudio = true, bool defaultSubtitles = true)
        => OpenAsyncPush(item, defaultVideo, defaultAudio, defaultSubtitles);

    /// <summary>
    /// Opens an external stream (audio/subtitles/video)
    /// </summary>
    /// <param name="extStream">The external stream to open</param>
    /// <param name="resync">Whether to force resync with other streams</param>
    /// <param name="defaultAudio">Whether to open the default audio stream from plugin suggestions</param>
    /// <param name="streamIndex">-2: None, -1: Suggested/Default, X: Specified embedded stream index</param>
    /// <returns></returns>
    public ExternalStreamOpenedArgs Open(ExternalStream extStream, bool resync = true, bool defaultAudio = true, int streamIndex = -1)
    {
        /* TODO
         * 
         * Decoder.Stop() should not be called on video input switch as it will close the other inputs as well (audio/subs)
         * If the input is from different plugin we don't dispose the current plugin (eg.  switching between recent/history plugin with torrents) (?)
         */

        ExternalStreamOpenedArgs args = null;

        try
        {
            int syncMs = decoder.GetCurTimeMs();

            if (LastError != null)
            {
                lastError = null;
                UI(() => LastError = LastError);
            }

            if (extStream is ExternalAudioStream)
            {
                if (decoder.VideoStream == null)
                    requiresBuffering = true;

                isAudioSwitch = true;
                Config.Audio.SetEnabled(true);
                args = decoder.Open(extStream, false, streamIndex);

                if (!args.Success)
                {
                    isAudioSwitch = false;
                    return args;
                }

                if (resync)
                    ReSync(decoder.AudioStream, syncMs);

                if (VideoDemuxer.VideoStream == null)
                {
                    isLive = MainDemuxer.IsLive;
                    duration = MainDemuxer.Duration;
                }

                isAudioSwitch = false;
            }
            else if (extStream is ExternalVideoStream)
            {
                bool shouldPlay = false;
                if (IsPlaying)
                {
                    shouldPlay = true;
                    Pause();
                }

                Initialize(Status.Opening, false, true);
                args = decoder.Open(extStream, defaultAudio, streamIndex);

                if (!args.Success || !CanPlay)
                    return args;

                decoder.Seek(syncMs, false, false);
                decoder.GetVideoFrame(syncMs * (long)10000);
                VideoDemuxer.Start();
                AudioDemuxer.Start();
                SubtitlesDemuxer.Start();
                decoder.PauseOnQueueFull();

                // Initialize will Reset those and is posible that Codec Changed will not be called (as they are not chaning necessary)
                DecoderOpenAudioStreamCompleted(null, null);
                DecoderOpenSubtitlesStreamCompleted(null, null);

                if (shouldPlay)
                    Play();
                else
                    ShowOneFrame();
            }
            else // ExternalSubtitlesStream
            {
                if (!Video.IsOpened)
                {
                    args.Error = "Subtitles require opened video stream";
                    return args;
                }

                Config.Subtitles.SetEnabled(true);
                args = decoder.Open(extStream, false, streamIndex);
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
                OnOpenExternalVideoStreamCompleted((OpenExternalVideoStreamCompletedArgs)args);
            else if (extStream is ExternalAudioStream)
                OnOpenExternalAudioStreamCompleted((OpenExternalAudioStreamCompletedArgs)args);
            else
                OnOpenExternalSubtitlesStreamCompleted((OpenExternalSubtitlesStreamCompletedArgs)args);
        }
    }

    /// <summary>
    /// Opens an external stream (audio/subtitles/video) without blocking
    /// You can get the results from <see cref="OpenExternalAudioStreamCompleted"/>, <see cref="OpenExternalVideoStreamCompleted"/>, <see cref="OpenExternalSubtitlesStreamCompleted"/>
    /// </summary>
    /// <param name="extStream">The external stream to open</param>
    /// <param name="resync">Whether to force resync with other streams</param>
    /// <param name="defaultAudio">Whether to open the default audio stream from plugin suggestions</param>
    public void OpenAsync(ExternalStream extStream, bool resync = true, bool defaultAudio = true)
        => OpenAsyncPush(extStream, resync, defaultAudio);

    /// <summary>
    /// Opens an embedded stream (audio/subtitles/video)
    /// </summary>
    /// <param name="stream">An existing Player's media stream</param>
    /// <param name="resync">Whether to force resync with other streams</param>
    /// <param name="defaultAudio">Whether to re-suggest audio based on the new video stream (has effect only on VideoStream)</param>
    /// <returns></returns>
    public StreamOpenedArgs Open(StreamBase stream, bool resync = true, bool defaultAudio = true)
    {
        StreamOpenedArgs args = new();

        try
        {
            long delay = DateTime.UtcNow.Ticks;
            long fromEnd = Duration - CurTime;

            if (stream.Demuxer.Type == MediaType.Video)
            {
                isVideoSwitch = true;
                requiresBuffering = true;
            }

            if (stream is AudioStream astream)
            {
                Config.Audio.SetEnabled(true);
                args = decoder.OpenAudioStream(astream);
            }
            else if (stream is VideoStream vstream)
                args = decoder.OpenVideoStream(vstream, defaultAudio);
            else if (stream is SubtitlesStream sstream)
            {
                Config.Subtitles.SetEnabled(true);
                args = decoder.OpenSubtitlesStream(sstream);
            }

            if (resync)
            {
                // Wait for at least on package before seek to update the HLS context first_time
                if (stream.Demuxer.IsHLSLive)
                {
                    while (stream.Demuxer.IsRunning && stream.Demuxer.GetPacketsPtr(stream.Type).Count < 3)
                        System.Threading.Thread.Sleep(20);

                    ReSync(stream, (int)((Duration - fromEnd - (DateTime.UtcNow.Ticks - delay)) / 10000));
                }
                else
                    ReSync(stream, (int)(CurTime / 10000), true);
            }
            else
                isVideoSwitch = false;

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
                OnOpenVideoStreamCompleted((OpenVideoStreamCompletedArgs)args);
            else if (stream is AudioStream)
                OnOpenAudioStreamCompleted((OpenAudioStreamCompletedArgs)args);
            else
                OnOpenSubtitlesStreamCompleted((OpenSubtitlesStreamCompletedArgs)args);
        }
    }

    /// <summary>
    /// Opens an embedded stream (audio/subtitles/video) without blocking
    /// You can get the results from <see cref="OpenAudioStreamCompleted"/>, <see cref="OpenVideoStreamCompleted"/>, <see cref="OpenSubtitlesStreamCompleted"/>
    /// </summary>
    /// <param name="stream">An existing Player's media stream</param>
    /// <param name="resync">Whether to force resync with other streams</param>
    /// <param name="defaultAudio">Whether to re-suggest audio based on the new video stream (has effect only on VideoStream)</param>
    public void OpenAsync(StreamBase stream, bool resync = true, bool defaultAudio = true)
        => OpenAsyncPush(stream, resync, defaultAudio);

    /// <summary>
    /// Gets a session that can be re-opened later on with <see cref="Open(Session)"/>
    /// </summary>
    /// <param name="item">The current selected playlist item if null</param>
    /// <returns></returns>
    public Session GetSession(PlaylistItem item = null)
        => Playlist.Selected != null && (item == null || item.Index == Playlist.Selected.Index)
        ? GetCurrentSession()
        : item != null && item.GetTag(playerSessionTag) != null ? (Session)item.GetTag(playerSessionTag) : null;
    string playerSessionTag = "_session";
    private Session GetCurrentSession()
    {
        Session session = new();
        var item = Playlist.Selected;

        session.Url = Playlist.Url;
        session.PlaylistItem = item.Index;

        if (item.ExternalAudioStream != null)
            session.ExternalAudioStream = item.ExternalAudioStream.Index;

        if (item.ExternalVideoStream != null)
            session.ExternalVideoStream = item.ExternalVideoStream.Index;

        if (item.ExternalSubtitlesStream != null)
            session.ExternalSubtitlesUrl = item.ExternalSubtitlesStream.Url;
        else if (decoder.SubtitlesStream != null)
            session.SubtitlesStream = decoder.SubtitlesStream.StreamIndex;

        if (decoder.AudioStream != null)
            session.AudioStream = decoder.AudioStream.StreamIndex;

        if (decoder.VideoStream != null)
            session.VideoStream = decoder.VideoStream.StreamIndex;

        session.CurTime = CurTime;
        session.AudioDelay = Config.Audio.Delay;
        session.SubtitlesDelay = Config.Subtitles.Delay;

        return session;
    }

    internal void ReSync(StreamBase stream, int syncMs = -1, bool accurate = false)
    {
        /* TODO
         * 
         * HLS live resync on stream switch should be from the end not from the start (could have different cache/duration)
         */

        if (stream == null)
            return;
        //if (stream == null || (syncMs == 0 || (syncMs == -1 && decoder.GetCurTimeMs() == 0))) return; // Avoid initial open resync?

        if (stream.Demuxer.Type == MediaType.Video)
        {
            isVideoSwitch = true;
            isAudioSwitch = true;
            isSubsSwitch = true;
            requiresBuffering = true;

            if (accurate && Video.IsOpened)
            {
                decoder.PauseDecoders();
                decoder.Seek(syncMs, false, false);
                decoder.GetVideoFrame(syncMs * (long)10000); // TBR: syncMs should not be -1 here
            }
            else
                decoder.Seek(syncMs, false, false);

            aFrame = null;
            isAudioSwitch = false;
            isVideoSwitch = false;
            sFrame = null;
            isSubsSwitch = false;

            if (!IsPlaying)
            {
                decoder.PauseDecoders();
                decoder.GetVideoFrame();
                ShowOneFrame();
            }
            else
            {
                Subtitles._subtitleText = "";
                if (Subtitles._subtitleText != "")
                    UI(() => Subtitles.SubtitleText = Subtitles.SubtitleText);
            }
        }
        else
        {
            if (stream.Demuxer.Type == MediaType.Audio)
            {
                isAudioSwitch = true;
                decoder.SeekAudio();
                aFrame = null;
                isAudioSwitch = false;
            }
            else
            {
                isSubsSwitch = true;
                decoder.SeekSubtitles();
                sFrame = null;
                Subtitles._subtitleText = "";
                if (Subtitles._subtitleText != "")
                    UI(() => Subtitles.SubtitleText = Subtitles.SubtitleText);
                isSubsSwitch = false;
            }

            if (IsPlaying)
            {
                stream.Demuxer.Start();
                decoder.GetDecoderPtr(stream.Type).Start();
            }
        }
    }
    #endregion

    #region OpenAsync Implementation
    private void OpenAsync()
    {
        lock (lockActions)
            if (taskOpenAsyncRuns)
                return;

        taskOpenAsyncRuns = true;

        Task.Run(() =>
        {
            if (IsDisposed)
                return;

            while (true)
            {
                if (openInputs.TryPop(out var data))
                {
                    openInputs.Clear();
                    decoder.Interrupt = true;
                    OpenInternal(data.url_iostream, data.defaultPlaylistItem, data.defaultVideo, data.defaultAudio, data.defaultSubtitles);
                }
                else if (openSessions.TryPop(out data))
                {
                    openSessions.Clear();
                    decoder.Interrupt = true;
                    Open(data.session);
                }
                else if (openItems.TryPop(out data))
                {
                    openItems.Clear();
                    decoder.Interrupt = true;
                    Open(data.playlistItem, data.defaultVideo, data.defaultAudio, data.defaultSubtitles);
                }
                else if (openVideo.TryPop(out data))
                {
                    openVideo.Clear();
                    if (data.extStream != null)
                        Open(data.extStream, data.resync, data.defaultAudio);
                    else
                        Open(data.stream, data.resync, data.defaultAudio);
                }
                else if (openAudio.TryPop(out data))
                {
                    openAudio.Clear();
                    if (data.extStream != null)
                        Open(data.extStream, data.resync);
                    else
                        Open(data.stream, data.resync);
                }
                else if (openSubtitles.TryPop(out data))
                {
                    openSubtitles.Clear();
                    if (data.url_iostream != null)
                        OpenSubtitles(data.url_iostream.ToString());
                    else if (data.extStream != null)
                        Open(data.extStream, data.resync);
                    else if (data.stream != null)
                        Open(data.stream, data.resync);
                }
                else
                {
                    lock (lockActions)
                    {
                        if (openInputs.IsEmpty && openSessions.IsEmpty && openItems.IsEmpty && openVideo.IsEmpty && openAudio.IsEmpty && openSubtitles.IsEmpty)
                        {
                            taskOpenAsyncRuns = false;
                            break;
                        }
                    }
                }
            }
        });
    }

    private void OpenAsyncPush(object url_iostream, bool defaultPlaylistItem = true, bool defaultVideo = true, bool defaultAudio = true, bool defaultSubtitles = true)
    {
        lock (lockActions)
        {
            if ((url_iostream is string) && ExtensionsSubtitles.Contains(GetUrlExtention(url_iostream.ToString())))
                openSubtitles.Push(new OpenAsyncData(url_iostream));
            else
            {
                decoder.Interrupt = true;
                openInputs.Push(new OpenAsyncData(url_iostream, defaultPlaylistItem, defaultVideo, defaultAudio, defaultSubtitles));
            }

            OpenAsync();
        }
    }
    private void OpenAsyncPush(Session session)
    {
        lock (lockActions)
        {
            if (!openInputs.IsEmpty)
                return;

            decoder.Interrupt = true;
            openSessions.Clear();
            openSessions.Push(new OpenAsyncData(session));

            OpenAsync();
        }
    }
    private void OpenAsyncPush(PlaylistItem playlistItem, bool defaultVideo = true, bool defaultAudio = true, bool defaultSubtitles = true)
    {
        lock (lockActions)
        {
            if (!openInputs.IsEmpty || !openSessions.IsEmpty)
                return;

            decoder.Interrupt = true;
            openItems.Push(new OpenAsyncData(playlistItem, defaultVideo, defaultAudio, defaultSubtitles));

            OpenAsync();
        }
    }
    private void OpenAsyncPush(ExternalStream extStream, bool resync = true, bool defaultAudio = true, int streamIndex = -1)
    {
        lock (lockActions)
        {
            if (!openInputs.IsEmpty || !openItems.IsEmpty || !openSessions.IsEmpty)
                return;

            if (extStream is ExternalAudioStream)
            {
                openAudio.Clear();
                openAudio.Push(new OpenAsyncData(extStream, resync, false, streamIndex));
            }
            else if (extStream is ExternalVideoStream)
            {
                openVideo.Clear();
                openVideo.Push(new OpenAsyncData(extStream, resync, defaultAudio, streamIndex));
            }
            else
            {
                openSubtitles.Clear();
                openSubtitles.Push(new OpenAsyncData(extStream, resync, false, streamIndex));
            }

            OpenAsync();
        }
    }
    private void OpenAsyncPush(StreamBase stream, bool resync = true, bool defaultAudio = true)
    {
        lock (lockActions)
        {
            if (!openInputs.IsEmpty || !openItems.IsEmpty || !openSessions.IsEmpty)
                return;

            if (stream is AudioStream)
            {
                openAudio.Clear();
                openAudio.Push(new OpenAsyncData(stream, resync));
            }
            else if (stream is VideoStream)
            {
                openVideo.Clear();
                openVideo.Push(new OpenAsyncData(stream, resync, defaultAudio));
            }
            else
            {
                openSubtitles.Clear();
                openSubtitles.Push(new OpenAsyncData(stream, resync));
            }

            OpenAsync();
        }
    }

    ConcurrentStack<OpenAsyncData> openInputs = new();
    ConcurrentStack<OpenAsyncData> openSessions = new();
    ConcurrentStack<OpenAsyncData> openItems = new();
    ConcurrentStack<OpenAsyncData> openVideo = new();
    ConcurrentStack<OpenAsyncData> openAudio = new();
    ConcurrentStack<OpenAsyncData> openSubtitles = new();
    #endregion
}
