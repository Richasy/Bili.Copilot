// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Controls.AI;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Pages;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using CommunityToolkit.Mvvm.Input;
using Humanizer;
using Microsoft.Extensions.Logging;
using Richasy.MpvKernel;
using Richasy.MpvKernel.Core.Enums;
using Richasy.WinUIKernel.AI.ViewModels;
using Richasy.WinUIKernel.Share;
using Richasy.WinUIKernel.Share.Base;
using Richasy.WinUIKernel.Share.Toolkits;
using Windows.Storage;
using Windows.System;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 设置页面视图模型.
/// </summary>
public sealed partial class SettingsPageViewModel : AISettingsViewModelBase
{
    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (_isInitialized)
        {
            await LoadCacheSizeAsync();
            return;
        }

        AppTheme = SettingsToolkit.ReadLocalSetting(SettingNames.AppTheme, ElementTheme.Default);
        CheckTheme();
        LogLevel = SettingsToolkit.ReadLocalSetting(SettingNames.MpvLogLevel, MpvLogLevel.Warn);
        HideMainWindowOnPlay = SettingsToolkit.ReadLocalSetting(SettingNames.HideMainWindowOnPlay, true);
        IsTopNavShown = SettingsToolkit.ReadLocalSetting(SettingNames.IsTopNavBarShown, true);
        IsAutoPlayNextRecommendVideo = SettingsToolkit.ReadLocalSetting(SettingNames.AutoPlayNextRecommendVideo, false);
        PlayNextWithoutTip = SettingsToolkit.ReadLocalSetting(SettingNames.PlayNextWithoutTip, false);
        EndWithPlaylist = SettingsToolkit.ReadLocalSetting(SettingNames.EndWithPlaylist, true);
        IsCopyScreenshot = SettingsToolkit.ReadLocalSetting(SettingNames.CopyAfterScreenshot, true);
        IsOpenScreenshotFile = SettingsToolkit.ReadLocalSetting(SettingNames.OpenAfterScreenshot, true);
        PlayerSpeedEnhancement = SettingsToolkit.ReadLocalSetting(SettingNames.IsPlayerSpeedEnhancement, false);
        GlobalPlayerSpeed = SettingsToolkit.ReadLocalSetting(SettingNames.IsPlayerSpeedShare, true);
        HideWhenCloseWindow = SettingsToolkit.ReadLocalSetting(SettingNames.HideWhenCloseWindow, false);
        IsNotificationEnabled = SettingsToolkit.ReadLocalSetting(SettingNames.IsNotificationEnabled, true);
        IsVideoMomentNotificationEnabled = SettingsToolkit.ReadLocalSetting(SettingNames.IsVideoMomentNotificationEnabled, true);
        NoP2P = SettingsToolkit.ReadLocalSetting(SettingNames.PlayWithoutP2P, false);
        PlayerDisplayModeCollection = Enum.GetValues<PlayerDisplayMode>().ToList();
        PreferCodecCollection = Enum.GetValues<PreferCodecType>().ToList();
        PreferQualityCollection = Enum.GetValues<PreferQualityType>().ToList();
        PreferDecodeCollection = Enum.GetValues<PreferDecodeType>().ToList();
        MTCBehaviorCollection = Enum.GetValues<MTCBehavior>().ToList();
        DefaultPlayerDisplayMode = SettingsToolkit.ReadLocalSetting(SettingNames.DefaultPlayerDisplayMode, PlayerDisplayMode.Default);
        PreferCodec = SettingsToolkit.ReadLocalSetting(SettingNames.PreferCodec, PreferCodecType.H264);
        PreferQuality = SettingsToolkit.ReadLocalSetting(SettingNames.PreferQuality, PreferQualityType.Auto);
        IsPopularNavVisible = this.Get<ISettingsToolkit>().ReadLocalSetting($"Is{typeof(PopularPage).Name}Visible", true);
        IsMomentNavVisible = this.Get<ISettingsToolkit>().ReadLocalSetting($"Is{typeof(MomentPage).Name}Visible", true);
        IsVideoNavVisible = this.Get<ISettingsToolkit>().ReadLocalSetting($"Is{typeof(VideoPartitionPage).Name}Visible", true);
        IsLiveNavVisible = this.Get<ISettingsToolkit>().ReadLocalSetting($"Is{typeof(LivePartitionPage).Name}Visible", true);
        IsAnimeNavVisible = this.Get<ISettingsToolkit>().ReadLocalSetting($"Is{typeof(AnimePage).Name}Visible", true);
        IsCinemaNavVisible = this.Get<ISettingsToolkit>().ReadLocalSetting($"Is{typeof(CinemaPage).Name}Visible", true);
        IsArticleNavVisible = this.Get<ISettingsToolkit>().ReadLocalSetting($"Is{typeof(ArticlePartitionPage).Name}Visible", true);
        StepBackwardSecond = SettingsToolkit.ReadLocalSetting(SettingNames.StepBackwardSecond, 10d);
        StepForwardSecond = SettingsToolkit.ReadLocalSetting(SettingNames.StepForwardSecond, 30d);
        PreferAudioChannelLayout = SettingsToolkit.ReadLocalSetting(SettingNames.PreferAudioChannelLayout, Richasy.MpvKernel.Core.Enums.AudioChannelLayoutType.Auto);
        MpvBuiltInProfile = SettingsToolkit.ReadLocalSetting(SettingNames.MpvBuiltInProfile, MpvBuiltInProfile.HighQuality);
        UseIntegrationWhenSinglePlayWindow = SettingsToolkit.ReadLocalSetting(SettingNames.UseIntegrationWhenSinglePlayWindow, true);
        MaxCacheSize = SettingsToolkit.ReadLocalSetting(SettingNames.MaxCacheSize, 300d); // Default to 300 MB
        MaxCacheSeconds = SettingsToolkit.ReadLocalSetting(SettingNames.MaxCacheSeconds, 300d); // Default to 300 seconds
        HrSeek = SettingsToolkit.ReadLocalSetting(SettingNames.HrSeek, HrSeekType.Default);
        MaxVolume = SettingsToolkit.ReadLocalSetting(SettingNames.MaxVolume, 100d);
        AudioExclusiveEnabled = SettingsToolkit.ReadLocalSetting(SettingNames.AudioExclusiveEnabled, false);
        CacheOnDisk = SettingsToolkit.ReadLocalSetting(SettingNames.CacheOnDisk, false);
        var cacheDir = SettingsToolkit.ReadLocalSetting(SettingNames.CacheDir, string.Empty);
        HasCustomCacheDir = !string.IsNullOrEmpty(cacheDir);
        CacheDirDescription = string.IsNullOrEmpty(cacheDir) ? "--demuxer-cache-dir" : cacheDir;
        ScrollAccelerate = SettingsToolkit.ReadLocalSetting(SettingNames.ScrollAccelerate, true);
        TempPlaybackRate = SettingsToolkit.ReadLocalSetting(SettingNames.TempPlaybackRate, 3.0);
        try
        {
            PreferDecode = SettingsToolkit.ReadLocalSetting(SettingNames.PreferDecode, PreferDecodeType.Auto);
        }
        catch (Exception)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.PreferDecode, PreferDecodeType.Auto);
            PreferDecode = PreferDecodeType.Auto;
        }

        MTCBehavior = SettingsToolkit.ReadLocalSetting(SettingNames.MTCBehavior, MTCBehavior.Automatic);
        BottomProgressVisible = SettingsToolkit.ReadLocalSetting(SettingNames.IsBottomProgressBarVisible, true);
        DefaultDownloadPath = SettingsToolkit.ReadLocalSetting(SettingNames.DownloadFolder, string.Empty);
        UseExternalBBDown = SettingsToolkit.ReadLocalSetting(SettingNames.UseExternalBBDown, false);
        OnlyCopyCommandWhenDownload = SettingsToolkit.ReadLocalSetting(SettingNames.OnlyCopyCommandWhenDownload, false);
        WithoutCredentialWhenGenDownloadCommand = SettingsToolkit.ReadLocalSetting(SettingNames.WithoutCredentialWhenGenDownloadCommand, false);
        FilterAISubtitle = SettingsToolkit.ReadLocalSetting(SettingNames.FilterAISubtitle, true);
        IsAIStreamingResponse = SettingsToolkit.ReadLocalSetting(SettingNames.IsAIStreamingResponse, true);
        if (string.IsNullOrEmpty(DefaultDownloadPath))
        {
            DefaultDownloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "Bili Downloads");
        }

        OpenFolderAfterDownload = SettingsToolkit.ReadLocalSetting(SettingNames.OpenFolderAfterDownload, true);
        DownloadWithDanmaku = SettingsToolkit.ReadLocalSetting(SettingNames.DownloadWithDanmaku, false);
        UseWebPlayerWhenLive = SettingsToolkit.ReadLocalSetting(SettingNames.UseWebPlayerWhenLive, false);
        ShowSearchRecommend = SettingsToolkit.ReadLocalSetting(SettingNames.ShowSearchRecommend, false);

        ScreenshotAction = SettingsToolkit.ReadLocalSetting(SettingNames.ScreenshotAction, ScreenshotAction.Open);
        var customScreenshotFolderPath = SettingsToolkit.ReadLocalSetting(SettingNames.CustomScreenshotFolderPath, string.Empty);
        if (string.IsNullOrEmpty(customScreenshotFolderPath) || !Directory.Exists(customScreenshotFolderPath))
        {
            ScreenshotFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Bili screenshots");
        }
        else
        {
            ScreenshotFolderPath = customScreenshotFolderPath;
        }

        var copyrightTemplate = ResourceToolkit.GetLocalizedString(StringNames.Copyright);
        Copyright = string.Format(copyrightTemplate, 2024);
        PackageVersion = this.Get<IAppToolkit>().GetPackageVersion();
        CheckMpvConfigVisible();
        LoadCustomLibmpvSettings();
        await LoadCacheSizeAsync();
        _isInitialized = true;
    }

    [RelayCommand]
    private async Task OpenMpvConfigInEditorAsync()
    {
        try
        {
            var path = await AppToolkit.EnsureMpvConfigExistAsync();
            var file = await StorageFile.GetFileFromPathAsync(path).AsTask();
            var isSuccess = await Launcher.LaunchFileAsync(file).AsTask();
            if (!isSuccess)
            {
                await AppToolkit.OpenAndSelectFileInExplorerAsync(file.Path, false);
            }

            var isFirstOpen = SettingsToolkit.ReadLocalSetting(SettingNames.FirstOpenMpvEditor, true);
            if (isFirstOpen)
            {
                var dialog = new ContentDialog
                {
                    Title = ResourceToolkit.GetLocalizedString(StringNames.Tip),
                    Content = ResourceToolkit.GetLocalizedString(StringNames.FirstOpenMpvConfigTip),
                    CloseButtonText = ResourceToolkit.GetLocalizedString(StringNames.Confirm),
                    DefaultButton = ContentDialogButton.Close,
                    XamlRoot = this.Get<IXamlRootProvider>().XamlRoot,
                };

                await dialog.ShowAsync();
                SettingsToolkit.WriteLocalSetting(SettingNames.FirstOpenMpvEditor, false);
            }
        }
        catch (Exception ex)
        {
            this.Get<ILogger<SettingsPageViewModel>>().LogError(ex, "打开MPV配置文件失败");
            this.Get<AppViewModel>().ShowTipCommand.Execute((ex.Message, InfoType.Error));
        }
    }

    [RelayCommand]
    private async Task ClearCacheAsync()
    {
        var dialog = new ContentDialog
        {
            Title = ResourceToolkit.GetLocalizedString(StringNames.ClearCache),
            Content = ResourceToolkit.GetLocalizedString(StringNames.ClearCacheTip),
            PrimaryButtonText = ResourceToolkit.GetLocalizedString(StringNames.Confirm),
            CloseButtonText = ResourceToolkit.GetLocalizedString(StringNames.Cancel),
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = this.Get<IXamlRootProvider>().XamlRoot,
        };
        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            var cacheFolder = await Microsoft.Windows.Storage.ApplicationData.GetDefault().LocalCacheFolder.CreateFolderAsync("ImageExCache", CreationCollisionOption.OpenIfExists);
            await cacheFolder.DeleteAsync(StorageDeleteOption.PermanentDelete);
            await LoadCacheSizeAsync();
        }
    }

    private async Task LoadCacheSizeAsync()
    {
        var cacheFolder = await Microsoft.Windows.Storage.ApplicationData.GetDefault().LocalCacheFolder.CreateFolderAsync("ImageExCache", CreationCollisionOption.OpenIfExists);
        var size = Directory.GetFiles(cacheFolder.Path, "*.*", SearchOption.AllDirectories)
            .Sum(file => new FileInfo(file).Length);
        IsTotalImageCacheClearEnabled = size > 0;
        if (size == 0)
        {
            TotalImageCacheSize = ResourceToolkit.GetLocalizedString(StringNames.NoCache);
        }
        else
        {
            TotalImageCacheSize = size.Bits().Humanize();
        }
    }

    private void LoadCustomLibmpvSettings()
    {
        var customLibmpvPath = SettingsToolkit.ReadLocalSetting(SettingNames.CustomLibmpvPath, string.Empty);
        var isValidMpvPath = !string.IsNullOrEmpty(customLibmpvPath) && File.Exists(customLibmpvPath);
        if (!isValidMpvPath && !string.IsNullOrEmpty(customLibmpvPath))
        {
            SettingsToolkit.DeleteLocalSetting(SettingNames.CustomLibmpvPath);
        }

        _initialCustomLibMpvPath = isValidMpvPath ? customLibmpvPath : ResourceToolkit.GetLocalizedString(StringNames.InternalLibMpv);
        CustomLibMpvPath = _initialCustomLibMpvPath;
        CanResetLibMpvPath = isValidMpvPath;
    }

    private void CheckRestartTip()
        => IsRestartTipShown = CustomLibMpvPath != _initialCustomLibMpvPath;

    [RelayCommand]
    private async Task SelectLibMpvFileAsync()
    {
        var file = await this.Get<IFileToolkit>().PickFileAsync(".dll", this.Get<AppViewModel>().ActivatedWindow);
        if (file is not null)
        {
            CustomLibMpvPath = file.Path;
            SettingsToolkit.WriteLocalSetting(SettingNames.CustomLibmpvPath, CustomLibMpvPath);
            CanResetLibMpvPath = true;
        }

        CheckRestartTip();
    }

    [RelayCommand]
    private void ResetLibMpvPath()
    {
        CustomLibMpvPath = ResourceToolkit.GetLocalizedString(StringNames.InternalLibMpv);
        SettingsToolkit.DeleteLocalSetting(SettingNames.CustomLibmpvPath);
        CanResetLibMpvPath = false;
        CheckRestartTip();
    }

    [RelayCommand]
    private async Task ChooseDownloadFolderAsync()
    {
        var folder = await this.Get<IFileToolkit>().PickFolderAsync(this.Get<AppViewModel>().ActivatedWindow);
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

    [RelayCommand]
    private async Task EditVideoSummarizeAsync()
    {
        if (!FileToolkit.IsLocalDataExist("video_summarize.txt", "Prompt"))
        {
            await this.Get<IFileToolkit>().WriteLocalDataAsync("video_summarize.txt", PromptConstants.VideoSummaryPrompt, default, "Prompt");
        }

        await new CustomPromptDialog("video_summarize", ResourceToolkit.GetLocalizedString(StringNames.VideoSummarize))
        { XamlRoot = this.Get<AppViewModel>().ActivatedWindow.Content.XamlRoot }.ShowAsync();
    }

    [RelayCommand]
    private async Task EditVideoEvaluationAsync()
    {
        if (!FileToolkit.IsLocalDataExist("video_evaluation.txt", "Prompt"))
        {
            await this.Get<IFileToolkit>().WriteLocalDataAsync("video_evaluation.txt", PromptConstants.VideoEvaluationPrompt, default, "Prompt");
        }

        await new CustomPromptDialog("video_evaluation", ResourceToolkit.GetLocalizedString(StringNames.VideoEvaluation))
        { XamlRoot = this.Get<AppViewModel>().ActivatedWindow.Content.XamlRoot }.ShowAsync();
    }

    [RelayCommand]
    private async Task EditArticleSummarizeAsync()
    {
        if (!FileToolkit.IsLocalDataExist("article_summarize.txt", "Prompt"))
        {
            await this.Get<IFileToolkit>().WriteLocalDataAsync("article_summarize.txt", PromptConstants.ArticleSummaryPrompt, default, "Prompt");
        }

        await new CustomPromptDialog("article_summarize", ResourceToolkit.GetLocalizedString(StringNames.ArticleSummarize))
        { XamlRoot = this.Get<AppViewModel>().ActivatedWindow.Content.XamlRoot }.ShowAsync();
    }

    [RelayCommand]
    private async Task EditArticleEvaluationAsync()
    {
        if (!FileToolkit.IsLocalDataExist("article_evaluation.txt", "Prompt"))
        {
            await this.Get<IFileToolkit>().WriteLocalDataAsync("article_evaluation.txt", PromptConstants.ArticleEvaluationPrompt, default, "Prompt");
        }

        await new CustomPromptDialog("article_evaluation", ResourceToolkit.GetLocalizedString(StringNames.ArticleEvaluation))
        { XamlRoot = this.Get<AppViewModel>().ActivatedWindow.Content.XamlRoot }.ShowAsync();
    }

    [RelayCommand]
    private async Task PickScreenshotFolderAsync()
    {
        var folder = await this.Get<IFileToolkit>().PickFolderAsync(this.Get<AppViewModel>().ActivatedWindow);
        if (folder != null)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.CustomScreenshotFolderPath, folder.Path);
            ScreenshotFolderPath = folder.Path;
        }
    }

    private void CheckMpvConfigVisible()
    {
        IsMpvCustomOptionVisible = PreferDecode == PreferDecodeType.Custom;
        if (IsMpvCustomOptionVisible)
        {
            IsMpvExtraSettingExpanded = false;
        }
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

    private void WriteNavVisibleSetting(Type pageType, bool isVisible)
    {
        this.Get<ISettingsToolkit>().WriteLocalSetting($"Is{pageType.Name}Visible", isVisible);
        this.Get<NavigationViewModel>().SetNavItemVisibility(pageType, isVisible);
    }

    partial void OnAppThemeChanged(ElementTheme value)
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.AppTheme, value);
        CheckTheme();
        this.Get<AppViewModel>().ChangeThemeCommand.Execute(value);
    }

    partial void OnLogLevelChanged(MpvLogLevel value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.MpvLogLevel, value);

    partial void OnHideMainWindowOnPlayChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.HideMainWindowOnPlay, value);

    partial void OnStepBackwardSecondChanged(double value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.StepBackwardSecond, value);

    partial void OnStepForwardSecondChanged(double value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.StepForwardSecond, value);

    partial void OnMaxCacheSizeChanged(double value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.MaxCacheSize, value);

    partial void OnMaxCacheSecondsChanged(double value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.MaxCacheSeconds, value);

    partial void OnMpvBuiltInProfileChanged(MpvBuiltInProfile value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.MpvBuiltInProfile, value);

    partial void OnHrSeekChanged(HrSeekType value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.HrSeek, value);

    partial void OnUseIntegrationWhenSinglePlayWindowChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.UseIntegrationWhenSinglePlayWindow, value);

    partial void OnPreferAudioChannelLayoutChanged(AudioChannelLayoutType value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.PreferAudioChannelLayout, value);

    partial void OnDefaultPlayerDisplayModeChanged(PlayerDisplayMode value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.DefaultPlayerDisplayMode, value);

    partial void OnPreferCodecChanged(PreferCodecType value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.PreferCodec, value);

    partial void OnPreferDecodeChanged(PreferDecodeType value)
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.PreferDecode, value);
        CheckMpvConfigVisible();
    }

    partial void OnIsCopyScreenshotChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.CopyAfterScreenshot, value);

    partial void OnIsOpenScreenshotFileChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.OpenAfterScreenshot, value);

    partial void OnPlayerSpeedEnhancementChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.IsPlayerSpeedEnhancement, value);

    partial void OnGlobalPlayerSpeedChanged(bool value)
    {
        if (!value)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.PlayerSpeed, 1.0d);
        }

        SettingsToolkit.WriteLocalSetting(SettingNames.IsPlayerSpeedShare, value);
    }

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
        => SettingsToolkit.WriteLocalSetting(SettingNames.IsBottomProgressBarVisible, value);

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

    partial void OnFilterAISubtitleChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.FilterAISubtitle, value);

    partial void OnPlayNextWithoutTipChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.PlayNextWithoutTip, value);

    partial void OnIsTopNavShownChanged(bool value)
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.IsTopNavBarShown, value);
        this.Get<NavigationViewModel>().IsTopNavBarShown = value;
    }

    partial void OnMTCBehaviorChanged(MTCBehavior value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.MTCBehavior, value);

    partial void OnIsAIStreamingResponseChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.IsAIStreamingResponse, value);

    partial void OnUseWebPlayerWhenLiveChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.UseWebPlayerWhenLive, value);

    partial void OnShowSearchRecommendChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.ShowSearchRecommend, value);

    partial void OnIsPopularNavVisibleChanged(bool value)
        => WriteNavVisibleSetting(typeof(PopularPage), value);

    partial void OnIsMomentNavVisibleChanged(bool value)
        => WriteNavVisibleSetting(typeof(MomentPage), value);

    partial void OnIsVideoNavVisibleChanged(bool value)
        => WriteNavVisibleSetting(typeof(VideoPartitionPage), value);

    partial void OnIsLiveNavVisibleChanged(bool value)
        => WriteNavVisibleSetting(typeof(LivePartitionPage), value);

    partial void OnIsAnimeNavVisibleChanged(bool value)
        => WriteNavVisibleSetting(typeof(AnimePage), value);

    partial void OnIsCinemaNavVisibleChanged(bool value)
        => WriteNavVisibleSetting(typeof(CinemaPage), value);

    partial void OnIsArticleNavVisibleChanged(bool value)
        => WriteNavVisibleSetting(typeof(ArticlePartitionPage), value);

    partial void OnScreenshotActionChanged(ScreenshotAction value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.ScreenshotAction, value);

    partial void OnScreenshotFolderPathChanged(string? value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.CustomScreenshotFolderPath, value ?? string.Empty);

    partial void OnMaxVolumeChanged(double value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.MaxVolume, value);

    partial void OnAudioExclusiveEnabledChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.AudioExclusiveEnabled, value);

    partial void OnCacheOnDiskChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.CacheOnDisk, value);

    partial void OnScrollAccelerateChanged(bool value)
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.ScrollAccelerate, value);
        if (value)
        {
            this.Get<AppViewModel>().UseQuickWheelScrollCommand.Execute(default);
        }
        else
        {
            this.Get<AppViewModel>().RestoreOriginalWheelScrollCommand.Execute(default);
        }
    }

    partial void OnTempPlaybackRateChanged(double value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.TempPlaybackRate, value);
}
