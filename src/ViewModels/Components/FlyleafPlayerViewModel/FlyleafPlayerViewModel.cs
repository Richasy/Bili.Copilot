// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Player;
using FlyleafLib;
using FlyleafLib.MediaPlayer;
using Microsoft.UI.Dispatching;
using Windows.ApplicationModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 使用 FFmpeg 的播放器视图模型.
/// </summary>
public sealed partial class FlyleafPlayerViewModel : ViewModelBase, IPlayerViewModel
{
    private bool _disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="FlyleafPlayerViewModel"/> class.
    /// </summary>
    public FlyleafPlayerViewModel()
        => _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

    /// <summary>
    /// 初始化.
    /// </summary>
    public void Initialize()
    {
        if (!AppViewModel.Instance.IsEngineStarted)
        {
            var currentFolder = Package.Current.InstalledPath;
            var arch = RuntimeInformation.ProcessArchitecture == Architecture.Arm64 ? "arm64" : "x64";
            var ffmpegFolder = System.IO.Path.Combine(currentFolder, "Assets", "ffmpeg", arch);
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

        var player = new Player(config);
        player.PropertyChanged += OnPlayerPropertyChanged;
        player.PlaybackStopped += OnPlayerPlaybackStopped;
        player.OpenPlaylistItemCompleted += OnOpenPlaylistItemCompleted;
        player.OpenCompleted += OnPlayerOpenCompleted;
        player.BufferingStarted += OnPlayerBufferingStarted;
        player.BufferingCompleted += OnPlayerBufferingCompleted;
        Player = player;
    }

    /// <inheritdoc/>
    public Task SetSourceAsync(SegmentInformation video, SegmentInformation audio, bool audioOnly)
    {
        _video = video;
        _audio = audio;
        _isStopped = false;
        LoadDashVideoSource(audioOnly);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public void SetLiveSource(string url, bool audioOnly)
    {
        _video = null;
        _audio = null;
        _isStopped = false;
        LoadDashLiveSource(url, audioOnly);
    }

    /// <inheritdoc/>
    public void Pause()
    {
        if (Player is Player player)
        {
            player.Pause();
        }
    }

    /// <inheritdoc/>
    public void Stop()
    {
        if (_isStopped)
        {
            return;
        }

        if (Player is Player player)
        {
            player.Stop();
        }

        _isStopped = true;
    }

    /// <inheritdoc/>
    public void Play()
    {
        if (Player is not Player player)
        {
            return;
        }

        if (Math.Abs(player.Duration - player.CurTime) < 3 * 1000 * 10000)
        {
            SeekTo(TimeSpan.Zero);
        }

        player.Play();
    }

    /// <inheritdoc/>
    public void SeekTo(TimeSpan time)
    {
        if (time.TotalMilliseconds >= Duration.TotalMilliseconds
            || Math.Abs(time.TotalSeconds - Position.TotalSeconds) < 1)
        {
            return;
        }

        if (Player is Player player)
        {
            player.Seek(Convert.ToInt32(time.TotalMilliseconds));
        }
    }

    /// <inheritdoc/>
    public void SetPlayRate(double rate)
        => ((Player)Player).Speed = rate;

    /// <inheritdoc/>
    public void SetVolume(int volume)
        => ((Player)Player).Audio.Volume = volume;

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
                Clear();
            }

            if (Player is Player player)
            {
                player.VideoDecoder.DestroySwapChain();
                player.VideoDecoder.DestroyRenderer();
                player.Dispose();
            }

            Player = null;
            _disposedValue = true;
        }
    }
}
