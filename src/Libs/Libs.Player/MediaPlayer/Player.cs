// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Threading;
using Bili.Copilot.Libs.Player.Core;
using Bili.Copilot.Libs.Player.Enums;
using Bili.Copilot.Libs.Player.MediaFramework.MediaContext;
using Bili.Copilot.Libs.Player.Misc;
using CommunityToolkit.Mvvm.ComponentModel;
using static Bili.Copilot.Libs.Player.Misc.Logger;
using static Bili.Copilot.Libs.Player.Utils;

namespace Bili.Copilot.Libs.Player.MediaPlayer;

/// <summary>
/// 表示播放器的类.
/// </summary>
/// <remarks>
/// 播放器类用于控制媒体的播放、暂停、停止等操作.
/// </remarks>
#pragma warning disable CA1063 // 正确实现 IDisposable
public unsafe partial class Player : ObservableObject, IDisposable
#pragma warning restore CA1063 // 正确实现 IDisposable
{
    /// <summary>
    /// 初始化 <see cref="Player"/> 类的新实例.
    /// </summary>
    /// <param name="config">配置参数.</param>
    public Player(Config config = null)
    {
        if (config != null)
        {
            if (config.Player.HasPlayer)
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

        if (Config.Player.Usage == PlayerUsage.Audio)
        {
            Config.Video.Enabled = false;
            Config.Subtitles.Enabled = false;
        }

        Decoder = new DecoderContext(Config, PlayerId) { Tag = this };
        Engine.AddPlayer(this);

        if (Decoder.VideoDecoder.Renderer != null)
        {
            Decoder.VideoDecoder.Renderer.forceNotExtractor = true;
        }

        Decoder.OpenAudioStreamCompleted += DecoderOpenAudioStreamCompleted;
        Decoder.OpenVideoStreamCompleted += DecoderOpenVideoStreamCompleted;
        Decoder.OpenSubtitlesStreamCompleted += DecoderOpenSubtitlesStreamCompleted;

        Decoder.OpenExternalAudioStreamCompleted += DecoderOpenExternalAudioStreamCompleted;
        Decoder.OpenExternalVideoStreamCompleted += DecoderOpenExternalVideoStreamCompleted;
        Decoder.OpenExternalSubtitlesStreamCompleted += DecoderOpenExternalSubtitlesStreamCompleted;

        AudioDecoder.CBufAlloc = () =>
        {
            Audio.ClearBuffer();
            AudioFrame = null;
        };
        AudioDecoder.CodecChanged = DecoderAudioCodecChanged;
        VideoDecoder.CodecChanged = DecoderVideoCodecChanged;
        Decoder.RecordingCompleted += (o, e) => { IsRecording = false; };

        Status = PlayerStatus.Stopped;
        Reset();
        Log.Debug("Created");
    }

    /// <summary>
    /// 释放播放器并从 FlyleafHost 中取消分配.
    /// </summary>
#pragma warning disable CA1063 // 正确实现 IDisposable
    public void Dispose() => Engine.DisposePlayer(this);
#pragma warning restore CA1063 // 正确实现 IDisposable

    /// <summary>
    /// 确定指定的对象是否等于当前对象.
    /// </summary>
    /// <param name="obj">要与当前对象进行比较的对象.</param>
    /// <returns>如果指定的对象等于当前对象，则为 true；否则为 false.</returns>
    public override bool Equals(object obj)
        => obj is null or not Player ? false : ((Player)obj).PlayerId == PlayerId;

    /// <summary>
    /// 用作默认哈希函数.
    /// </summary>
    /// <returns>当前对象的哈希代码.</returns>
    public override int GetHashCode() => PlayerId.GetHashCode();

    /// <summary>
    /// 更新当前时间.
    /// </summary>
    internal void UpdateCurTime()
    {
        lock (_seeks)
        {
            if (MainDemuxer == null || !_seeks.IsEmpty)
            {
                return;
            }

            if (MainDemuxer.IsHLSLive)
            {
                _curTime = MainDemuxer.CurTime; // *speed ?
                Duration = MainDemuxer.Duration;
            }
        }

        SetProperty(CurTime, _curTime, null, nameof(CurTime));
        UpdateBufferedDuration();
    }

    /// <summary>
    /// 更新缓冲时长.
    /// </summary>
    internal void UpdateBufferedDuration()
    {
        if (BufferedDuration != MainDemuxer.BufferedDuration)
        {
            BufferedDuration = MainDemuxer.BufferedDuration;
        }
    }

    internal void DisposeInternal()
    {
        lock (_lockActions)
        {
            if (IsDisposed)
            {
                return;
            }

            try
            {
                Initialize();
                Audio.Dispose();
                Decoder.Dispose();
                TransportControls?.Disposed();
                Log.Info("Disposed");
            }
            catch (Exception e)
            {
                Log.Warn($"Disposed ({e.Message})");
            }

            IsDisposed = true;
        }
    }

    internal void RefreshMaxVideoFrames()
    {
        lock (_lockActions)
        {
            if (!Video.IsOpened)
            {
                return;
            }

            var wasPlaying = IsPlaying;
            Pause();
            VideoDecoder.RefreshMaxVideoFrames();
            ReSync(Decoder.VideoStream, (int)(CurTime / 10000), true);

            if (wasPlaying)
            {
                Play();
            }
        }
    }

    internal void UIAdd(Action action) => _uiActions.Enqueue(action);

    internal void UIAll()
    {
        while (!_uiActions.IsEmpty)
        {
            if (_uiActions.TryDequeue(out var action))
            {
                UI(action);
            }
        }
    }

    private void ResetMe()
    {
        UIAdd(() =>
        {
            CanPlay = false;
            BitRate = 0;
            CurTime = 0;
            Duration = 0;
            IsLive = false;
            LastError = null;
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

    private void Initialize(PlayerStatus status = PlayerStatus.Stopped, bool andDecoder = true, bool isSwitch = false)
    {
        if (CanDebug)
        {
            Log.Debug($"Initializing");
        }

        lock (_lockActions) // Required in case of OpenAsync and Stop requests
        {
            try
            {
                Engine.TimeBeginPeriod1();

                Status = status;
                CanPlay = false;
                _isVideoSwitch = false;
                _seeks.Clear();

                while (_taskPlayRuns || _taskSeekRuns)
                {
                    Thread.Sleep(5);
                }

                if (andDecoder)
                {
                    if (isSwitch)
                    {
                        Decoder.InitializeSwitch();
                    }
                    else
                    {
                        Decoder.Initialize();
                    }
                }

                Reset();
                VideoDemuxer.DisableReversePlayback();
                ReversePlayback = false;

                if (CanDebug)
                {
                    Log.Debug($"Initialized");
                }
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
}
