// Copyright (c) Bili Copilot. All rights reserved.

using System;

using FFmpeg.AutoGen;
using static FFmpeg.AutoGen.ffmpeg;

using Bili.Copilot.Libs.Player.MediaFramework.MediaDemuxer;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaStream;

public unsafe class AudioStream : StreamBase
{
    public int Bits { get; set; }
    public int Channels { get; set; }
    public ulong ChannelLayout { get; set; }
    public string ChannelLayoutStr { get; set; }
    public AVSampleFormat SampleFormat { get; set; }
    public string SampleFormatStr { get; set; }
    public int SampleRate { get; set; }
    public AVCodecID CodecIDOrig { get; set; }

    public override string GetDump()
        => $"[{Type} #{StreamIndex}-{Language.IdSubLanguage}{(Title != null ? "(" + Title + ")" : "")}] {Codec} {SampleFormatStr}@{Bits} {SampleRate / 1000}KHz {ChannelLayoutStr} | [BR: {BitRate}] | {Utils.TicksToTime((long)(AVStream->start_time * Timebase))}/{Utils.TicksToTime((long)(AVStream->duration * Timebase))} | {Utils.TicksToTime(StartTime)}/{Utils.TicksToTime(Duration)}";

    public AudioStream() { }
    public AudioStream(Demuxer demuxer, AVStream* st) : base(demuxer, st) => Refresh();

    public override void Refresh()
    {
        base.Refresh();

        SampleFormat = (AVSampleFormat)Enum.ToObject(typeof(AVSampleFormat), AVStream->codecpar->format);
        SampleFormatStr = av_get_sample_fmt_name(SampleFormat);
        SampleRate = AVStream->codecpar->sample_rate;

        if (AVStream->codecpar->ch_layout.order == AVChannelOrder.AV_CHANNEL_ORDER_UNSPEC)
            av_channel_layout_default(&AVStream->codecpar->ch_layout, AVStream->codecpar->ch_layout.nb_channels);

        ChannelLayout = AVStream->codecpar->ch_layout.u.mask;
        Channels = AVStream->codecpar->ch_layout.nb_channels;
        Bits = AVStream->codecpar->bits_per_coded_sample;

        // https://trac.ffmpeg.org/ticket/7321
        CodecIDOrig = CodecID;
        if (CodecID == AVCodecID.AV_CODEC_ID_MP2 && (SampleFormat == AVSampleFormat.AV_SAMPLE_FMT_FLTP || SampleFormat == AVSampleFormat.AV_SAMPLE_FMT_FLT))
            CodecID = AVCodecID.AV_CODEC_ID_MP3; // OR? st->codecpar->format = (int) AVSampleFormat.AV_SAMPLE_FMT_S16P;

        byte[] buf = new byte[50];
        fixed (byte* bufPtr = buf)
        {
            av_channel_layout_describe(&AVStream->codecpar->ch_layout, bufPtr, (ulong)buf.Length);
            ChannelLayoutStr = Utils.BytePtrToStringUTF8(bufPtr);
        }
    }
}
