// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using Bili.Copilot.Libs.Player.Enums;
using FFmpeg.AutoGen;
using Windows.Globalization;
using static FFmpeg.AutoGen.ffmpeg;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaStream;

/// <summary>
/// 基流.
/// </summary>
public abstract unsafe class StreamBase
{
    /// <summary>
    /// 初始化 <see cref="StreamBase"/> 类的新实例.
    /// </summary>
    public StreamBase()
    {
    }

    /// <summary>
    /// 使用指定的解复用器和 AV 流初始化 <see cref="StreamBase"/> 类的新实例.
    /// </summary>
    /// <param name="demuxer">解复用器.</param>
    /// <param name="st">AV 流.</param>
    public StreamBase(Demuxer demuxer, AVStream* st)
    {
        Demuxer = demuxer;
        AVStream = st;
    }

    /// <summary>
    /// 获取或设置外部流.
    /// </summary>
    public ExternalStream ExternalStream { get; set; }

    /// <summary>
    /// 获取或设置解复用器.
    /// </summary>
    public Demuxer Demuxer { get; internal set; }

    /// <summary>
    /// 获取或设置 AV 流.
    /// </summary>
    public AVStream* AVStream { get; internal set; }

    /// <summary>
    /// HLS 播放列表.
    /// </summary>
    public HLSPlaylist* HlsPlaylist { get; set; }

    /// <summary>
    /// 获取或设置流索引.
    /// </summary>
    public int StreamIndex { get; internal set; } = -1;

    /// <summary>
    /// 获取或设置时间基.
    /// </summary>
    public double TimeBase { get; internal set; }

    /// <summary>
    /// 获取或设置是否启用.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// 获取或设置比特率.
    /// </summary>
    public long BitRate { get; internal set; }

    /// <summary>
    /// 获取或设置语言.
    /// </summary>
    public Language Language { get; internal set; }

    /// <summary>
    /// 获取或设置标题.
    /// </summary>
    public string Title { get; internal set; }

    /// <summary>
    /// 获取或设置编解码器.
    /// </summary>
    public string Codec { get; internal set; }

    /// <summary>
    /// 获取或设置编解码器 ID.
    /// </summary>
    public AVCodecID CodecId { get; internal set; }

    /// <summary>
    /// 获取或设置起始时间.
    /// </summary>
    public long StartTime { get; internal set; }

    /// <summary>
    /// 获取或设置起始时间（以时间戳表示）.
    /// </summary>
    public long StartTimePts { get; internal set; }

    /// <summary>
    /// 获取或设置持续时间.
    /// </summary>
    public long Duration { get; internal set; }

    /// <summary>
    /// 获取或设置元数据.
    /// </summary>
    public Dictionary<string, string> Metadata { get; internal set; } = new Dictionary<string, string>();

    /// <summary>
    /// 获取或设置媒体类型.
    /// </summary>
    public MediaType Type { get; internal set; }

    /// <summary>
    /// 获取转储信息.
    /// </summary>
    /// <returns>转储信息.</returns>
    public abstract string GetDump();

    /// <summary>
    /// 刷新流信息.
    /// </summary>
    public virtual void Refresh()
    {
        BitRate = AVStream->codecpar->bit_rate;
        CodecId = AVStream->codecpar->codec_id;
        Codec = avcodec_get_name(AVStream->codecpar->codec_id);
        StreamIndex = AVStream->index;
        TimeBase = av_q2d(AVStream->time_base) * 10000.0 * 1000.0;
        StartTime = AVStream->start_time != AV_NOPTS_VALUE && Demuxer.hlsCtx == null ? (long)(AVStream->start_time * TimeBase) : Demuxer.StartTime;
        StartTimePts = AVStream->start_time != AV_NOPTS_VALUE ? AVStream->start_time : av_rescale_q(StartTime / 10, av_get_time_base_q(), AVStream->time_base);
        Duration = AVStream->duration != AV_NOPTS_VALUE ? (long)(AVStream->duration * TimeBase) : Demuxer.Duration;
        Type = this is VideoStream ? MediaType.Video : (this is AudioStream ? MediaType.Audio : MediaType.Subtitle);

        if (Demuxer.hlsCtx != null)
        {
            for (var i = 0; i < Demuxer.hlsCtx->n_playlists; i++)
            {
                var playlists = (HLSPlaylist**)Demuxer.hlsCtx->playlists;
                for (var l = 0; l < playlists[i]->n_main_streams; l++)
                {
                    if (playlists[i]->main_streams[l]->index == StreamIndex)
                    {
                        Demuxer.Log.Debug($"Stream #{StreamIndex} Found in playlist {i}");
                        HlsPlaylist = playlists[i];
                        break;
                    }
                }
            }
        }

        Metadata.Clear();

        AVDictionaryEntry* b = null;
        while (true)
        {
            b = av_dict_get(AVStream->metadata, string.Empty, b, AV_DICT_IGNORE_SUFFIX);
            if (b == null)
            {
                break;
            }

            Metadata.Add(Utils.BytePtrToStringUtf8(b->key), Utils.BytePtrToStringUtf8(b->value));
        }

        foreach (var kv in Metadata)
        {
            var keyLower = kv.Key.ToLower();

            if (Language == null && (keyLower == "language" || keyLower == "lang"))
            {
                Language = Language.Get(kv.Value);
            }
            else if (keyLower == "title")
            {
                Title = kv.Value;
            }
        }

        Language ??= Language.Unknown;
    }
}
}
