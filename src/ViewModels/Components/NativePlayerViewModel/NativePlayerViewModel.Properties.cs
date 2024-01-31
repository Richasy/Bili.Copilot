﻿// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Player;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Dispatching;
using Windows.Media.Playback;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 原生播放器视图模型.
/// </summary>
public sealed partial class NativePlayerViewModel
{
    private readonly DispatcherQueue _dispatcherQueue;

    private SegmentInformation _video;
    private SegmentInformation _audio;
    private WebDavVideoInformation _webDavVideo;

    private bool _isStopped;

    [ObservableProperty]
    private bool _isLoop;

    [ObservableProperty]
    private object _player;

    /// <inheritdoc/>
    public event EventHandler MediaOpened;

    /// <inheritdoc/>
    public event EventHandler MediaEnded;

    /// <inheritdoc/>
    public event EventHandler<MediaStateChangedEventArgs> StateChanged;

    /// <inheritdoc/>
    public event EventHandler<MediaPositionChangedEventArgs> PositionChanged;

    /// <inheritdoc/>
    public event EventHandler<WebDavSubtitleListChangedEventArgs> WebDavSubtitleListChanged;

    /// <inheritdoc/>
    public event EventHandler<string> WebDavSubtitleChanged;

    /// <inheritdoc/>
    public bool IsRecording => false;

    /// <inheritdoc/>
    public TimeSpan Position => Player == null ? TimeSpan.Zero : ((MediaPlayer)Player).Position;

    /// <inheritdoc/>
    public TimeSpan Duration => Player == null ? TimeSpan.Zero : ((MediaPlayer)Player).NaturalDuration;

    /// <inheritdoc/>
    public double Volume => Player is not MediaPlayer player ? 100 : player.Volume * 100;

    /// <inheritdoc/>
    public double PlayRate => Player is not MediaPlayer player ? 1 : player.PlaybackRate;

    /// <inheritdoc/>
    public string LastError { get; private set; }

    /// <inheritdoc/>
    public PlayerStatus Status { get; set; }

    /// <inheritdoc/>
    public bool IsPlayerReady => Player is MediaPlayer player && player.CanSeek;

    /// <inheritdoc/>
    public bool IsRecordingSupported => false;

    /// <inheritdoc/>
    public bool IsMediaStatsSupported => false;

    /// <inheritdoc/>
    public bool IsStatsUpdated { get; set; }
}
