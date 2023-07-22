// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Runtime.InteropServices;
using System.Threading;
using Bili.Copilot.Libs.Player.Core;
using Bili.Copilot.Libs.Player.Core.Engines;
using Bili.Copilot.Libs.Player.Enums;
using Bili.Copilot.Libs.Player.MediaFramework.MediaFrame;
using Bili.Copilot.Libs.Player.MediaFramework.MediaStream;
using FFmpeg.AutoGen;
using static Bili.Copilot.Libs.Player.Misc.Logger;
using static FFmpeg.AutoGen.ffmpeg;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaDecoder;

/// <summary>
/// 音频解码器.
/// </summary>
public unsafe partial class AudioDecoder
{
    private readonly object _lockSpeed = new();
    private AVFilterContext* _aBufferCtx;
    private AVFilterContext* _aBufferSinkCtx;
    private AVFilterGraph* _filterGraph;
    private bool _aBufferDrained;
    private long _curSamples;
    private double _missedSamples;
    private long _filterFirstPts;
    private bool _setFirstPts;

    /// <summary>
    /// 更新过滤器.
    /// </summary>
    /// <param name="filterId">过滤器ID.</param>
    /// <param name="key">键.</param>
    /// <param name="value">值.</param>
    /// <returns>更新结果.</returns>
    public int UpdateFilter(string filterId, string key, string value)
    {
        lock (_lockCodecCtx)
        {
            return _filterGraph != null ? UpdateFilterInternal(filterId, key, value) : -1;
        }
    }

    /// <summary>
    /// 重新加载过滤器.
    /// </summary>
    /// <returns>重新加载结果.</returns>
    public int ReloadFilters()
    {
        lock (LockActions)
            lock (_lockCodecCtx)
            {
                return !Engine.FFmpeg.FiltersLoaded || Config.Audio.FiltersEnabled ? -1 : SetupFilters();
            }
    }

    /// <summary>
    /// 修复音频帧的样本.
    /// </summary>
    /// <param name="frame">音频帧.</param>
    /// <param name="oldSpeed">旧速度.</param>
    /// <param name="speed">速度.</param>
    internal void FixSample(AudioFrame frame, double oldSpeed, double speed)
    {
        var oldDataLen = frame.DataLen;
        frame.DataLen = Utils.Align((int)(oldDataLen * oldSpeed / speed), _aSampleBytes);
        fixed (byte* cBufStartPosPtr = &_cBuf[0])
        {
            var curOffset = (long)frame.DataPtr - (long)cBufStartPosPtr;

            if (speed < oldSpeed)
            {
                if (curOffset + frame.DataLen >= _cBuf.Length)
                {
                    frame.DataPtr = (IntPtr)cBufStartPosPtr;
                    curOffset = 0;
                    oldDataLen = 0;
                }

                // 填充静音
                for (var p = oldDataLen; p < frame.DataLen; p++)
                {
                    _cBuf[curOffset + p] = 0;
                }
            }
        }
    }

    /// <summary>
    /// 设置过滤器或Swr.
    /// </summary>
    /// <returns>设置结果.</returns>
    internal int SetupFiltersOrSwr()
    {
        lock (_lockSpeed)
        {
            var ret = -1;

            if (Disposed)
            {
                return ret;
            }

            if (Config.Audio.FiltersEnabled && Engine.FFmpeg.FiltersLoaded)
            {
                ret = SetupFilters();

                if (ret != 0)
                {
                    Log.Error($"设置过滤器失败.回退到Swr.");
                    ret = SetupSwr();
                }
                else
                {
                    DisposeSwr();
                }
            }
            else
            {
                DisposeFilters();
                ret = SetupSwr();
            }

            return ret;
        }
    }

    /// <inheritdoc/>
    protected override void OnSpeedChanged(double value)
    {
        // Possible Task to avoid locking UI thread as lockAtempo can wait for the Frames queue to be freed (will cause other issues and couldnt reproduce the possible dead lock)
        _cBufTimesCur = _cBufTimesSize;
        lock (_lockSpeed)
        {
            if (_filterGraph != null)
            {
                DrainFilters();
            }

            _cBufTimesCur = 1;
            oldSpeed = speed;
            speed = value;

            var frames = Frames.ToArray();
            for (var i = 0; i < frames.Length; i++)
            {
                FixSample(frames[i], oldSpeed, speed);
            }

            if (_filterGraph != null)
            {
                SetupFilters();
            }
        }
    }

