// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Libs.Player.Enums;
using Bili.Copilot.Libs.Player.MediaFramework.MediaDecoder;
using Bili.Copilot.Libs.Player.MediaFramework.MediaDemuxer;
using Bili.Copilot.Libs.Player.MediaFramework.MediaRemuxer;
using Bili.Copilot.Libs.Player.MediaFramework.MediaStream;
using Bili.Copilot.Libs.Player.Misc;

namespace Bili.Copilot.Libs.Player.MediaFramework.MediaContext;

/// <summary>
/// 解码器上下文类.
/// </summary>
public partial class DecoderContext
{
    private bool _shouldDispose;
    private int _oldMaxAudioFrames;
    private bool _recHasVideo;

    /// <summary>
    /// 录制完成时触发.
    /// </summary>
    public event EventHandler RecordingCompleted;

    /// <summary>
    /// 是否正在录制.
    /// </summary>
    public bool IsRecording => VideoDecoder.isRecording || AudioDecoder.isRecording;

    /// <summary>
    /// 上层对象（例如播放器、下载器），主要用于插件访问.
    /// </summary>
    public object Tag { get; set; }

    /// <summary>
    /// 是否启用解码.
    /// </summary>
    public bool EnableDecoding { get; set; }

    /// <summary>
    /// 中断标志.
    /// </summary>
    /// <remarks>
    /// 设置为 true 时，会强制中断视频、音频和字幕的解复用器.
    /// 设置为 false 时，会取消中断视频、音频和字幕的解复用器.
    /// </remarks>
    public new bool Interrupt
    {
        get => base.Interrupt;
        set
        {
            base.Interrupt = value;

            if (value)
            {
                VideoDemuxer.Interrupter.ForceInterrupt = 1;
                AudioDemuxer.Interrupter.ForceInterrupt = 1;
                SubtitlesDemuxer.Interrupter.ForceInterrupt = 1;
            }
            else
            {
                VideoDemuxer.Interrupter.ForceInterrupt = 0;
                AudioDemuxer.Interrupter.ForceInterrupt = 0;
                SubtitlesDemuxer.Interrupter.ForceInterrupt = 0;
            }
        }
    }

    /// <summary>
    /// 是否需要重新同步.
    /// </summary>
    /// <remarks>
    /// 设置为 true 时，需要手动调用 ReSync() 方法进行重新同步.
    /// </remarks>
    public bool RequiresResync { get; set; }

    /// <summary>
    /// 扩展名.
    /// </summary>
    public string Extension => VideoDemuxer.Disposed ? AudioDemuxer.Extension : VideoDemuxer.Extension;

    /// <summary>
    /// 主要的分离器.
    /// </summary>
    public Demuxer MainDemuxer { get; private set; }

    /// <summary>
    /// 音频分离器.
    /// </summary>
    public Demuxer AudioDemuxer { get; private set; }

    /// <summary>
    /// 视频分离器.
    /// </summary>
    public Demuxer VideoDemuxer { get; private set; }

    /// <summary>
    /// 字幕分离器.
    /// </summary>
    public Demuxer SubtitlesDemuxer { get; private set; }

    /// <summary>
    /// 音频解码器.
    /// </summary>
    public AudioDecoder AudioDecoder { get; private set; }

    /// <summary>
    /// 视频解码器.
    /// </summary>
    public VideoDecoder VideoDecoder { get; internal set; }

    /// <summary>
    /// 字幕解码器.
    /// </summary>
    public SubtitlesDecoder SubtitlesDecoder { get; private set; }

    /// <summary>
    /// 获取音频流.如果视频解复用器存在音频流，则返回该音频流；否则返回音频解复用器的音频流.
    /// </summary>
    public AudioStream AudioStream => VideoDemuxer?.AudioStream ?? AudioDemuxer.AudioStream;

    /// <summary>
    /// 获取视频流.如果视频解复用器存在视频流，则返回该视频流.
    /// </summary>
    public VideoStream VideoStream => VideoDemuxer?.VideoStream;

    /// <summary>
    /// 获取字幕流.如果视频解复用器存在字幕流，则返回该字幕流；否则返回字幕解复用器的字幕流.
    /// </summary>
    public SubtitlesStream SubtitlesStream => VideoDemuxer?.SubtitlesStream ?? SubtitlesDemuxer.SubtitlesStream;

    /// <summary>
    /// 已关闭的音频流.
    /// </summary>
    public Tuple<ExternalAudioStream, int> ClosedAudioStream { get; private set; }

    /// <summary>
    /// 已关闭的视频流.
    /// </summary>
    public Tuple<ExternalVideoStream, int> ClosedVideoStream { get; private set; }

    /// <summary>
    /// 已关闭的字幕流.
    /// </summary>
    public Tuple<ExternalSubtitleStream, int> ClosedSubtitlesStream { get; private set; }

    /// <summary>
    /// 日志.
    /// </summary>
    internal LogHandler Log { get; set; }

    internal Remuxer Recorder { get; set; }

    /// <summary>
    /// 获取指定类型的解复用器指针.
    /// </summary>
    /// <param name="type">媒体类型.</param>
    /// <returns>解复用器指针.</returns>
    public Demuxer GetDemuxerPtr(MediaType type) => type == MediaType.Audio ? AudioDemuxer : (type == MediaType.Video ? VideoDemuxer : SubtitlesDemuxer);

    /// <summary>
    /// 获取指定类型的解码器指针.
    /// </summary>
    /// <param name="type">媒体类型.</param>
    /// <returns>解码器指针.</returns>
    public DecoderBase GetDecoderPtr(MediaType type) => type == MediaType.Audio ? AudioDecoder : (type == MediaType.Video ? VideoDecoder : SubtitlesDecoder);
}
