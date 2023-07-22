// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using Bili.Copilot.Libs.Player.Core;
using Bili.Copilot.Libs.Player.Enums;
using Bili.Copilot.Libs.Player.MediaFramework.MediaContext;
using Bili.Copilot.Libs.Player.MediaFramework.MediaStream;
using Bili.Copilot.Libs.Player.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.Libs.Player.MediaPlayer;

/// <summary>
/// 视频类，实现了可观察对象接口.
/// </summary>
public partial class Video : ObservableObject
{
    private readonly Player _player;

    /// <summary>
    /// 输入是否有视频并且已配置.
    /// </summary>
    [ObservableProperty]
    private bool _isOpened;

    [ObservableProperty]
    private string _codec;

    [ObservableProperty]
    private double _bitRate;

    [ObservableProperty]
    private AspectRatio _aspectRatio;

    [ObservableProperty]
    private int _framesDropped;

    [ObservableProperty]
    private int _framesDisplayed;

    [ObservableProperty]
    private double _fps;

    [ObservableProperty]
    private double _fpsCurrent;

    [ObservableProperty]
    private string _pixelFormat;

    [ObservableProperty]
    private int _width;

    [ObservableProperty]
    private int _height;

    [ObservableProperty]
    private bool _videoAcceleration;

    [ObservableProperty]
    private bool _zeroCopy;

    /// <summary>
    /// 总帧数.
    /// </summary>
    [ObservableProperty]
    private int _framesTotal;

    /// <summary>
    /// 构造函数，初始化视频类的实例.
    /// </summary>
    /// <param name="player">播放器对象.</param>
    public Video(Player player) => _player = player;

    /// <summary>
    /// 嵌入的流.
    /// </summary>
    public ObservableCollection<VideoStream> Streams => Decoder?.VideoDemuxer.VideoStreams;

    internal DecoderContext Decoder => _player.Decoder;
    internal Config Config => _player.Config;

    /// <summary>
    /// 切换视频状态.
    /// </summary>
    public void Toggle() => Config.Video.Enabled = !Config.Video.Enabled;

    /// <summary>
    /// 切换保持纵横比状态.
    /// </summary>
    public void ToggleKeepRatio()
    {
        if (Config.Video.AspectRatio == AspectRatio.Keep)
        {
            Config.Video.AspectRatio = AspectRatio.Fill;
        }
        else if (Config.Video.AspectRatio == AspectRatio.Fill)
        {
            Config.Video.AspectRatio = AspectRatio.Keep;
        }
    }

    /// <summary>
    /// 切换视频加速状态.
    /// </summary>
    public void ToggleVideoAcceleration()
        => Config.Video.VideoAcceleration = !Config.Video.VideoAcceleration;

    /// <summary>
    /// 重置视频属性.
    /// </summary>
    internal void Reset()
    {
        Codec = null;
        AspectRatio = new AspectRatio(0, 0);
        BitRate = 0;
        Fps = 0;
        PixelFormat = null;
        Width = 0;
        Height = 0;
        FramesTotal = 0;
        VideoAcceleration = false;
        ZeroCopy = false;
        IsOpened = false;
    }

    /// <summary>
    /// 刷新视频属性.
    /// </summary>
    internal void Refresh()
    {
        if (Decoder.VideoStream == null)
        {
            Reset();
            return;
        }

        Codec = Decoder.VideoStream.Codec;
        AspectRatio = Decoder.VideoStream.AspectRatio;
        Fps = Decoder.VideoStream.FPS;
        PixelFormat = Decoder.VideoStream.PixelFormatStr;
        Width = Decoder.VideoStream.Width;
        Height = Decoder.VideoStream.Height;
        FramesTotal = Decoder.VideoStream.TotalFrames;
        VideoAcceleration = Decoder.VideoDecoder.VideoAccelerated;
        ZeroCopy = Decoder.VideoDecoder.ZeroCopy;
        IsOpened = !Decoder.VideoDecoder.Disposed;
        FramesDisplayed = 0;
        FramesDropped = 0;
    }

    /// <summary>
    /// 启用视频.
    /// </summary>
    internal void Enable()
    {
        if (_player.VideoDemuxer.Disposed || Config.Player.Usage == PlayerUsage.Audio)
        {
            return;
        }

        var wasPlaying = _player.IsPlaying;
        _player.Pause();
        Decoder.OpenSuggestedVideo();
        _player.ReSync(Decoder.VideoStream, (int)(_player.CurTime / 10000), true);

        if (wasPlaying || Config.Player.AutoPlay)
        {
            _player.Play();
        }
    }

    /// <summary>
    /// 禁用视频.
    /// </summary>
    internal void Disable()
    {
        if (!IsOpened)
        {
            return;
        }

        var wasPlaying = _player.IsPlaying;

        _player.Pause();
        Decoder.CloseVideo();
        _player.Subtitles.SubtitleText = string.Empty;
        _player.UIAdd(() => _player.Subtitles.SubtitleText = _player.Subtitles.SubtitleText);

        if (!_player.Audio.IsOpened)
        {
            _player.CanPlay = false;
            _player.UIAdd(() => _player.CanPlay = _player.CanPlay);
        }

        Reset();
        _player.UIAll();

        if (wasPlaying || Config.Player.AutoPlay)
        {
            _player.Play();
        }
    }
}
