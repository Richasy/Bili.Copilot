// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Forms;
using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Windowing;
using Microsoft.Windows.BadgeNotifications;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Models.Media;
using Richasy.MpvKernel;
using Richasy.WinUIKernel.Share.Base;
using Richasy.WinUIKernel.Share.Toolkits;
using Richasy.WinUIKernel.Share.ViewModels;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.Storage;
using Windows.System;
using WinUIEx;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 应用视图模型.
/// </summary>
public sealed partial class AppViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppViewModel"/> class.
    /// </summary>
    public AppViewModel(
        ILogger<AppViewModel> logger,
        IBiliTokenResolver tokenResolver)
    {
        _logger = logger;
        _tokenResolver = tokenResolver;
    }

    public void OpenVideo(VideoSnapshot? snapshot, List<VideoInformation>? videos = default)
    {
        var sourceVM = this.Get<VideoSourceViewModel>();
        sourceVM.InjectSnapshot(snapshot);
        if (videos is not null)
        {
            sourceVM.InjectPlaylist(videos);
        }

        var uiProvider = new VideoUIProvider(sourceVM);
        var windowVM = new MpvPlayerWindowViewModel(sourceVM, uiProvider);
        windowVM.InitializeCommand.Execute(default);

        //if (videos is not null)
        //{
        //    new PlayerWindow().OpenVideo((videos!, snapshot!));
        //}
        //else
        //{
        //    new PlayerWindow().OpenVideo(snapshot);
        //}
    }

    [RelayCommand]
    private static void Restart()
    {
        Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().UnregisterKey();
        Microsoft.Windows.AppLifecycle.AppInstance.Restart(default);
    }

    [RelayCommand]
    private async Task InitializeExternalAsync()
    {
        var architecture = RuntimeInformation.ProcessArchitecture;
        var identifier = architecture == Architecture.Arm64 ? "arm64" : "x64";
        var libFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/libmpv/{identifier}/libmpv-2.dll")).AsTask();
        var libPath = libFile.Path;
        MpvNative.Initialize(libPath);

        var bbdownFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/BBDown/{identifier}/BBDown.exe")).AsTask();
        BBDownPath = bbdownFile.Path;

        var ffmpegFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/ffmpeg/{identifier}/ffmpeg.exe")).AsTask();
        FFmpegPath = ffmpegFile.Path;
    }

    [RelayCommand]
    private void Launch()
    {
        // 清除所有之前的badges.
        BadgeNotificationManager.Current.ClearBadge();
        if (_tokenResolver.GetToken() is not null)
        {
            IsInitialLoading = true;
            new MainWindow().Activate();
            InitializeExternalCommand.Execute(default);
        }
        else
        {
            new StartupWindow().Activate();
        }
    }

    [RelayCommand]
    private void ChangeTheme(ElementTheme theme)
    {
        foreach (var window in Windows)
        {
            (window.Content as FrameworkElement).RequestedTheme = theme;
        }
    }

    [RelayCommand]
    private void MakeCurrentWindowEnterFullScreen()
    {
        var window = ActivatedWindow;
        window.SetWindowPresenter(AppWindowPresenterKind.FullScreen);
        if (window is IPlayerHostWindow hostWindow)
        {
            hostWindow.EnterPlayerHostMode(PlayerDisplayMode.FullScreen);
        }
    }

    [RelayCommand]
    private void MakeCurrentWindowExitFullScreen()
    {
        var window = ActivatedWindow;
        window.SetWindowPresenter(AppWindowPresenterKind.Default);
        if (window is IPlayerHostWindow hostWindow)
        {
            hostWindow.ExitPlayerHostMode();
        }
    }

    [RelayCommand]
    private void MakeCurrentWindowEnterOverlap()
    {
        var window = ActivatedWindow;
        if (window.AppWindow.Presenter is not OverlappedPresenter)
        {
            window.SetWindowPresenter(AppWindowPresenterKind.Overlapped);
        }

        if (window is IPlayerHostWindow hostWindow)
        {
            hostWindow.EnterPlayerHostMode(PlayerDisplayMode.FullWindow);
        }
    }

    [RelayCommand]
    private void MakeCurrentWindowExitOverlap()
    {
        var window = ActivatedWindow;
        window.SetWindowPresenter(AppWindowPresenterKind.Default);
        if (window is IPlayerHostWindow hostWindow)
        {
            hostWindow.ExitPlayerHostMode();
        }
    }

    [RelayCommand]
    private void MakeCurrentWindowEnterCompactOverlay()
    {
        var window = ActivatedWindow;
        (window as WindowBase).MinHeight = 320;
        (window as WindowBase).MinWidth = 560;
        window.SetWindowPresenter(AppWindowPresenterKind.CompactOverlay);
        window.AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Standard;
        if (window is IPlayerHostWindow hostWindow)
        {
            hostWindow.EnterPlayerHostMode(PlayerDisplayMode.CompactOverlay);
        }
    }

    [RelayCommand]
    private void MakeCurrentWindowExitCompactOverlay()
    {
        var window = ActivatedWindow;
        (window as WindowBase).MinHeight = 480;
        (window as WindowBase).MinWidth = 640;
        window.SetWindowPresenter(AppWindowPresenterKind.Default);
        window.AppWindow.TitleBar.PreferredHeightOption = window is MainWindow ? TitleBarHeightOption.Tall : TitleBarHeightOption.Standard;
        if (window is IPlayerHostWindow hostWindow)
        {
            hostWindow.ExitPlayerHostMode();
        }
    }

    /// <summary>
    /// 显示提示.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    [RelayCommand]
    private async Task ShowTipAsync((string, InfoType) data)
    {
        if (ActivatedWindow is ITipWindow tipWindow)
        {
            await tipWindow.ShowTipAsync(data.Item1, data.Item2);
        }
        else
        {
            var firstWindow = Windows.OfType<ITipWindow>().FirstOrDefault();
            if (firstWindow is not null)
            {
                await firstWindow.ShowTipAsync(data.Item1, data.Item2);
            }
        }
    }

    [RelayCommand]
    private void CheckUpate()
    {
        var localVersion = SettingsToolkit.ReadLocalSetting(SettingNames.AppVersion, string.Empty);
        var currentVersion = this.Get<IAppToolkit>().GetPackageVersion();
        if (localVersion != currentVersion)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.AppVersion, currentVersion);
            IsUpdateShown = true;
        }
    }

    [RelayCommand]
    private void HideUpdate()
        => IsUpdateShown = false;

    [RelayCommand]
    private async Task ShowUpdateAsync()
    {
        var packVersion = this.Get<IAppToolkit>().GetPackageVersion();
        var url = $"https://github.com/Richasy/Bili.Copilot/releases/tag/v{packVersion}";
        await Launcher.LaunchUriAsync(new Uri(url));
        HideUpdate();
    }

    [RelayCommand]
    private async Task CheckGpuIsAmdAsync()
    {
        try
        {
            await Task.Run(() =>
            {
                // Initialize the process with required settings
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "wmic",
                    Arguments = "path win32_videocontroller get name",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

                using (var process = new Process { StartInfo = processStartInfo })
                {
                    process.Start();
                    var output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    // Process the output to check for AMD GPU
                    var lines = output.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines)
                    {
                        if (line.Contains("amd", StringComparison.CurrentCultureIgnoreCase))
                        {
                            IsAmdGpu = true;
                            break;
                        }
                    }
                }

                // Write local setting
                IsAmdGpu ??= false;
                SettingsToolkit.WriteLocalSetting(SettingNames.IsGpuChecked, true);
            });

            if (IsAmdGpu == true)
            {
                this.Get<Microsoft.UI.Dispatching.DispatcherQueue>().TryEnqueue(async () =>
                {
                    var dialog = new ContentDialog
                    {
                        Title = ResourceToolkit.GetLocalizedString(StringNames.Tip),
                        Content = ResourceToolkit.GetLocalizedString(StringNames.AmdGpuWarning),
                        XamlRoot = ActivatedWindow.Content.XamlRoot,
                        CloseButtonText = ResourceToolkit.GetLocalizedString(StringNames.Confirm),
                        DefaultButton = ContentDialogButton.Close,
                    };

                    await dialog.ShowAsync();
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "检查GPU类型时失败");
        }
    }

    private bool IsUseMpv()
        => SettingsToolkit.ReadLocalSetting(SettingNames.PlayerType, PlayerType.Island) == PlayerType.Island;

    partial void OnIsInitialLoadingChanged(bool value)
    {
        if (!value)
        {
            this.Get<NotificationViewModel>().TryStartCommand.Execute(default);
        }
    }
}
