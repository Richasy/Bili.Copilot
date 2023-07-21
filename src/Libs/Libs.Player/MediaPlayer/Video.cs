using System;
using System.Collections.ObjectModel;

using Bili.Copilot.Libs.Player.MediaFramework.MediaContext;
using Bili.Copilot.Libs.Player.MediaFramework.MediaStream;

namespace Bili.Copilot.Libs.Player.MediaPlayer;

public class Video : ObservableObject
{
    /// <summary>
    /// Embedded Streams
    /// </summary>
    public ObservableCollection<VideoStream>
                        Streams         => decoder?.VideoDemuxer.VideoStreams;

    /// <summary>
    /// Whether the input has video and it is configured
    /// </summary>
    public bool         IsOpened        { get => isOpened;          internal set => Set(ref _IsOpened, value); }
    internal bool   _IsOpened, isOpened;

    public string       Codec           { get => codec;             internal set => Set(ref _Codec, value); }
    internal string _Codec, codec;

    ///// <summary>
    ///// Video bitrate (Kbps)
    ///// </summary>
    public double       BitRate         { get => bitRate;           internal set => Set(ref _BitRate, value); }
    internal double _BitRate, bitRate;

    public AspectRatio  AspectRatio     { get => aspectRatio;       internal set => Set(ref _AspectRatio, value); }
    internal AspectRatio 
                    _AspectRatio, aspectRatio;

    ///// <summary>
    ///// Total Dropped Frames
    ///// </summary>
    public int          FramesDropped   { get => framesDropped;     internal set => Set(ref _FramesDropped, value); }
    internal int    _FramesDropped, framesDropped;

    /// <summary>
    /// Total Frames
    /// </summary>
    public int          FramesTotal     { get => framesTotal;       internal set => Set(ref _FramesTotal, value); }
    internal int    _FramesTotal, framesTotal;

    public int          FramesDisplayed { get => framesDisplayed;   internal set => Set(ref _FramesDisplayed, value); }
    internal int    _FramesDisplayed, framesDisplayed;

    public double       FPS             { get => fps;               internal set => Set(ref _FPS, value); }
    internal double _FPS, fps;

    /// <summary>
    /// Actual Frames rendered per second (FPS)
    /// </summary>
    public double       FPSCurrent      { get => fpsCurrent;        internal set => Set(ref _FPSCurrent, value); }
    internal double _FPSCurrent, fpsCurrent;

    public string       PixelFormat     { get => pixelFormat;       internal set => Set(ref _PixelFormat, value); }
    internal string _PixelFormat, pixelFormat;

    public int          Width           { get => width;             internal set => Set(ref _Width, value); }
    internal int    _Width, width;

    public int          Height          { get => height;            internal set => Set(ref _Height, value); }
    internal int    _Height, height;

    public bool         VideoAcceleration
                                                { get => videoAcceleration; internal set => Set(ref _VideoAcceleration, value); }
    internal bool   _VideoAcceleration, videoAcceleration;

    public bool         ZeroCopy        { get => zeroCopy;          internal set => Set(ref _ZeroCopy, value); }
    internal bool   _ZeroCopy, zeroCopy;

    Action uiAction;
    Player player;
    DecoderContext decoder => player.decoder;
    Config Config => player.Config;

    public Video(Player player)
    {
        this.player = player;

        uiAction = () =>
        {
            IsOpened            = IsOpened;
            Codec               = Codec;
            AspectRatio         = AspectRatio;
            FramesTotal         = FramesTotal;
            FPS                 = FPS;
            PixelFormat         = PixelFormat;
            Width               = Width;
            Height              = Height;
            VideoAcceleration   = VideoAcceleration;
            ZeroCopy            = ZeroCopy;

            FramesDisplayed     = FramesDisplayed;
            FramesDropped       = FramesDropped;
        };
    }

    internal void Reset()
    {
        codec              = null;
        aspectRatio        = new AspectRatio(0, 0);
        bitRate            = 0;
        fps                = 0;
        pixelFormat        = null;
        width              = 0;
        height             = 0;
        framesTotal        = 0;
        videoAcceleration  = false;
        zeroCopy           = false;
        isOpened           = false;

        player.UIAdd(uiAction);
    }
    internal void Refresh()
    {
        if (decoder.VideoStream == null) { Reset(); return; }

        codec       = decoder.VideoStream.Codec;
        aspectRatio = decoder.VideoStream.AspectRatio;
        fps         = decoder.VideoStream.FPS;
        pixelFormat = decoder.VideoStream.PixelFormatStr;
        width       = decoder.VideoStream.Width;
        height      = decoder.VideoStream.Height;
        framesTotal = decoder.VideoStream.TotalFrames;
        videoAcceleration
                    = decoder.VideoDecoder.VideoAccelerated;
        zeroCopy    = decoder.VideoDecoder.ZeroCopy;
        isOpened    =!decoder.VideoDecoder.Disposed;

        framesDisplayed = 0;
        framesDropped   = 0;

        player.UIAdd(uiAction);
    }

    internal void Enable()
    {
        if (player.VideoDemuxer.Disposed || Config.Player.Usage == Usage.Audio)
            return;

        bool wasPlaying = player.IsPlaying;

        player.Pause();
        decoder.OpenSuggestedVideo();
        player.ReSync(decoder.VideoStream, (int) (player.CurTime / 10000), true);

        if (wasPlaying || Config.Player.AutoPlay)
            player.Play();
    }
    internal void Disable()
    {
        if (!IsOpened)
            return;

        bool wasPlaying = player.IsPlaying;

        player.Pause();
        decoder.CloseVideo();
        player.Subtitles._subtitleText = "";
        player.UIAdd(() => player.Subtitles.SubtitleText = player.Subtitles.SubtitleText);

        if (!player.Audio.IsOpened)
        {
            player.canPlay = false;
            player.UIAdd(() => player.CanPlay = player.CanPlay);
        }

        Reset();
        player.UIAll();

        if (wasPlaying || Config.Player.AutoPlay)
            player.Play();
    }

    public void Toggle() => Config.Video.Enabled = !Config.Video.Enabled;
    public void ToggleKeepRatio()
    {
        if (Config.Video.AspectRatio == AspectRatio.Keep)
            Config.Video.AspectRatio = AspectRatio.Fill;
        else if (Config.Video.AspectRatio == AspectRatio.Fill)
            Config.Video.AspectRatio = AspectRatio.Keep;
    }
    public void ToggleVideoAcceleration() => Config.Video.VideoAcceleration = !Config.Video.VideoAcceleration;
}
