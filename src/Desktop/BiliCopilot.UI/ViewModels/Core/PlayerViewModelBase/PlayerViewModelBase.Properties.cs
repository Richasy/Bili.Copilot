// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Mpv.Core.Enums.Player;
using Richasy.WinUI.Share.ViewModels;
using Windows.System.Display;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 播放器视图模型基类.
/// </summary>
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1401 // Fields should be private
#pragma warning disable SA1600 // Elements should be documented
public abstract partial class PlayerViewModelBase : ViewModelBase, IPlayerViewModel
{
    /// <summary>
    /// 视频用户代理.
    /// </summary>
    protected const string VideoUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36 Edg/116.0.1938.69";

    /// <summary>
    /// 直播用户代理.
    /// </summary>
    protected const string LiveUserAgent = "Mozilla/5.0 BiliDroid/1.12.0 (bbcallen@gmail.com)";

    /// <summary>
    /// 视频来源.
    /// </summary>
    protected const string VideoReferer = "https://www.bilibili.com";

    /// <summary>
    /// 直播来源.
    /// </summary>
    protected const string LiveReferer = "https://live.bilibili.com";

    protected readonly ILogger<PlayerViewModelBase> _logger;
    protected readonly DispatcherQueue _dispatcherQueue;

    protected string? _videoUrl;
    protected string? _audioUrl;
    protected bool _autoPlay;
    protected Action<int, int> _progressAction;
    protected Action<PlayerState> _stateAction;
    protected Action _endAction;
    protected DisplayRequest _displayRequest;

    protected bool _isInitialized;

    [ObservableProperty]
    private bool _isPlayerInitializing;

    [ObservableProperty]
    private bool _isPlayerDataLoading;

    [ObservableProperty]
    private bool _isPaused;

    [ObservableProperty]
    private int _position;

    [ObservableProperty]
    private int _duration;

    [ObservableProperty]
    private int _volume;

    [ObservableProperty]
    private double _speed;

    [ObservableProperty]
    private double _maxSpeed;

    [ObservableProperty]
    private bool _isFullScreen;

    [ObservableProperty]
    private bool _isCompactOverlay;

    [ObservableProperty]
    private bool _isBuffering;

    [ObservableProperty]
    private bool _isBottomProgressVisible;

    /// <summary>
    /// 播放数据加载完成.
    /// </summary>
    public event EventHandler PlayerDataLoaded;

    /// <summary>
    /// 请求显示通知.
    /// </summary>
    public event EventHandler<PlayerNotificationItemViewModel> RequestShowNotification;

    /// <summary>
    /// 是否为直播准备.
    /// </summary>
    public bool IsLive { get; set; }

    /// <summary>
    /// 是否为 PGC 播放.
    /// </summary>
    public bool IsPgc { get; set; }
}
#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore SA1401 // Fields should be private
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
