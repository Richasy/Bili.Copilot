// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using CommunityToolkit.Mvvm.Input;
using Richasy.WinUI.Share.ViewModels;
using Windows.System;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 设置页面视图模型.
/// </summary>
public sealed partial class SettingsPageViewModel : ViewModelBase
{
    [RelayCommand]
    private void Initialize()
    {
        if (_isInitialized)
        {
            return;
        }

        AppTheme = SettingsToolkit.ReadLocalSetting(SettingNames.AppTheme, ElementTheme.Default);
        CheckTheme();
        IsAutoPlayWhenLoaded = SettingsToolkit.ReadLocalSetting(SettingNames.ShouldAutoPlay, true);
        IsAutoPlayNextRecommendVideo = SettingsToolkit.ReadLocalSetting(SettingNames.AutoPlayNextRecommendVideo, false);
        AutoPlayNext = SettingsToolkit.ReadLocalSetting(SettingNames.AutoPlayNext, false);
        SingleFastForwardAndRewindSpan = SettingsToolkit.ReadLocalSetting(SettingNames.SingleFastForwardAndRewindSpan, 15d);
        IsCopyScreenshot = SettingsToolkit.ReadLocalSetting(SettingNames.CopyAfterScreenshot, true);
        IsOpenScreenshotFile = SettingsToolkit.ReadLocalSetting(SettingNames.OpenAfterScreenshot, true);
        PlayerSpeedEnhancement = SettingsToolkit.ReadLocalSetting(SettingNames.IsPlayerSpeedEnhancement, false);
        GlobalPlayerSpeed = SettingsToolkit.ReadLocalSetting(SettingNames.IsPlayerSpeedShare, true);
        HideWhenCloseWindow = SettingsToolkit.ReadLocalSetting(SettingNames.HideWhenCloseWindow, false);
        AutoLoadHistory = SettingsToolkit.ReadLocalSetting(SettingNames.AutoLoadHistory, true);
        IsNotificationEnabled = SettingsToolkit.ReadLocalSetting(SettingNames.IsNotificationEnabled, true);
        IsVideoMomentNotificationEnabled = SettingsToolkit.ReadLocalSetting(SettingNames.IsVideoMomentNotificationEnabled, true);
        NoP2P = SettingsToolkit.ReadLocalSetting(SettingNames.PlayWithoutP2P, false);
        PlayerDisplayModeCollection = Enum.GetValues<PlayerDisplayMode>().ToList();
        PreferCodecCollection = Enum.GetValues<PreferCodecType>().ToList();
        PreferQualityCollection = Enum.GetValues<PreferQualityType>().ToList();
        PreferDecodeCollection = Enum.GetValues<PreferDecodeType>().ToList();
        PlayerTypeCollection = Enum.GetValues<PlayerType>().ToList();
        DefaultPlayerDisplayMode = SettingsToolkit.ReadLocalSetting(SettingNames.DefaultPlayerDisplayMode, PlayerDisplayMode.Default);
        PreferCodec = SettingsToolkit.ReadLocalSetting(SettingNames.PreferCodec, PreferCodecType.H264);
        PreferQuality = SettingsToolkit.ReadLocalSetting(SettingNames.PreferQuality, PreferQualityType.Auto);
        PreferDecode = SettingsToolkit.ReadLocalSetting(SettingNames.PreferDecode, PreferDecodeType.Software);
        PlayerType = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerType, PlayerType.Native);
        BottomProgressVisible = SettingsToolkit.ReadLocalSetting(SettingNames.IsBottomProgressVisible, true);
        DefaultDownloadPath = SettingsToolkit.ReadLocalSetting(SettingNames.DownloadFolder, string.Empty);
        UseExternalBBDown = SettingsToolkit.ReadLocalSetting(SettingNames.UseExternalBBDown, false);
        OnlyCopyCommandWhenDownload = SettingsToolkit.ReadLocalSetting(SettingNames.OnlyCopyCommandWhenDownload, false);
        WithoutCredentialWhenGenDownloadCommand = SettingsToolkit.ReadLocalSetting(SettingNames.WithoutCredentialWhenGenDownloadCommand, false);
        if (string.IsNullOrEmpty(DefaultDownloadPath))
        {
            DefaultDownloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "Bili Downloads");
        }

        OpenFolderAfterDownload = SettingsToolkit.ReadLocalSetting(SettingNames.OpenFolderAfterDownload, true);
        DownloadWithDanmaku = SettingsToolkit.ReadLocalSetting(SettingNames.DownloadWithDanmaku, false);

        var copyrightTemplate = ResourceToolkit.GetLocalizedString(StringNames.Copyright);
        Copyright = string.Format(copyrightTemplate, 2024);
        PackageVersion = AppToolkit.GetPackageVersion();

        InitializeWebDavConfigCommand.Execute(default);

        _isInitialized = true;
    }

    [RelayCommand]
    private async Task ChooseDownloadFolderAsync()
    {
        var folder = await FileToolkit.PickFolderAsync(this.Get<AppViewModel>().ActivatedWindow);
        if (folder != null)
        {
            DefaultDownloadPath = folder.Path;
        }
    }

    [RelayCommand]
    private async Task OpenDownloadFolderAsync()
    {
        if (!Directory.Exists(DefaultDownloadPath))
        {
            Directory.CreateDirectory(DefaultDownloadPath);
        }

        await Launcher.LaunchFolderPathAsync(DefaultDownloadPath);
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
        this.Get<AppViewModel>().ChangeThemeCommand.Execute(value);
    }

    partial void OnAutoPlayNextChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.AutoPlayNext, value);

    partial void OnAutoLoadHistoryChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.AutoLoadHistory, value);

    partial void OnIsAutoPlayWhenLoadedChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.ShouldAutoPlay, value);

    partial void OnDefaultPlayerDisplayModeChanged(PlayerDisplayMode value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.DefaultPlayerDisplayMode, value);

    partial void OnPreferCodecChanged(PreferCodecType value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.PreferCodec, value);

    partial void OnPreferDecodeChanged(PreferDecodeType value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.PreferDecode, value);

    partial void OnPlayerTypeChanged(PlayerType value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.PlayerType, value);

    partial void OnSingleFastForwardAndRewindSpanChanged(double value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.SingleFastForwardAndRewindSpan, value);

    partial void OnIsCopyScreenshotChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.CopyAfterScreenshot, value);

    partial void OnIsOpenScreenshotFileChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.OpenAfterScreenshot, value);

    partial void OnPlayerSpeedEnhancementChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.IsPlayerSpeedEnhancement, value);

    partial void OnGlobalPlayerSpeedChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.IsPlayerSpeedShare, value);

    partial void OnIsAutoPlayNextRecommendVideoChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.AutoPlayNextRecommendVideo, value);

    partial void OnPreferQualityChanged(PreferQualityType value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.PreferQuality, value);

    partial void OnHideWhenCloseWindowChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.HideWhenCloseWindow, value);

    partial void OnIsNotificationEnabledChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.IsNotificationEnabled, value);

    partial void OnIsVideoMomentNotificationEnabledChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.IsVideoMomentNotificationEnabled, value);

    partial void OnBottomProgressVisibleChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.IsBottomProgressVisible, value);

    partial void OnNoP2PChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.PlayWithoutP2P, value);

    partial void OnDefaultDownloadPathChanged(string value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.DownloadFolder, value);

    partial void OnDownloadWithDanmakuChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.DownloadWithDanmaku, value);

    partial void OnOpenFolderAfterDownloadChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.OpenFolderAfterDownload, value);

    partial void OnUseExternalBBDownChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.UseExternalBBDown, value);

    partial void OnOnlyCopyCommandWhenDownloadChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.OnlyCopyCommandWhenDownload, value);

    partial void OnWithoutCredentialWhenGenDownloadCommandChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.WithoutCredentialWhenGenDownloadCommand, value);

    partial void OnIsWebDavEnabledChanged(bool value)
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.IsWebDavEnabled, value);
        this.Get<NavigationViewModel>().CheckWebDavItemCommand.Execute(default);
    }

    partial void OnSelectedWebDavChanged(WebDavConfig value)
    {
        if (value is null)
        {
            SettingsToolkit.DeleteLocalSetting(SettingNames.SelectedWebDavConfigId);
        }
        else
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.SelectedWebDavConfigId, value.Id);
        }
    }
}
