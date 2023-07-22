// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using Bili.Copilot.Libs.Player.Controls;
using Bili.Copilot.Libs.Player.Core;
using Bili.Copilot.Libs.Player.Enums;
using Bili.Copilot.Libs.Player.MediaFramework.MediaContext;
using Bili.Copilot.Libs.Player.MediaFramework.MediaDecoder;
using Bili.Copilot.Libs.Player.MediaFramework.MediaDemuxer;
using Bili.Copilot.Libs.Player.MediaFramework.MediaFrame;
using Bili.Copilot.Libs.Player.MediaFramework.MediaPlaylist;
using Bili.Copilot.Libs.Player.MediaFramework.MediaRenderer;
using Bili.Copilot.Libs.Player.Misc;
using Bili.Copilot.Libs.Player.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using static Bili.Copilot.Libs.Player.Utils;

namespace Bili.Copilot.Libs.Player.MediaPlayer;

/// <summary>
/// 玩家类，表示一个播放器.
/// </summary>
public partial class Player
{
    private readonly ConcurrentStack<SeekData> _seeks = new();
    private readonly ConcurrentQueue<Action> _uiActions = new();

    private readonly object _lockActions = new();
    private readonly object _lockSubtitles = new();
    private readonly string _playerSessionTag = "_session";
    private readonly Stopwatch _sw = new();

    private readonly ConcurrentStack<OpenAsyncData> _openInputs = new();
    private readonly ConcurrentStack<OpenAsyncData> _openSessions = new();
    private readonly ConcurrentStack<OpenAsyncData> _openItems = new();
    private readonly ConcurrentStack<OpenAsyncData> _openVideo = new();
    private readonly ConcurrentStack<OpenAsyncData> _openAudio = new();
    private readonly ConcurrentStack<OpenAsyncData> _openSubtitles = new();

    private bool _stoppedWithError;
    private bool _taskSeekRuns;
    private bool _taskPlayRuns;
    private bool _taskOpenAsyncRuns;
    private bool _isAudioSwitch;
    private bool _isSubsSwitch;
    private bool _reversePlaybackResync;
    private bool _isVideoSwitch;
    private long _curTime;
    private bool _reversePlayback;
    private double _speed = 1;
    private bool _isRecording;
    private VideoFrame _videoFrame;

    private long _onBufferingStarted;
    private long _onBufferingCompleted;

    private int _vDistanceMs;
    private int _aDistanceMs;
    private int _sDistanceMs;
    private int _sleepMs;

    private long _elapsedTicks;
    private long _elapsedSec;
    private long _startTicks;

    private int _allowedLateAudioDrops;
    private long _lastSpeedChangeTicks;
    private long _curLatency;
    private long _curAudioDeviceDelay;

    /// <summary>
    /// 媒体传输控件.
    /// </summary>
    [ObservableProperty]
    private IMediaTransportControls _transportControls;

    /// <summary>
    /// 播放器的状态.
    /// </summary>
    [ObservableProperty]
    private PlayerStatus _status = PlayerStatus.Stopped;

    /// <summary>
    /// 获取或设置一个值，指示播放器的状态是否能够接受播放命令.
    /// </summary>
    [ObservableProperty]
    private bool _canPlay;

    /// <summary>
    /// 获取或设置一个对象，表示播放器的标签.
    /// </summary>
    [ObservableProperty]
    private object _tag;

    /// <summary>
    /// 获取或设置最后的错误信息.
    /// </summary>
    [ObservableProperty]
    private string _lastError;

    /// <summary>
    /// 输入的持续时间.
    /// </summary>
    [ObservableProperty]
    private long _duration;

    /// <summary>
    /// 总比特率（单位：Kbps）.
    /// </summary>
    [ObservableProperty]
    private double _bitRate;

    /// <summary>
    /// 解复用器中当前缓冲的持续时间.
    /// </summary>
    [ObservableProperty]
    private long _bufferedDuration;

