// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Libs.Flyleaf;
using Bili.Copilot.Libs.Flyleaf.MediaPlayer;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Player;
using Microsoft.UI.Dispatching;
using Windows.ApplicationModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 使用 FFmpeg 的播放器视图模型.
/// </summary>
public sealed partial class MediaPlayerViewModel : ViewModelBase, IDisposable
{
    private bool _disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="MediaPlayerViewModel"/> class.
    /// </summary>
    public MediaPlayerViewModel()
        => _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

    /// <summary>
    /// 初始化.
    /// </summary>
    public void Initialize(bool isLive)
    {
        if (!AppViewModel.Instance.IsEngineStarted)
        {
            var currentFolder = Package.Current.InstalledPath;
            var ffmpegFolder = System.IO.Path.Combine(currentFolder, "Assets", "ffmpeg");
            var engineConfig = new EngineConfig()
            {
                FFmpegPath = ffmpegFolder,
                FFmpegDevices = false,
                FFmpegLogLevel = FFmpegLogLevel.Warning,
#if DEBUG
                LogLevel = LogLevel.Debug,
                LogOutput = ":debug",
#endif
                UIRefresh = false,
                UICurTimePerSecond = true,
            };
            Engine.Start(engineConfig);

            AppViewModel.Instance.IsEngineStarted = true;
        }

        var config = new Config();
        config.Player.SeekAccurate = true;
        config.Decoder.ZeroCopy = ZeroCopy.Enabled;
        config.Decoder.AllowProfileMismatch = true;
        config.Decoder.MaxVideoFrames = 20;
        config.Decoder.MaxAudioFrames = 20;
        config.Demuxer.CloseTimeout = TimeSpan.FromSeconds(10).Ticks;
        config.Video.VideoAcceleration = SettingsToolkit.ReadLocalSetting(SettingNames.VideoAcceleration, true);
        config.Video.SwsForce = SettingsToolkit.ReadLocalSetting(SettingNames.DecodeType, DecodeType.HardwareDecode) == DecodeType.SoftwareDecode;
        config.Video.SwsHighQuality = true;
        config.Video.VSync = 1;
        config.Audio.FiltersEnabled = true;
        config.Decoder.ShowCorrupted = true;
        config.Player.MinBufferDuration = TimeSpan.FromSeconds(5).Ticks;

        Player = new Player(config);
        Player.PropertyChanged += OnPlayerPropertyChanged;
        Player.PlaybackStopped += OnPlayerPlaybackStopped;
        Player.OpenPlaylistItemCompleted += OnOpenPlaylistItemCompleted;
        Player.OpenCompleted += OnPlayerOpenCompleted;
        Player.BufferingStarted += OnPlayerBufferingStarted;
        Player.BufferingCompleted += OnPlayerBufferingCompleted;
    }

    /// <summary>
    /// 设置音频、视频源.
    /// </summary>
    /// <param name="video">视频源.</param>
    /// <param name="audio">音频源.</param>
    public void SetSource(SegmentInformation video, SegmentInformation audio, bool audioOnly)
    {
        _video = video;
        _audio = audio;
        LoadDashVideoSource(audioOnly);
    }

    /// <summary>
    /// 设置直播源.
    /// </summary>
    /// <param name="url">直播地址.</param>
    public void SetLiveSource(string url, bool audioOnly)
    {
        _video = null;
        _audio = null;
        LoadDashLiveSource(url, audioOnly);
    }

    /// <summary>
    /// 暂停.
    /// </summary>
    public void Pause()
        => Player?.Pause();

    /// <summary>
    /// 播放.
    /// </summary>
    public void Play()
    {
        if (Player == null)
        {
            return;
        }

        if (Math.Abs(Player.Duration - Player.CurTime) < 100)
        {
            SeekTo(TimeSpan.Zero);
        }

        Player?.Play();
    }

    /// <summary>
    /// 跳转至.
    /// </summary>
    /// <param name="time">指定的时间.</param>
    public void SeekTo(TimeSpan time)
        => Player?.Seek(Convert.ToInt32(time.TotalMilliseconds));

    /// <summary>
    /// 设置播放速率.
    /// </summary>
    /// <param name="rate">播放速率.</param>
    public void SetPlayRate(double rate)
        => Player.Speed = rate;

    /// <summary>
    /// 设置音量.
    /// </summary>
    /// <param name="volume">音量.</param>
    public void SetVolume(int volume)
        => Player.Audio.Volume = Convert.ToInt32(volume);

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                Player?.Dispose();
            }

            Player = null;
            _disposedValue = true;
        }
    }
}
