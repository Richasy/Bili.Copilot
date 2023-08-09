﻿// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Threading.Tasks;
using Bili.Copilot.Models.Data.Player;
using Microsoft.UI.Dispatching;
using Windows.Media.Playback;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 原生播放器视图模型.
/// </summary>
public sealed partial class NativePlayerViewModel : ViewModelBase, IPlayerViewModel
{
    private bool _disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="NativePlayerViewModel"/> class.
    /// </summary>
    public NativePlayerViewModel()
        => _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

    /// <inheritdoc/>
    public void Initialize()
    {
        return;
    }

    /// <inheritdoc/>
    public async Task SetSourceAsync(SegmentInformation video, SegmentInformation audio, bool audioOnly)
    {
        _video = video;
        _audio = audio;
        Clear();
        await LoadDashVideoSourceAsync(audioOnly);
    }

    /// <inheritdoc/>
    public async void SetLiveSource(string url, bool audioOnly)
    {
        _video = null;
        _audio = null;
        _isStopped = false;
        Clear();
        await LoadLiveSourceAsync(url);
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
            player.Pause();
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

        if (Math.Abs(player.NaturalDuration.TotalMilliseconds - player.Position.TotalMilliseconds) < 3 * 1000)
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

        if (Player is MediaPlayer player
            && player.CanSeek)
        {
            player.Position = time;
        }
    }

    /// <inheritdoc/>
    public void SetPlayRate(double rate)
        => ((MediaPlayer)Player).PlaybackRate = rate;

    /// <inheritdoc/>
    public void SetVolume(int volume)
        => ((MediaPlayer)Player).Volume = Convert.ToInt32(volume);

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
                Stop();
                Clear();
            }

            Player = null;
            _disposedValue = true;
        }
    }
}
