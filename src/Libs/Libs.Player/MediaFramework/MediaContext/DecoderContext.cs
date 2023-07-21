// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Libs.Player.Core;
using Bili.Copilot.Libs.Player.Enums;
using Bili.Copilot.Libs.Player.MediaFramework.MediaDecoder;
using Bili.Copilot.Libs.Player.MediaFramework.MediaDemuxer;
using Bili.Copilot.Libs.Player.MediaFramework.MediaPlaylist;
using Bili.Copilot.Libs.Player.MediaFramework.MediaRemuxer;
using Bili.Copilot.Libs.Player.MediaFramework.MediaStream;
using Bili.Copilot.Libs.Player.Misc;
using Bili.Copilot.Libs.Player.Plugins;
using FFmpeg.AutoGen;
using static Bili.Copilot.Libs.Player.Misc.Logger;

using static FFmpeg.AutoGen.AVMediaType;
using static FFmpeg.AutoGen.ffmpeg;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaContext;

/// <summary>
/// 解码器上下文类，继承自插件处理器.
/// </summary>
public unsafe partial class DecoderContext : PluginHandler
{
    /// <summary>
    /// 构造函数，用于创建DecoderContext实例.
    /// </summary>
    /// <param name="config">配置对象，默认为null.</param>
    /// <param name="uniqueId">唯一标识符，默认为-1.</param>
    /// <param name="enableDecoding">是否启用解码，默认为true.</param>
    public DecoderContext(Config config = null, int uniqueId = -1, bool enableDecoding = true)
        : base(config, uniqueId)
    {
        Log = new LogHandler(("[#" + UniqueId + "]").PadRight(8, ' ') + " [DecoderContext] ");
        Playlist.decoder = this;

        EnableDecoding = enableDecoding;

        AudioDemuxer = new Demuxer(Config.Demuxer, MediaType.Audio, UniqueId, EnableDecoding);
        VideoDemuxer = new Demuxer(Config.Demuxer, MediaType.Video, UniqueId, EnableDecoding);
        SubtitlesDemuxer = new Demuxer(Config.Demuxer, MediaType.Subtitle, UniqueId, EnableDecoding);

        Recorder = new Remuxer(UniqueId);

        VideoDecoder = new VideoDecoder(Config, UniqueId);
        AudioDecoder = new AudioDecoder(Config, UniqueId, VideoDecoder);
        SubtitlesDecoder = new SubtitlesDecoder(Config, UniqueId);

        if (EnableDecoding && config.Player.Usage != PlayerUsage.Audio)
        {
            VideoDecoder.CreateRenderer();
        }

        VideoDecoder.RecCompleted = RecordCompleted;
        AudioDecoder.RecCompleted = RecordCompleted;
    }

    /// <summary>
    /// 初始化解码器上下文.
    /// </summary>
    public void Initialize()
    {
        if (!Config.Video.ClearScreenOnOpen)
        {
            VideoDecoder.Renderer?.ClearScreen();
        }

        RequiresResync = false;

        OnInitializing();
        Stop();
        OnInitialized();
    }

    /// <summary>
    /// 初始化切换.
    /// </summary>
    public void InitializeSwitch()
    {
        if (!Config.Video.ClearScreenOnOpen)
        {
            VideoDecoder.Renderer?.ClearScreen();
        }

        RequiresResync = false;
        ClosedAudioStream = null;
        ClosedVideoStream = null;
        ClosedSubtitlesStream = null;

        OnInitializingSwitch();
        Stop();
        OnInitializedSwitch();
    }

