using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

using FFmpeg.AutoGen;
using static FFmpeg.AutoGen.ffmpeg;
using static FFmpeg.AutoGen.AVCodecID;

using Vortice.DXGI;
using Vortice.Direct3D11;

using ID3D11Texture2D = Vortice.Direct3D11.ID3D11Texture2D;

using Bili.Copilot.Libs.Player.MediaFramework.MediaStream;
using Bili.Copilot.Libs.Player.MediaFramework.MediaFrame;
using Bili.Copilot.Libs.Player.MediaFramework.MediaRenderer;
using Bili.Copilot.Libs.Player.MediaFramework.MediaRemuxer;

using static FlyleafLib.Logger;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaDecoder;

public unsafe class VideoDecoder : DecoderBase
{
    public ConcurrentQueue<VideoFrame>
                            Frames              { get; protected set; } = new ConcurrentQueue<VideoFrame>();
    public Renderer         Renderer            { get; private set; }
    public bool             VideoAccelerated    { get; internal set; }
    public bool             ZeroCopy            { get; internal set; }
    
    public VideoStream      VideoStream         => (VideoStream) Stream;

    public long             StartTime           { get; internal set; } = AV_NOPTS_VALUE;
    public long             StartRecordTime     { get; internal set; } = AV_NOPTS_VALUE;

    const AVPixelFormat     PIX_FMT_HWACCEL     = AVPixelFormat.AV_PIX_FMT_D3D11;
    const int               SCALING_HQ          = SWS_ACCURATE_RND | SWS_BITEXACT | SWS_LANCZOS | SWS_FULL_CHR_H_INT | SWS_FULL_CHR_H_INP;
    const int               SCALING_LQ          = SWS_BICUBIC;

    internal SwsContext*    swsCtx;
    IntPtr                  swsBufferPtr;
    internal byte_ptrArray4 swsData;
    internal int_array4     swsLineSize;

    internal bool           swFallback;
    internal bool           keyFrameRequired;

    // Reverse Playback
    ConcurrentStack<List<IntPtr>>
                            curReverseVideoStack    = new();
    List<IntPtr>            curReverseVideoPackets  = new();
    List<VideoFrame>        curReverseVideoFrames   = new();
    int                     curReversePacketPos     = 0;

    public VideoDecoder(Config config, int uniqueId = -1) : base(config, uniqueId)
        => getHWformat = new AVCodecContext_get_format(get_format);

    protected override void OnSpeedChanged(double value) { oldSpeed = speed; speed = value; speed = speed < 1 ? 1 : (int)speed; }

    public void CreateRenderer() // TBR: It should be in the constructor but DecoderContext will not work with null VideoDecoder for AudioOnly
    {
        if (Renderer == null)
            Renderer = new Renderer(this, IntPtr.Zero, UniqueId);
        else if (Renderer.Disposed)
            Renderer.Initialize();

        Disposed = false; // We don't dipose the renderer (decoderContext does)
    }
    public void DestroyRenderer() => Renderer?.Dispose();
    public void CreateSwapChain(IntPtr handle)
    {
        CreateRenderer();
        Renderer.InitializeSwapChain(handle);
    }
    public void CreateSwapChain(Action<IDXGISwapChain2> swapChainWinUIClbk)
    {
        Renderer.SwapChainWinUIClbk = swapChainWinUIClbk;
        if (Renderer.SwapChainWinUIClbk != null)
            Renderer.InitializeWinUISwapChain();

    }
    public void DestroySwapChain() => Renderer?.DisposeSwapChain();

    #region Video Acceleration (Should be disposed seperately)
    const int               AV_CODEC_HW_CONFIG_METHOD_HW_DEVICE_CTX = 0x01;
    const AVHWDeviceType    HW_DEVICE = AVHWDeviceType.AV_HWDEVICE_TYPE_D3D11VA;

    internal ID3D11Texture2D
                            textureFFmpeg;
    AVCodecContext_get_format 
                            getHWformat;
    bool                    disableGetFormat;
    AVBufferRef*            hwframes;
    AVBufferRef*            hw_device_ctx;

    internal static bool CheckCodecSupport(AVCodec* codec)
    {
        for (int i = 0; ; i++)
        {
            var config = avcodec_get_hw_config(codec, i);
            if (config == null) break;
            if ((config->methods & AV_CODEC_HW_CONFIG_METHOD_HW_DEVICE_CTX) == 0 || config->pix_fmt == AVPixelFormat.AV_PIX_FMT_NONE) continue;
            
            if (config->device_type == HW_DEVICE && config->pix_fmt == PIX_FMT_HWACCEL) return true;
        }

        return false;
    }
    internal int InitVA()
    {
        int ret;
        AVHWDeviceContext*      device_ctx;
        AVD3D11VADeviceContext* d3d11va_device_ctx;

        if (Renderer.Device == null || hw_device_ctx != null) return -1;

        hw_device_ctx  = av_hwdevice_ctx_alloc(HW_DEVICE);

        device_ctx          = (AVHWDeviceContext*) hw_device_ctx->data;
        d3d11va_device_ctx  = (AVD3D11VADeviceContext*) device_ctx->hwctx;
        d3d11va_device_ctx->device
                            = (FFmpeg.AutoGen.ID3D11Device*) Renderer.Device.NativePointer;

        ret = av_hwdevice_ctx_init(hw_device_ctx);
        if (ret != 0)
        {
            Log.Error($"VA Failed - {FFmpegEngine.ErrorCodeToMsg(ret)} ({ret})");
            
            fixed(AVBufferRef** ptr = &hw_device_ctx)
                av_buffer_unref(ptr);

            hw_device_ctx = null;
        }

        Renderer.Device.AddRef(); // Important to give another reference for FFmpeg so we can dispose without issues

        return ret;
    }

    private AVPixelFormat get_format(AVCodecContext* avctx, AVPixelFormat* pix_fmts)
    {
        if (disableGetFormat)
            return avcodec_default_get_format(avctx, pix_fmts);

        if (CanDebug) Log.Debug($"Codec profile '{VideoStream.Codec} {avcodec_profile_name(codecCtx->codec_id, codecCtx->profile)}'");

        int  ret = 0;
        bool foundHWformat = false;
        
        while (*pix_fmts != AVPixelFormat.AV_PIX_FMT_NONE)
        {
            if (CanTrace)
                Log.Trace($"{*pix_fmts}");

            if ((*pix_fmts) == PIX_FMT_HWACCEL)
            {
                foundHWformat = true;
                break;
            }

            pix_fmts++;
        }

        ret = ShouldAllocateNew();

        if (foundHWformat && ret == 0)
        {
            if (CanTrace)
                Log.Trace("HW frames already allocated");

            if (hwframes != null && codecCtx->hw_frames_ctx == null)
                codecCtx->hw_frames_ctx = av_buffer_ref(hwframes);

            return PIX_FMT_HWACCEL;
        }

        lock (lockCodecCtx)
        {
            if (!foundHWformat || !VideoAccelerated || AllocateHWFrames() != 0)
            {
                if (CanWarn)
                    Log.Warn("HW format not found. Fallback to sw format");

                swFallback = true;
                return avcodec_default_get_format(avctx, pix_fmts);
            }
            
            if (CanDebug)
                Log.Debug("HW frame allocation completed");

            // TBR: Catch codec changed on live streams (check codec/profiles and check even on sw frames)
            if (ret == 2)
            {
                Log.Warn($"Codec changed {VideoStream.CodecID} {VideoStream.Width}x{VideoStream.Height} => {codecCtx->codec_id} {codecCtx->width}x{codecCtx->height}");
                filledFromCodec = false;
            }

            return PIX_FMT_HWACCEL;
        }
    }
    private int ShouldAllocateNew() // 0: No, 1: Yes, 2: Yes+Codec Changed
    {
        if (hwframes == null)
            return 1;

        AVHWFramesContext* t2 = (AVHWFramesContext*) hwframes->data;

        if (codecCtx->coded_width != t2->width)
            return 2;

        if (codecCtx->coded_height != t2->height)
            return 2;

        // TBR: Codec changed (seems ffmpeg changes codecCtx by itself
        //if (codecCtx->codec_id != VideoStream.CodecID)
        //    return 2;

        //var fmt = codecCtx->sw_pix_fmt == (AVPixelFormat)AV_PIX_FMT_YUV420P10LE ? (AVPixelFormat)AV_PIX_FMT_P010LE : (codecCtx->sw_pix_fmt == (AVPixelFormat)AV_PIX_FMT_P010BE ? (AVPixelFormat)AV_PIX_FMT_P010BE : AVPixelFormat.AV_PIX_FMT_NV12);
        //if (fmt != t2->sw_format)
        //    return 2;

        return 0;
    }

