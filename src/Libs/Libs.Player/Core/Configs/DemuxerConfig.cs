// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using Bili.Copilot.Libs.Player.Enums;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.Libs.Player.Core.Configs;

/// <summary>
/// 解复用器配置类，用于配置解复用器的参数和选项.
/// </summary>
public sealed partial class DemuxerConfig : ObservableObject
{
    private MediaPlayer.Player _player;
    private Config _config;
    private long _bufferDuration = 30 * 1000L * 10000;

    /// <summary>
    /// 默认的视频解复用器格式选项.
    /// </summary>
    public static Dictionary<string, string> DefaultVideoFormatOpt { get; } = new()
    {
        // General
        { "probesize", (50 * 1024L * 1024).ToString() }, // (Bytes) Default 5MB | Higher for weird formats (such as .ts?) and 4K/Hevc
        { "analyzeduration", (10 * 1000L * 1000).ToString() }, // (Microseconds) Default 5 seconds | Higher for network streams

        // HTTP
        { "reconnect", "1" }, // 在 EOF 之前自动重新连接断开的连接
        { "reconnect_streamed", "1" }, // 自动重新连接流式/不可寻址的流
        { "reconnect_delay_max", "7" }, // 最大重新连接延迟（秒），超过此延迟将放弃
        { "user_agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36" },

        // RTSP
        { "rtsp_transport", "tcp" }, // UDP 似乎会引起问题
    };

    /// <summary>
    /// 是否允许在打开时执行 avformat_find_stream_info（避免这样做可以更快地打开输入，但可能会引起其他问题）.
    /// </summary>
    public bool AllowFindStreamInfo { get; set; } = true;

    /// <summary>
    /// 是否启用解复用器的自定义中断回调（用于超时和中断）.
    /// </summary>
    public bool AllowInterrupts { get; set; } = true;

    /// <summary>
    /// 是否允许在 av_read_frame 期间中断.
    /// </summary>
    public bool AllowReadInterrupts { get; set; } = true;

    /// <summary>
    /// 是否允许在中断回调中进行超时检查.
    /// </summary>
    public bool AllowTimeouts { get; set; } = true;

    /// <summary>
    /// 要从中断中排除的 FFmpeg 格式列表.
    /// </summary>
    public List<string> ExcludeInterruptFormats { get; set; } = new() { "rtsp" };

    /// <summary>
    /// 缓冲的最大持续时间（以 ticks 为单位）.
    /// </summary>
    public long BufferDuration
    {
        get => _bufferDuration;
        set
        {
            if (!SetProperty(ref _bufferDuration, value))
            {
                return;
            }

            if (_config != null && value < _config.Player.MinBufferDuration)
            {
                _config.Player.MinBufferDuration = value;
            }
        }
    }

    /// <summary>
    /// 缓冲的最大数据包数（与 BufferDuration 一起作为额外的检查）.
    /// </summary>
    public long BufferPackets { get; set; }

    /// <summary>
    /// 最大允许的音频数据包数（当达到此数目时，将丢弃额外的数据包并触发 AudioLimit 事件）.
    /// </summary>
    public long MaxAudioPackets { get; set; }

    /// <summary>
    /// 停止之前允许的最大错误数.
    /// </summary>
    public int MaxErrors { get; set; } = 30;

    /// <summary>
    /// AVIO 上下文的自定义 IO 流缓冲区大小（以字节为单位）.
    /// </summary>
    public int IOStreamBufferSize { get; set; } = 0x200000;

    /// <summary>
    /// avformat_close_input 的超时时间（以 ticks 为单位），适用于支持中断的协议.
    /// </summary>
    public long CloseTimeout { get; set; } = 1 * 1000 * 10000;

    /// <summary>
    /// avformat_open_input + avformat_find_stream_info 的超时时间（以 ticks 为单位），适用于支持中断的协议（应与 probesize/analyzeduration 相关）.
    /// </summary>
    public long OpenTimeout { get; set; } = 5 * 60 * 1000L * 10000;

    /// <summary>
    /// av_read_frame 的超时时间（以 ticks 为单位），适用于支持中断的协议.
    /// </summary>
    public long ReadTimeout { get; set; } = 10 * 1000 * 10000;

    /// <summary>
    /// av_seek_frame 的超时时间（以 ticks 为单位），适用于支持中断的协议.
    /// </summary>
    public long SeekTimeout { get; set; } = 8 * 1000 * 10000;

    /// <summary>
    /// 强制指定输入格式.
    /// </summary>
    public string ForceFormat { get; set; }

    /// <summary>
    /// 解复用器的格式标志（参见 https://ffmpeg.org/doxygen/trunk/avformat_8h.html）.
    /// 例如：FormatFlags |= 0x40; // 对于 AVFMT_FLAG_NOBUFFER.
    /// </summary>
    public int FormatFlags { get; set; } = FFmpeg.AutoGen.ffmpeg.AVFMT_FLAG_DISCARD_CORRUPT;

    /// <summary>
    /// 某些复用器和解复用器进行嵌套（它们打开一个或多个额外的内部格式上下文）.这将将 FormatOpt 传递给底层上下文.
    /// </summary>
    public bool FormatOptToUnderlying { get; set; }

    /// <summary>
    /// 解复用器的格式选项.
    /// </summary>
    public Dictionary<string, string> FormatOpt { get; set; } = DefaultVideoFormatOpt;

    /// <summary>
    /// 音频解复用器的格式选项.
    /// </summary>
    public Dictionary<string, string> AudioFormatOpt { get; set; } = DefaultVideoFormatOpt;

    /// <summary>
    /// 字幕解复用器的格式选项.
    /// </summary>
    public Dictionary<string, string> SubtitlesFormatOpt { get; set; } = DefaultVideoFormatOpt;

    /// <summary>
    /// 获取指定媒体类型的格式选项.
    /// </summary>
    /// <param name="type">媒体类型.</param>
    /// <returns>格式选项.</returns>
    public Dictionary<string, string> GetFormatOptPtr(MediaType type)
        => type == MediaType.Video ? FormatOpt : type == MediaType.Audio ? AudioFormatOpt : SubtitlesFormatOpt;

    /// <summary>
    /// 克隆解复用器配置.
    /// </summary>
    /// <returns>克隆后的解复用器配置.</returns>
    public DemuxerConfig Clone()
    {
        var demuxer = (DemuxerConfig)MemberwiseClone();

        demuxer.FormatOpt = new Dictionary<string, string>();
        demuxer.AudioFormatOpt = new Dictionary<string, string>();
        demuxer.SubtitlesFormatOpt = new Dictionary<string, string>();

        foreach (var kv in FormatOpt)
        {
            demuxer.FormatOpt.Add(kv.Key, kv.Value);
        }

        foreach (var kv in AudioFormatOpt)
        {
            demuxer.AudioFormatOpt.Add(kv.Key, kv.Value);
        }

        foreach (var kv in SubtitlesFormatOpt)
        {
            demuxer.SubtitlesFormatOpt.Add(kv.Key, kv.Value);
        }

        demuxer._player = null;
        demuxer._config = null;

        return demuxer;
    }

    internal void SetConfig(Config config)
        => _config = config;

    internal void SetPlayer(MediaPlayer.Player player)
        => _player = player;
}