    /// <summary>
    /// 执行定位操作.
    /// </summary>
    /// <param name="ms">定位的时间点，默认为-1.</param>
    /// <param name="forward">是否向前定位，默认为false.</param>
    /// <param name="seekInQueue">是否在队列中进行定位，默认为true.</param>
    /// <returns>定位结果.</returns>
    public int Seek(long ms = -1, bool forward = false, bool seekInQueue = true)
    {
        var ret = 0;

        if (ms == -1)
        {
            ms = GetCurTimeMs();
        }

        // Review decoder locks (lockAction should be added to avoid dead locks with flush mainly before lockCodecCtx)
        AudioDecoder.resyncWithVideoRequired = false; // Temporary to avoid dead lock on AudioDecoder.lockCodecCtx
        lock (VideoDecoder.lockCodecCtx)
            lock (AudioDecoder.lockCodecCtx)
                lock (SubtitlesDecoder.lockCodecCtx)
                {
                    var seekTimestamp = CalcSeekTimestamp(VideoDemuxer, ms, ref forward);

                    // Should exclude seek in queue for all "local/fast" files
                    lock (VideoDemuxer._lockActions)
                    {
                        if (Playlist.InputType == InputType.Torrent || ms == 0 || !seekInQueue || VideoDemuxer.SeekInQueue(seekTimestamp, forward) != 0)
                        {
                            VideoDemuxer.Interrupter.ForceInterrupt = 1;
                            OpenedPlugin.OnBuffering();
                            lock (VideoDemuxer.lockFmtCtx)
                            {
                                if (VideoDemuxer.Disposed)
                                {
                                    VideoDemuxer.Interrupter.ForceInterrupt = 0;
                                    return -1;
                                }

                                ret = VideoDemuxer.Seek(seekTimestamp, forward);
                            }
                        }
                    }

                    VideoDecoder.Flush();
                    if (AudioStream != null && AudioDecoder.OnVideoDemuxer)
                    {
                        AudioDecoder.Flush();
                    }

                    if (SubtitlesStream != null && SubtitlesDecoder.OnVideoDemuxer)
                    {
                        SubtitlesDecoder.Flush();
                    }
                }

        if (AudioStream != null && !AudioDecoder.OnVideoDemuxer)
        {
            AudioDecoder.Pause();
            AudioDecoder.Flush();
            AudioDemuxer.PauseOnQueueFull = true;
            RequiresResync = true;
        }

        if (SubtitlesStream != null && !SubtitlesDecoder.OnVideoDemuxer)
        {
            SubtitlesDecoder.Pause();
            SubtitlesDecoder.Flush();
            SubtitlesDemuxer.PauseOnQueueFull = true;
            RequiresResync = true;
        }

        return ret;
    }

    /// <summary>
    /// 执行音频定位操作.
    /// </summary>
    /// <param name="ms">定位的时间点，默认为-1.</param>
    /// <param name="forward">是否向前定位，默认为false.</param>
    /// <returns>定位结果.</returns>
    public int SeekAudio(long ms = -1, bool forward = false)
    {
        var ret = 0;

        if (AudioDemuxer.Disposed || AudioDecoder.OnVideoDemuxer || !Config.Audio.Enabled)
        {
            return -1;
        }

        if (ms == -1)
        {
            ms = GetCurTimeMs();
        }

        var seekTimestamp = CalcSeekTimestamp(AudioDemuxer, ms, ref forward);

        AudioDecoder.resyncWithVideoRequired = false; // Temporary to avoid dead lock on AudioDecoder.lockCodecCtx
        lock (AudioDecoder._lockActions)
            lock (AudioDecoder.lockCodecCtx)
            {
                lock (AudioDemuxer._lockActions)
                {
                    if (AudioDemuxer.SeekInQueue(seekTimestamp, forward) != 0)
                    {
                        ret = AudioDemuxer.Seek(seekTimestamp, forward);
                    }
                }

                AudioDecoder.Flush();
                if (VideoDecoder.IsRunning)
                {
                    AudioDemuxer.Start();
                    AudioDecoder.Start();
                }
            }

        return ret;
    }

    /// <summary>
    /// 在视频中查找字幕.
    /// </summary>
    /// <param name="ms">要查找的时间戳（以毫秒为单位），默认为-1.</param>
    /// <param name="forward">是否向前查找，默认为false.</param>
    /// <returns>查找结果.</returns>
    public int SeekSubtitles(long ms = -1, bool forward = false)
    {
        var ret = 0;

        if (SubtitlesDemuxer.Disposed || SubtitlesDecoder.OnVideoDemuxer || !Config.Subtitles.Enabled)
        {
            return -1;
        }

        if (ms == -1)
        {
            ms = GetCurTimeMs();
        }

        var seekTimestamp = CalcSeekTimestamp(SubtitlesDemuxer, ms, ref forward);

        lock (SubtitlesDecoder._lockActions)
            lock (SubtitlesDecoder.lockCodecCtx)
            {
                ret = SubtitlesDemuxer.Seek(seekTimestamp, forward);

                SubtitlesDecoder.Flush();
                if (VideoDecoder.IsRunning)
                {
                    SubtitlesDemuxer.Start();
                    SubtitlesDecoder.Start();
                }
            }

        return ret;
    }

    /// <summary>
    /// 获取当前时间.
    /// </summary>
    /// <returns>当前时间.</returns>
    public long GetCurTime() => !VideoDemuxer.Disposed ? VideoDemuxer.CurTime : !AudioDemuxer.Disposed ? AudioDemuxer.CurTime : 0;