    private int AllocateHWFrames()
    {
        if (hwframes != null)
            fixed(AVBufferRef** ptr = &hwframes)
                av_buffer_unref(ptr);
        
        hwframes = null;

        if (codecCtx->hw_frames_ctx != null)
            av_buffer_unref(&codecCtx->hw_frames_ctx);

        if (avcodec_get_hw_frames_parameters(codecCtx, codecCtx->hw_device_ctx, PIX_FMT_HWACCEL, &codecCtx->hw_frames_ctx) != 0)
            return -1;

        AVHWFramesContext* hw_frames_ctx = (AVHWFramesContext*)codecCtx->hw_frames_ctx->data;
        hw_frames_ctx->initial_pool_size += Config.Decoder.MaxVideoFrames;

        AVD3D11VAFramesContext *va_frames_ctx = (AVD3D11VAFramesContext *)hw_frames_ctx->hwctx;
        va_frames_ctx->BindFlags  |= (uint)BindFlags.Decoder | (uint)BindFlags.ShaderResource;
        
        hwframes = av_buffer_ref(codecCtx->hw_frames_ctx);

        int ret = av_hwframe_ctx_init(codecCtx->hw_frames_ctx);
        if (ret == 0)
        {
            lock (Renderer.lockDevice)
            {
                textureFFmpeg   = new ID3D11Texture2D((IntPtr) va_frames_ctx->texture);
                ZeroCopy = Config.Decoder.ZeroCopy == FlyleafLib.ZeroCopy.Enabled || (Config.Decoder.ZeroCopy == FlyleafLib.ZeroCopy.Auto && codecCtx->width == textureFFmpeg.Description.Width && codecCtx->height == textureFFmpeg.Description.Height);
                filledFromCodec = false;
            }
        }

        return ret;
    }
    internal void RecalculateZeroCopy()
    {
        lock (Renderer.lockDevice)
        {
            bool save = ZeroCopy;
            ZeroCopy = VideoAccelerated && (Config.Decoder.ZeroCopy == FlyleafLib.ZeroCopy.Enabled || (Config.Decoder.ZeroCopy == FlyleafLib.ZeroCopy.Auto && codecCtx->width == textureFFmpeg.Description.Width && codecCtx->height == textureFFmpeg.Description.Height));
            if (save != ZeroCopy)
            {
                Renderer?.ConfigPlanes();
                CodecChanged?.Invoke(this);
            }
        }
    }
    #endregion

    protected override int Setup(AVCodec* codec)
    {
        // Ensures we have a renderer (no swap chain is required)
        CreateRenderer();
        
        VideoAccelerated = false;

        if (!swFallback && Config.Video.VideoAcceleration && Renderer.Device.FeatureLevel >= Vortice.Direct3D.FeatureLevel.Level_10_0)
        {
            if (CheckCodecSupport(codec))
            {
                if (InitVA() == 0)
                {
                    codecCtx->hw_device_ctx = av_buffer_ref(hw_device_ctx);
                    VideoAccelerated = true;
                    Log.Debug("VA Success");
                }
            }
            else
                Log.Info($"VA {codec->id} not supported");
        }
        else
            Log.Debug("VA Disabled");

        // Can't get data from here?
        //var t1 = av_stream_get_side_data(VideoStream.AVStream, AVPacketSideDataType.AV_PKT_DATA_MASTERING_DISPLAY_METADATA, null);
        //var t2 = av_stream_get_side_data(VideoStream.AVStream, AVPacketSideDataType.AV_PKT_DATA_CONTENT_LIGHT_LEVEL, null);
        
        keyFrameRequired= true;
        ZeroCopy        = false;
        filledFromCodec = false;

        if (VideoAccelerated)
        {
            codecCtx->thread_count = 1;
            codecCtx->hwaccel_flags |= AV_HWACCEL_FLAG_IGNORE_LEVEL;
            if (Config.Decoder.AllowProfileMismatch)
                codecCtx->hwaccel_flags |= AV_HWACCEL_FLAG_ALLOW_PROFILE_MISMATCH;

            codecCtx->pix_fmt = PIX_FMT_HWACCEL;
            codecCtx->get_format = getHWformat;
            disableGetFormat = false;
        }
        else
            codecCtx->thread_count = Math.Min(Config.Decoder.VideoThreads, codecCtx->codec_id == AV_CODEC_ID_HEVC ? 32 : 16);

        return 0;
    }
    internal bool SetupSws()
    {
        Marshal.FreeHGlobal(swsBufferPtr);
        var fmt         = AVPixelFormat.AV_PIX_FMT_RGBA;
        swsData         = new byte_ptrArray4();
        swsLineSize     = new int_array4();
        int outBufferSize
                        = av_image_get_buffer_size(fmt, codecCtx->width, codecCtx->height, 1);
        swsBufferPtr    = Marshal.AllocHGlobal(outBufferSize);
        av_image_fill_arrays(ref swsData, ref swsLineSize, (byte*) swsBufferPtr, fmt, codecCtx->width, codecCtx->height, 1);
        swsCtx          = sws_getContext(codecCtx->coded_width, codecCtx->coded_height, codecCtx->pix_fmt, codecCtx->width, codecCtx->height, fmt, Config.Video.SwsHighQuality ? SCALING_HQ : SCALING_LQ, null, null, null);

        if (swsCtx == null)
        {
            Log.Error($"Failed to allocate SwsContext");
            return false;
        }

        return true;
    }
    internal void Flush()
    {
        lock (lockActions)
            lock (lockCodecCtx)
            {
                if (Disposed) return;

                if (Status == Status.Ended)
                    Status = Status.Stopped;
                else if (Status == Status.Draining)
                    Status = Status.Stopping;

                DisposeFrames();
                avcodec_flush_buffers(codecCtx);
            
                keyFrameRequired = true;
                StartTime = AV_NOPTS_VALUE;
                curSpeedFrame = (int)speed;
            }
    }

