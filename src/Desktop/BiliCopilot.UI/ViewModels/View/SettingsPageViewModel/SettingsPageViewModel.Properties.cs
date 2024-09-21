// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 设置页面视图模型.
/// </summary>
public sealed partial class SettingsPageViewModel
{
    private bool _isInitialized;
    private bool _shouldSaveChatServices;

    [ObservableProperty]
    private ElementTheme _appTheme;

    [ObservableProperty]
    private string _appThemeText;

    [ObservableProperty]
    private bool _isTopNavShown;

    [ObservableProperty]
    private bool _isAutoPlayWhenLoaded;

    [ObservableProperty]
    private bool _isAutoPlayNextRecommendVideo;

    [ObservableProperty]
    private PlayerDisplayMode _defaultPlayerDisplayMode;

    [ObservableProperty]
    private bool _autoPlayNext;

    [ObservableProperty]
    private bool _playNextWithoutTip;

    [ObservableProperty]
    private bool _endWithPlaylist;

    [ObservableProperty]
    private PreferCodecType _preferCodec;

    [ObservableProperty]
    private PreferQualityType _preferQuality;

    [ObservableProperty]
    private PreferDecodeType _preferDecode;

    [ObservableProperty]
    private PlayerType _playerType;

    [ObservableProperty]
    private ExternalPlayerType _externalPlayerType;

    [ObservableProperty]
    private bool _isExternalPlayerType;

    [ObservableProperty]
    private double _singleFastForwardAndRewindSpan;

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
    private bool _autoLoadHistory;

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
    private bool _isWebDavEnabled;

    [ObservableProperty]
    private bool _isWebDavEmpty;

    [ObservableProperty]
    private WebDavConfig _selectedWebDav;

    [ObservableProperty]
    private List<PlayerDisplayMode> _playerDisplayModeCollection;

    [ObservableProperty]
    private List<PreferCodecType> _preferCodecCollection;

    [ObservableProperty]
    private List<PreferQualityType> _preferQualityCollection;

    [ObservableProperty]
    private List<PreferDecodeType> _preferDecodeCollection;

    [ObservableProperty]
    private List<PlayerType> _playerTypeCollection;

    [ObservableProperty]
    private List<ExternalPlayerType> _externalPlayerTypeCollection;

    /// <summary>
    /// WebDav 配置.
    /// </summary>
    public ObservableCollection<WebDavConfig> WebDavConfigs { get; } = new();

    /// <summary>
    /// AI服务.
    /// </summary>
    public ObservableCollection<AIServiceItemViewModel> OnlineChatServices { get; } = new();
}