    /// <summary>
    /// 获取当前时间（以毫秒为单位）.
    /// </summary>
    /// <returns>当前时间（以毫秒为单位）.</returns>
    public int GetCurTimeMs() => !VideoDemuxer.Disposed ? (int)(VideoDemuxer.CurTime / 10000) : (!AudioDemuxer.Disposed ? (int)(AudioDemuxer.CurTime / 10000) : 0);

    /// <summary>
    /// 暂停播放.
    /// </summary>
    public void Pause()
    {
        VideoDecoder.Pause();
        AudioDecoder.Pause();
        SubtitlesDecoder.Pause();

        VideoDemuxer.Pause();
        AudioDemuxer.Pause();
        SubtitlesDemuxer.Pause();
    }

    /// <summary>
    /// 暂停解码器.
    /// </summary>
    public void PauseDecoders()
    {
        VideoDecoder.Pause();
        AudioDecoder.Pause();
        SubtitlesDecoder.Pause();
    }

    /// <summary>
    /// 当队列满时暂停.
    /// </summary>
    public void PauseOnQueueFull()
    {
        VideoDemuxer.PauseOnQueueFull = true;
        AudioDemuxer.PauseOnQueueFull = true;
        SubtitlesDemuxer.PauseOnQueueFull = true;
    }

    /// <summary>
    /// 开始播放.
    /// </summary>
    public void Start()
    {
        if (Config.Audio.Enabled)
        {
            AudioDemuxer.Start();
            AudioDecoder.Start();
        }

        if (Config.Video.Enabled)
        {
            VideoDemuxer.Start();
            VideoDecoder.Start();
        }

        if (Config.Subtitles.Enabled)
        {
            SubtitlesDemuxer.Start();
            SubtitlesDecoder.Start();
        }
    }

    /// <summary>
    /// 停止播放.
    /// </summary>
    public void Stop()
    {
        Interrupt = true;

        VideoDecoder.Dispose();
        AudioDecoder.Dispose();
        SubtitlesDecoder.Dispose();
        AudioDemuxer.Dispose();
        SubtitlesDemuxer.Dispose();
        VideoDemuxer.Dispose();

        Interrupt = false;
    }

    /// <summary>
    /// 停止线程.
    /// </summary>
    public void StopThreads()
    {
        Interrupt = true;

        VideoDecoder.Stop();
        AudioDecoder.Stop();
        SubtitlesDecoder.Stop();
        AudioDemuxer.Stop();
        SubtitlesDemuxer.Stop();
        VideoDemuxer.Stop();

        Interrupt = false;
    }

    /// <summary>
    /// 重新同步.
    /// </summary>
    /// <param name="timestamp">时间戳，默认为-1.</param>
    public void Resync(long timestamp = -1)
    {
        var isRunning = VideoDemuxer.IsRunning;

        if (AudioStream != null && AudioStream.Demuxer.Type != MediaType.Video && Config.Audio.Enabled)
        {
            if (timestamp == -1)
            {
                timestamp = VideoDemuxer.CurTime;
            }

            if (CanInfo)
            {
                Log.Info($"Resync audio to {Utils.TicksToTime(timestamp)}");
            }

            SeekAudio(timestamp / 10000);
            if (isRunning)
            {
                AudioDemuxer.Start();
                AudioDecoder.Start();
            }
        }

        if (SubtitlesStream != null && SubtitlesStream.Demuxer.Type != MediaType.Video && Config.Subtitles.Enabled)
        {
            if (timestamp == -1)
            {
                timestamp = VideoDemuxer.CurTime;
            }

            if (CanInfo)
            {
                Log.Info($"Resync subs to {Utils.TicksToTime(timestamp)}");
            }

            SeekSubtitles(timestamp / 10000);
            if (isRunning)
            {
                SubtitlesDemuxer.Start();
                SubtitlesDecoder.Start();
            }
        }

        RequiresResync = false;
    }

    /// <summary>
    /// 重新同步字幕.
    /// </summary>
    /// <param name="timestamp">时间戳，默认为-1.</param>
    public void ResyncSubtitles(long timestamp = -1)
    {
        if (SubtitlesStream != null && Config.Subtitles.Enabled)
        {
            if (timestamp == -1)
            {
                timestamp = VideoDemuxer.CurTime;
            }

            if (CanInfo)
            {
                Log.Info($"Resync subs to {Utils.TicksToTime(timestamp)}");
            }

            if (SubtitlesStream.Demuxer.Type != MediaType.Video)
            {
                SeekSubtitles(timestamp / 10000);
            }
            else

            if (VideoDemuxer.IsRunning)
            {
                SubtitlesDemuxer.Start();
                SubtitlesDecoder.Start();
            }
        }
    }

