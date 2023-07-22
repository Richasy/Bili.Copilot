// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using FFmpeg.AutoGen;
using static FFmpeg.AutoGen.AVMediaType;
using static FFmpeg.AutoGen.ffmpeg;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaRemuxer;

/// <summary>
/// 重新封装器，用于重新封装音视频流.
/// </summary>
public unsafe class Remuxer
{
    private readonly Dictionary<IntPtr, IntPtr> _mapInOutStreams = new();
    private readonly Dictionary<int, IntPtr> _mapInInStream = new();
    private readonly Dictionary<IntPtr, IntPtr> _mapInOutStreams2 = new();
    private readonly Dictionary<int, IntPtr> _mapInInStream2 = new();
    private readonly Dictionary<int, long> _mapInStreamToDts2 = new();
    private readonly Mutex _mutex = new();

    private Dictionary<int, long> _mapInStreamToDts = new();
    private AVFormatContext* _fmtCtx;
    private AVOutputFormat* _fmt;

    /// <summary>
    /// 初始化 <see cref="Remuxer"/> 类的新实例.
    /// </summary>
    /// <param name="uniqueId">唯一标识符.</param>
    public Remuxer(int uniqueId = -1)
        => UniqueId = uniqueId == -1 ? Utils.GetUniqueId() : uniqueId;

    /// <summary>
    /// 获取或设置唯一标识符.
    /// </summary>
    public int UniqueId { get; set; }

    /// <summary>
    /// 获取或设置一个值，指示是否已释放.
    /// </summary>
    public bool Disposed { get; private set; } = true;

    /// <summary>
    /// 获取或设置文件名.
    /// </summary>
    public string Filename { get; private set; }

    /// <summary>
    /// 获取一个值，指示是否存在音视频流.
    /// </summary>
    public bool HasStreams => _mapInOutStreams2.Count > 0 || _mapInOutStreams.Count > 0;

    /// <summary>
    /// 获取或设置一个值，指示是否已写入头部.
    /// </summary>
    public bool HeaderWritten { get; private set; }

    /// <summary>
    /// 打开文件.
    /// </summary>
    /// <param name="filename">文件名.</param>
    /// <returns>返回打开结果.</returns>
    public int Open(string filename)
    {
        int ret;
        Filename = filename;

        fixed (AVFormatContext** ptr = &_fmtCtx)
        {
            ret = avformat_alloc_output_context2(ptr, null, null, Filename);
        }

        if (ret < 0)
        {
            return ret;
        }

        _fmt = _fmtCtx->oformat;
        _mapInStreamToDts = new Dictionary<int, long>();
        Disposed = false;

        return 0;
    }

    /// <summary>
    /// 添加音视频流.
    /// </summary>
    /// <param name="in_stream">输入流.</param>
    /// <param name="isAudioDemuxer">是否为音频解复用器.</param>
    /// <returns>返回添加结果.</returns>
    public int AddStream(AVStream* in_stream, bool isAudioDemuxer = false)
    {
        var ret = -1;

        if (in_stream == null || (in_stream->codecpar->codec_type != AVMEDIA_TYPE_VIDEO && in_stream->codecpar->codec_type != AVMEDIA_TYPE_AUDIO))
        {
            return ret;
        }

        AVStream* out_stream;
        var in_codecpar = in_stream->codecpar;

        out_stream = avformat_new_stream(_fmtCtx, null);
        if (out_stream == null)
        {
            return -1;
        }

        ret = avcodec_parameters_copy(out_stream->codecpar, in_codecpar);
        if (ret < 0)
        {
            return ret;
        }

        // 复制元数据（目前只复制语言）
        AVDictionaryEntry* b = null;
        while (true)
        {
            b = av_dict_get(in_stream->metadata, string.Empty, b, AV_DICT_IGNORE_SUFFIX);
            if (b == null)
            {
                break;
            }

            if (Utils.BytePtrToStringUtf8(b->key).ToLower() == "language" || Utils.BytePtrToStringUtf8(b->key).ToLower() == "lang")
            {
                av_dict_set(&out_stream->metadata, Utils.BytePtrToStringUtf8(b->key), Utils.BytePtrToStringUtf8(b->value), 0);
            }
        }

        out_stream->codecpar->codec_tag = 0;

        if (isAudioDemuxer)
        {
            _mapInOutStreams2.Add((IntPtr)in_stream, (IntPtr)out_stream);
            _mapInInStream2.Add(in_stream->index, (IntPtr)in_stream);
        }
        else
        {
            _mapInOutStreams.Add((IntPtr)in_stream, (IntPtr)out_stream);
            _mapInInStream.Add(in_stream->index, (IntPtr)in_stream);
        }

        return 0;
    }