    protected override void RunInternal()
    {
        if (demuxer.IsReversePlayback)
        {
            RunInternalReverse();
            return;
        }

        int ret = 0;
        int allowedErrors = Config.Decoder.MaxErrors;
        int sleepMs = Config.Decoder.MaxVideoFrames > 2 && Config.Player.MaxLatency == 0 ? 10 : 2;
        AVPacket *packet;

        do
        {
            // Wait until Queue not Full or Stopped
            if (Frames.Count >= Config.Decoder.MaxVideoFrames)
            {
                lock (lockStatus)
                    if (Status == Status.Running) Status = Status.QueueFull;

                while (Frames.Count >= Config.Decoder.MaxVideoFrames && Status == Status.QueueFull)
                    Thread.Sleep(sleepMs);

                lock (lockStatus)
                {
                    if (Status != Status.QueueFull) break;
                    Status = Status.Running;
                }
            }

            // While Packets Queue Empty (Drain | Quit if Demuxer stopped | Wait until we get packets)
            if (demuxer.VideoPackets.Count == 0)
            {
                CriticalArea = true;

                lock (lockStatus)
                    if (Status == Status.Running) Status = Status.QueueEmpty;

                while (demuxer.VideoPackets.Count == 0 && Status == Status.QueueEmpty)
                {
                    if (demuxer.Status == Status.Ended)
                    {
                        lock (lockStatus)
                        {
                            // TODO: let the demuxer push the draining packet
                            Log.Debug("Draining");
                            Status = Status.Draining;
                            var drainPacket = av_packet_alloc();
                            drainPacket->data = null;
                            drainPacket->size = 0;
                            demuxer.VideoPackets.Enqueue(drainPacket);
                        }
                        
                        break;
                    }
                    else if (!demuxer.IsRunning)
                    {
                        if (CanDebug) Log.Debug($"Demuxer is not running [Demuxer Status: {demuxer.Status}]");

                        int retries = 5;

                        while (retries > 0)
                        {
                            retries--;
                            Thread.Sleep(10);
                            if (demuxer.IsRunning) break;
                        }

                        lock (demuxer.lockStatus)
                        lock (lockStatus)
                        {
                            if (demuxer.Status == Status.Pausing || demuxer.Status == Status.Paused)
                                Status = Status.Pausing;
                            else if (demuxer.Status != Status.Ended)
                                Status = Status.Stopping;
                            else
                                continue;
                        }

                        break;
                    }
                    
                    Thread.Sleep(sleepMs);
                }

                lock (lockStatus)
                {
                    CriticalArea = false;
                    if (Status != Status.QueueEmpty && Status != Status.Draining) break;
                    if (Status != Status.Draining) Status = Status.Running;
                }
            }

            lock (lockCodecCtx)
            {
                if (Status == Status.Stopped)
                    continue;

                packet = demuxer.VideoPackets.Dequeue();

                if (packet == null)
                    continue;

                if (isRecording)
                {
                    if (!recGotKeyframe && (packet->flags & AV_PKT_FLAG_KEY) != 0)
                    {
                        recGotKeyframe = true;
                        StartRecordTime = (long)(packet->pts * VideoStream.Timebase) - demuxer.StartTime;
                    }

                    if (recGotKeyframe)
                        curRecorder.Write(av_packet_clone(packet));
                }

                // TBR: AVERROR(EAGAIN) means avcodec_receive_frame but after resend the same packet
                ret = avcodec_send_packet(codecCtx, packet);

                if (swFallback) // Should use 'global' packet to reset it in get_format (same packet should use also from DecoderContext)
                {
                    SWFallback();
                    ret = avcodec_send_packet(codecCtx, packet);
                }

                if (ret != 0 && ret != AVERROR(EAGAIN))
                {
                    av_packet_free(&packet);

                    if (ret == AVERROR_EOF)
                    {
                        if (demuxer.VideoPackets.Count > 0) { avcodec_flush_buffers(codecCtx); continue; } // TBR: Happens on HLS while switching video streams
                        Status = Status.Ended;
                        break;
                    }
                    else
                    {
                        allowedErrors--;
                        if (CanWarn) Log.Warn($"{FFmpegEngine.ErrorCodeToMsg(ret)} ({ret})");

                        if (allowedErrors == 0) { Log.Error("Too many errors!"); Status = Status.Stopping; break; }

                        continue;
                    }
                }
                
                while (true)
                {
                    ret = avcodec_receive_frame(codecCtx, frame);
                    if (ret != 0) { av_frame_unref(frame); break; }

                    if (frame->best_effort_timestamp != AV_NOPTS_VALUE)
                        frame->pts = frame->best_effort_timestamp;
                    else if (frame->pts == AV_NOPTS_VALUE)
                        { av_frame_unref(frame); continue; }

                    if (keyFrameRequired)
                    {
                        if (frame->pict_type != AVPictureType.AV_PICTURE_TYPE_I && frame->key_frame != 1)
                        {
                            if (CanWarn) Log.Warn($"Seek to keyframe failed [{frame->pict_type} | {frame->key_frame}]");
                            av_frame_unref(frame);
                            continue;
                        }
                        else
                        {
                            StartTime = (long)(frame->pts * VideoStream.Timebase) - demuxer.StartTime;
                            keyFrameRequired = false;
                        }
                    }

                    if (!filledFromCodec) // Ensures we have a proper frame before filling from codec
                    {
                        ret = FillFromCodec(frame);
                        if (ret == -1234)
                        {
                            Status = Status.Stopping;
                            break;
                        }
                    }
                    
                    if (speed != 1)
                    {
                        curSpeedFrame++;
                        if (curSpeedFrame < speed)
                        {
                            av_frame_unref(frame);
                            continue;
                        }
                        curSpeedFrame = 0; 
                    }

                    var mFrame = Renderer.FillPlanes(frame);
                    if (mFrame != null) Frames.Enqueue(mFrame); // TBR: Does not respect Config.Decoder.MaxVideoFrames
                }

                av_packet_free(&packet);
            }

        } while (Status == Status.Running);

        if (isRecording) { StopRecording(); recCompleted(MediaType.Video); }

        if (Status == Status.Draining) Status = Status.Ended;
    }

    internal int FillFromCodec(AVFrame* frame)
    {
        lock (Renderer.lockDevice)
        {
            int ret = 0;

            filledFromCodec = true;

            avcodec_parameters_from_context(Stream.AVStream->codecpar, codecCtx);
            VideoStream.AVStream->time_base = codecCtx->pkt_timebase;
            VideoStream.Refresh(codecCtx->sw_pix_fmt != AVPixelFormat.AV_PIX_FMT_NONE ? codecCtx->sw_pix_fmt : codecCtx->pix_fmt);

            if (!(VideoStream.FPS > 0)) // NaN
            {
                VideoStream.FPS             = av_q2d(codecCtx->framerate) > 0 ? av_q2d(codecCtx->framerate) : 0;
                VideoStream.FrameDuration   = VideoStream.FPS > 0 ? (long) (10000000 / VideoStream.FPS) : 0;
            }

            CodecChanged?.Invoke(this);

            if (!Renderer.ConfigPlanes())
            {
                Log.Error("[Pixel Format] Unknown");
                return -1234;
            }

            return ret;
        }
    }

    internal string SWFallback()
    {
        lock (Renderer.lockDevice)
        {
            string ret;

            DisposeInternal();
            swFallback = true;
            ret = Open2(Stream, null, false); // TBR:  Dispose() on failure could cause a deadlock
            swFallback = false;
            filledFromCodec = false;

            return ret;
        }
    }

    private void RunInternalReverse()
    {
        int ret = 0;
        int allowedErrors = Config.Decoder.MaxErrors;
        AVPacket *packet;

        do
        {
            // While Packets Queue Empty (Drain | Quit if Demuxer stopped | Wait until we get packets)
            if (demuxer.VideoPacketsReverse.IsEmpty && curReverseVideoStack.IsEmpty && curReverseVideoPackets.Count == 0)
            {
                CriticalArea = true;

                lock (lockStatus)
                    if (Status == Status.Running) Status = Status.QueueEmpty;
                
                while (demuxer.VideoPacketsReverse.IsEmpty && Status == Status.QueueEmpty)
                {
                    if (demuxer.Status == Status.Ended) // TODO
                    {
                        lock (lockStatus) Status = Status.Ended;
                        
                        break;
                    }
                    else if (!demuxer.IsRunning)
                    {
                        if (CanDebug) Log.Debug($"Demuxer is not running [Demuxer Status: {demuxer.Status}]");

                        int retries = 5;

                        while (retries > 0)
                        {
                            retries--;
                            Thread.Sleep(10);
                            if (demuxer.IsRunning) break;
                        }

                        lock (demuxer.lockStatus)
                        lock (lockStatus)
                        {
                            if (demuxer.Status == Status.Pausing || demuxer.Status == Status.Paused)
                                Status = Status.Pausing;
                            else if (demuxer.Status != Status.Ended)
                                Status = Status.Stopping;
                            else
                                continue;
                        }

                        break;
                    }
                    
                    Thread.Sleep(20);
                }
                
                lock (lockStatus)
                {
                    CriticalArea = false;
                    if (Status != Status.QueueEmpty) break;
                    Status = Status.Running;
                }
            }

            if (curReverseVideoPackets.Count == 0)
            {
                if (curReverseVideoStack.IsEmpty)
                    demuxer.VideoPacketsReverse.TryDequeue(out curReverseVideoStack);

                curReverseVideoStack.TryPop(out curReverseVideoPackets);
                curReversePacketPos = 0;
            }

            keyFrameRequired = false;

            while (curReverseVideoPackets.Count > 0 && Status == Status.Running)
            {
                // Wait until Queue not Full or Stopped
                if (Frames.Count + curReverseVideoFrames.Count >= Config.Decoder.MaxVideoFrames)
                {
                    lock (lockStatus)
                        if (Status == Status.Running) Status = Status.QueueFull;

                    while (Frames.Count + curReverseVideoFrames.Count >= Config.Decoder.MaxVideoFrames && Status == Status.QueueFull) Thread.Sleep(20);

                    lock (lockStatus)
                    {
                        if (Status != Status.QueueFull) break;
                        Status = Status.Running;
                    }
                }

                lock (lockCodecCtx)
                {
                    if (keyFrameRequired == true)
                    {
                        curReversePacketPos = 0;
                        break;
                    }

                    packet = (AVPacket*)curReverseVideoPackets[curReversePacketPos++];
                    ret = avcodec_send_packet(codecCtx, packet);

                    if (ret != 0 && ret != AVERROR(EAGAIN))
                    {
                        if (ret == AVERROR_EOF) { Status = Status.Ended; break; }

                        if (CanWarn) Log.Warn($"{FFmpegEngine.ErrorCodeToMsg(ret)} ({ret})");

                        allowedErrors--;
                        if (allowedErrors == 0) { Log.Error("Too many errors!"); Status = Status.Stopping; break; }

                        for (int i=curReverseVideoPackets.Count-1; i>=curReversePacketPos-1; i--)
                        {
                            packet = (AVPacket*)curReverseVideoPackets[i];
                            av_packet_free(&packet);
                            curReverseVideoPackets[curReversePacketPos - 1] = IntPtr.Zero;
                            curReverseVideoPackets.RemoveAt(i);
                        }

                        avcodec_flush_buffers(codecCtx);
                        curReversePacketPos = 0;

                        for (int i=curReverseVideoFrames.Count -1; i>=0; i--)
                            Frames.Enqueue(curReverseVideoFrames[i]);

                        curReverseVideoFrames.Clear();

                        continue;
                    }

                    while (true)
                    {
                        ret = avcodec_receive_frame(codecCtx, frame);
                        if (ret != 0) { av_frame_unref(frame); break; }

                        if (frame->best_effort_timestamp != AV_NOPTS_VALUE)
                            frame->pts = frame->best_effort_timestamp;
                        else if (frame->pts == AV_NOPTS_VALUE)
                            { av_frame_unref(frame); continue; }

                        bool shouldProcess = curReverseVideoPackets.Count - curReversePacketPos < Config.Decoder.MaxVideoFrames;

                        if (shouldProcess)
                        {
                            av_packet_free(&packet);
                            curReverseVideoPackets[curReversePacketPos - 1] = IntPtr.Zero;
                            var mFrame = Renderer.FillPlanes(frame);
                            if (mFrame != null) curReverseVideoFrames.Add(mFrame);
                        }
                        else
                        av_frame_unref(frame);
                    }

                    if (curReversePacketPos == curReverseVideoPackets.Count)
                    {
                        curReverseVideoPackets.RemoveRange(Math.Max(0, curReverseVideoPackets.Count - Config.Decoder.MaxVideoFrames), Math.Min(curReverseVideoPackets.Count, Config.Decoder.MaxVideoFrames) );
                        avcodec_flush_buffers(codecCtx);
                        curReversePacketPos = 0;

                        for (int i=curReverseVideoFrames.Count -1; i>=0; i--)
                            Frames.Enqueue(curReverseVideoFrames[i]);

                        curReverseVideoFrames.Clear();
                        
                        break; // force recheck for max queues etc...
                    }

                } // Lock CodecCtx

                // Import Sleep required to prevent delay during Renderer.Present
                // TBR: Might Monitor.TryEnter with priorities between decoding and rendering will work better
                Thread.Sleep(10);
                
            } // while curReverseVideoPackets.Count > 0

        } while (Status == Status.Running);

        if (Status != Status.Pausing && Status != Status.Paused)
            curReversePacketPos = 0;
    }