    /// <summary>
    /// 清空缓冲区.
    /// </summary>
    public void Flush()
    {
        VideoDemuxer.DisposePackets();
        AudioDemuxer.DisposePackets();
        SubtitlesDemuxer.DisposePackets();

        VideoDecoder.Flush();
        AudioDecoder.Flush();
        SubtitlesDecoder.Flush();
    }

    /// <summary>
    /// 获取视频帧.
    /// </summary>
    /// <param name="timestamp">时间戳，默认为-1.</param>
    /// <returns>视频帧的时间戳.</returns>
    public long GetVideoFrame(long timestamp = -1)
    {
        int ret;
        var packet = av_packet_alloc();
        var frame = av_frame_alloc();

        lock (VideoDemuxer.lockFmtCtx)
            lock (VideoDecoder.lockCodecCtx)
            {
                while (VideoDemuxer.VideoStream != null && !Interrupt)
                {
                    if (VideoDemuxer.VideoPackets.Count == 0)
                    {
                        VideoDemuxer.Interrupter.Request(Requester.Read);
                        ret = av_read_frame(VideoDemuxer.FormatContext, packet);
                        if (ret != 0)
                        {
                            return -1;
                        }
                    }
                    else
                    {
                        packet = VideoDemuxer.VideoPackets.Dequeue();
                    }

                    if (!VideoDemuxer.EnabledStreams.Contains(packet->stream_index))
                    {
                        av_packet_unref(packet);
                        continue;
                    }

                    // Early check for keyframe (in demux instead of decode)
                    var codecType = VideoDemuxer.FormatContext->streams[packet->stream_index]->codecpar->codec_type;
                    if (VideoDecoder.keyFrameRequired && (codecType != AVMEDIA_TYPE_VIDEO || (packet->flags & AV_PKT_FLAG_KEY) == 0))
                    {
                        av_packet_unref(packet);
                        continue;
                    } // early key check to avoid decoding

                    if (VideoDemuxer.IsHLSLive)
                    {
                        VideoDemuxer.UpdateHLSTime();
                    }

                    if (CanTrace)
                    {
                        var stream = VideoDemuxer.AVStreamToStream[packet->stream_index];
                        var dts = packet->dts == AV_NOPTS_VALUE ? -1 : (long)(packet->dts * stream.TimeBase);
                        var pts = packet->pts == AV_NOPTS_VALUE ? -1 : (long)(packet->pts * stream.TimeBase);
                        Log.Trace($"[{stream.Type}] DTS: {(dts == -1 ? "-" : Utils.TicksToTime(dts))} PTS: {(pts == -1 ? "-" : Utils.TicksToTime(pts))} | FLPTS: {(pts == -1 ? "-" : Utils.TicksToTime(pts - VideoDemuxer.StartTime))} | CurTime: {Utils.TicksToTime(VideoDemuxer.CurTime)} | Buffered: {Utils.TicksToTime(VideoDemuxer.BufferedDuration)}");
                    }

                    switch (codecType)
                    {
                        case AVMEDIA_TYPE_AUDIO:
                            if ((timestamp == -1 && !VideoDecoder.keyFrameRequired) || (long)(packet->pts * AudioStream.TimeBase) - VideoDemuxer.StartTime + (VideoStream.FrameDuration / 2) > timestamp)
                            {
                                VideoDemuxer.AudioPackets.Enqueue(packet);
                            }

                            packet = av_packet_alloc();

                            continue;

                        case AVMEDIA_TYPE_SUBTITLE:
                            if ((timestamp == -1 && !VideoDecoder.keyFrameRequired) || (long)(packet->pts * SubtitlesStream.TimeBase) - VideoDemuxer.StartTime + (VideoStream.FrameDuration / 2) > timestamp)
                            {
                                VideoDemuxer.SubtitlesPackets.Enqueue(packet);
                            }

                            packet = av_packet_alloc();

                            continue;

                        case AVMEDIA_TYPE_VIDEO:
                            ret = avcodec_send_packet(VideoDecoder.CodecCtx, packet);

                            if (VideoDecoder.swFallback)
                            {
                                VideoDecoder.SWFallback();
                                ret = avcodec_send_packet(VideoDecoder.CodecCtx, packet);
                            }

                            if (ret != 0)
                            {
                                av_packet_free(&packet);
                                return -1;
                            }

                            while (VideoDemuxer.VideoStream != null && !Interrupt)
                            {
                                ret = avcodec_receive_frame(VideoDecoder.CodecCtx, frame);
                                if (ret != 0)
                                {
                                    av_frame_unref(frame);
                                    break;
                                }

                                if (frame->best_effort_timestamp != AV_NOPTS_VALUE)
                                {
                                    frame->pts = frame->best_effort_timestamp;
                                }
                                else if (frame->pts == AV_NOPTS_VALUE)
                                {
                                    av_frame_unref(frame);
                                    continue;
                                }

                                if (VideoDecoder.keyFrameRequired && frame->pict_type != AVPictureType.AV_PICTURE_TYPE_I && frame->key_frame != 1)
                                {
                                    if (CanWarn)
                                    {
                                        Log.Warn($"Seek to keyframe failed [{frame->pict_type} | {frame->key_frame}]");
                                    }

                                    av_frame_unref(frame);
                                    continue;
                                }

                                VideoDecoder.keyFrameRequired = false;

                                if (!VideoDecoder.filledFromCodec)
                                {
                                    ret = VideoDecoder.FillFromCodec(frame);

                                    if (ret == -1234)
                                    {
                                        av_packet_free(&packet);
                                        av_frame_free(&frame);
                                        return -1;
                                    }
                                }

                                // Accurate seek with +- half frame distance
                                if (timestamp != -1 && (long)(frame->pts * VideoStream.TimeBase) - VideoDemuxer.StartTime + (VideoStream.FrameDuration / 2) < timestamp)
                                {
                                    av_frame_unref(frame);
                                    continue;
                                }

                                VideoDecoder.StartTime = (long)(frame->pts * VideoStream.TimeBase) - VideoDemuxer.StartTime;

                                var mFrame = VideoDecoder.Renderer.FillPlanes(frame);
                                if (mFrame != null)
                                {
                                    VideoDecoder.Frames.Enqueue(mFrame);
                                }

                                do
                                {
                                    av_frame_free(&frame);
                                    frame = av_frame_alloc();
                                    ret = avcodec_receive_frame(VideoDecoder.CodecCtx, frame);
                                    if (ret != 0)
                                    {
                                        break;
                                    }

                                    mFrame = VideoDecoder.Renderer.FillPlanes(frame);
                                    if (mFrame != null)
                                    {
                                        VideoDecoder.Frames.Enqueue(mFrame);
                                    }
                                }
                                while (!VideoDemuxer.Disposed && !Interrupt);

                                av_packet_free(&packet);
                                av_frame_free(&frame);
                                return mFrame.Timestamp;
                            }

                            av_packet_free(&packet);
                            packet = av_packet_alloc();

                            break;
                    }
                }
            }

        av_packet_free(&packet);
        av_frame_free(&frame);
        return -1;
    }

