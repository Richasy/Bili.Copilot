// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using Bili.Copilot.Libs.Player.Core;
using Bili.Copilot.Libs.Player.Misc;
using Bili.Copilot.Libs.Player.Models;
using static Bili.Copilot.Libs.Player.Utils;

namespace Bili.Copilot.Libs.Player.MediaPlayer;

public unsafe sealed partial class Player : ObservableObject, IDisposable
{
    internal void UpdateCurTime()
    {
        lock (_seeks)
        {
            if (MainDemuxer == null || !_seeks.IsEmpty)
                return;

            if (MainDemuxer.IsHLSLive)
            {
                _curTime = MainDemuxer.CurTime; // *speed ?
                duration = MainDemuxer.Duration;
                Duration = Duration;
            }
        }

        Set(ref _curTime, curTime, true, nameof(CurTime));

        UpdateBufferedDuration();
    }

    internal void UpdateBufferedDuration()
    {
        if (_bufferedDuration != MainDemuxer.BufferedDuration)
        {
            _bufferedDuration = MainDemuxer.BufferedDuration;
            Raise(nameof(BufferedDuration));
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Player"/> class.
    /// </summary>
    /// <param name="config"></param>
    public Player(Config config = null)
    {
        if (config != null)
        {
            if (config.Player.player != null)
            {
                throw new Exception("Player's configuration is already assigned to another player");
            }

            Config = config;
        }
        else
        {
            Config = new Config();
        }

        PlayerId = GetUniqueId();
        Log = new LogHandler(("[#" + PlayerId + "]").PadRight(8, ' ') + " [Player        ] ");
        Log.Debug($"Creating Player (Usage = {Config.Player.Usage})");

        Activity = new Activity(this);
        Audio = new Audio(this);
        Video = new Video(this);
        Subtitles = new Subtitles(this);

        Config.SetPlayer(this);

        if (Config.Player.Usage == Usage.Audio)
        {
            Config.Video.Enabled = false;
            Config.Subtitles.Enabled = false;
        }

        Decoder = new DecoderContext(Config, PlayerId) { Tag = this };
        Engine.AddPlayer(this);

        if (Decoder.VideoDecoder.Renderer != null)
            Decoder.VideoDecoder.Renderer.forceNotExtractor = true;

        //decoder.OpenPlaylistItemCompleted              += Decoder_OnOpenExternalSubtitlesStreamCompleted;

        Decoder.OpenAudioStreamCompleted += Decoder_OpenAudioStreamCompleted;
        Decoder.OpenVideoStreamCompleted += Decoder_OpenVideoStreamCompleted;
        Decoder.OpenSubtitlesStreamCompleted += Decoder_OpenSubtitlesStreamCompleted;

        Decoder.OpenExternalAudioStreamCompleted += Decoder_OpenExternalAudioStreamCompleted;
        Decoder.OpenExternalVideoStreamCompleted += Decoder_OpenExternalVideoStreamCompleted;
        Decoder.OpenExternalSubtitlesStreamCompleted += Decoder_OpenExternalSubtitlesStreamCompleted;

        AudioDecoder.CBufAlloc = () => { Audio.ClearBuffer(); AudioFrame = null; };
        AudioDecoder.CodecChanged = Decoder_AudioCodecChanged;
        VideoDecoder.CodecChanged = Decoder_VideoCodecChanged;
        Decoder.RecordingCompleted += (o, e) => { IsRecording = false; };

        status = Status.Stopped;
        Reset();
        Log.Debug("Created");
    }

    /// <summary>
    /// Disposes the Player and de-assigns it from FlyleafHost
    /// </summary>
    public void Dispose() => Engine.DisposePlayer(this);
    internal void DisposeInternal()
    {
        lock (_lockActions)
        {
            if (IsDisposed)
                return;

            try
            {
                Initialize();
                Audio.Dispose();
                Decoder.Dispose();
                TransportControls?.Player_Disposed();
                Log.Info("Disposed");
            }
            catch (Exception e) { Log.Warn($"Disposed ({e.Message})"); }

            IsDisposed = true;
        }
    }
    internal void RefreshMaxVideoFrames()
    {
        lock (_lockActions)
        {
            if (!Video.isOpened)
                return;

            bool wasPlaying = IsPlaying;
            Pause();
            VideoDecoder.RefreshMaxVideoFrames();
            ReSync(Decoder.VideoStream, (int)(CurTime / 10000), true);

            if (wasPlaying)
                Play();
        }
    }

    private void ResetMe()
    {
        canPlay = false;
        bitRate = 0;
        curTime = 0;
        duration = 0;
        isLive = false;
        lastError = null;

        UIAdd(() =>
        {
            BitRate = BitRate;
            Duration = Duration;
            IsLive = IsLive;
            Status = Status;
            CanPlay = CanPlay;
            LastError = LastError;
            BufferedDuration = 0;
            Set(ref _curTime, curTime, true, nameof(CurTime));
        });
    }
    private void Reset()
    {
        ResetMe();
        Video.Reset();
        Audio.Reset();
        Subtitles.Reset();
        UIAll();
    }
    private void Initialize(Status status = Status.Stopped, bool andDecoder = true, bool isSwitch = false)
    {
        if (CanDebug)
            Log.Debug($"Initializing");

        lock (_lockActions) // Required in case of OpenAsync and Stop requests
        {
            try
            {
                Engine.TimeBeginPeriod1();

                this.status = status;
                canPlay = false;
                _isVideoSwitch = false;
                _seeks.Clear();

                while (taskPlayRuns || taskSeekRuns)
                    Thread.Sleep(5);

                if (andDecoder)
                {
                    if (isSwitch)
                        Decoder.InitializeSwitch();
                    else
                        Decoder.Initialize();
                }

                Reset();
                VideoDemuxer.DisableReversePlayback();
                ReversePlayback = false;

                if (CanDebug)
                    Log.Debug($"Initialized");

            }
            catch (Exception e)
            {
                Log.Error($"Initialize() Error: {e.Message}");

            }
            finally
            {
                Engine.TimeEndPeriod1();
            }
        }
    }

    internal void UIAdd(Action action) => _uIActions.Enqueue(action);
    internal void UIAll()
    {
        while (!_uIActions.IsEmpty)
        {
            if (_uIActions.TryDequeue(out var action))
            {
                UI(action);
            }
        }
    }

    public override bool Equals(object obj)
        => obj == null || !(obj is Player) ? false : ((Player)obj).PlayerId == PlayerId;
    public override int GetHashCode() => PlayerId.GetHashCode();
}