    public void RefreshMaxVideoFrames()
    {
        lock (lockActions)
        {
            if (VideoStream == null)
                return;

            bool wasRunning = IsRunning;

            var stream = Stream;

            Dispose();
            Open(stream);

            if (wasRunning)
                Start();
        }
    }

    public int GetFrameNumber(long timestamp)
    {
        // Incoming timestamps are zero-base from demuxer start time (not from video stream start time)
        timestamp -= VideoStream.StartTime - demuxer.StartTime;

        if (timestamp < 1)
            return 0;

        // offset 2ms
        return (int) ((timestamp + 20000) / VideoStream.FrameDuration);
    }

    /// <summary>
    /// Performs accurate seeking to the requested video frame and returns it
    /// </summary>
    /// <param name="index">Zero based frame index</param>
    /// <returns>The requested VideoFrame or null on failure</returns>
    public VideoFrame GetFrame(int index)
    {
        int ret;

        // Calculation of FrameX timestamp (based on fps/avgFrameDuration) | offset 2ms
        long frameTimestamp = VideoStream.StartTime + (index * VideoStream.FrameDuration) - 20000;
        //Log.Debug($"Searching for {Utils.TicksToTime(frameTimestamp)}");

        demuxer.Pause();
        Pause();

        // TBR
        //if (demuxer.FormatContext->pb != null)
        //    avio_flush(demuxer.FormatContext->pb);
        //avformat_flush(demuxer.FormatContext);

        // Seeking at frameTimestamp or previous I/Key frame and flushing codec | Temp fix (max I/distance 3sec) for ffmpeg bug that fails to seek on keyframe with HEVC
        // More issues with mpegts seeking backwards (those should be used also in the reverse playback in the demuxer)
        demuxer.Interrupter.Request(MediaDemuxer.Requester.Seek);
        ret = codecCtx->codec_id == AV_CODEC_ID_HEVC || (demuxer.FormatContext->iformat != null && demuxer.FormatContext->iformat->read_seek.Pointer == IntPtr.Zero)
            ? av_seek_frame(demuxer.FormatContext, -1, Math.Max(0, frameTimestamp - (3 * (long)1000 * 10000)) / 10, AVSEEK_FLAG_ANY)
            : av_seek_frame(demuxer.FormatContext, -1, frameTimestamp / 10, AVSEEK_FLAG_FRAME | AVSEEK_FLAG_BACKWARD);

        demuxer.DisposePackets();

        if (demuxer.Status == Status.Ended) demuxer.Status = Status.Stopped;
        if (ret < 0) return null; // handle seek error
        Flush();
        keyFrameRequired = false;
        StartTime = frameTimestamp - VideoStream.StartTime; // required for audio sync

        // Decoding until requested frame/timestamp
        bool checkExtraFrames = false;

        while (GetFrameNext(checkExtraFrames) == 0)
        {
            // Skip frames before our actual requested frame
            if ((long)(frame->pts * VideoStream.Timebase) < frameTimestamp)
            {
                //Log.Debug($"[Skip] [pts: {frame->pts}] [time: {Utils.TicksToTime((long)(frame->pts * VideoStream.Timebase))}] | [fltime: {Utils.TicksToTime(((long)(frame->pts * VideoStream.Timebase) - demuxer.StartTime))}]");
                av_frame_unref(frame);
                checkExtraFrames = true;
                continue; 
            }

            //Log.Debug($"[Found] [pts: {frame->pts}] [time: {Utils.TicksToTime((long)(frame->pts * VideoStream.Timebase))}] | {Utils.TicksToTime(VideoStream.StartTime + (index * VideoStream.FrameDuration))} | [fltime: {Utils.TicksToTime(((long)(frame->pts * VideoStream.Timebase) - demuxer.StartTime))}]");
            return Renderer.FillPlanes(frame);
        }

        return null;
    }

    /// <summary>
    /// Demuxes until the next valid video frame (will be stored in AVFrame* frame)
    /// </summary>
    /// <returns>0 on success</returns>
    /// 
    public VideoFrame GetFrameNext()
        => GetFrameNext(true) != 0 ? null : Renderer.FillPlanes(frame);

    /// <summary>
    /// Pushes the demuxer and the decoder to the next available video frame
    /// </summary>
    /// <param name="checkExtraFrames">Whether to check for extra frames within the decoder's cache. Set to true if not sure.</param>
    /// <returns></returns>
    public int GetFrameNext(bool checkExtraFrames)
    {
        // TODO: Should know if draining to be able to get more than one drained frames

        int ret;
        int allowedErrors = Config.Decoder.MaxErrors;

        if (checkExtraFrames)
        {
            ret = avcodec_receive_frame(codecCtx, frame);

            if (ret == 0)
            {
                if (frame->best_effort_timestamp != AV_NOPTS_VALUE)
                    frame->pts = frame->best_effort_timestamp;
                else if (frame->pts == AV_NOPTS_VALUE)
                {
                    av_frame_unref(frame);
                    return GetFrameNext(true);
                }

                return 0;
            }

            if (ret != AVERROR(EAGAIN)) return ret;
        }

        while (true)
        {
            ret = demuxer.GetNextVideoPacket();
            if (ret != 0 && demuxer.Status != Status.Ended)
                return ret;

            ret = avcodec_send_packet(codecCtx, demuxer.packet);
            av_packet_unref(demuxer.packet);

            if (ret != 0)
            {
                if (allowedErrors < 1 || demuxer.Status == Status.Ended) return ret;
                allowedErrors--;
                continue;
            }

            ret = avcodec_receive_frame(codecCtx, frame);
            
            if (ret == AVERROR(EAGAIN))
                continue;

            if (ret != 0)
            {
                av_frame_unref(frame);
                return ret;
            }

            if (frame->best_effort_timestamp != AV_NOPTS_VALUE)
                frame->pts = frame->best_effort_timestamp;
            else if (frame->pts == AV_NOPTS_VALUE)
            {
                av_frame_unref(frame);
                return GetFrameNext(true);
            }

            return 0;
        }
    }

