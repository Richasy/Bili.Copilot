// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Libs.Flyleaf.MediaPlayer;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Player;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Dispatching;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 使用 FFmpeg 的播放器视图模型.
/// </summary>
public sealed partial class MediaPlayerViewModel
{
    private readonly DispatcherQueue _dispatcherQueue;

    private SegmentInformation _video;
    private SegmentInformation _audio;

    [ObservableProperty]
    private bool _isLoop;

    [ObservableProperty]
    private Player _player;

    /// <summary>
    /// 媒体打开时触发的事件.
    /// </summary>
    public event EventHandler MediaOpened;

    /// <summary>
    /// 媒体结束时触发的事件.
    /// </summary>
    public event EventHandler MediaEnded;

    /// <summary>
    /// 媒体状态改变时触发的事件.
    /// </summary>
    public event EventHandler<MediaStateChangedEventArgs> StateChanged;

    /// <summary>
    /// 媒体位置改变时触发的事件.
    /// </summary>
    public event EventHandler<MediaPositionChangedEventArgs> PositionChanged;

    /// <summary>
    /// 获取当前媒体的位置.
    /// </summary>
    public TimeSpan Position => TimeSpan.FromTicks(Player?.CurTime ?? 0);

    /// <summary>
    /// 获取媒体的总时长.
    /// </summary>
    public TimeSpan Duration => _video != null ? TimeSpan.Zero : TimeSpan.FromMinutes(1);

    /// <summary>
    /// 获取或设置媒体的音量.
    /// </summary>
    public double Volume => Player.Audio.Volume;

    /// <summary>
    /// 获取媒体的播放速率.
    /// </summary>
    public double PlayRate => Player.Speed;

    /// <summary>
    /// 获取或设置媒体的状态.
    /// </summary>
    public PlayerStatus Status { get; set; }

    /// <summary>
    /// 获取媒体播放器是否准备就绪.
    /// </summary>
    public bool IsPlayerReady => Player != null && Player.CanPlay;
}