    private void ProcessFilters(AVFrame* frame)
    {
        if (_setFirstPts && frame != null)
        {
            _setFirstPts = false;
            _filterFirstPts = frame->pts;
            _curSamples = 0;
            _missedSamples = 0;
        }

        int ret;

        if ((ret = av_buffersrc_add_frame(_aBufferCtx, frame)) < 0)
        {
            Log.Warn($"[buffersrc] {FFmpegEngine.ErrorCodeToMsg(ret)} ({ret})");
            Status = ThreadStatus.Stopping;
            return;
        }

        while (true)
        {
            if ((ret = av_buffersink_get_frame_flags(_aBufferSinkCtx, frame, 0)) < 0) // Sometimes we get AccessViolationException while we UpdateFilter (possible related with .NET7 debug only bug)
            {
                return; // EAGAIN (Some filters will send EAGAIN even if EOF currently we handled cause our Status will be Draining)
            }

            if (frame->pts == AV_NOPTS_VALUE) // we might desync here (we dont count frames->nb_samples) ?
            {
                av_frame_unref(frame);
                continue;
            }

            ProcessFilter(frame);

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

                while (Frames.Count >= Config.Decoder.MaxAudioFrames * _cBufTimesCur && (Status == ThreadStatus.QueueFull || Status == ThreadStatus.Draining))
                {
                    Thread.Sleep(20);
                }

                Monitor.Enter(_lockCodecCtx);

                lock (LockStatus)
                {
                    if (Status == ThreadStatus.QueueFull)
                    {
                        Status = ThreadStatus.Running;
                    }
                    else if (Status != ThreadStatus.Draining)
                    {
                        return;
                    }
                }
            }
        }
    }

    private void DrainFilters()
    {
        if (_aBufferDrained)
        {
            return;
        }

        _aBufferDrained = true;

        int ret;

        if ((ret = av_buffersrc_add_frame(_aBufferCtx, null)) < 0)
        {
            Log.Warn($"[buffersrc] {FFmpegEngine.ErrorCodeToMsg(ret)} ({ret})");
            return;
        }

        var frame = av_frame_alloc();

        while (true)
        {
            if ((ret = av_buffersink_get_frame_flags(_aBufferSinkCtx, frame, 0)) < 0)
            {
                return;
            }

            if (frame->pts == AV_NOPTS_VALUE)
            {
                av_frame_unref(frame);
                return;
            }

            ProcessFilter(frame);
        }
    }

    private void ProcessFilter(AVFrame* frame)
    {
        AudioFrame mFrame = new();
        var newPts = _filterFirstPts + av_rescale_q((long)(_curSamples + _missedSamples), codecCtx->time_base, AudioStream.AVStream->time_base);
        mFrame.Timestamp = (long)((newPts * AudioStream.TimeBase) - demuxer.StartTime + Config.Audio.Delay);
        mFrame.DataLen = frame->nb_samples * _aSampleBytes;
        if (CanTrace)
        {
            Log.Trace($"Processes {Utils.TicksToTime(mFrame.Timestamp)}");
        }

        var samplesSpeed1 = frame->nb_samples * speed;
        _missedSamples += samplesSpeed1 - (int)samplesSpeed1;
        _curSamples += (int)samplesSpeed1;

        if (frame->nb_samples > _cBufSamples)
        {
            AllocateCircularBuffer(frame->nb_samples);
        }
        else if (_cBufPos + mFrame.DataLen >= _cBuf.Length)
        {
            _cBufPos = 0;
        }

        fixed (byte* circularBufferPosPtr = &_cBuf[_cBufPos])
        {
            mFrame.DataPtr = (IntPtr)circularBufferPosPtr;
        }

        Marshal.Copy((IntPtr)frame->data[0], _cBuf, _cBufPos, mFrame.DataLen);
        _cBufPos += mFrame.DataLen;

        Frames.Enqueue(mFrame);
        av_frame_unref(frame);
    }

    private AVFilterContext* CreateFilter(string name, string args, AVFilterContext* prevCtx = null, string id = null)
    {
        int ret;
        AVFilterContext* filterCtx;
        AVFilter* filter;

        id ??= name;

        filter = avfilter_get_by_name(name);
        if (filter == null)
        {
            throw new Exception($"[Filter {name}] not found");
        }

        ret = avfilter_graph_create_filter(&filterCtx, filter, id, args, null, _filterGraph);
        if (ret < 0)
        {
            throw new Exception($"[Filter {name}] avfilter_graph_create_filter failed ({FFmpegEngine.ErrorCodeToMsg(ret)})");
        }

        if (prevCtx == null)
        {
            return filterCtx;
        }

        ret = avfilter_link(prevCtx, 0, filterCtx, 0);

        return ret != 0
            ? throw new Exception($"[Filter {name}] avfilter_link failed ({FFmpegEngine.ErrorCodeToMsg(ret)})")
            : filterCtx;
    }

    private int SetupFilters()
    {
        var ret = -1;
        try
        {
            DisposeFilters();

            AVFilterContext* linkCtx;

            _filterGraph = avfilter_graph_alloc();
            _setFirstPts = true;
            _aBufferDrained = false;

            // IN (abuffersrc)
            linkCtx = _aBufferCtx = CreateFilter(
                "abuffer",
                $"channel_layout={AudioStream.ChannelLayoutStr}:sample_fmt={AudioStream.SampleFormatStr}:sample_rate={codecCtx->sample_rate}:time_base={codecCtx->time_base.num}/{codecCtx->time_base.den}");

            // USER DEFINED
            if (Config.Audio.Filters != null)
            {
                foreach (var filter in Config.Audio.Filters)
                {
                    try
                    {
                        linkCtx = CreateFilter(filter.Name, filter.Args, linkCtx, filter.Id);
                    }
                    catch (Exception e)
                    {
                        Log.Error($"{e.Message}");
                    }
                }
            }

            // SPEED (atempo up to 3) | [0.125 - 0.25](3), [0.25 - 0.5](2), [0.5 - 2.0](1), [2.0 - 4.0](2), [4.0 - X](3)
            if (speed != 1)
            {
                if (speed >= 0.5 && speed <= 2)
                {
                    linkCtx = CreateFilter("atempo", $"tempo={speed.ToString("0.0000000000", System.Globalization.CultureInfo.InvariantCulture)}", linkCtx);
                }
                else if ((speed > 2 & speed <= 4) || (speed >= 0.25 && speed < 0.5))
                {
                    var singleAtempoSpeed = Math.Sqrt(speed);
                    linkCtx = CreateFilter("atempo", $"tempo={singleAtempoSpeed.ToString("0.0000000000", System.Globalization.CultureInfo.InvariantCulture)}", linkCtx);
                    linkCtx = CreateFilter("atempo", $"tempo={singleAtempoSpeed.ToString("0.0000000000", System.Globalization.CultureInfo.InvariantCulture)}", linkCtx);
                }
                else if (speed > 4 || (speed >= 0.125 && speed < 0.25))
                {
                    var singleAtempoSpeed = Math.Pow(speed, 1.0 / 3);
                    linkCtx = CreateFilter("atempo", $"tempo={singleAtempoSpeed.ToString("0.0000000000", System.Globalization.CultureInfo.InvariantCulture)}", linkCtx);
                    linkCtx = CreateFilter("atempo", $"tempo={singleAtempoSpeed.ToString("0.0000000000", System.Globalization.CultureInfo.InvariantCulture)}", linkCtx);
                    linkCtx = CreateFilter("atempo", $"tempo={singleAtempoSpeed.ToString("0.0000000000", System.Globalization.CultureInfo.InvariantCulture)}", linkCtx);
                }
            }

            // OUT (abuffersink)
            _aBufferSinkCtx = CreateFilter("abuffersink", null, null);

            var sample_fmts = new AVSampleFormat[] { _aOutSampleFormat, AVSampleFormat.AV_SAMPLE_FMT_NONE };
            var sample_rates = new int[] { AudioStream.SampleRate, -1 };

            fixed (AVSampleFormat* ptr = &sample_fmts[0])
            {
                ret = av_opt_set_bin(_aBufferSinkCtx, "sample_fmts", (byte*)ptr, sizeof(AVSampleFormat) * 2, AV_OPT_SEARCH_CHILDREN);
            }

            fixed (int* ptr = &sample_rates[0])
            {
                ret = av_opt_set_bin(_aBufferSinkCtx, "sample_rates", (byte*)ptr, sizeof(int), AV_OPT_SEARCH_CHILDREN);
            }

            // if ch_layouts is not set, all valid channel layouts are accepted except for UNSPEC layouts, unless all_channel_counts is set
            ret = av_opt_set_int(_aBufferSinkCtx, "all_channel_counts", 0, AV_OPT_SEARCH_CHILDREN);
            ret = av_opt_set(_aBufferSinkCtx, "ch_layouts", "stereo", AV_OPT_SEARCH_CHILDREN);
            avfilter_link(linkCtx, 0, _aBufferSinkCtx, 0);

            // GRAPH CONFIG
            ret = avfilter_graph_config(_filterGraph, null);

            return ret < 0
                ? throw new Exception($"[FilterGraph] {FFmpegEngine.ErrorCodeToMsg(ret)} ({ret})")
                : 0;
        }
        catch (Exception e)
        {
            fixed (AVFilterGraph** filterGraphPtr = &_filterGraph)
            {
                avfilter_graph_free(filterGraphPtr);
            }

            Log.Error($"{e.Message}");
            return ret;
        }
    }

    private void DisposeFilters()
    {
        if (_filterGraph == null)
        {
            return;
        }

        fixed (AVFilterGraph** filterGraphPtr = &_filterGraph)
        {
            avfilter_graph_free(filterGraphPtr);
        }

        _aBufferCtx = null;
        _aBufferSinkCtx = null;
        _filterGraph = null;
    }

    private int UpdateFilterInternal(string filterId, string key, string value)
    {
        var ret = avfilter_graph_send_command(_filterGraph, filterId, key, value, null, 0, 0);
        Log.Info($"[{filterId}] {key}={value} {(ret >= 0 ? "success" : "failed")}");

        return ret;
    }
}
