// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Libs.Player.MediaFramework.MediaDemuxer;
using FFmpeg.AutoGen;
using static FFmpeg.AutoGen.ffmpeg;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaStream;

/// <summary>
/// 表示音频流.
/// </summary>
public unsafe class AudioStream : StreamBase
{
    /// <summary>
    /// 初始化 AudioStream 类的新实例.
    /// </summary>
    public AudioStream()
    {
    }

    /// <summary>
    /// 使用指定的解复用器和音频流指针初始化 AudioStream 类的新实例，并刷新音频流的信息.
    /// </summary>
    /// <param name="demuxer">解复用器.</param>
    /// <param name="st">音频流指针.</param>
    public AudioStream(Demuxer demuxer, AVStream* st)
        : base(demuxer, st) => Refresh();

    /// <summary>
    /// 获取或设置音频的位数.
    /// </summary>
    public int Bits { get; set; }

    /// <summary>
    /// 获取或设置音频的声道数.
    /// </summary>
    public int Channels { get; set; }

    /// <summary>
    /// 获取或设置音频的声道布局.
    /// </summary>
    public ulong ChannelLayout { get; set; }

    /// <summary>
    /// 获取或设置音频的声道布局字符串.
    /// </summary>
    public string ChannelLayoutStr { get; set; }

    /// <summary>
    /// 获取或设置音频的采样格式.
    /// </summary>
    public AVSampleFormat SampleFormat { get; set; }

    /// <summary>
    /// 获取或设置音频的采样格式字符串.
    /// </summary>
    public string SampleFormatStr { get; set; }

    /// <summary>
    /// 获取或设置音频的采样率.
    /// </summary>
    public int SampleRate { get; set; }

    /// <summary>
    /// 获取或设置音频的原始编解码器 ID.
    /// </summary>
    public AVCodecID CodecIdOrig { get; set; }

    /// <summary>
    /// 获取音频流的详细信息.
    /// </summary>
    /// <returns>音频流的详细信息.</returns>
    public override string GetDump()
        => $"[{Type} #{StreamIndex}-{Language.IdSubLanguage}{(Title != null ? "(" + Title + ")" : string.Empty)}] {Codec} {SampleFormatStr}@{Bits} {SampleRate / 1000}KHz {ChannelLayoutStr} | [BR: {BitRate}] | {Utils.TicksToTime((long)(AVStream->start_time * TimeBase))}/{Utils.TicksToTime((long)(AVStream->duration * TimeBase))} | {Utils.TicksToTime(StartTime)}/{Utils.TicksToTime(Duration)}";

    /// <summary>
    /// 刷新音频流的信息.
    /// </summary>
    public override void Refresh()
    {
        base.Refresh();

        SampleFormat = (AVSampleFormat)Enum.ToObject(typeof(AVSampleFormat), AVStream->codecpar->format);
        SampleFormatStr = av_get_sample_fmt_name(SampleFormat);
        SampleRate = AVStream->codecpar->sample_rate;

        if (AVStream->codecpar->ch_layout.order == AVChannelOrder.AV_CHANNEL_ORDER_UNSPEC)
        {
            av_channel_layout_default(&AVStream->codecpar->ch_layout, AVStream->codecpar->ch_layout.nb_channels);
        }

        ChannelLayout = AVStream->codecpar->ch_layout.u.mask;
        Channels = AVStream->codecpar->ch_layout.nb_channels;
        Bits = AVStream->codecpar->bits_per_coded_sample;

        // https://trac.ffmpeg.org/ticket/7321
        CodecIdOrig = CodecId;
        if (CodecId == AVCodecID.AV_CODEC_ID_MP2 && (SampleFormat == AVSampleFormat.AV_SAMPLE_FMT_FLTP || SampleFormat == AVSampleFormat.AV_SAMPLE_FMT_FLT))
        {
            CodecId = AVCodecID.AV_CODEC_ID_MP3; // OR? st->codecpar->format = (int) AVSampleFormat.AV_SAMPLE_FMT_S16P;
        }

        var buf = new byte[50];
        fixed (byte* bufPtr = buf)
        {
            av_channel_layout_describe(&AVStream->codecpar->ch_layout, bufPtr, (ulong)buf.Length);
            ChannelLayoutStr = Utils.BytePtrToStringUtf8(bufPtr);
        }
    }
}