    /// <summary>
    /// 释放资源.
    /// </summary>
    public new void Dispose()
    {
        _shouldDispose = true;
        Stop();
        Interrupt = true;
        VideoDecoder.DestroyRenderer();
        base.Dispose();
    }

    /// <summary>
    /// 打印统计信息.
    /// </summary>
    public void PrintStats()
    {
        var dump = "\r\n-===== Streams / Packets / Frames =====-\r\n";
        dump += $"\r\n AudioPackets      ({VideoDemuxer.AudioStreams.Count}): {VideoDemuxer.AudioPackets.Count}";
        dump += $"\r\n VideoPackets      ({VideoDemuxer.VideoStreams.Count}): {VideoDemuxer.VideoPackets.Count}";
        dump += $"\r\n SubtitlesPackets  ({VideoDemuxer.SubtitlesStreams.Count}): {VideoDemuxer.SubtitlesPackets.Count}";
        dump += $"\r\n AudioPackets      ({AudioDemuxer.AudioStreams.Count}): {AudioDemuxer.AudioPackets.Count} (AudioDemuxer)";
        dump += $"\r\n SubtitlesPackets  ({SubtitlesDemuxer.SubtitlesStreams.Count}): {SubtitlesDemuxer.SubtitlesPackets.Count} (SubtitlesDemuxer)";

        dump += $"\r\n Video Frames         : {VideoDecoder.Frames.Count}";
        dump += $"\r\n Audio Frames         : {AudioDecoder.Frames.Count}";
        dump += $"\r\n Subtitles Frames     : {SubtitlesDecoder.Frames.Count}";

        if (CanInfo)
        {
            Log.Info(dump);
        }
    }

