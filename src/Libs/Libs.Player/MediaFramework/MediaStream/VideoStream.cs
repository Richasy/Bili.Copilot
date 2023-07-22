// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Libs.Player.Enums;
using Bili.Copilot.Libs.Player.MediaFramework.MediaDemuxer;
using Bili.Copilot.Libs.Player.Models;
using FFmpeg.AutoGen;
using static FFmpeg.AutoGen.ffmpeg;
using static FFmpeg.AutoGen.FFmpegEx;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaStream;

/// <summary>
/// 表示视频流的类.
/// </summary>
public unsafe class VideoStream : StreamBase
{
    /// <summary>
    /// 初始化 VideoStream 类的新实例.
    /// </summary>
    public VideoStream()
    {
    }

    /// <summary>
    /// 使用指定的解复用器和 AVStream 指针初始化 VideoStream 类的新实例.
    /// </summary>
    /// <param name="demuxer">解复用器.</param>
    /// <param name="st">AVStream 指针.</param>
    public VideoStream(Demuxer demuxer, AVStream* st)
        : base(demuxer, st)
    {
        Demuxer = demuxer;
        AVStream = st;
        Refresh();
    }

    /// <summary>
    /// 获取或设置视频的宽高比.
    /// </summary>
    public AspectRatio AspectRatio { get; set; }

    /// <summary>
    /// 获取或设置视频的色彩范围.
    /// </summary>
    public ColorRange ColorRange { get; set; }

    /// <summary>
    /// 获取或设置视频的色彩空间.
    /// </summary>
    public ColorSpace ColorSpace { get; set; }

    /// <summary>
    /// 获取或设置视频的色彩转换特性.
    /// </summary>
    public AVColorTransferCharacteristic ColorTransfer { get; set; }

    /// <summary>
    /// 获取或设置视频的旋转角度.
    /// </summary>
    public double Rotation { get; set; }

    /// <summary>
    /// 获取或设置视频的帧率.
    /// </summary>
    public double FPS { get; set; }

    /// <summary>
    /// 获取或设置视频的帧时长.
    /// </summary>
    public long FrameDuration { get; set; }

    /// <summary>
    /// 获取或设置视频的高度.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// 获取或设置视频是否为 RGB 格式.
    /// </summary>
    public bool IsRGB { get; set; }

    /// <summary>
    /// 获取或设置视频的像素组件描述符数组.
    /// </summary>
    public AVComponentDescriptor[] PixelComps { get; set; }

    /// <summary>
    /// 获取或设置视频的像素组件 0 的深度.
    /// </summary>
    public int PixelComp0Depth { get; set; }

    /// <summary>
    /// 获取或设置视频的像素格式.
    /// </summary>
    public AVPixelFormat PixelFormat { get; set; }

    /// <summary>
    /// 获取或设置视频的像素格式描述符.
    /// </summary>
    public AVPixFmtDescriptor* PixelFormatDesc { get; set; }

    /// <summary>
    /// 获取或设置视频的像素格式字符串.
    /// </summary>
    public string PixelFormatStr { get; set; }

    /// <summary>
    /// 获取或设置视频的像素平面数.
    /// </summary>
    public int PixelPlanes { get; set; }

    /// <summary>
    /// 获取或设置视频的像素深度是否相同.
    /// </summary>
    public bool PixelSameDepth { get; set; }

    /// <summary>
    /// 获取或设置视频的像素是否交错.
    /// </summary>
    public bool PixelInterleaved { get; set; }

    /// <summary>
    /// 获取或设置视频的总帧数.
    /// </summary>
    public int TotalFrames { get; set; }

    /// <summary>
    /// 获取或设置视频的宽度.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// 获取视频的详细信息.
    /// </summary>
    /// <returns>视频的详细信息.</returns>
    public override string GetDump()
        => $"[{Type} #{StreamIndex}] {Codec} {PixelFormatStr} {Width}x{Height} @ {FPS:#.###} | [Color: {ColorSpace}] [BR: {BitRate}] | {Utils.TicksToTime((long)(AVStream->start_time * TimeBase))}/{Utils.TicksToTime((long)(AVStream->duration * TimeBase))} | {Utils.TicksToTime(StartTime)}/{Utils.TicksToTime(Duration)}";