    #region Dispose
    public void DisposeFrames()
    {
        while (!Frames.IsEmpty)
        {
            Frames.TryDequeue(out var frame);
            DisposeFrame(frame);
        }

        DisposeFramesReverse();
    }
    private void DisposeFramesReverse()
    {
        while (!curReverseVideoStack.IsEmpty)
        {
            curReverseVideoStack.TryPop(out var t2);
            for (int i = 0; i<t2.Count; i++)
            { 
                if (t2[i] == IntPtr.Zero) continue;
                AVPacket* packet = (AVPacket*)t2[i];
                av_packet_free(&packet);
            }
        }

        for (int i = 0; i<curReverseVideoPackets.Count; i++)
        { 
            if (curReverseVideoPackets[i] == IntPtr.Zero) continue;
            AVPacket* packet = (AVPacket*)curReverseVideoPackets[i];
            av_packet_free(&packet);
        }

        curReverseVideoPackets.Clear();

        for (int i=0; i<curReverseVideoFrames.Count; i++)
            DisposeFrame(curReverseVideoFrames[i]);

        curReverseVideoFrames.Clear();
    }
    public static void DisposeFrame(VideoFrame frame)
    {
        if (frame == null)
            return;

        if (frame.textures != null)
            for (int i=0; i<frame.textures.Length; i++)
                frame.textures[i].Dispose();

        if (frame.srvs != null)
            for (int i=0; i<frame.srvs.Length; i++)
                frame.srvs[i].Dispose();

        if (frame.bufRef != null)
            fixed (AVBufferRef** ptr = &frame.bufRef)
                av_buffer_unref(ptr);

        frame.srvs      = null;
        frame.textures  = null;
        frame.bufRef    = null;
    }
    protected override void DisposeInternal()
    {
        lock (lockCodecCtx)
        {
            DisposeFrames();

            if (codecCtx != null)
            {
                avcodec_close(codecCtx);
                fixed (AVCodecContext** ptr = &codecCtx) avcodec_free_context(ptr);

                codecCtx = null;
            }

            if (hwframes != null)
            fixed(AVBufferRef** ptr = &hwframes)
                av_buffer_unref(ptr);
            
            if (hw_device_ctx != null)
                fixed(AVBufferRef** ptr = &hw_device_ctx)
                    av_buffer_unref(ptr);

            if (swsCtx != null)
                sws_freeContext(swsCtx);

            swFallback  = false;
            hwframes    = null;
            swsCtx      = null;
            StartTime   = AV_NOPTS_VALUE;
        }
    }
    #endregion

    #region Recording
    internal Action<MediaType> recCompleted;
    Remuxer curRecorder;
    bool recGotKeyframe;
    internal bool isRecording;

    internal void StartRecording(Remuxer remuxer)
    {
        if (Disposed || isRecording) return;

        StartRecordTime     = AV_NOPTS_VALUE;
        curRecorder         = remuxer;
        recGotKeyframe      = false;
        isRecording         = true;
    }
    internal void StopRecording() => isRecording = false;
    #endregion

