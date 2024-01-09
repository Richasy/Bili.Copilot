// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Player;
using CommunityToolkit.Mvvm.ComponentModel;
using LibVLCSharp.Shared;
using Microsoft.UI.Dispatching;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// VLC 播放器视图模型.
/// </summary>
public sealed partial class VlcPlayerViewModel
{
    private readonly DispatcherQueue _dispatcherQueue;

    private SegmentInformation _video;
    private SegmentInformation _audio;
#pragma warning disable CA2213 // 应释放可释放的字段
    private LibVLC _libVlc;
#pragma warning restore CA2213 // 应释放可释放的字段
    private Media _currentMedia;
    private HttpMediaInput _currentInput;

    private bool _isStopped;
    private bool _isOpened;

    [ObservableProperty]
    private bool _isLoop;

    [ObservableProperty]
    private object _player;

    [ObservableProperty]
    private bool _isRecording;

    [ObservableProperty]
    private string _lastError;

    /// <inheritdoc/>
    public event EventHandler MediaOpened;

    /// <inheritdoc/>
    public event EventHandler MediaEnded;

    /// <inheritdoc/>
    public event EventHandler<Models.App.Args.MediaStateChangedEventArgs> StateChanged;

    /// <inheritdoc/>
    public event EventHandler<MediaPositionChangedEventArgs> PositionChanged;

    /// <inheritdoc/>
    public TimeSpan Position => Player == null ? TimeSpan.Zero : TimeSpan.FromMilliseconds(((MediaPlayer)Player).Time);

    /// <inheritdoc/>
    public TimeSpan Duration => Player == null ? TimeSpan.Zero : TimeSpan.FromMilliseconds(((MediaPlayer)Player).Length);

    /// <inheritdoc/>
    public double Volume => ((MediaPlayer)Player).Volume;

    /// <inheritdoc/>
    public double PlayRate => ((MediaPlayer)Player).Rate;

    /// <inheritdoc/>
    public PlayerStatus Status { get; set; }

    /// <inheritdoc/>
    public bool IsPlayerReady => Player is MediaPlayer player && player.IsSeekable;

    /// <inheritdoc/>
    public bool IsRecordingSupported => false;

    /// <inheritdoc/>
    public bool IsMediaStatsSupported => false;
}
