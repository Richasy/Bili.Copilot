// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Concurrent;
using System.Threading;
using Bili.Copilot.Libs.Player.Core;
using Bili.Copilot.Libs.Player.Core.Engines;
using Bili.Copilot.Libs.Player.Enums;
using Bili.Copilot.Libs.Player.MediaFramework.MediaFrame;
using Bili.Copilot.Libs.Player.MediaFramework.MediaRemuxer;
using Bili.Copilot.Libs.Player.MediaFramework.MediaStream;
using FFmpeg.AutoGen;
using static Bili.Copilot.Libs.Player.Misc.Logger;
using static FFmpeg.AutoGen.ffmpeg;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaDecoder;

/// <summary>
/// 音频解码器类，继承自DecoderBase类.
/// </summary>
public unsafe partial class AudioDecoder : DecoderBase
{
    /// <summary>
    /// 输出音频样本格式.
    /// </summary>
    private static readonly AVSampleFormat _aOutSampleFormat = AVSampleFormat.AV_SAMPLE_FMT_S16;

    /// <summary>
    /// 输出音频样本格式的字符串表示.
    /// </summary>
    private static readonly string _aOutSampleFormatStr = av_get_sample_fmt_name(_aOutSampleFormat);

    /// <summary>
    /// 输出音频通道数.
    /// </summary>
    private static readonly int _aOutChannels = _aOutChannelLayout.nb_channels;

    /// <summary>
    /// 每个样本的字节数.
    /// </summary>
    private static readonly int _aSampleBytes = av_get_bytes_per_sample(_aOutSampleFormat) * _aOutChannels;

    /// <summary>
    /// 缓冲区时间大小.
    /// </summary>
    private static readonly int _cBufTimesSize = 4;

    /// <summary>
    /// 输出音频通道布局.
    /// </summary>
    private static AVChannelLayout _aOutChannelLayout = new() { order = AVChannelOrder.AV_CHANNEL_ORDER_NATIVE, nb_channels = 2, u = new AVChannelLayout_u() { mask = AV_CH_FRONT_LEFT | AV_CH_FRONT_RIGHT } };

    /// <summary>
    /// 当前缓冲区时间.
    /// </summary>
    private int _cBufTimesCur = 1;

    /// <summary>
    /// 缓冲区.
    /// </summary>
    private byte[] _cBuf;

    /// <summary>
    /// 缓冲区位置.
    /// </summary>
    private int _cBufPos;

    /// <summary>
    /// 缓冲区样本数.
    /// </summary>
    private int _cBufSamples;

    /// <summary>
    /// SwrContext指针.
    /// </summary>
    private SwrContext* _swrCtx;

    /// <summary>
    /// 当前录制器.
    /// </summary>
    private Remuxer _curRecorder;

    /// <summary>
    /// 是否已获取关键帧.
    /// </summary>
    private bool _recGotKeyframe;

    /// <summary>
    /// 构造函数，用于创建AudioDecoder实例.
    /// </summary>
    /// <param name="config">配置信息.</param>
    /// <param name="uniqueId">唯一标识符.</param>
    /// <param name="syncDecoder">同步解码器.</param>
    public AudioDecoder(Config config, int uniqueId = -1, VideoDecoder syncDecoder = null)
        : base(config, uniqueId)
        => VideoDecoder = syncDecoder;

    /// <summary>
    /// 获取关联的VideoDecoder实例.
    /// </summary>
    public VideoDecoder VideoDecoder { get; }

    /// <summary>
    /// 获取音频流.
    /// </summary>
    public AudioStream AudioStream => (AudioStream)Stream;

    /// <summary>
    /// 获取音频帧队列.
    /// </summary>
    public ConcurrentQueue<AudioFrame> Frames { get; protected set; } = new();

    /// <summary>
    /// 获取或设置是否需要与视频进行重新同步.
    /// </summary>
    internal bool ResyncWithVideoRequired { get; set; }

    /// <summary>
    /// 获取或设置是否正在录制.
    /// </summary>
    internal bool IsRecording { get; private set; }

    /// <summary>
    /// 获取或设置缓冲区分配的动作.
    /// </summary>
    internal Action CBufAlloc { get; set; }

    /// <summary>
    /// 获取或设置录制完成的动作.
    /// </summary>
    internal Action<MediaType> RecCompleted { get; set; }

    /// <summary>
    /// 释放帧.
    /// </summary>
    public void DisposeFrames()
        => Frames = new();

