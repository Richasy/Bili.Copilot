// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Player;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel.Background;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 设置视图模型.
/// </summary>
public sealed partial class SettingsPageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPageViewModel"/> class.
    /// </summary>
    private SettingsPageViewModel()
    {
        PlayerDisplayModeCollection = new ObservableCollection<PlayerDisplayMode>();
        PreferCodecCollection = new ObservableCollection<PreferCodec>();
        DecodeTypeCollection = new ObservableCollection<DecodeType>();
        PreferQualities = new ObservableCollection<PreferQuality>();
        PreferAudioQualities = new ObservableCollection<PreferAudio>();
        PlayerTypes = new ObservableCollection<PlayerType>();

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
        PlayerWindowBehavior = ReadSetting(SettingNames.PlayerWindowBehaviorType, PlayerWindowBehavior.Single);
        PreferCodecInit();
        DecodeInit();
        PlayerModeInit();
        PlayerTypeInit();

        // BackgroundTaskInitAsync();
        RoamingInit();
        PreferQualityInit();
        PreferAudioInit();

        var copyrightTemplate = ResourceToolkit.GetLocalizedString(StringNames.Copyright);
        Copyright = string.Format(copyrightTemplate, 2023);
        PackageVersion = AppToolkit.GetPackageVersion();
        PropertyChanged += OnPropertyChanged;
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
            case nameof(IsOpenDynamicNotification):
                WriteSetting(SettingNames.IsOpenNewDynamicNotify, IsOpenDynamicNotification);
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
                WriteSetting(SettingNames.PlayerType, PlayerType);
                break;
            case nameof(PlayerWindowBehavior):
                WriteSetting(SettingNames.PlayerWindowBehaviorType, PlayerWindowBehavior);
                break;
            default:
                break;
        }
    }

    private async void BackgroundTaskInitAsync()
    {
        IsOpenDynamicNotification = ReadSetting(SettingNames.IsOpenNewDynamicNotify, true);
        var status = await BackgroundExecutionManager.RequestAccessAsync();
        IsEnableBackgroundTask = status.ToString().Contains("Allowed");
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
        if (PlayerTypes.Count == 0)
        {
            PlayerTypes.Add(PlayerType.Native);
            PlayerTypes.Add(PlayerType.FFmpeg);

            // PlayerTypes.Add(PlayerType.Vlc);
        }

        PlayerType = ReadSetting(SettingNames.PlayerType, PlayerType.Native);
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

    partial void OnAppThemeChanged(ElementTheme value)
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.AppTheme, value);
        CheckTheme();
    }
}