    /// <summary>
    /// 写入头部.
    /// </summary>
    /// <returns>返回写入结果.</returns>
    public int WriteHeader()
    {
        if (!HasStreams)
        {
            throw new Exception("未配置任何流用于重新封装");
        }

        int ret;

        ret = avio_open(&_fmtCtx->pb, Filename, AVIO_FLAG_WRITE);
        if (ret < 0)
        {
            Dispose();
            return ret;
        }

        ret = avformat_write_header(_fmtCtx, null);

        if (ret < 0)
        {
            Dispose();
            return ret;
        }

        HeaderWritten = true;
        return 0;
    }

    /// <summary>
    /// 写入数据包.
    /// </summary>
    /// <param name="packet">数据包.</param>
    /// <param name="isAudioDemuxer">是否为音频解复用器.</param>
    /// <returns>返回写入结果.</returns>
    public int Write(AVPacket* packet, bool isAudioDemuxer = false)
    {
#pragma warning disable CA2002 // 不要对具有弱标识的对象进行锁定
        lock (_mutex)
        {
            var mapInInStream = !isAudioDemuxer ? _mapInInStream : _mapInInStream2;
            var mapInOutStreams = !isAudioDemuxer ? _mapInOutStreams : _mapInOutStreams2;
            var mapInStreamToDts = !isAudioDemuxer ? _mapInStreamToDts : _mapInStreamToDts2;

            var in_stream = (AVStream*)mapInInStream[packet->stream_index];
            var out_stream = (AVStream*)mapInOutStreams[(IntPtr)in_stream];

            if (packet->dts != AV_NOPTS_VALUE)
            {
                if (!mapInStreamToDts.ContainsKey(in_stream->index))
                {
                    // TODO: 在音频解复用器的情况下，计算与视频解复用器的差异并添加到其中之一 - 所有流 - （以确保为正数）
                    mapInStreamToDts.Add(in_stream->index, packet->dts);
                }

                packet->pts = packet->pts == AV_NOPTS_VALUE ? AV_NOPTS_VALUE : av_rescale_q_rnd(packet->pts - mapInStreamToDts[in_stream->index], in_stream->time_base, out_stream->time_base, AVRounding.AV_ROUND_NEAR_INF | AVRounding.AV_ROUND_PASS_MINMAX);
                packet->dts = av_rescale_q_rnd(packet->dts - mapInStreamToDts[in_stream->index], in_stream->time_base, out_stream->time_base, AVRounding.AV_ROUND_NEAR_INF | AVRounding.AV_ROUND_PASS_MINMAX);
            }
            else
            {
                packet->pts = packet->pts == AV_NOPTS_VALUE ? AV_NOPTS_VALUE : av_rescale_q_rnd(packet->pts, in_stream->time_base, out_stream->time_base, AVRounding.AV_ROUND_NEAR_INF | AVRounding.AV_ROUND_PASS_MINMAX);
                packet->dts = AV_NOPTS_VALUE;
            }

            packet->duration = av_rescale_q(packet->duration, in_stream->time_base, out_stream->time_base);
            packet->stream_index = out_stream->index;
            packet->pos = -1;

            var ret = av_interleaved_write_frame(_fmtCtx, packet);
            av_packet_free(&packet);

            return ret;
        }
#pragma warning restore CA2002 // 不要对具有弱标识的对象进行锁定
    }

    /// <summary>
    /// 写入尾部.
    /// </summary>
    /// <returns>返回写入结果.</returns>
    public int WriteTrailer() => Dispose();

    /// <summary>
    /// 释放资源.
    /// </summary>
    /// <returns>返回释放结果.</returns>
    public int Dispose()
    {
        if (Disposed)
        {
            return -1;
        }

        var ret = 0;

        if (_fmtCtx != null)
        {
            if (HeaderWritten)
            {
                ret = av_write_trailer(_fmtCtx);
                avio_closep(&_fmtCtx->pb);
            }

            avformat_free_context(_fmtCtx);
        }

        _fmtCtx = null;
        Filename = null;
        Disposed = true;
        HeaderWritten = false;
        _mapInOutStreams.Clear();
        _mapInInStream.Clear();
        _mapInOutStreams2.Clear();
        _mapInInStream2.Clear();

        return ret;
    }

    private void Log(string msg)
        => Debug.WriteLine($"[{DateTime.Now:hh.mm.ss.fff}] [#{UniqueId}] [Remuxer] {msg}");
}
