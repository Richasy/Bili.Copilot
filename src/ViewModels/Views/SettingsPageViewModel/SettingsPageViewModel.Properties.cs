// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Player;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 设置视图模型.
/// </summary>
public sealed partial class SettingsPageViewModel
{
    [ObservableProperty]
    private ElementTheme _appTheme;

    [ObservableProperty]
    private string _appThemeText;

    [ObservableProperty]
    private bool _isAutoPlayWhenLoaded;

    [ObservableProperty]
    private bool _isAutoPlayNextRelatedVideo;

    [ObservableProperty]
    private PlayerDisplayMode _defaultPlayerDisplayMode;

    [ObservableProperty]
    private bool _isContinuePlay;

    [ObservableProperty]
    private bool _isAutoCloseWindowWhenEnded;

    [ObservableProperty]
    private PreferCodec _preferCodec;

    [ObservableProperty]
    private DecodeType _decodeType;

    [ObservableProperty]
    private PreferQuality _preferQuality;

    [ObservableProperty]
    private PreferAudio _preferAudioQuality;

    [ObservableProperty]
    private PlayerType _playerType;

    [ObservableProperty]
    private double _singleFastForwardAndRewindSpan;

    [ObservableProperty]
    private bool _playbackRateEnhancement;

    [ObservableProperty]
    private bool _globalPlaybackRate;

    [ObservableProperty]
    private string _packageVersion;

    [ObservableProperty]
    private string _copyright;

    [ObservableProperty]
    private bool _isSupportContinuePlay;

    [ObservableProperty]
    private bool _isCopyScreenshot;

    [ObservableProperty]
    private bool _isOpenScreenshotFile;

    [ObservableProperty]
    private bool _isOpenRoaming;

    [ObservableProperty]
    private bool _isGlobeProxy;

    [ObservableProperty]
    private string _roamingVideoAddress;

    [ObservableProperty]
    private string _roamingViewAddress;

    [ObservableProperty]
    private string _roamingSearchAddress;

    [ObservableProperty]
    private bool _isOpenDynamicNotification;

    [ObservableProperty]
    private bool _isEnableBackgroundTask;

    [ObservableProperty]
    private bool _isFullTraditionalChinese;

    [ObservableProperty]
    private bool _hideWhenCloseWindow;

    [ObservableProperty]
    private PlayerWindowBehavior _playerWindowBehavior;

    /// <summary>
    /// 实例.
    /// </summary>
    public static SettingsPageViewModel Instance { get; } = new();

    /// <summary>
    /// 播放器显示模式可选集合.
    /// </summary>
    public ObservableCollection<PlayerDisplayMode> PlayerDisplayModeCollection { get; }

    /// <summary>
    /// 偏好的解码模式可选集合.
    /// </summary>
    public ObservableCollection<PreferCodec> PreferCodecCollection { get; }

    /// <summary>
    /// 解码类型可选集合.
    /// </summary>
    public ObservableCollection<DecodeType> DecodeTypeCollection { get; }

    /// <summary>
    /// 偏好的画质可选集合.
    /// </summary>
    public ObservableCollection<PreferQuality> PreferQualities { get; }

    /// <summary>
    /// 偏好的音质可选集合.
    /// </summary>
    public ObservableCollection<PreferAudio> PreferAudioQualities { get; }

    /// <summary>
    /// 播放器类型集合.
    /// </summary>
    public ObservableCollection<PlayerType> PlayerTypes { get; }
}