    /// <summary>
    /// 刷新视频流的属性.
    /// </summary>
    /// <param name="format">像素格式.</param>
    public void Refresh(AVPixelFormat format = AVPixelFormat.AV_PIX_FMT_NONE)
    {
        base.Refresh();

        PixelFormat = format == AVPixelFormat.AV_PIX_FMT_NONE ? (AVPixelFormat)AVStream->codecpar->format : format;
        PixelFormatStr = PixelFormat.ToString().Replace("AV_PIX_FMT_", string.Empty).ToLower();
        Width = AVStream->codecpar->width;
        Height = AVStream->codecpar->height;
        FPS = av_q2d(AVStream->avg_frame_rate) > 0 ? av_q2d(AVStream->avg_frame_rate) : av_q2d(AVStream->r_frame_rate);
        FrameDuration = FPS > 0 ? (long)(10000000 / FPS) : 0;
        TotalFrames = AVStream->duration > 0 && FrameDuration > 0 ? (int)(AVStream->duration * TimeBase / FrameDuration) : (FrameDuration > 0 ? (int)(Demuxer.Duration / FrameDuration) : 0);

        var gcd = Utils.GCD(Width, Height);
        if (gcd != 0)
        {
            AspectRatio = new AspectRatio(Width / gcd, Height / gcd);
        }

        if (PixelFormat != AVPixelFormat.AV_PIX_FMT_NONE)
        {
            ColorRange = AVStream->codecpar->color_range == AVColorRange.AVCOL_RANGE_JPEG ? ColorRange.Full : ColorRange.Limited;

            if (AVStream->codecpar->color_space == AVColorSpace.AVCOL_SPC_BT470BG)
            {
                ColorSpace = ColorSpace.BT601;
            }
            else if (AVStream->codecpar->color_space == AVColorSpace.AVCOL_SPC_BT709)
            {
                ColorSpace = ColorSpace.BT709;
            }
            else
            {
                ColorSpace = AVStream->codecpar->color_space == AVColorSpace.AVCOL_SPC_BT2020_CL || AVStream->codecpar->color_space == AVColorSpace.AVCOL_SPC_BT2020_NCL
                ? ColorSpace.BT2020
                : Height > 576 ? ColorSpace.BT709 : ColorSpace.BT601;
            }

            ColorTransfer = AVStream->codecpar->color_trc;

            Rotation = av_display_rotation_get(av_stream_get_side_data(AVStream, AVPacketSideDataType.AV_PKT_DATA_DISPLAYMATRIX, null));

            PixelFormatDesc = av_pix_fmt_desc_get(PixelFormat);
            var comps = PixelFormatDesc->comp.ToArray();
            PixelComps = new AVComponentDescriptor[PixelFormatDesc->nb_components];
            for (var i = 0; i < PixelComps.Length; i++)
            {
                PixelComps[i] = comps[i];
            }

            PixelInterleaved = PixelFormatDesc->log2_chroma_w != PixelFormatDesc->log2_chroma_h;
            IsRGB = (PixelFormatDesc->flags & AV_PIX_FMT_FLAG_RGB) != 0;

            PixelSameDepth = true;
            PixelPlanes = 0;
            if (PixelComps.Length > 0)
            {
                PixelComp0Depth = PixelComps[0].depth;
                var prevBit = PixelComp0Depth;
                for (var i = 0; i < PixelComps.Length; i++)
                {
                    if (PixelComps[i].plane > PixelPlanes)
                    {
                        PixelPlanes = PixelComps[i].plane;
                    }

                    if (prevBit != PixelComps[i].depth)
                    {
                        PixelSameDepth = false;
                    }
                }

                PixelPlanes++;
            }
        }
    }
}