    #region TODO Decoder Profiles
    //internal enum DecoderProfiles
    //{
    //    DXVA_ModeMPEG2and1_VLD,
    //    DXVA_ModeMPEG1_VLD,
    //    DXVA2_ModeMPEG2_VLD,
    //    DXVA2_ModeMPEG2_IDCT,
    //    DXVA2_ModeMPEG2_MoComp,
    //    DXVA_ModeH264_A,
    //    DXVA_ModeH264_B,
    //    DXVA_ModeH264_C,
    //    DXVA_ModeH264_D,
    //    DXVA_ModeH264_E,
    //    DXVA_ModeH264_F,
    //    DXVA_ModeH264_VLD_Stereo_Progressive_NoFGT,
    //    DXVA_ModeH264_VLD_Stereo_NoFGT,
    //    DXVA_ModeH264_VLD_Multiview_NoFGT,
    //    DXVA_ModeWMV8_A,
    //    DXVA_ModeWMV8_B,
    //    DXVA_ModeWMV9_A,
    //    DXVA_ModeWMV9_B,
    //    DXVA_ModeWMV9_C,
    //    DXVA_ModeVC1_A,
    //    DXVA_ModeVC1_B,
    //    DXVA_ModeVC1_C,
    //    DXVA_ModeVC1_D,
    //    DXVA_ModeVC1_D2010,
    //    DXVA_ModeMPEG4pt2_VLD_Simple,
    //    DXVA_ModeMPEG4pt2_VLD_AdvSimple_NoGMC,
    //    DXVA_ModeMPEG4pt2_VLD_AdvSimple_GMC,
    //    DXVA_ModeHEVC_VLD_Main,
    //    DXVA_ModeHEVC_VLD_Main10,
    //    DXVA_ModeVP8_VLD,
    //    DXVA_ModeVP9_VLD_Profile0,
    //    DXVA_ModeVP9_VLD_10bit_Profile2,
    //    DXVA_ModeMPEG1_A,
    //    DXVA_ModeMPEG2_A,
    //    DXVA_ModeMPEG2_B,
    //    DXVA_ModeMPEG2_C,
    //    DXVA_ModeMPEG2_D,
    //    DXVA_ModeH261_A,
    //    DXVA_ModeH261_B,
    //    DXVA_ModeH263_A,
    //    DXVA_ModeH263_B,
    //    DXVA_ModeH263_C,
    //    DXVA_ModeH263_D,
    //    DXVA_ModeH263_E,
    //    DXVA_ModeH263_F,
    //    DXVA_ModeH264_VLD_WithFMOASO_NoFGT,
    //    DXVA_ModeH264_VLD_Multiview,
    //    DXVADDI_Intel_ModeH264_A,
    //    DXVADDI_Intel_ModeH264_C,
    //    DXVA_Intel_H264_NoFGT_ClearVideo,
    //    DXVA_ModeH264_VLD_NoFGT_Flash,
    //    DXVA_Intel_VC1_ClearVideo,
    //    DXVA_Intel_VC1_ClearVideo_2,
    //    DXVA_nVidia_MPEG4_ASP,
    //    DXVA_ModeMPEG4pt2_VLD_AdvSimple_Avivo,
    //    DXVA_ModeHEVC_VLD_Main_Intel,
    //    DXVA_ModeHEVC_VLD_Main10_Intel,
    //    DXVA_ModeHEVC_VLD_Main12_Intel,
    //    DXVA_ModeHEVC_VLD_Main422_10_Intel,
    //    DXVA_ModeHEVC_VLD_Main422_12_Intel,
    //    DXVA_ModeHEVC_VLD_Main444_Intel,
    //    DXVA_ModeHEVC_VLD_Main444_10_Intel,
    //    DXVA_ModeHEVC_VLD_Main444_12_Intel,
    //    DXVA_ModeH264_VLD_SVC_Scalable_Baseline,
    //    DXVA_ModeH264_VLD_SVC_Restricted_Scalable_Baseline,
    //    DXVA_ModeH264_VLD_SVC_Scalable_High,
    //    DXVA_ModeH264_VLD_SVC_Restricted_Scalable_High_Progressive,
    //    DXVA_ModeVP9_VLD_Intel,
    //    DXVA_ModeAV1_VLD_Profile0,
    //    DXVA_ModeAV1_VLD_Profile1,
    //    DXVA_ModeAV1_VLD_Profile2,
    //    DXVA_ModeAV1_VLD_12bit_Profile2,
    //    DXVA_ModeAV1_VLD_12bit_Profile2_420
    //}
    //internal static Dictionary<Guid, DecoderProfiles> DXVADecoderProfiles = new()
    //{
    //    { new(0x86695f12, 0x340e, 0x4f04, 0x9f, 0xd3, 0x92, 0x53, 0xdd, 0x32, 0x74, 0x60), DecoderProfiles.DXVA_ModeMPEG2and1_VLD },
    //    { new(0x6f3ec719, 0x3735, 0x42cc, 0x80, 0x63, 0x65, 0xcc, 0x3c, 0xb3, 0x66, 0x16), DecoderProfiles.DXVA_ModeMPEG1_VLD },
    //    { new(0xee27417f, 0x5e28,0x4e65, 0xbe, 0xea, 0x1d, 0x26, 0xb5, 0x08, 0xad, 0xc9), DecoderProfiles.DXVA2_ModeMPEG2_VLD },
    //    { new(0xbf22ad00, 0x03ea,0x4690, 0x80, 0x77, 0x47, 0x33, 0x46, 0x20, 0x9b, 0x7e), DecoderProfiles.DXVA2_ModeMPEG2_IDCT },
    //    { new(0xe6a9f44b, 0x61b0,0x4563, 0x9e, 0xa4, 0x63, 0xd2, 0xa3, 0xc6, 0xfe, 0x66), DecoderProfiles.DXVA2_ModeMPEG2_MoComp },
    //    { new(0x1b81be64, 0xa0c7,0x11d3, 0xb9, 0x84, 0x00, 0xc0, 0x4f, 0x2e, 0x73, 0xc5), DecoderProfiles.DXVA_ModeH264_A },
    //    { new(0x1b81be65, 0xa0c7,0x11d3, 0xb9, 0x84, 0x00, 0xc0, 0x4f, 0x2e, 0x73, 0xc5), DecoderProfiles.DXVA_ModeH264_B },
    //    { new(0x1b81be66, 0xa0c7,0x11d3, 0xb9, 0x84, 0x00, 0xc0, 0x4f, 0x2e, 0x73, 0xc5), DecoderProfiles.DXVA_ModeH264_C },
    //    { new(0x1b81be67, 0xa0c7,0x11d3, 0xb9, 0x84, 0x00, 0xc0, 0x4f, 0x2e, 0x73, 0xc5), DecoderProfiles.DXVA_ModeH264_D },
    //    { new(0x1b81be68, 0xa0c7,0x11d3, 0xb9, 0x84, 0x00, 0xc0, 0x4f, 0x2e, 0x73, 0xc5), DecoderProfiles.DXVA_ModeH264_E },
    //    { new(0x1b81be69, 0xa0c7,0x11d3, 0xb9, 0x84, 0x00, 0xc0, 0x4f, 0x2e, 0x73, 0xc5), DecoderProfiles.DXVA_ModeH264_F },
    //    { new(0xd79be8da, 0x0cf1,0x4c81, 0xb8, 0x2a, 0x69, 0xa4, 0xe2, 0x36, 0xf4, 0x3d), DecoderProfiles.DXVA_ModeH264_VLD_Stereo_Progressive_NoFGT },
    //    { new(0xf9aaccbb, 0xc2b6,0x4cfc, 0x87, 0x79, 0x57, 0x07, 0xb1, 0x76, 0x05, 0x52), DecoderProfiles.DXVA_ModeH264_VLD_Stereo_NoFGT },
    //    { new(0x705b9d82, 0x76cf,0x49d6, 0xb7, 0xe6, 0xac, 0x88, 0x72, 0xdb, 0x01, 0x3c), DecoderProfiles.DXVA_ModeH264_VLD_Multiview_NoFGT },
    //    { new(0x1b81be80, 0xa0c7,0x11d3, 0xb9, 0x84, 0x00, 0xc0, 0x4f, 0x2e, 0x73, 0xc5), DecoderProfiles.DXVA_ModeWMV8_A },
    //    { new(0x1b81be81, 0xa0c7,0x11d3, 0xb9, 0x84, 0x00, 0xc0, 0x4f, 0x2e, 0x73, 0xc5), DecoderProfiles.DXVA_ModeWMV8_B },
    //    { new(0x1b81be90, 0xa0c7,0x11d3, 0xb9, 0x84, 0x00, 0xc0, 0x4f, 0x2e, 0x73, 0xc5), DecoderProfiles.DXVA_ModeWMV9_A },
    //    { new(0x1b81be91, 0xa0c7,0x11d3, 0xb9, 0x84, 0x00, 0xc0, 0x4f, 0x2e, 0x73, 0xc5), DecoderProfiles.DXVA_ModeWMV9_B },
    //    { new(0x1b81be94, 0xa0c7,0x11d3, 0xb9, 0x84, 0x00, 0xc0, 0x4f, 0x2e, 0x73, 0xc5), DecoderProfiles.DXVA_ModeWMV9_C },
    //    { new(0x1b81beA0, 0xa0c7,0x11d3, 0xb9, 0x84, 0x00, 0xc0, 0x4f, 0x2e, 0x73, 0xc5), DecoderProfiles.DXVA_ModeVC1_A },
    //    { new(0x1b81beA1, 0xa0c7,0x11d3, 0xb9, 0x84, 0x00, 0xc0, 0x4f, 0x2e, 0x73, 0xc5), DecoderProfiles.DXVA_ModeVC1_B },
    //    { new(0x1b81beA2, 0xa0c7,0x11d3, 0xb9, 0x84, 0x00, 0xc0, 0x4f, 0x2e, 0x73, 0xc5), DecoderProfiles.DXVA_ModeVC1_C },
    //    { new(0x1b81beA3, 0xa0c7,0x11d3, 0xb9, 0x84, 0x00, 0xc0, 0x4f, 0x2e, 0x73, 0xc5), DecoderProfiles.DXVA_ModeVC1_D },
    //    { new(0x1b81bea4, 0xa0c7,0x11d3, 0xb9, 0x84, 0x00, 0xc0, 0x4f, 0x2e, 0x73, 0xc5), DecoderProfiles.DXVA_ModeVC1_D2010 },
    //    { new(0xefd64d74, 0xc9e8,0x41d7, 0xa5, 0xe9, 0xe9, 0xb0, 0xe3, 0x9f, 0xa3, 0x19), DecoderProfiles.DXVA_ModeMPEG4pt2_VLD_Simple },
    //    { new(0xed418a9f, 0x010d,0x4eda, 0x9a, 0xe3, 0x9a, 0x65, 0x35, 0x8d, 0x8d, 0x2e), DecoderProfiles.DXVA_ModeMPEG4pt2_VLD_AdvSimple_NoGMC },
    //    { new(0xab998b5b, 0x4258,0x44a9, 0x9f, 0xeb, 0x94, 0xe5, 0x97, 0xa6, 0xba, 0xae), DecoderProfiles.DXVA_ModeMPEG4pt2_VLD_AdvSimple_GMC },
    //    { new(0x5b11d51b, 0x2f4c,0x4452, 0xbc, 0xc3, 0x09, 0xf2, 0xa1, 0x16, 0x0c, 0xc0), DecoderProfiles.DXVA_ModeHEVC_VLD_Main },
    //    { new(0x107af0e0, 0xef1a,0x4d19, 0xab, 0xa8, 0x67, 0xa1, 0x63, 0x07, 0x3d, 0x13), DecoderProfiles.DXVA_ModeHEVC_VLD_Main10 },
    //    { new(0x90b899ea, 0x3a62,0x4705, 0x88, 0xb3, 0x8d, 0xf0, 0x4b, 0x27, 0x44, 0xe7), DecoderProfiles.DXVA_ModeVP8_VLD },
    //    { new(0x463707f8, 0xa1d0,0x4585, 0x87, 0x6d, 0x83, 0xaa, 0x6d, 0x60, 0xb8, 0x9e), DecoderProfiles.DXVA_ModeVP9_VLD_Profile0 },
    //    { new(0xa4c749ef, 0x6ecf,0x48aa, 0x84, 0x48, 0x50, 0xa7, 0xa1, 0x16, 0x5f, 0xf7), DecoderProfiles.DXVA_ModeVP9_VLD_10bit_Profile2 },
    //    { new(0x1b81be09, 0xa0c7, 0x11d3, 0xb9, 0x84, 0x00, 0xc0, 0x4f, 0x2e, 0x73, 0xc5), DecoderProfiles.DXVA_ModeMPEG1_A },
    //    { new(0x1b81be0A, 0xa0c7, 0x11d3, 0xb9, 0x84, 0x00, 0xc0, 0x4f, 0x2e, 0x73, 0xc5), DecoderProfiles.DXVA_ModeMPEG2_A },
    //    { new(0x1b81be0B, 0xa0c7, 0x11d3, 0xb9, 0x84, 0x00, 0xc0, 0x4f, 0x2e, 0x73, 0xc5), DecoderProfiles.DXVA_ModeMPEG2_B },
    //    { new(0x1b81be0C, 0xa0c7, 0x11d3, 0xb9, 0x84, 0x00, 0xc0, 0x4f, 0x2e, 0x73, 0xc5), DecoderProfiles.DXVA_ModeMPEG2_C },
    //    { new(0x1b81be0D, 0xa0c7, 0x11d3, 0xb9, 0x84, 0x00, 0xc0, 0x4f, 0x2e, 0x73, 0xc5), DecoderProfiles.DXVA_ModeMPEG2_D },
    //    { new(0x1b81be01, 0xa0c7, 0x11d3, 0xb9, 0x84, 0x00, 0xc0, 0x4f, 0x2e, 0x73, 0xc5), DecoderProfiles.DXVA_ModeH261_A },
    //    { new(0x1b81be02, 0xa0c7, 0x11d3, 0xb9, 0x84, 0x00, 0xc0, 0x4f, 0x2e, 0x73, 0xc5), DecoderProfiles.DXVA_ModeH261_B },
    //    { new(0x1b81be03, 0xa0c7, 0x11d3, 0xb9, 0x84, 0x00, 0xc0, 0x4f, 0x2e, 0x73, 0xc5), DecoderProfiles.DXVA_ModeH263_A },
    //    { new(0x1b81be04, 0xa0c7, 0x11d3, 0xb9, 0x84, 0x00, 0xc0, 0x4f, 0x2e, 0x73, 0xc5), DecoderProfiles.DXVA_ModeH263_B },
    //    { new(0x1b81be05, 0xa0c7, 0x11d3, 0xb9, 0x84, 0x00, 0xc0, 0x4f, 0x2e, 0x73, 0xc5), DecoderProfiles.DXVA_ModeH263_C },
    //    { new(0x1b81be06, 0xa0c7, 0x11d3, 0xb9, 0x84, 0x00, 0xc0, 0x4f, 0x2e, 0x73, 0xc5), DecoderProfiles.DXVA_ModeH263_D },
    //    { new(0x1b81be07, 0xa0c7, 0x11d3, 0xb9, 0x84, 0x00, 0xc0, 0x4f, 0x2e, 0x73, 0xc5), DecoderProfiles.DXVA_ModeH263_E },
    //    { new(0x1b81be08, 0xa0c7, 0x11d3, 0xb9, 0x84, 0x00, 0xc0, 0x4f, 0x2e, 0x73, 0xc5), DecoderProfiles.DXVA_ModeH263_F },
    //    { new(0xd5f04ff9, 0x3418, 0x45d8, 0x95, 0x61, 0x32, 0xa7, 0x6a, 0xae, 0x2d, 0xdd), DecoderProfiles.DXVA_ModeH264_VLD_WithFMOASO_NoFGT },
    //    { new(0x9901CCD3, 0xca12, 0x4b7e, 0x86, 0x7a, 0xe2, 0x22, 0x3d, 0x92, 0x55, 0xc3), DecoderProfiles.DXVA_ModeH264_VLD_Multiview },
    //    { new(0x604F8E64, 0x4951, 0x4c54, 0x88, 0xFE, 0xAB, 0xD2, 0x5C, 0x15, 0xB3, 0xD6), DecoderProfiles.DXVADDI_Intel_ModeH264_A },
    //    { new(0x604F8E66, 0x4951, 0x4c54, 0x88, 0xFE, 0xAB, 0xD2, 0x5C, 0x15, 0xB3, 0xD6), DecoderProfiles.DXVADDI_Intel_ModeH264_C },
    //    { new(0x604F8E68, 0x4951, 0x4c54, 0x88, 0xFE, 0xAB, 0xD2, 0x5C, 0x15, 0xB3, 0xD6), DecoderProfiles.DXVA_Intel_H264_NoFGT_ClearVideo },
    //    { new(0x4245F676, 0x2BBC, 0x4166, 0xa0, 0xBB, 0x54, 0xE7, 0xB8, 0x49, 0xC3, 0x80), DecoderProfiles.DXVA_ModeH264_VLD_NoFGT_Flash },
    //    { new(0xBCC5DB6D, 0xA2B6, 0x4AF0, 0xAC, 0xE4, 0xAD, 0xB1, 0xF7, 0x87, 0xBC, 0x89), DecoderProfiles.DXVA_Intel_VC1_ClearVideo },
    //    { new(0xE07EC519, 0xE651, 0x4CD6, 0xAC, 0x84, 0x13, 0x70, 0xCC, 0xEE, 0xC8, 0x51), DecoderProfiles.DXVA_Intel_VC1_ClearVideo_2 },
    //    { new(0x9947EC6F, 0x689B, 0x11DC, 0xA3, 0x20, 0x00, 0x19, 0xDB, 0xBC, 0x41, 0x84), DecoderProfiles.DXVA_nVidia_MPEG4_ASP },
    //    { new(0x7C74ADC6, 0xe2ba, 0x4ade, 0x86, 0xde, 0x30, 0xbe, 0xab, 0xb4, 0x0c, 0xc1), DecoderProfiles.DXVA_ModeMPEG4pt2_VLD_AdvSimple_Avivo },
    //    { new(0x8c56eb1e, 0x2b47, 0x466f, 0x8d, 0x33, 0x7d, 0xbc, 0xd6, 0x3f, 0x3d, 0xf2), DecoderProfiles.DXVA_ModeHEVC_VLD_Main_Intel },
    //    { new(0x75fc75f7, 0xc589, 0x4a07, 0xa2, 0x5b, 0x72, 0xe0, 0x3b, 0x03, 0x83, 0xb3), DecoderProfiles.DXVA_ModeHEVC_VLD_Main10_Intel },
    //    { new(0x8ff8a3aa, 0xc456, 0x4132, 0xb6, 0xef, 0x69, 0xd9, 0xdd, 0x72, 0x57, 0x1d), DecoderProfiles.DXVA_ModeHEVC_VLD_Main12_Intel },
    //    { new(0xe484dcb8, 0xcac9, 0x4859, 0x99, 0xf5, 0x5c, 0x0d, 0x45, 0x06, 0x90, 0x89), DecoderProfiles.DXVA_ModeHEVC_VLD_Main422_10_Intel },
    //    { new(0xc23dd857, 0x874b, 0x423c, 0xb6, 0xe0, 0x82, 0xce, 0xaa, 0x9b, 0x11, 0x8a), DecoderProfiles.DXVA_ModeHEVC_VLD_Main422_12_Intel },
    //    { new(0x41a5af96, 0xe415, 0x4b0c, 0x9d, 0x03, 0x90, 0x78, 0x58, 0xe2, 0x3e, 0x78), DecoderProfiles.DXVA_ModeHEVC_VLD_Main444_Intel },
    //    { new(0x6a6a81ba, 0x912a, 0x485d, 0xb5, 0x7f, 0xcc, 0xd2, 0xd3, 0x7b, 0x8d, 0x94), DecoderProfiles.DXVA_ModeHEVC_VLD_Main444_10_Intel },
    //    { new(0x5b08e35d, 0x0c66, 0x4c51, 0xa6, 0xf1, 0x89, 0xd0, 0x0c, 0xb2, 0xc1, 0x97), DecoderProfiles.DXVA_ModeHEVC_VLD_Main444_12_Intel },
    //    { new(0xc30700c4, 0xe384, 0x43e0, 0xb9, 0x82, 0x2d, 0x89, 0xee, 0x7f, 0x77, 0xc4), DecoderProfiles.DXVA_ModeH264_VLD_SVC_Scalable_Baseline },
    //    { new(0x9b8175d4, 0xd670, 0x4cf2, 0xa9, 0xf0, 0xfa, 0x56, 0xdf, 0x71, 0xa1, 0xae), DecoderProfiles.DXVA_ModeH264_VLD_SVC_Restricted_Scalable_Baseline },
    //    { new(0x728012c9, 0x66a8, 0x422f, 0x97, 0xe9, 0xb5, 0xe3, 0x9b, 0x51, 0xc0, 0x53), DecoderProfiles.DXVA_ModeH264_VLD_SVC_Scalable_High },
    //    { new(0x8efa5926, 0xbd9e, 0x4b04, 0x8b, 0x72, 0x8f, 0x97, 0x7d, 0xc4, 0x4c, 0x36), DecoderProfiles.DXVA_ModeH264_VLD_SVC_Restricted_Scalable_High_Progressive },
    //    { new(0x76988a52, 0xdf13, 0x419a, 0x8e, 0x64, 0xff, 0xcf, 0x4a, 0x33, 0x6c, 0xf5), DecoderProfiles.DXVA_ModeVP9_VLD_Intel },
    //    { new(0xb8be4ccb, 0xcf53, 0x46ba, 0x8d, 0x59, 0xd6, 0xb8, 0xa6, 0xda, 0x5d, 0x2a), DecoderProfiles.DXVA_ModeAV1_VLD_Profile0 },
    //    { new(0x6936ff0f, 0x45b1, 0x4163, 0x9c, 0xc1, 0x64, 0x6e, 0xf6, 0x94, 0x61, 0x08), DecoderProfiles.DXVA_ModeAV1_VLD_Profile1 },
    //    { new(0x0c5f2aa1, 0xe541, 0x4089, 0xbb, 0x7b, 0x98, 0x11, 0x0a, 0x19, 0xd7, 0xc8), DecoderProfiles.DXVA_ModeAV1_VLD_Profile2 },
    //    { new(0x17127009, 0xa00f, 0x4ce1, 0x99, 0x4e, 0xbf, 0x40, 0x81, 0xf6, 0xf3, 0xf0), DecoderProfiles.DXVA_ModeAV1_VLD_12bit_Profile2 },
    //    { new(0x2d80bed6, 0x9cac, 0x4835, 0x9e, 0x91, 0x32, 0x7b, 0xbc, 0x4f, 0x9e, 0xe8), DecoderProfiles.DXVA_ModeAV1_VLD_12bit_Profile2_420 },


