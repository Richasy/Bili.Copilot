using System;
using System.Runtime.InteropServices;
using System.Threading;

using FFmpeg.AutoGen;
using static FFmpeg.AutoGen.ffmpeg;

using Bili.Copilot.Libs.Player.MediaFramework.MediaStream;
using Bili.Copilot.Libs.Player.MediaFramework.MediaFrame;

using static FlyleafLib.Logger;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaDecoder;

public unsafe partial class AudioDecoder
{
    AVFilterContext*        abufferCtx;
    AVFilterContext*        abufferSinkCtx;
    AVFilterGraph*          filterGraph;
    bool                    abufferDrained;
    long                    curSamples;
    double                  missedSamples;
    long                    filterFirstPts;
    bool                    setFirstPts;
    object                  lockSpeed = new();

    private AVFilterContext* CreateFilter(string name, string args, AVFilterContext* prevCtx = null, string id = null)
    {
        int ret;
        AVFilterContext*    filterCtx;
        AVFilter*           filter;

        id ??= name;

        filter  = avfilter_get_by_name(name);
        if (filter == null)
            throw new Exception($"[Filter {name}] not found");

        ret     = avfilter_graph_create_filter(&filterCtx, filter, id, args, null, filterGraph);
        if (ret < 0)
            throw new Exception($"[Filter {name}] avfilter_graph_create_filter failed ({FFmpegEngine.ErrorCodeToMsg(ret)})");

        if (prevCtx == null)
            return filterCtx;

        ret     = avfilter_link(prevCtx, 0, filterCtx, 0);

        return ret != 0
            ? throw new Exception($"[Filter {name}] avfilter_link failed ({FFmpegEngine.ErrorCodeToMsg(ret)})")
            : filterCtx;
    }
    private int SetupFilters()
    {
        int ret = -1;

        try
        {
            DisposeFilters();

            AVFilterContext* linkCtx;

            filterGraph     = avfilter_graph_alloc();
            setFirstPts     = true;
            abufferDrained  = false;

            // IN (abuffersrc)
            linkCtx = abufferCtx = CreateFilter("abuffer", 
                $"channel_layout={AudioStream.ChannelLayoutStr}:sample_fmt={AudioStream.SampleFormatStr}:sample_rate={codecCtx->sample_rate}:time_base={codecCtx->time_base.num}/{codecCtx->time_base.den}");

            // USER DEFINED
            if (Config.Audio.Filters != null)
                foreach (var filter in Config.Audio.Filters)
                    try
                    {
                        linkCtx = CreateFilter(filter.Name, filter.Args, linkCtx, filter.Id);
                    }
                    catch (Exception e) { Log.Error($"{e.Message}"); }

            // SPEED (atempo up to 3) | [0.125 - 0.25](3), [0.25 - 0.5](2), [0.5 - 2.0](1), [2.0 - 4.0](2), [4.0 - X](3)
            if (speed != 1)
            {
                if (speed >= 0.5 && speed <= 2)
                    linkCtx = CreateFilter("atempo", $"tempo={speed.ToString("0.0000000000", System.Globalization.CultureInfo.InvariantCulture)}", linkCtx);   
                else if ((speed > 2 & speed <= 4) || (speed >= 0.25 && speed < 0.5))
                {
                    var singleAtempoSpeed = Math.Sqrt(speed);
                    linkCtx = CreateFilter("atempo", $"tempo={singleAtempoSpeed.ToString("0.0000000000", System.Globalization.CultureInfo.InvariantCulture)}", linkCtx);
                    linkCtx = CreateFilter("atempo", $"tempo={singleAtempoSpeed.ToString("0.0000000000", System.Globalization.CultureInfo.InvariantCulture)}", linkCtx);
                }
                else if (speed > 4 || speed >= 0.125 && speed < 0.25)
                {
                    var singleAtempoSpeed = Math.Pow(speed, 1.0 / 3);
                    linkCtx = CreateFilter("atempo", $"tempo={singleAtempoSpeed.ToString("0.0000000000", System.Globalization.CultureInfo.InvariantCulture)}", linkCtx);
                    linkCtx = CreateFilter("atempo", $"tempo={singleAtempoSpeed.ToString("0.0000000000", System.Globalization.CultureInfo.InvariantCulture)}", linkCtx);
                    linkCtx = CreateFilter("atempo", $"tempo={singleAtempoSpeed.ToString("0.0000000000", System.Globalization.CultureInfo.InvariantCulture)}", linkCtx);
                }
            }

            // OUT (abuffersink)
            abufferSinkCtx = CreateFilter("abuffersink", null, null);

            AVSampleFormat[] sample_fmts = new AVSampleFormat[] { AOutSampleFormat, AVSampleFormat.AV_SAMPLE_FMT_NONE };
            int[] sample_rates = new int[] { AudioStream.SampleRate, -1 };

            fixed (AVSampleFormat* ptr = &sample_fmts[0])
                ret = av_opt_set_bin(abufferSinkCtx , "sample_fmts"         , (byte*)ptr, sizeof(AVSampleFormat) * 2    , AV_OPT_SEARCH_CHILDREN);
            fixed(int* ptr = &sample_rates[0])
                ret = av_opt_set_bin(abufferSinkCtx , "sample_rates"        , (byte*)ptr, sizeof(int)                   , AV_OPT_SEARCH_CHILDREN);
            // if ch_layouts is not set, all valid channel layouts are accepted except for UNSPEC layouts, unless all_channel_counts is set
            ret = av_opt_set_int(abufferSinkCtx     , "all_channel_counts"  , 0                                         , AV_OPT_SEARCH_CHILDREN);
            ret = av_opt_set(abufferSinkCtx         , "ch_layouts"          , "stereo"                                  , AV_OPT_SEARCH_CHILDREN);
            avfilter_link(linkCtx, 0, abufferSinkCtx, 0);
            
            // GRAPH CONFIG
            ret = avfilter_graph_config(filterGraph, null);

            return ret < 0 
                ? throw new Exception($"[FilterGraph] {FFmpegEngine.ErrorCodeToMsg(ret)} ({ret})") 
                : 0;
        }
        catch (Exception e)
        {
            fixed(AVFilterGraph** filterGraphPtr = &filterGraph)
                avfilter_graph_free(filterGraphPtr);

            Log.Error($"{e.Message}");

            return ret;
        }
    }
    private void DisposeFilters()
    {
        if (filterGraph == null)
            return;
        
        fixed(AVFilterGraph** filterGraphPtr = &filterGraph)
            avfilter_graph_free(filterGraphPtr);
        
        abufferCtx      = null;
        abufferSinkCtx  = null;
        filterGraph     = null;
    }
    protected override void OnSpeedChanged(double value)
    {
        // Possible Task to avoid locking UI thread as lockAtempo can wait for the Frames queue to be freed (will cause other issues and couldnt reproduce the possible dead lock)
        cBufTimesCur = cBufTimesSize;
        lock (lockSpeed)
        {
            if (filterGraph != null)
                DrainFilters();

            cBufTimesCur= 1;
            oldSpeed    = speed;
            speed       = value;

            var frames = Frames.ToArray();
            for (int i = 0; i < frames.Length; i++)
                FixSample(frames[i], oldSpeed, speed);

            if (filterGraph != null)
                SetupFilters();
        }
    }
    internal void FixSample(AudioFrame frame, double oldSpeed, double speed)
    {
        var oldDataLen = frame.dataLen;
        frame.dataLen = Utils.Align((int) (oldDataLen * oldSpeed / speed), ASampleBytes);
        fixed (byte* cBufStartPosPtr = &cBuf[0])
        {
            var curOffset = (long)frame.dataPtr - (long)cBufStartPosPtr;

            if (speed < oldSpeed)
            {
                if (curOffset + frame.dataLen >= cBuf.Length)
                {
                    frame.dataPtr = (IntPtr)cBufStartPosPtr;
                    curOffset  = 0;
                    oldDataLen = 0;
                }

                // fill silence
                for (int p = oldDataLen; p < frame.dataLen; p++)
                    cBuf[curOffset + p] = 0;
            }
        }
    }
    private int UpdateFilterInternal(string filterId, string key, string value)
    {
        int ret = avfilter_graph_send_command(filterGraph, filterId, key, value, null, 0, 0);
        Log.Info($"[{filterId}] {key}={value} {(ret >=0 ? "success" : "failed")}");

        return ret;
    }
    internal int SetupFiltersOrSwr()
    {
        lock (lockSpeed)
        {
            int ret = -1;

            if (Disposed)
                return ret;

            if (Config.Audio.FiltersEnabled && Engine.FFmpeg.FiltersLoaded)
            {
                ret = SetupFilters();

                if (ret != 0)
                {
                    Log.Error($"Setup filters failed. Fallback to Swr.");
                    ret = SetupSwr();
                }
                else
                    DisposeSwr();
            }
            else
            {
                DisposeFilters();
                ret = SetupSwr();
            }

            return ret;
        }
    }