    /// <summary>
    /// 输入是否为实时流（实时流的持续时间可能不为0，以允许实时定位，例如HLS）.
    /// </summary>
    [ObservableProperty]
    private bool _isLive;

    /// <summary>
    /// 获取或设置一个值，指示播放器是否已被释放.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// 播放器的活动状态（空闲/活动/全活动）.
    /// </summary>
    public Activity Activity { get; private set; }

    /// <summary>
    /// 播放列表.
    /// </summary>
    public Playlist Playlist => Decoder.Playlist;

    /// <summary>
    /// 播放器的音频.
    /// </summary>
    public Audio Audio { get; private set; }

    /// <summary>
    /// 播放器的视频.
    /// </summary>
    public Video Video { get; private set; }

    /// <summary>
    /// 播放器的字幕.
    /// </summary>
    public Subtitles Subtitles { get; private set; }

    /// <summary>
    /// 播放器的渲染器.
    /// </summary>
    public Renderer Renderer => Decoder.VideoDecoder.Renderer;

    /// <summary>
    /// 播放器的解码器上下文.
    /// </summary>
    public DecoderContext Decoder { get; private set; }

    /// <summary>
    /// 音频解码器.
    /// </summary>
    public AudioDecoder AudioDecoder => Decoder.AudioDecoder;

    /// <summary>
    /// 视频解码器.
    /// </summary>
    public VideoDecoder VideoDecoder => Decoder.VideoDecoder;

    /// <summary>
    /// 字幕解码器.
    /// </summary>
    public SubtitlesDecoder SubtitlesDecoder => Decoder.SubtitlesDecoder;

    /// <summary>
    /// 主解复用器（如果视频被禁用或仅有音频，则可以是音频解复用器而不是视频解复用器）.
    /// </summary>
    public Demuxer MainDemuxer => Decoder.MainDemuxer;

    /// <summary>
    /// 音频解复用器.
    /// </summary>
    public Demuxer AudioDemuxer => Decoder.AudioDemuxer;

    /// <summary>
    /// 视频解复用器.
    /// </summary>
    public Demuxer VideoDemuxer => Decoder.VideoDemuxer;

    /// <summary>
    /// 字幕解复用器.
    /// </summary>
    public Demuxer SubtitlesDemuxer => Decoder.SubtitlesDemuxer;

    /// <summary>
    /// 播放器的递增唯一标识符.
    /// </summary>
    public int PlayerId { get; private set; }

    /// <summary>
    /// 播放器的配置（在构造函数中设置一次）.
    /// </summary>
    public Config Config { get; private set; }

    /// <summary>
    /// 获取一个值，指示播放器是否正在播放.
    /// </summary>
    public bool IsPlaying => Status == PlayerStatus.Playing;

    /// <summary>
    /// 章节列表.
    /// </summary>
    public List<Demuxer.Chapter> Chapters => VideoDemuxer?.Chapters;

    /// <summary>
    /// 播放器的当前时间或用户当前的搜索时间（根据 Config.Player.SeekAccurate 使用向后搜索或精确搜索）.
    /// </summary>
    /// <remarks>
    /// 注意：向前搜索会对某些格式造成问题，并且可能会有严重的延迟（但是，使用 H264 的 DASH，使用 VP9 的 DASH 可以正常工作）.
    /// </remarks>
    public long CurTime
    {
        get => _curTime;
        set
        {
            if (Config.Player.SeekAccurate)
            {
                SeekAccurate((int)(value / 10000));
            }
            else
            {
                Seek((int)(value / 10000), false);
            }
        }
    }

    /// <summary>
    /// 播放器是否正在录制.
    /// </summary>
    public bool IsRecording
    {
        get => Decoder != null && Decoder.IsRecording;
        private set
        {
            if (_isRecording == value)
            {
                return;
            }

            _isRecording = value;
            UI(() => SetProperty(ref _isRecording, value));
        }
    }

    /// <summary>
    /// X轴偏移量，用于改变X轴位置.
    /// </summary>
    public int PanXOffset
    {
        get => Renderer.PanXOffset;
        set
        {
            Renderer.PanXOffset = value;
            OnPropertyChanged(nameof(PanXOffset));
        }
    }

