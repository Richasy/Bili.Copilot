// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Bili.Copilot.Models.Data.Player;
using LibVLCSharp.Shared;
using Microsoft.UI.Dispatching;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// VLC 播放器视图模型.
/// </summary>
public sealed partial class VlcPlayerViewModel : ViewModelBase, IPlayerViewModel
{
    private bool _disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="VlcPlayerViewModel"/> class.
    /// </summary>
    public VlcPlayerViewModel()
        => _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

    /// <inheritdoc/>
    public void Initialize()
    {
        if (_libVlc != null)
        {
            var player = new MediaPlayer(_libVlc);
            player.EncounteredError += OnError;
            player.Opening += OnOpening;
            player.Playing += OnPlaying;
            player.Paused += OnPaused;
            player.Stopped += OnStopped;
            player.TimeChanged += OnTimeChanged;
            player.SeekableChanged += OnSeekableChanged;
            player.EndReached += OnEndReached;
            player.Buffering += OnBuffering;
            player.Corked += OnCorked;
            player.Uncorked += OnUncorked;
            player.AudioDevice += OnAudioDevice;
            Player = player;
        }
    }

    /// <inheritdoc/>
    public Task SetSourceAsync(SegmentInformation video, SegmentInformation audio, bool audioOnly)
    {
        _video = video;
        _audio = audio;
        _isStopped = false;
        LoadDashVideoSourceAsync(audioOnly);
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
        if (Player is MediaPlayer player)
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

        if (Player is MediaPlayer player)
        {
            player.Stop();
        }

        _isStopped = true;
    }

    /// <inheritdoc/>
    public void Play()
    {
        if (Player is not MediaPlayer player)
        {
            return;
        }

        if (Math.Abs(player.Length - player.Time) < TimeSpan.FromSeconds(1).TotalMilliseconds
            || Status == Models.Constants.App.PlayerStatus.End)
        {
            SeekTo(TimeSpan.Zero);
        }

        player.Play();
    }

    /// <inheritdoc/>
    public void SeekTo(TimeSpan time)
    {
        if (Player is MediaPlayer player)
        {
            player.SeekTo(time);
        }
    }

    /// <inheritdoc/>
    public void SetPlayRate(double rate)
        => ((MediaPlayer)Player).SetRate((float)rate);

    /// <inheritdoc/>
    public void SetVolume(int volume)
        => ((MediaPlayer)Player).Volume = volume;

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public void SetSwapChainOptions(string[] swapChainOptions)
    {
        _libVlc = new LibVLC(enableDebugLogs: true, swapChainOptions);
    }

    /// <summary>
    /// 释放 LibVLC.
    /// </summary>
    public void ReleaseLibVlc()
    {
        _libVlc?.Dispose();
        _libVlc = null;
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                Clear();
            }

            if (Player is MediaPlayer player)
            {
                player.Stop();
                player.Dispose();
            }

            Player = null;
            _disposedValue = true;
        }
    }
}
