// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 设置页面视图模型.
/// </summary>
public sealed partial class SettingsPageViewModel
{
    private bool _isInitialized;

    [ObservableProperty]
    private ElementTheme _appTheme;

    [ObservableProperty]
    private string _appThemeText;

    [ObservableProperty]
    private bool _isAutoPlayWhenLoaded;

    [ObservableProperty]
    private bool _isAutoPlayNextRecommendVideo;

    [ObservableProperty]
    private PlayerDisplayMode _defaultPlayerDisplayMode;

    [ObservableProperty]
    private bool _autoPlayNext;

    [ObservableProperty]
    private PreferCodecType _preferCodec;

    [ObservableProperty]
    private PreferQualityType _preferQuality;

    [ObservableProperty]
    private PreferDecodeType _preferDecode;

    [ObservableProperty]
    private PlayerType _playerType;

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
    private IReadOnlyCollection<PlayerDisplayMode> _playerDisplayModeCollection;

    [ObservableProperty]
    private IReadOnlyCollection<PreferCodecType> _preferCodecCollection;

    [ObservableProperty]
    private IReadOnlyCollection<PreferQualityType> _preferQualityCollection;

    [ObservableProperty]
    private IReadOnlyCollection<PreferDecodeType> _preferDecodeCollection;

    [ObservableProperty]
    private IReadOnlyCollection<PlayerType> _playerTypeCollection;
}