    //};
    //internal static Dictionary<DecoderProfiles, string> DXVADecoderProfilesDesc = new()
    //{
    //    { DecoderProfiles.DXVA_ModeMPEG1_A, "MPEG-1 decoder, restricted profile A" },
    //    { DecoderProfiles.DXVA_ModeMPEG2_A, "MPEG-2 decoder, restricted profile A" },
    //    { DecoderProfiles.DXVA_ModeMPEG2_B, "MPEG-2 decoder, restricted profile B" },
    //    { DecoderProfiles.DXVA_ModeMPEG2_C, "MPEG-2 decoder, restricted profile C" },
    //    { DecoderProfiles.DXVA_ModeMPEG2_D, "MPEG-2 decoder, restricted profile D" },
    //    { DecoderProfiles.DXVA2_ModeMPEG2_VLD, "MPEG-2 variable-length decoder" },
    //    { DecoderProfiles.DXVA_ModeMPEG2and1_VLD, "MPEG-2 & MPEG-1 variable-length decoder" },
    //    { DecoderProfiles.DXVA2_ModeMPEG2_MoComp, "MPEG-2 motion compensation" },
    //    { DecoderProfiles.DXVA2_ModeMPEG2_IDCT, "MPEG-2 inverse discrete cosine transform" },
    //    { DecoderProfiles.DXVA_ModeMPEG1_VLD, "MPEG-1 variable-length decoder, no D pictures" },
    //    { DecoderProfiles.DXVA_ModeH264_F, "H.264 variable-length decoder, film grain technology" },
    //    { DecoderProfiles.DXVA_ModeH264_E, "H.264 variable-length decoder, no film grain technology" },
    //    { DecoderProfiles.DXVA_Intel_H264_NoFGT_ClearVideo, "H.264 variable-length decoder, no film grain technology (Intel ClearVideo)" },
    //    { DecoderProfiles.DXVA_ModeH264_VLD_WithFMOASO_NoFGT, "H.264 variable-length decoder, no film grain technology, FMO/ASO" },
    //    { DecoderProfiles.DXVA_ModeH264_VLD_NoFGT_Flash, "H.264 variable-length decoder, no film grain technology, Flash" },
    //    { DecoderProfiles.DXVA_ModeH264_D, "H.264 inverse discrete cosine transform, film grain technology" },
    //    { DecoderProfiles.DXVA_ModeH264_C, "H.264 inverse discrete cosine transform, no film grain technology" },
    //    { DecoderProfiles.DXVADDI_Intel_ModeH264_C, "H.264 inverse discrete cosine transform, no film grain technology (Intel)" },
    //    { DecoderProfiles.DXVA_ModeH264_B, "H.264 motion compensation, film grain technology" },
    //    { DecoderProfiles.DXVA_ModeH264_A, "H.264 motion compensation, no film grain technology" },
    //    { DecoderProfiles.DXVADDI_Intel_ModeH264_A, "H.264 motion compensation, no film grain technology (Intel)" },
    //    { DecoderProfiles.DXVA_ModeH264_VLD_Stereo_Progressive_NoFGT, "H.264 stereo high profile, mbs flag set" },
    //    { DecoderProfiles.DXVA_ModeH264_VLD_Stereo_NoFGT, "H.264 stereo high profile" },
    //    { DecoderProfiles.DXVA_ModeH264_VLD_Multiview_NoFGT, "H.264 multiview high profile" },
    //    { DecoderProfiles.DXVA_ModeH264_VLD_SVC_Scalable_Baseline, "H.264 scalable video coding, Scalable Baseline Profile" },
    //    { DecoderProfiles.DXVA_ModeH264_VLD_SVC_Restricted_Scalable_Baseline, "H.264 scalable video coding, Scalable Constrained Baseline Profile" },
    //    { DecoderProfiles.DXVA_ModeH264_VLD_SVC_Scalable_High, "H.264 scalable video coding, Scalable High Profile" },
    //    { DecoderProfiles.DXVA_ModeH264_VLD_SVC_Restricted_Scalable_High_Progressive, "H.264 scalable video coding, Scalable Constrained High Profile" },
    //    { DecoderProfiles.DXVA_ModeWMV8_B, "Windows Media Video 8 motion compensation" },
    //    { DecoderProfiles.DXVA_ModeWMV8_A, "Windows Media Video 8 post processing" },
    //    { DecoderProfiles.DXVA_ModeWMV9_C, "Windows Media Video 9 IDCT" },
    //    { DecoderProfiles.DXVA_ModeWMV9_B, "Windows Media Video 9 motion compensation" },
    //    { DecoderProfiles.DXVA_ModeWMV9_A, "Windows Media Video 9 post processing" },
    //    { DecoderProfiles.DXVA_ModeVC1_D, "VC-1 variable-length decoder" },
    //    { DecoderProfiles.DXVA_ModeVC1_D2010, "VC-1 variable-length decoder" },
    //    { DecoderProfiles.DXVA_Intel_VC1_ClearVideo_2, "VC-1 variable-length decoder 2 (Intel)" },
    //    { DecoderProfiles.DXVA_Intel_VC1_ClearVideo, "VC-1 variable-length decoder (Intel)" },
    //    { DecoderProfiles.DXVA_ModeVC1_C, "VC-1 inverse discrete cosine transform" },
    //    { DecoderProfiles.DXVA_ModeVC1_B, "VC-1 motion compensation" },
    //    { DecoderProfiles.DXVA_ModeVC1_A, "VC-1 post processing" },
    //    { DecoderProfiles.DXVA_nVidia_MPEG4_ASP, "MPEG-4 Part 2 nVidia bitstream decoder" },
    //    { DecoderProfiles.DXVA_ModeMPEG4pt2_VLD_Simple, "MPEG-4 Part 2 variable-length decoder, Simple Profile" },
    //    { DecoderProfiles.DXVA_ModeMPEG4pt2_VLD_AdvSimple_NoGMC, "MPEG-4 Part 2 variable-length decoder, Simple&Advanced Profile, no GMC" },
    //    { DecoderProfiles.DXVA_ModeMPEG4pt2_VLD_AdvSimple_GMC, "MPEG-4 Part 2 variable-length decoder, Simple&Advanced Profile, GMC" },
    //    { DecoderProfiles.DXVA_ModeMPEG4pt2_VLD_AdvSimple_Avivo, "MPEG-4 Part 2 variable-length decoder, Simple&Advanced Profile, Avivo" },
    //    { DecoderProfiles.DXVA_ModeHEVC_VLD_Main_Intel, "HEVC Main profile (Intel)" },
    //    { DecoderProfiles.DXVA_ModeHEVC_VLD_Main10_Intel, "HEVC Main 10 profile (Intel)" },
    //    { DecoderProfiles.DXVA_ModeHEVC_VLD_Main12_Intel, "HEVC Main profile 4:2:2 Range Extension (Intel)" },
    //    { DecoderProfiles.DXVA_ModeHEVC_VLD_Main422_10_Intel, "HEVC Main 10 profile 4:2:2 Range Extension (Intel)" },
    //    { DecoderProfiles.DXVA_ModeHEVC_VLD_Main422_12_Intel, "HEVC Main 12 profile 4:2:2 Range Extension (Intel)" },
    //    { DecoderProfiles.DXVA_ModeHEVC_VLD_Main444_Intel, "HEVC Main profile 4:4:4 Range Extension (Intel)" },
    //    { DecoderProfiles.DXVA_ModeHEVC_VLD_Main444_10_Intel, "HEVC Main 10 profile 4:4:4 Range Extension (Intel)" },
    //    { DecoderProfiles.DXVA_ModeHEVC_VLD_Main444_12_Intel, "HEVC Main 12 profile 4:4:4 Range Extension (Intel)" },
    //    { DecoderProfiles.DXVA_ModeHEVC_VLD_Main, "HEVC Main profile" },
    //    { DecoderProfiles.DXVA_ModeHEVC_VLD_Main10, "HEVC Main 10 profile" },
    //    { DecoderProfiles.DXVA_ModeH261_A, "H.261 decoder, restricted profile A" },
    //    { DecoderProfiles.DXVA_ModeH261_B, "H.261 decoder, restricted profile B" },
    //    { DecoderProfiles.DXVA_ModeH263_A, "H.263 decoder, restricted profile A" },
    //    { DecoderProfiles.DXVA_ModeH263_B, "H.263 decoder, restricted profile B" },
    //    { DecoderProfiles.DXVA_ModeH263_C, "H.263 decoder, restricted profile C" },
    //    { DecoderProfiles.DXVA_ModeH263_D, "H.263 decoder, restricted profile D" },
    //    { DecoderProfiles.DXVA_ModeH263_E, "H.263 decoder, restricted profile E" },
    //    { DecoderProfiles.DXVA_ModeH263_F, "H.263 decoder, restricted profile F" },
    //    { DecoderProfiles.DXVA_ModeVP8_VLD, "VP8" },
    //    { DecoderProfiles.DXVA_ModeVP9_VLD_Profile0, "VP9 profile 0" },
    //    { DecoderProfiles.DXVA_ModeVP9_VLD_10bit_Profile2, "VP9 profile" },
    //    { DecoderProfiles.DXVA_ModeVP9_VLD_Intel, "VP9 profile Intel" },
    //    { DecoderProfiles.DXVA_ModeAV1_VLD_Profile0, "AV1 Main profile" },
    //    { DecoderProfiles.DXVA_ModeAV1_VLD_Profile1, "AV1 High profile" },
    //};
    #endregion
}