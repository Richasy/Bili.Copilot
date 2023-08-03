// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.ObjectModel;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.Live;
using Bili.Copilot.Models.Data.Pgc;
using Bili.Copilot.Models.Data.Player;
using Bili.Copilot.Models.Data.Video;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Windows.System.Display;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 媒体播放器视图模型.
/// </summary>
public sealed partial class PlayerDetailViewModel
{
    private readonly DispatcherQueue _dispatcherQueue;
    private readonly Window _attachedWindow;
    private VideoType _videoType;
    private object _viewData;
    private bool _isInPrivate;
    private VideoIdentifier _currentPart;
    private EpisodeInformation _currentEpisode;
    private LivePlaylineInformation _currentPlayLine;
    private LivePlayUrl _currentLiveUrl;
    private LiveMediaInformation _liveMediaInformation;
    private MediaInformation _mediaInformation;
    private SegmentInformation _video;
    private SegmentInformation _audio;
    private TimeSpan _lastReportProgress;
    private TimeSpan _initializeProgress;
    private Action _playNextAction;
    private DisplayRequest _displayRequest;

    private DispatcherTimer _unitTimer;

    private double _originalPlayRate;
    private double _originalDanmakuSpeed;
    private double _presetVolumeHoldTime;

    [ObservableProperty]
    private bool _isReloading;

    [ObservableProperty]
    private PlayerStatus _status;

    [ObservableProperty]
    private PlayerDisplayMode _displayMode;

    [ObservableProperty]
    private FormatInformation _currentFormat;

    [ObservableProperty]
    private int _volume;

    [ObservableProperty]
    private double _playbackRate;

    [ObservableProperty]
    private double _maxPlaybackRate;

    [ObservableProperty]
    private double _playbackRateStep;

    [ObservableProperty]
    private double _durationSeconds;

    [ObservableProperty]
    private double _progressSeconds;

    [ObservableProperty]
    private string _progressText;

    [ObservableProperty]
    private string _durationText;

    [ObservableProperty]
    private bool _isLoop;

    [ObservableProperty]
    private bool _isError;

    [ObservableProperty]
    private string _errorText;

    [ObservableProperty]
    private bool _isShowProgressTip;

    [ObservableProperty]
    private string _progressTip;

    [ObservableProperty]
    private bool _isAudioOnly;

    [ObservableProperty]
    private string _fullScreenText;

    [ObservableProperty]
    private string _compactOverlayText;

    [ObservableProperty]
    private bool _isShowMediaTransport;

    [ObservableProperty]
    private string _nextVideoTipText;

    [ObservableProperty]
    private bool _isShowNextVideoTip;

    [ObservableProperty]
    private double _nextVideoCountdown;

    [ObservableProperty]
    private double _progressTipCountdown;

    [ObservableProperty]
    private bool _isInteractionVideo;

    [ObservableProperty]
    private bool _isShowInteractionChoices;

    [ObservableProperty]
    private bool _isInteractionEnd;

    [ObservableProperty]
    private bool _isBuffering;

    [ObservableProperty]
    private bool _isMediaPause;

    [ObservableProperty]
    private string _cover;

    [ObservableProperty]
    private bool _canPlayNextPart;

    [ObservableProperty]
    private MediaPlayerViewModel _player;

    /// <summary>
    /// 当需要显示临时消息时触发的事件.
    /// </summary>
    public event EventHandler<string> RequestShowTempMessage;

    /// <summary>
    /// 请求在网页中打开.
    /// </summary>
    public event EventHandler RequestOpenInBrowser;

    /// <summary>
    /// 当媒体播放结束时触发的事件.
    /// </summary>
    public event EventHandler MediaEnded;

    /// <summary>
    /// 当内部部分发生变化时触发的事件.
    /// </summary>
    public event EventHandler<VideoIdentifier> InternalPartChanged;

    /// <summary>
    /// 媒体格式的可观察集合.
    /// </summary>
    public ObservableCollection<FormatInformation> Formats { get; }

    /// <summary>
    /// 播放速率的可观察集合.
    /// </summary>
    public ObservableCollection<PlaybackRateItemViewModel> PlaybackRates { get; }

    /// <summary>
    /// 字幕模块的视图模型.
    /// </summary>
    public SubtitleModuleViewModel SubtitleViewModel { get; }

    /// <summary>
    /// 弹幕模块的视图模型.
    /// </summary>
    public DanmakuModuleViewModel DanmakuViewModel { get; }

    /// <summary>
    /// 交互模块的视图模型.
    /// </summary>
    public InteractionModuleViewModel InteractionViewModel { get; }
}