    public int UpdateFilter(string filterId, string key, string value)
    {
        lock (lockCodecCtx)
            return filterGraph != null ? UpdateFilterInternal(filterId, key, value) : -1;
    }
    public int ReloadFilters()
    {
        lock (lockActions)
            lock (lockCodecCtx)
                return !Engine.FFmpeg.FiltersLoaded || Config.Audio.FiltersEnabled ? -1 : SetupFilters();
    }

    private void ProcessFilters(AVFrame* frame)
    {
        if (setFirstPts && frame != null)
        {
            setFirstPts     = false;
            filterFirstPts  = frame->pts;
            curSamples      = 0;
            missedSamples   = 0;
        }
        
        int ret;

        if ((ret = av_buffersrc_add_frame(abufferCtx, frame)) < 0) 
        {
            Log.Warn($"[buffersrc] {FFmpegEngine.ErrorCodeToMsg(ret)} ({ret})");
            Status = Status.Stopping;
            return; 
        }
        
        while (true)
        {
            if ((ret = av_buffersink_get_frame_flags(abufferSinkCtx, frame, 0)) < 0) // Sometimes we get AccessViolationException while we UpdateFilter (possible related with .NET7 debug only bug)
                return; // EAGAIN (Some filters will send EAGAIN even if EOF currently we handled cause our Status will be Draining)

            if (frame->pts == AV_NOPTS_VALUE) // we might desync here (we dont count frames->nb_samples) ?
            {
                av_frame_unref(frame);
                continue;
            }

            ProcessFilter(frame);

            // Wait until Queue not Full or Stopped
            if (Frames.Count >= Config.Decoder.MaxAudioFrames * cBufTimesCur)
            {
                Monitor.Exit(lockCodecCtx);
                lock (lockStatus)
                    if (Status == Status.Running)
                        Status = Status.QueueFull;
                
                while (Frames.Count >= Config.Decoder.MaxAudioFrames * cBufTimesCur && (Status == Status.QueueFull || Status == Status.Draining))
                    Thread.Sleep(20);
                
                Monitor.Enter(lockCodecCtx);

                lock (lockStatus)
                {
                    if (Status == Status.QueueFull)
                        Status = Status.Running;
                    else if (Status != Status.Draining)
                        return;
                }
            }
        }
    }
    private void DrainFilters()
    {
        if (abufferDrained)
            return;

        abufferDrained = true;

        int ret;
        
        if ((ret = av_buffersrc_add_frame(abufferCtx, null)) < 0) 
        {
            Log.Warn($"[buffersrc] {FFmpegEngine.ErrorCodeToMsg(ret)} ({ret})");
            return; 
        }

        AVFrame* frame = av_frame_alloc();

        while (true)
        {
            if ((ret = av_buffersink_get_frame_flags(abufferSinkCtx, frame, 0)) < 0)
                return;
            
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
        AudioFrame mFrame   = new();
        long newPts         = filterFirstPts + av_rescale_q((long)(curSamples + missedSamples), codecCtx->time_base, AudioStream.AVStream->time_base);
        mFrame.timestamp    = (long)((newPts * AudioStream.Timebase) - demuxer.StartTime + Config.Audio.Delay);
        mFrame.dataLen      = frame->nb_samples * ASampleBytes;
        if (CanTrace) Log.Trace($"Processes {Utils.TicksToTime(mFrame.timestamp)}");

        var samplesSpeed1   = frame->nb_samples * speed;
        missedSamples      += samplesSpeed1 - (int)samplesSpeed1;
        curSamples         += (int)samplesSpeed1;

        if (frame->nb_samples > cBufSamples)
            AllocateCircularBuffer(frame->nb_samples);
        else if (cBufPos + mFrame.dataLen >= cBuf.Length)
            cBufPos = 0;

        fixed (byte* circularBufferPosPtr = &cBuf[cBufPos])
            mFrame.dataPtr = (IntPtr)circularBufferPosPtr;

        Marshal.Copy((IntPtr) frame->data[0], cBuf, cBufPos, mFrame.dataLen);
        cBufPos += mFrame.dataLen;
            
        Frames.Enqueue(mFrame);
        av_frame_unref(frame);
    }
}

/// <summary>
/// FFmpeg Filter
/// </summary>
public class Filter
{
    /// <summary>
    /// <para>
    /// FFmpeg valid filter id
    /// (Required only to send commands)
    /// </para>
    /// </summary>
    public string Id    { get; set; }

    /// <summary>
    /// FFmpeg valid filter name
    /// </summary>
    public string Name  { get; set; }

    /// <summary>
    /// FFmpeg valid filter args
    /// </summary>
    public string Args  { get; set; }
}
