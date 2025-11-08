// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using CommunityToolkit.Mvvm.ComponentModel;
using Richasy.MpvKernel;
using Richasy.MpvKernel.Core.Enums;
using System.Collections.ObjectModel;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 设置页面视图模型.
/// </summary>
public sealed partial class SettingsPageViewModel
{
    private bool _isInitialized;
    private string? _initialCustomLibMpvPath;
    private bool _initScrollAccelerate;

    [ObservableProperty]
    public partial bool IsRestartTipShown { get; set; }

    [ObservableProperty]
    private ElementTheme _appTheme;

    [ObservableProperty]
    private string _appThemeText;

    [ObservableProperty]
    private bool _isTopNavShown;

    [ObservableProperty]
    private bool _isAutoPlayNextRecommendVideo;

    [ObservableProperty]
    private PlayerDisplayMode _defaultPlayerDisplayMode;

    [ObservableProperty]
    private bool _playNextWithoutTip;

    [ObservableProperty]
    private bool _endWithPlaylist;

    [ObservableProperty]
    private bool _isMpvCustomOptionVisible;

    [ObservableProperty]
    private PreferCodecType _preferCodec;

    [ObservableProperty]
    private PreferQualityType _preferQuality;

    [ObservableProperty]
    private PreferDecodeType _preferDecode;

    [ObservableProperty]
    private MTCBehavior _mTCBehavior;

    [ObservableProperty]
    private bool _playerSpeedEnhancement;

    [ObservableProperty]
    private bool _globalPlayerSpeed;

    [ObservableProperty]
    private string _packageVersion;

    [ObservableProperty]
    private string _copyright;

    [ObservableProperty]
    private bool _isCopyScreenshot;

    [ObservableProperty]
    private bool _isOpenScreenshotFile;

    [ObservableProperty]
    private bool _hideWhenCloseWindow;

    [ObservableProperty]
    private bool _isNotificationEnabled;

    [ObservableProperty]
    private bool _isVideoMomentNotificationEnabled;

    [ObservableProperty]
    private bool _bottomProgressVisible;

    [ObservableProperty]
    private bool _noP2P;

    [ObservableProperty]
    private string _defaultDownloadPath;

    [ObservableProperty]
    private bool _downloadWithDanmaku;

    [ObservableProperty]
    private bool _openFolderAfterDownload;

    [ObservableProperty]
    private bool _useExternalBBDown;

    [ObservableProperty]
    private bool _onlyCopyCommandWhenDownload;

    [ObservableProperty]
    private bool _withoutCredentialWhenGenDownloadCommand;

    [ObservableProperty]
    private bool _filterAISubtitle;

    [ObservableProperty]
    private bool _isAIStreamingResponse;

    [ObservableProperty]
    private bool _isWebDavEnabled;

    [ObservableProperty]
    private bool _isWebDavEmpty;

    [ObservableProperty]
    private List<PlayerDisplayMode> _playerDisplayModeCollection;

    [ObservableProperty]
    private List<PreferCodecType> _preferCodecCollection;

    [ObservableProperty]
    private List<PreferQualityType> _preferQualityCollection;

    [ObservableProperty]
    private List<PreferDecodeType> _preferDecodeCollection;

    [ObservableProperty]
    private List<ExternalPlayerType> _externalPlayerTypeCollection;

    [ObservableProperty]
    private List<MTCBehavior> _mTCBehaviorCollection;

    [ObservableProperty]
    private bool _useWebPlayerWhenLive;

    [ObservableProperty]
    private bool _showSearchRecommend;

    [ObservableProperty]
    private bool _isPopularNavVisible;

    [ObservableProperty]
    private bool _isMomentNavVisible;

    [ObservableProperty]
    private bool _isVideoNavVisible;

    [ObservableProperty]
    private bool _isLiveNavVisible;

    [ObservableProperty]
    private bool _isAnimeNavVisible;

    [ObservableProperty]
    private bool _isCinemaNavVisible;

    [ObservableProperty]
    private bool _isArticleNavVisible;

    [ObservableProperty]
    public partial ScreenshotAction ScreenshotAction { get; set; }

    [ObservableProperty]
    public partial string? ScreenshotFolderPath { get; set; }

    [ObservableProperty]
    public partial double TempPlaybackRate { get; set; }

    [ObservableProperty]
    public partial bool ScrollAccelerate { get; set; }

    [ObservableProperty]
    public partial bool CacheOnDisk { get; set; }

    [ObservableProperty]
    public partial string? CacheDirDescription { get; set; }

    [ObservableProperty]
    public partial bool HasCustomCacheDir { get; set; }

    [ObservableProperty]
    public partial bool AudioExclusiveEnabled { get; set; }

    [ObservableProperty]
    public partial string? CustomLibMpvPath { get; set; }

    [ObservableProperty]
    public partial bool CanResetLibMpvPath { get; set; }

    [ObservableProperty]
    public partial bool IsMpvExtraSettingExpanded { get; set; }

    [ObservableProperty]
    public partial double MaxVolume { get; set; }

    [ObservableProperty]
    public partial HrSeekType HrSeek { get; set; }

    [ObservableProperty]
    public partial double MaxCacheSize { get; set; }

    [ObservableProperty]
    public partial double MaxBackCacheSize { get; set; }

    [ObservableProperty]
    public partial double MaxCacheSeconds { get; set; }

    [ObservableProperty]
    public partial bool UseIntegrationWhenSinglePlayWindow { get; set; }

    [ObservableProperty]
    public partial MpvBuiltInProfile MpvBuiltInProfile { get; set; }

    [ObservableProperty]
    public partial string TotalImageCacheSize { get; set; }

    [ObservableProperty]
    public partial AudioChannelLayoutType PreferAudioChannelLayout { get; set; }

    [ObservableProperty]
    public partial bool IsTotalImageCacheClearEnabled { get; set; }

    [ObservableProperty]
    public partial double StepForwardSecond { get; set; }

    [ObservableProperty]
    public partial double StepBackwardSecond { get; set; }

    [ObservableProperty]
    public partial MpvLogLevel LogLevel { get; set; }

    [ObservableProperty]
    public partial bool HideMainWindowOnPlay { get; set; }

    [ObservableProperty]
    public partial string D3D11AdapterName { get; set; }

    [ObservableProperty]
    public partial string VulkanDeviceName { get; set; }

    /// <summary>
    /// GPU设备列表.
    /// </summary>
    public ObservableCollection<string> GpuDevices { get; } = [];
}
