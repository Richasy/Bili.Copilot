// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Forms;
using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Resolvers;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Dispatching;
using Richasy.MpvKernel.Core;
using Richasy.MpvKernel.Core.Models;
using Richasy.MpvKernel.Player;
using Richasy.WinUIKernel.Share.Toolkits;
using System.Collections.ObjectModel;
using Windows.Media;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 播放器视图模型.
/// </summary>
public sealed partial class PlayerViewModel
{
    private MediaSnapshot _snapshot;
    private MediaSourceResolverBase? _sourceResolver;
    private IMpvMediaHistoryResolver? _historyResolver;
    private bool _uiElementSetted;
    private bool _isDanmakuInitialized;
    private double _lastSpeed;
    private double? _continuePosition;
    private Windows.Media.Playback.MediaPlayer? _nativeMp;
    private SystemMediaTransportControls? _smtc;
    private double _verticalSubtitlePosition = 100;
    private double _horizontalSubtitlePosition = 100;
    private bool _isRightKeyTripleSpeed;
    private DispatcherQueueTimer? _rightKeyLongPressTimer;
    private DateTimeOffset _lastPlayNextOrPrevTime = DateTimeOffset.MinValue;

    private const int RightKeyLongPressDelay = 500;
    private bool _isRightKeyDown;
    private bool _isTlsFailed;
    private double _prevPosition;
    private bool _isBroken;

    public event EventHandler<string> WarningOccurred;
    public event EventHandler ChapterInitialized;
    public event EventHandler<double> ProgressChanged;
    public event EventHandler<MpvCacheStateEventArgs> CacheStateChanged;
    public event EventHandler RequestShowNextTip;
    public event EventHandler RequestHideNextTip;

    public MpvClient? Client { get; private set; }

    public PlayerWindow? Window { get; private set; }

    public bool IsNextTipShown { get; set; }

    public bool IsNextTipShownInThisMedia { get; set; }

    [ObservableProperty]
    public partial MpvPlayer Player { get; set; }

    [ObservableProperty]
    public partial string? Title { get; set; }

    [ObservableProperty]
    public partial string? PositionText { get; set; }

    [ObservableProperty]
    public partial string? DurationText { get; set; }

    [ObservableProperty]
    public partial bool IsBlackBackgroundVisible { get; set; } = true;

    [ObservableProperty]
    public partial bool IsControlsVisible { get; set; }

    [ObservableProperty]
    public partial bool IsTouchControlsVisible { get; set; }

    [ObservableProperty]
    public partial double CurrentVolume { get; set; }

    [ObservableProperty]
    public partial double PreviewPosition { get; set; }

    [ObservableProperty]
    public partial bool IsProgressChanging { get; set; }

    [ObservableProperty]
    public partial bool IsPreviewProgressChanging { get; set; }

    [ObservableProperty]
    public partial bool IsVolumeChanging { get; set; }

    [ObservableProperty]
    public partial bool IsSpeedChanging { get; set; }

    [ObservableProperty]
    public partial bool IsSourceSelectable { get; set; }

    [ObservableProperty]
    public partial bool IsSubtitleEmpty { get; set; }

    [ObservableProperty]
    public partial string? CacheSpeedText { get; set; }

    [ObservableProperty]
    public partial IPlayerConnectorViewModel? Connector { get; set; }

    [ObservableProperty]
    public partial bool IsPrevButtonEnabled { get; set; }

    [ObservableProperty]
    public partial bool IsNextButtonEnabled { get; set; }

    [ObservableProperty]
    public partial string? PrevButtonToolTip { get; set; }

    [ObservableProperty]
    public partial string? NextButtonToolTip { get; set; }

    [ObservableProperty]
    public partial bool IsVideoNavigationAvailable { get; private set; }

    [ObservableProperty]
    public partial bool IsExtraPanelVisible { get; set; }

    [ObservableProperty]
    public partial bool IsHoldingSpeedChanging { get; set; }

    [ObservableProperty]
    public partial bool IsConnecting { get; set; }

    [ObservableProperty]
    public partial string? ErrorMessage { get; set; }

    [ObservableProperty]
    public partial bool IsDanmakuEnabled { get; set; }

    [ObservableProperty]
    public partial bool IsDanmakuControlVisible { get; set; }

    [ObservableProperty]
    public partial bool IsDanmakuLoading { get; set; }

    [ObservableProperty]
    public partial SourceItemViewModel? SelectedSource { get; set; }

    [ObservableProperty]
    public partial string? BackwardTip { get; set; }

    [ObservableProperty]
    public partial string? ForwardTip { get; set; }

    [ObservableProperty]
    public partial bool UseIntegrationOperation { get; set; }

    [ObservableProperty]
    public partial bool IsSubtitleSettingsVisible { get; set; }

    [ObservableProperty]
    public partial bool IsVerticalScreen { get; set; }

    [ObservableProperty]
    public partial double SubtitlePosition { get; set; }

    [ObservableProperty]
    public partial double SubtitleFontSize { get; set; }

    [ObservableProperty]
    public partial string SubtitleFontFamily { get; set; }

    [ObservableProperty]
    public partial bool IsStatsOverlayShown { get; set; }

    [ObservableProperty]
    public partial double SubtitleDelaySeconds { get; set; }

    [ObservableProperty]
    public partial bool IsBottomProgressBarVisible { get; set; }

    [ObservableProperty]
    public partial bool IsChapterVisible { get; set; }

    [ObservableProperty]
    public partial bool IsSubtitleEnabled { get; set; } = true;

    [ObservableProperty]
    public partial double MaxVolume { get; set; }

    [ObservableProperty]
    public partial bool IsEnd { get; set; }

    [ObservableProperty]
    public partial bool UsingPanscan { get; set; }

    [ObservableProperty]
    public partial bool IsTopMost { get; set; }

    [ObservableProperty]
    public partial bool IsWindowDeactivated { get; set; }

    [ObservableProperty]
    public partial bool IsMute { get; set; }

    [ObservableProperty]
    public partial bool IsDanmakuOptionsVisible { get; set; }

    [ObservableProperty]
    public partial Anime4KMode Anime4KMode { get; set; }

    [ObservableProperty]
    public partial ArtCNNMode ArtCNNMode { get; set; }

    [ObservableProperty]
    public partial Nnedi3Mode Nnedi3Mode { get; set; }

    [ObservableProperty]
    public partial RavuMode RavuMode { get; set; }

    [ObservableProperty]
    public partial VsrScale VsrScale { get; set; }

    [ObservableProperty]
    public partial bool HasNvidiaGpu { get; set; }

    [ObservableProperty]
    public partial bool IsClearShaderEnabled { get; set; }

    public bool IsPopupVisible { get; set; }

    public DateTimeOffset? LastVolumeChangingTime { get; set; }

    public DateTimeOffset? LastProgressChangingTime { get; set; }

    public DateTimeOffset? LastSpeedChangingTime { get; set; }

    public DanmakuRenderViewModel Danmaku { get; } = new();

    public DanmakuSendViewModel DanmakuSend { get; } = GlobalDependencies.Kernel.GetRequiredService<DanmakuSendViewModel>();

    public ObservableCollection<SourceItemViewModel> Sources { get; } = [];

    public ObservableCollection<SubtitleItemViewModel> Subtitles { get; } = [];

    public ObservableCollection<ChapterItemViewModel> Chapters { get; } = [];

    public ObservableCollection<SystemFont> Fonts { get; } = [];
}