    /// <summary>
    /// 刷新.
    /// </summary>
    public void Flush()
    {
        lock (LockActions)
            lock (_lockCodecCtx)
            {
                if (Disposed)
                {
                    return;
                }

                if (Status == ThreadStatus.Ended)
                {
                    Status = ThreadStatus.Stopped;
                }
                else if (Status == ThreadStatus.Draining)
                {
                    Status = ThreadStatus.Stopping;
                }

                ResyncWithVideoRequired = !VideoDecoder.Disposed;

                DisposeFrames();
                avcodec_flush_buffers(codecCtx);
                if (_filterGraph != null)
                {
                    SetupFilters();
                }
            }
    }

    internal void StartRecording(Remuxer remuxer)
    {
        if (Disposed || IsRecording)
        {
            return;
        }

        _curRecorder = remuxer;
        IsRecording = true;
        _recGotKeyframe = VideoDecoder.Disposed || VideoDecoder.Stream == null;
    }

    internal void StopRecording() => IsRecording = false;

    /// <summary>
    /// 重写基类的Setup方法.
    /// </summary>
    /// <param name="codec">AVCodec指针.</param>
    /// <returns>返回0.</returns>
    protected override int Setup(AVCodec* codec) => 0;

    /// <inheritdoc/>
    protected override void DisposeInternal()
    {
        DisposeFrames();
        DisposeSwr();
        DisposeFilters();

        _cBuf = null;
        _cBufSamples = 0;
        filledFromCodec = false;
    }

