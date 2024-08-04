// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using System.ComponentModel;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Player;
using Bili.Copilot.ViewModels.Components;
using FlyleafLib;
using Microsoft.UI.Xaml;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 设置视图模型.
/// </summary>
public sealed partial class SettingsPageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPageViewModel"/> class.
    /// </summary>
    public SettingsPageViewModel()
    {
        PlayerDisplayModeCollection = new ObservableCollection<PlayerDisplayMode>();
        PreferCodecCollection = new ObservableCollection<PreferCodec>();
        DecodeTypeCollection = new ObservableCollection<DecodeType>();
        PreferQualities = new ObservableCollection<PreferQuality>();
        PreferAudioQualities = new ObservableCollection<PreferAudio>();
        BiliPlayerTypes = new ObservableCollection<PlayerType>();
        WebDavPlayerTypes = new ObservableCollection<PlayerType>();
        WebDavConfigs = new ObservableCollection<WebDavConfig>();

        InitializeSettings();
    }

    /// <summary>
    /// 初始化设置.
    /// </summary>
    public void InitializeSettings()
    {
        PropertyChanged -= OnPropertyChanged;
        AppTheme = ReadSetting(SettingNames.AppTheme, ElementTheme.Default);
        CheckTheme();
        IsAutoPlayWhenLoaded = ReadSetting(SettingNames.IsAutoPlayWhenLoaded, true);
        IsAutoPlayNextRelatedVideo = ReadSetting(SettingNames.IsAutoPlayNextRelatedVideo, false);
        IsContinuePlay = ReadSetting(SettingNames.IsContinuePlay, true);
        IsAutoCloseWindowWhenEnded = ReadSetting(SettingNames.IsAutoCloseWindowWhenEnded, false);
        SingleFastForwardAndRewindSpan = ReadSetting(SettingNames.SingleFastForwardAndRewindSpan, 30d);
        IsSupportContinuePlay = ReadSetting(SettingNames.SupportContinuePlay, true);
        IsCopyScreenshot = ReadSetting(SettingNames.CopyScreenshotAfterSave, true);
        IsOpenScreenshotFile = ReadSetting(SettingNames.OpenScreenshotAfterSave, false);
        PlaybackRateEnhancement = ReadSetting(SettingNames.PlaybackRateEnhancement, false);
        GlobalPlaybackRate = ReadSetting(SettingNames.GlobalPlaybackRate, false);
        IsFullTraditionalChinese = ReadSetting(SettingNames.IsFullTraditionalChinese, false);
        HideWhenCloseWindow = ReadSetting(SettingNames.HideWhenCloseWindow, false);
        PlayerWindowBehavior = ReadSetting(SettingNames.PlayerWindowBehaviorType, PlayerWindowBehavior.Main);
        AutoLoadHistory = ReadSetting(SettingNames.IsAutoLoadHistoryWhenLoaded, true);
        IsPlayerControlModeManual = ReadSetting(SettingNames.IsPlayerControlModeManual, false);
        IsNotificationEnabled = ReadSetting(SettingNames.IsNotifyEnabled, true);
        IsVideoDynamicNotificationEnabled = ReadSetting(SettingNames.DynamicNotificationEnabled, true);
        VideoProcessor = ReadSetting(SettingNames.VideoProcessor, VideoProcessors.D3D11);
        BottomProgressVisible = ReadSetting(SettingNames.BottomProgressVisible, true);
        IsPlaybackRateSliderVisible = ReadSetting(SettingNames.PlaybackRateSliderEnabled, false);
        UseMpvPlayer = ReadSetting(SettingNames.UseMpvPlayer, false);
        NoP2P = ReadSetting(SettingNames.NoP2P, false);
        WebPlayerInit();
        PreferCodecInit();
        DecodeInit();
        PlayerModeInit();
        PlayerTypeInit();

        RoamingInit();
        PreferQualityInit();
        PreferAudioInit();

        var copyrightTemplate = ResourceToolkit.GetLocalizedString(StringNames.Copyright);
        Copyright = string.Format(copyrightTemplate, 2024);
        PackageVersion = AppToolkit.GetPackageVersion();

        IsVideoNativePlayer = PlayerType == PlayerType.Native;
        InitializeWebDavConfigCommand.Execute(default);

        PropertyChanged += OnPropertyChanged;
        AppViewModel.Instance.CheckMpvAvailableCommand.Execute(default);
    }

    /// <summary>
    /// 网页播放器设置初始化.
    /// </summary>
    public void WebPlayerInit()
    {
        IsWebSignIn = ReadSetting(SettingNames.IsWebSignIn, false);
        UseWebPlayer = ReadSetting(SettingNames.UseWebPlayer, false) && IsWebSignIn;
        CheckWebSignInText();
    }

    private static void WriteSetting(SettingNames name, object value) => SettingsToolkit.WriteLocalSetting(name, value);

    private static T ReadSetting<T>(SettingNames name, T defaultValue) => SettingsToolkit.ReadLocalSetting(name, defaultValue);

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(IsAutoPlayWhenLoaded):
                WriteSetting(SettingNames.IsAutoPlayWhenLoaded, IsAutoPlayWhenLoaded);
                break;
            case nameof(DefaultPlayerDisplayMode):
                WriteSetting(SettingNames.DefaultPlayerDisplayMode, DefaultPlayerDisplayMode);
                break;
            case nameof(IsContinuePlay):
                WriteSetting(SettingNames.IsContinuePlay, IsContinuePlay);
                break;
            case nameof(IsAutoCloseWindowWhenEnded):
                WriteSetting(SettingNames.IsAutoCloseWindowWhenEnded, IsAutoCloseWindowWhenEnded);
                break;
            case nameof(PreferCodec):
                WriteSetting(SettingNames.PreferCodec, PreferCodec);
                break;
            case nameof(SingleFastForwardAndRewindSpan):
                WriteSetting(SettingNames.SingleFastForwardAndRewindSpan, SingleFastForwardAndRewindSpan);
                break;
            case nameof(IsSupportContinuePlay):
                WriteSetting(SettingNames.SupportContinuePlay, IsSupportContinuePlay);
                break;
            case nameof(IsCopyScreenshot):
                WriteSetting(SettingNames.CopyScreenshotAfterSave, IsCopyScreenshot);
                break;
            case nameof(IsOpenScreenshotFile):
                WriteSetting(SettingNames.OpenScreenshotAfterSave, IsOpenScreenshotFile);
                break;
            case nameof(PlaybackRateEnhancement):
                WriteSetting(SettingNames.PlaybackRateEnhancement, PlaybackRateEnhancement);
                break;
            case nameof(GlobalPlaybackRate):
                WriteSetting(SettingNames.GlobalPlaybackRate, GlobalPlaybackRate);
                break;
            case nameof(IsAutoPlayNextRelatedVideo):
                WriteSetting(SettingNames.IsAutoPlayNextRelatedVideo, IsAutoPlayNextRelatedVideo);
                break;
            case nameof(AutoLoadHistory):
                WriteSetting(SettingNames.IsAutoLoadHistoryWhenLoaded, AutoLoadHistory);
                break;
            case nameof(IsOpenRoaming):
                WriteSetting(SettingNames.IsOpenRoaming, IsOpenRoaming);
                break;
            case nameof(IsGlobeProxy):
                WriteSetting(SettingNames.IsGlobeProxy, IsGlobeProxy);
                break;
            case nameof(RoamingVideoAddress):
                WriteSetting(SettingNames.RoamingVideoAddress, RoamingVideoAddress);
                break;
            case nameof(RoamingViewAddress):
                WriteSetting(SettingNames.RoamingViewAddress, RoamingViewAddress);
                break;
            case nameof(RoamingSearchAddress):
                WriteSetting(SettingNames.RoamingSearchAddress, RoamingSearchAddress);
                break;
            case nameof(IsFullTraditionalChinese):
                WriteSetting(SettingNames.IsFullTraditionalChinese, IsFullTraditionalChinese);
                break;
            case nameof(DecodeType):
                WriteSetting(SettingNames.DecodeType, DecodeType);
                break;
            case nameof(PreferQuality):
                WriteSetting(SettingNames.PreferQuality, PreferQuality);
                break;
            case nameof(PreferAudioQuality):
                WriteSetting(SettingNames.PreferAudioQuality, PreferAudioQuality);
                break;
            case nameof(HideWhenCloseWindow):
                WriteSetting(SettingNames.HideWhenCloseWindow, HideWhenCloseWindow);
                break;
            case nameof(PlayerType):
                IsVideoNativePlayer = PlayerType == PlayerType.Native;
                WriteSetting(SettingNames.PlayerType, PlayerType);
                break;
            case nameof(VideoProcessor):
                WriteSetting(SettingNames.VideoProcessor, VideoProcessor);
                break;
            case nameof(PlayerWindowBehavior):
                WriteSetting(SettingNames.PlayerWindowBehaviorType, PlayerWindowBehavior);
                break;
            case nameof(IsWebDavEnabled):
                WriteSetting(SettingNames.IsWebDavEnabled, IsWebDavEnabled);
                AppViewModel.Instance.CheckWebDavVisibilityCommand.Execute(default);
                break;
            case nameof(IsWebSignIn):
                WriteSetting(SettingNames.IsWebSignIn, IsWebSignIn);
                break;
            case nameof(UseWebPlayer):
                WriteSetting(SettingNames.UseWebPlayer, UseWebPlayer);
                break;
            case nameof(UseMpvPlayer):
                WriteSetting(SettingNames.UseMpvPlayer, UseMpvPlayer);
                break;
            case nameof(SelectedWebDav):
                if (SelectedWebDav != null && SelectedWebDav.Id != SettingsToolkit.ReadLocalSetting(SettingNames.SelectedWebDav, string.Empty))
                {
                    WriteSetting(SettingNames.SelectedWebDav, SelectedWebDav.Id);
                    WriteSetting(SettingNames.WebDavLastPath, "/");
                }
                else if (SelectedWebDav == null)
                {
                    SettingsToolkit.DeleteLocalSetting(SettingNames.SelectedWebDav);
                }

                break;
            case nameof(IsPlayerControlModeManual):
                WriteSetting(SettingNames.IsPlayerControlModeManual, IsPlayerControlModeManual);
                break;
            case nameof(IsNotificationEnabled):
                WriteSetting(SettingNames.IsNotifyEnabled, IsNotificationEnabled);
                if (IsNotificationEnabled)
                {
                    NotificationViewModel.Instance.TryStartCommand.Execute(default);
                }
                else
                {
                    NotificationViewModel.Instance.TryStopCommand.Execute(default);
                }

                break;
            case nameof(IsVideoDynamicNotificationEnabled):
                WriteSetting(SettingNames.DynamicNotificationEnabled, IsVideoDynamicNotificationEnabled);
                break;
            case nameof(BottomProgressVisible):
                WriteSetting(SettingNames.BottomProgressVisible, BottomProgressVisible);
                break;
            case nameof(IsPlaybackRateSliderVisible):
                WriteSetting(SettingNames.PlaybackRateSliderEnabled, IsPlaybackRateSliderVisible);
                break;
            case nameof(NoP2P):
                WriteSetting(SettingNames.NoP2P, NoP2P);
                break;
            default:
                break;
        }
    }

    private void PlayerModeInit()
    {
        if (PlayerDisplayModeCollection.Count == 0)
        {
            PlayerDisplayModeCollection.Add(PlayerDisplayMode.Default);
            PlayerDisplayModeCollection.Add(PlayerDisplayMode.FullScreen);
            PlayerDisplayModeCollection.Add(PlayerDisplayMode.CompactOverlay);
        }

        DefaultPlayerDisplayMode = ReadSetting(SettingNames.DefaultPlayerDisplayMode, PlayerDisplayMode.Default);
    }

    private void PreferCodecInit()
    {
        if (PreferCodecCollection.Count == 0)
        {
            PreferCodecCollection.Add(PreferCodec.H264);
            PreferCodecCollection.Add(PreferCodec.H265);
            PreferCodecCollection.Add(PreferCodec.Av1);
        }

        PreferCodec = ReadSetting(SettingNames.PreferCodec, PreferCodec.H264);
    }

    private void PlayerTypeInit()
    {
        if (BiliPlayerTypes.Count == 0)
        {
            BiliPlayerTypes.Add(PlayerType.Native);
            BiliPlayerTypes.Add(PlayerType.FFmpeg);

            // PlayerTypes.Add(PlayerType.Vlc);
        }

        if (WebDavPlayerTypes.Count == 0)
        {
            WebDavPlayerTypes.Add(PlayerType.Native);
            WebDavPlayerTypes.Add(PlayerType.FFmpeg);
            WebDavPlayerTypes.Add(PlayerType.Mpv);
        }

        PlayerType = ReadSetting(SettingNames.PlayerType, PlayerType.Native);
        WebDavPlayerType = ReadSetting(SettingNames.WebDavPlayerType, PlayerType.FFmpeg);
    }

    private void DecodeInit()
    {
        if (DecodeTypeCollection.Count == 0)
        {
            DecodeTypeCollection.Add(DecodeType.HardwareDecode);
            DecodeTypeCollection.Add(DecodeType.SoftwareDecode);
        }

        DecodeType = ReadSetting(SettingNames.DecodeType, DecodeType.HardwareDecode);
    }

    private void PreferQualityInit()
    {
        if (PreferQualities.Count == 0)
        {
            PreferQualities.Add(PreferQuality.Auto);
            PreferQualities.Add(PreferQuality.HDFirst);
            PreferQualities.Add(PreferQuality.UHDFirst);
            PreferQualities.Add(PreferQuality.HighQuality);
        }

        PreferQuality = ReadSetting(SettingNames.PreferQuality, PreferQuality.HDFirst);
    }

    private void PreferAudioInit()
    {
        if (PreferAudioQualities.Count == 0)
        {
            PreferAudioQualities.Add(PreferAudio.Standard);
            PreferAudioQualities.Add(PreferAudio.HighQuality);
            PreferAudioQualities.Add(PreferAudio.Near);
        }

        PreferAudioQuality = ReadSetting(SettingNames.PreferAudioQuality, PreferAudio.Standard);
    }

    private void RoamingInit()
    {
        IsOpenRoaming = ReadSetting(SettingNames.IsOpenRoaming, false);
        IsGlobeProxy = ReadSetting(SettingNames.IsGlobeProxy, false);
        RoamingVideoAddress = ReadSetting(SettingNames.RoamingVideoAddress, string.Empty);
        RoamingViewAddress = ReadSetting(SettingNames.RoamingViewAddress, string.Empty);
        RoamingSearchAddress = ReadSetting(SettingNames.RoamingSearchAddress, string.Empty);
    }

    private void CheckTheme()
    {
        AppThemeText = AppTheme switch
        {
            ElementTheme.Light => ResourceToolkit.GetLocalizedString(StringNames.LightTheme),
            ElementTheme.Dark => ResourceToolkit.GetLocalizedString(StringNames.DarkTheme),
            _ => ResourceToolkit.GetLocalizedString(StringNames.SystemDefault),
        };
    }

    private void CheckWebSignInText()
        => WebSignInStatus = IsWebSignIn ? ResourceToolkit.GetLocalizedString(StringNames.SignedIn) : ResourceToolkit.GetLocalizedString(StringNames.NotVerify);

    partial void OnAppThemeChanged(ElementTheme value)
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.AppTheme, value);
        CheckTheme();
    }

    partial void OnIsWebSignInChanged(bool value)
        => CheckWebSignInText();

    partial void OnWebDavPlayerTypeChanged(PlayerType oldValue, PlayerType newValue)
    {
        if (WebDavPlayerType == PlayerType.Mpv && !AppViewModel.Instance.IsMpvExist)
        {
            AppViewModel.Instance.ShowMessage(ResourceToolkit.GetLocalizedString(StringNames.NeedInstallMpv));
            WebDavPlayerType = oldValue;
            OnPropertyChanged(nameof(WebDavPlayerType));
            return;
        }

        WriteSetting(SettingNames.WebDavPlayerType, WebDavPlayerType);
    }
}