    /// <summary>
    /// 开始录制.
    /// </summary>
    /// <param name="filename">文件名.</param>
    /// <param name="useRecommendedExtension">是否使用推荐的扩展名，默认为true.</param>
    public void StartRecording(ref string filename, bool useRecommendedExtension = true)
    {
        if (IsRecording)
        {
            StopRecording();
        }

        _oldMaxAudioFrames = -1;
        _recHasVideo = false;

        if (CanInfo)
        {
            Log.Info("Record Start");
        }

        _recHasVideo = !VideoDecoder.Disposed && VideoDecoder.Stream != null;

        if (useRecommendedExtension)
        {
            filename = $"{filename}.{(_recHasVideo ? VideoDecoder.Stream.Demuxer.Extension : AudioDecoder.Stream.Demuxer.Extension)}";
        }

        Recorder.Open(filename);

        bool failed;

        if (_recHasVideo)
        {
            failed = Recorder.AddStream(VideoDecoder.Stream.AVStream) != 0;
            if (CanInfo)
            {
                Log.Info(failed ? "Failed to add video stream" : "Video stream added to the recorder");
            }
        }

        if (!AudioDecoder.Disposed && AudioDecoder.Stream != null)
        {
            failed = Recorder.AddStream(AudioDecoder.Stream.AVStream, !AudioDecoder.OnVideoDemuxer) != 0;
            if (CanInfo)
            {
                Log.Info(failed ? "Failed to add audio stream" : "Audio stream added to the recorder");
            }
        }

        if (!Recorder.HasStreams || Recorder.WriteHeader() != 0)
        {
            return;
        }

        // Check also buffering and possible Diff of first audio/video timestamp to remuxer to ensure sync between each other (shouldn't be more than 30-50ms)
        _oldMaxAudioFrames = Config.Decoder.MaxAudioFrames;
        Config.Decoder.MaxAudioFrames = Config.Decoder.MaxVideoFrames;

        VideoDecoder.StartRecording(Recorder);
        AudioDecoder.StartRecording(Recorder);
    }

    /// <summary>
    /// 停止录制.
    /// </summary>
    public void StopRecording()
    {
        if (_oldMaxAudioFrames != -1)
        {
            Config.Decoder.MaxAudioFrames = _oldMaxAudioFrames;
        }

        VideoDecoder.StopRecording();
        AudioDecoder.StopRecording();
        Recorder.Dispose();
        _oldMaxAudioFrames = -1;
        if (CanInfo)
        {
            Log.Info("Record Completed"); // 记录完成.
        }
    }

    /// <summary>
    /// 录制完成的回调方法.
    /// </summary>
    /// <param name="type">媒体类型.</param>
    internal void RecordCompleted(MediaType type)
    {
        if (!_recHasVideo || (_recHasVideo && type == MediaType.Video))
        {
            StopRecording(); // 停止录制.
            RecordingCompleted?.Invoke(this, new EventArgs());
        }
    }

    /// <summary>
    /// 计算查找时间戳.
    /// </summary>
    /// <param name="demuxer">解复用器.</param>
    /// <param name="ms">要查找的时间戳（以毫秒为单位）.</param>
    /// <param name="forward">是否向前查找.</param>
    /// <returns>查找时间戳.</returns>
    private long CalcSeekTimestamp(Demuxer demuxer, long ms, ref bool forward)
    {
        var startTime = demuxer.hlsCtx == null ? demuxer.StartTime : demuxer.hlsCtx->first_timestamp * 10;
        var ticks = (ms * 10000) + startTime;

        if (demuxer.Type == MediaType.Audio)
        {
            ticks -= Config.Audio.Delay;
        }

        if (demuxer.Type == MediaType.Subtitle)
        {
            ticks -= Config.Subtitles.Delay + (2 * 1000 * 10000); // We even want the previous subtitles
        }

        if (ticks < startTime)
        {
            ticks = startTime;
            forward = true;
        }
        else if (ticks > startTime + (!VideoDemuxer.Disposed ? VideoDemuxer.Duration : AudioDemuxer.Duration) - (50 * 10000))
        {
            ticks = startTime + demuxer.Duration - (50 * 10000);
            forward = false;
        }

        return ticks;
    }
}