    /// <summary>
    /// Y轴偏移量，用于改变Y轴位置.
    /// </summary>
    public int PanYOffset
    {
        get => Renderer.PanYOffset;
        set
        {
            Renderer.PanYOffset = value;
            OnPropertyChanged(nameof(PanYOffset));
        }
    }

    /// <summary>
    /// 播放速度（x1 - x4）.
    /// </summary>
    public double Speed
    {
        get => _speed;
        set
        {
            var newValue = Math.Round(value, 3);
            if (value < 0.125)
            {
                newValue = 0.125;
            }
            else if (value > 16)
            {
                newValue = 16;
            }

            if (newValue == _speed || (newValue > 1 && ReversePlayback))
            {
                return;
            }

            AudioDecoder.Speed = newValue;
            VideoDecoder.Speed = newValue;
            _speed = newValue;
            Decoder.RequiresResync = true;
            RequiresBuffering = true;
            Subtitles.SubtitleText = string.Empty;

            UI(() =>
            {
                Subtitles.SubtitleText = Subtitles.SubtitleText;
                OnPropertyChanged(nameof(Speed));
            });
        }
    }

    /// <summary>
    /// 缩放百分比（100表示100%）.
    /// </summary>
    public int Zoom
    {
        get => (int)(Renderer.Zoom * 100);
        set
        {
            Renderer.SetZoom(Renderer.Zoom = value / 100.0);
            OnPropertyChanged(nameof(Zoom));
        }
    }

    /// <summary>
    /// 旋转角度（对于D3D11 VP，允许的值只有0、90、180、270）.
    /// </summary>
    public uint Rotation
    {
        get => Renderer.Rotation;
        set
        {
            Renderer.Rotation = value;
            OnPropertyChanged(nameof(Rotation));
        }
    }

    /// <summary>
    /// 是否使用反向播放模式.
    /// </summary>
    public bool ReversePlayback
    {
        get => _reversePlayback;

        set
        {
            if (_reversePlayback == value)
            {
                return;
            }

            _reversePlayback = value;
            UI(() => SetProperty(ref _reversePlayback, value));

            if (!Video.IsOpened || !CanPlay | IsLive)
            {
                return;
            }

            lock (_lockActions)
            {
                var shouldPlay = IsPlaying;
                Pause();
                SubtitleFrame = null;
                Subtitles.SubtitleText = string.Empty;
                if (!string.IsNullOrEmpty(Subtitles.SubtitleText))
                {
                    UI(() => Subtitles.SubtitleText = Subtitles.SubtitleText);
                }

                Decoder.StopThreads();
                Decoder.Flush();

                if (value)
                {
                    Speed = 1;
                    VideoDemuxer.EnableReversePlayback(CurTime);
                }
                else
                {
                    VideoDemuxer.DisableReversePlayback();

                    if (Status == PlayerStatus.Ended)
                    {
                        Status = PlayerStatus.Paused;
                    }

                    var vFrame = VideoDecoder.GetFrame(VideoDecoder.GetFrameNumber(CurTime));
                    VideoDecoder.DisposeFrame(vFrame);
                    vFrame = null;
                    Decoder.RequiresResync = true;
                }

                _reversePlaybackResync = false;
                if (shouldPlay)
                {
                    Play();
                }
            }
        }
    }

    internal AudioFrame AudioFrame { get; set; }
    internal SubtitleFrame SubtitleFrame { get; set; }
    internal SubtitleFrame SubtitleFramePrev { get; set; }
    internal PlayerStats Stats { get; set; } = new();
    internal LogHandler Log { get; set; }
    internal bool RequiresBuffering { get; set; }

    /// <summary>
    /// 媒体解码器是否已经结束.
    /// </summary>
    private bool DecoderHasEnded =>
        Decoder != null
        && (VideoDecoder.Status == ThreadStatus.Ended
            || (VideoDecoder.Disposed && AudioDecoder.Status == ThreadStatus.Ended));
}