    /// <inheritdoc/>
    protected override void RunInternal()
    {
        var ret = 0;
        var allowedErrors = Config.Decoder.MaxErrors;
        var sleepMs = Config.Decoder.MaxAudioFrames > 5 && Config.Player.MaxLatency == 0 ? 10 : 4;
        AVPacket* packet;

        do
        {
            // Wait until Queue not Full or Stopped
            if (Frames.Count >= Config.Decoder.MaxAudioFrames)
            {
                lock (LockStatus)
                {
                    if (Status == ThreadStatus.Running)
                    {
                        Status = ThreadStatus.QueueFull;
                    }
                }

                while (Frames.Count >= Config.Decoder.MaxAudioFrames && Status == ThreadStatus.QueueFull)
                {
                    Thread.Sleep(sleepMs);
                }

                lock (LockStatus)
                {
                    if (Status != ThreadStatus.QueueFull)
                    {
                        break;
                    }

                    Status = ThreadStatus.Running;
                }
            }

            // While Packets Queue Empty (Ended | Quit if Demuxer stopped | Wait until we get packets)
            if (demuxer.AudioPackets.Count == 0)
            {
                CriticalArea = true;

                lock (LockStatus)
                {
                    if (Status == ThreadStatus.Running)
                    {
                        Status = ThreadStatus.QueueEmpty;
                    }
                }

                while (demuxer.AudioPackets.Count == 0 && Status == ThreadStatus.QueueEmpty)
                {
                    if (demuxer.Status == ThreadStatus.Ended)
                    {
                        lock (LockStatus)
                        {
                            // TODO: let the demuxer push the draining packet
                            Log.Debug("Draining");
                            Status = ThreadStatus.Draining;
                            var drainPacket = av_packet_alloc();
                            drainPacket->data = null;
                            drainPacket->size = 0;
                            demuxer.AudioPackets.Enqueue(drainPacket);
                        }

                        break;
                    }
                    else if (!demuxer.IsRunning)
                    {
                        if (CanDebug)
                        {
                            Log.Debug($"Demuxer is not running [Demuxer Status: {demuxer.Status}]");
                        }

                        var retries = 5;

                        while (retries > 0)
                        {
                            retries--;
                            Thread.Sleep(10);
                            if (demuxer.IsRunning)
                            {
                                break;
                            }
                        }

                        lock (demuxer.LockStatus)
                            lock (LockStatus)
                            {
                                if (demuxer.Status == ThreadStatus.Pausing || demuxer.Status == ThreadStatus.Paused)
                                {
                                    Status = ThreadStatus.Pausing;
                                }
                                else if (demuxer.Status != ThreadStatus.Ended)
                                {
                                    Status = ThreadStatus.Stopping;
                                }
                                else
                                {
                                    continue;
                                }
                            }

                        break;
                    }

                    Thread.Sleep(sleepMs);
                }

                lock (LockStatus)
                {
                    CriticalArea = false;
                    if (Status != ThreadStatus.QueueEmpty && Status != ThreadStatus.Draining)
                    {
                        break;
                    }

                    if (Status != ThreadStatus.Draining)
                    {
                        Status = ThreadStatus.Running;
                    }
                }
            }

            Monitor.Enter(_lockCodecCtx); // restore the old lock / add interrupters similar to the demuxer
            try
            {
                if (Status == ThreadStatus.Stopped)
                {
                    Monitor.Exit(_lockCodecCtx);
                    continue;
                }

                packet = demuxer.AudioPackets.Dequeue();

                if (packet == null)
                {
                    Monitor.Exit(_lockCodecCtx);
                    continue;
                }

                if (IsRecording)
                {
                    if (!_recGotKeyframe && VideoDecoder.StartRecordTime != AV_NOPTS_VALUE && (long)(packet->pts * AudioStream.TimeBase) - demuxer.StartTime > VideoDecoder.StartRecordTime)
                    {
                        _recGotKeyframe = true;
                    }

                    if (_recGotKeyframe)
                    {
                        _curRecorder.Write(av_packet_clone(packet), !OnVideoDemuxer);
                    }
                }

                ret = avcodec_send_packet(codecCtx, packet);
                av_packet_free(&packet);

                if (ret != 0 && ret != AVERROR(EAGAIN))
                {
                    if (ret == AVERROR_EOF)
                    {
                        Status = ThreadStatus.Ended;
                        break;
                    }
                    else
                    {
                        allowedErrors--;
                        if (CanWarn)
                        {
                            Log.Warn($"{FFmpegEngine.ErrorCodeToMsg(ret)} ({ret})");
                        }

                        if (allowedErrors == 0)
                        {
                            Log.Error("Too many errors!");
                            Status = ThreadStatus.Stopping;
                            break;
                        }

                        Monitor.Exit(_lockCodecCtx);
                        continue;
                    }
                }

                while (true)
                {
                    ret = avcodec_receive_frame(codecCtx, frame);
                    if (ret != 0)
                    {
                        av_frame_unref(frame);

                        if (ret == AVERROR_EOF && _filterGraph != null)
                        {
                            lock (_lockSpeed)
                            {
                                DrainFilters();
                                Status = ThreadStatus.Ended;
                            }
                        }

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

                    var codecChanged = AudioStream.SampleFormat != codecCtx->sample_fmt || AudioStream.SampleRate != codecCtx->sample_rate || AudioStream.ChannelLayout != codecCtx->ch_layout.u.mask;

                    if (!filledFromCodec || codecChanged)
                    {
                        if (codecChanged && filledFromCodec)
                        {
                            var buf = new byte[50];
                            fixed (byte* bufPtr = buf)
                            {
                                av_channel_layout_describe(&codecCtx->ch_layout, bufPtr, (ulong)buf.Length);
                                Log.Warn($"Codec changed {AudioStream.CodecIdOrig} {AudioStream.SampleFormat} {AudioStream.SampleRate} {AudioStream.ChannelLayoutStr} => {codecCtx->codec_id} {codecCtx->sample_fmt} {codecCtx->sample_rate} {Utils.BytePtrToStringUtf8(bufPtr)}");
                            }
                        }

                        DisposeInternal();
                        filledFromCodec = true;

                        avcodec_parameters_from_context(Stream.AVStream->codecpar, codecCtx);
                        AudioStream.AVStream->time_base = codecCtx->pkt_timebase;
                        AudioStream.Refresh();
                        ResyncWithVideoRequired = !VideoDecoder.Disposed;

                        ret = SetupFiltersOrSwr();

                        CodecChanged?.Invoke(this);

                        if (ret != 0)
                        {
                            Status = ThreadStatus.Stopping;
                            break;
                        }
                    }

                    if (ResyncWithVideoRequired)
                    {
                        // TODO: in case of long distance will spin (CPU issue), possible reseek?
                        var ts = (long)(frame->pts * AudioStream.TimeBase) - demuxer.StartTime + Config.Audio.Delay;

                        while (VideoDecoder.StartTime == AV_NOPTS_VALUE && VideoDecoder.IsRunning && ResyncWithVideoRequired)
                        {
                            Thread.Sleep(10);
                        }

                        if (ts < VideoDecoder.StartTime)
                        {
                            if (CanTrace)
                            {
                                Log.Trace($"Drops {Utils.TicksToTime(ts)} (< V: {Utils.TicksToTime(VideoDecoder.StartTime)})");
                            }

                            av_frame_unref(frame);
                            continue;
                        }
                        else
                        {
                            ResyncWithVideoRequired = false;
                        }
                    }

                    lock (_lockSpeed)
                    {
                        if (_filterGraph != null)
                        {
                            ProcessFilters(frame);
                        }
                        else
                        {
                            Process(frame);
                        }
                    }
                }
            }
            catch
            {
            }

            Monitor.Exit(_lockCodecCtx);
        }
        while (Status == ThreadStatus.Running);

        if (IsRecording)
        {
            StopRecording();
            RecCompleted(MediaType.Audio);
        }

        if (Status == ThreadStatus.Draining)
        {
            Status = ThreadStatus.Ended;
        }
    }

    private int SetupSwr()
    {
        int ret;

        DisposeSwr();
        _swrCtx = swr_alloc();

        av_opt_set_chlayout(_swrCtx, "in_chlayout", &codecCtx->ch_layout, 0);
        av_opt_set_int(_swrCtx, "in_sample_rate", codecCtx->sample_rate, 0);
        av_opt_set_sample_fmt(_swrCtx, "in_sample_fmt", codecCtx->sample_fmt, 0);

        fixed (AVChannelLayout* ptr = &_aOutChannelLayout)
        {
            av_opt_set_chlayout(_swrCtx, "out_chlayout", ptr, 0);
        }

        av_opt_set_int(_swrCtx, "out_sample_rate", codecCtx->sample_rate, 0);
        av_opt_set_sample_fmt(_swrCtx, "out_sample_fmt", _aOutSampleFormat, 0);

        ret = swr_init(_swrCtx);
        if (ret < 0)
        {
            Log.Error($"Swr setup failed {FFmpegEngine.ErrorCodeToMsg(ret)} ({ret})");
        }

        return ret;
    }

    private void DisposeSwr()
    {
        if (_swrCtx == null)
        {
            return;
        }

        swr_close(_swrCtx);

        fixed (SwrContext** ptr = &_swrCtx)
        {
            swr_free(ptr);
        }

        _swrCtx = null;
    }

    private void Process(AVFrame* frame)
    {
        try
        {
            var dataLen = frame->nb_samples * _aSampleBytes;
            var speedDataLen = Utils.Align((int)(dataLen / speed), _aSampleBytes);

            AudioFrame mFrame = new()
            {
                Timestamp = (long)(frame->pts * AudioStream.TimeBase) - demuxer.StartTime + Config.Audio.Delay,
                DataLen = speedDataLen,
            };
            if (CanTrace)
            {
                Log.Trace($"Processes {Utils.TicksToTime(mFrame.Timestamp)}");
            }

            if (frame->nb_samples > _cBufSamples)
            {
                AllocateCircularBuffer(frame->nb_samples);
            }
            else if (_cBufPos + Math.Max(dataLen, speedDataLen) >= _cBuf.Length)
            {
                _cBufPos = 0;
            }

            fixed (byte* circularBufferPosPtr = &_cBuf[_cBufPos])
            {
                var ret = swr_convert(_swrCtx, &circularBufferPosPtr, frame->nb_samples, (byte**)&frame->data, frame->nb_samples);
                if (ret < 0)
                {
                    return;
                }

                mFrame.DataPtr = (IntPtr)circularBufferPosPtr;
            }

            // Fill silence
            if (speed < 1)
            {
                for (var p = dataLen; p < speedDataLen; p++)
                {
                    _cBuf[_cBufPos + p] = 0;
                }
            }

            _cBufPos += Math.Max(dataLen, speedDataLen);
            Frames.Enqueue(mFrame);

            // Wait until Queue not Full or Stopped
            if (Frames.Count >= Config.Decoder.MaxAudioFrames * _cBufTimesCur)
            {
                Monitor.Exit(_lockCodecCtx);
                lock (LockStatus)
                {
                    if (Status == ThreadStatus.Running)
                    {
                        Status = ThreadStatus.QueueFull;
                    }
                }

                while (Frames.Count >= Config.Decoder.MaxAudioFrames * _cBufTimesCur && Status == ThreadStatus.QueueFull)
                {
                    Thread.Sleep(20);
                }

                Monitor.Enter(_lockCodecCtx);

                lock (LockStatus)
                {
                    if (Status != ThreadStatus.QueueFull)
                    {
                        return;
                    }

                    Status = ThreadStatus.Running;
                }
            }
        }
        catch (Exception e)
        {
            Log.Error($"Failed to process frame ({e.Message})");

            return;
        }
        finally
        {
            av_frame_unref(frame);
        }
    }

    private void AllocateCircularBuffer(int samples)
    {
        /* TBR
        * 1. If we change to different in/out sample rates we need to calculate delay
        * 2. By destorying the cBuf can create critical issues while the audio decoder reads the data? (add lock) | we need to copy the lost data and change the pointers
        * 3. Recalculate on Config.Decoder.MaxAudioFrames change (greater)
        * 4. cBufTimesSize cause filters can pass the limit when we need to use lockSpeed
        */

        samples = Math.Max(10000, samples); // 10K samples to ensure that currently we will not re-allocate?
        var size = Config.Decoder.MaxAudioFrames * samples * _aSampleBytes * _cBufTimesSize;
        Log.Debug($"Re-allocating circular buffer ({samples} > {_cBufSamples}) with {size}bytes");

        DisposeFrames(); // TODO: copy data
        CBufAlloc?.Invoke();
        _cBuf = new byte[size];
        _cBufPos = 0;
        _cBufSamples = samples;
    }
}
