﻿// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Player;
using CommunityToolkit.Mvvm.ComponentModel;
using FlyleafLib.MediaPlayer;
using Microsoft.UI.Dispatching;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 使用 FFmpeg 的播放器视图模型.
/// </summary>
public sealed partial class FlyleafPlayerViewModel
{
    private readonly DispatcherQueue _dispatcherQueue;

    private SegmentInformation _video;
    private SegmentInformation _audio;

    private string _recordingFileName;
    private bool _isStopped;

    [ObservableProperty]
    private bool _isLoop;

    [ObservableProperty]
    private object _player;

    [ObservableProperty]
    private bool _isRecording;

    /// <inheritdoc/>
    public event EventHandler MediaOpened;

    /// <inheritdoc/>
    public event EventHandler MediaEnded;

    /// <inheritdoc/>
    public event EventHandler<MediaStateChangedEventArgs> StateChanged;

    /// <inheritdoc/>
    public event EventHandler<MediaPositionChangedEventArgs> PositionChanged;

    /// <inheritdoc/>
    public TimeSpan Position => Player == null ? TimeSpan.Zero : TimeSpan.FromTicks(((Player)Player).CurTime);

    /// <inheritdoc/>
    public TimeSpan Duration => Player == null ? TimeSpan.Zero : TimeSpan.FromTicks(((Player)Player).Duration);

    /// <inheritdoc/>
    public double Volume => Player is Player player ? player.Audio.Volume : 0d;

    /// <inheritdoc/>
    public double PlayRate => Player is Player player ? player.Speed : 1d;

    /// <inheritdoc/>
    public string LastError => Player == null ? string.Empty : ((Player)Player).LastError;

    /// <inheritdoc/>
    public PlayerStatus Status { get; set; }

    /// <inheritdoc/>
    public bool IsPlayerReady => Player is Player player && player.CanPlay;

    /// <inheritdoc/>
    public bool IsRecordingSupported => true;

    /// <inheritdoc/>
    public bool IsMediaStatsSupported => true;
}
