// Copyright (c) Bili Copilot. All rights reserved.

using System.Runtime.InteropServices;
using BiliCopilot.UI.Forms;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Windowing;
using Mpv.Core;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.WinUI.Share.Base;
using Richasy.WinUI.Share.ViewModels;
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
        Resolver.SetCustomMpvPath(libPath);

        var bbdownFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/BBDown/{identifier}/BBDown.exe")).AsTask();
        BBDownPath = bbdownFile.Path;

        var ffmpegFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/ffmpeg/ffmpeg.exe")).AsTask();
        FFmpegPath = ffmpegFile.Path;
    }

    [RelayCommand]
    private void Launch()
    {
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
            hostWindow.EnterPlayerHostMode();
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
            hostWindow.EnterPlayerHostMode();
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
            hostWindow.EnterPlayerHostMode();
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
        var currentVersion = AppToolkit.GetPackageVersion();
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
        var packVersion = AppToolkit.GetPackageVersion();
        var url = $"https://github.com/Richasy/Bili.Copilot/releases/tag/v{packVersion}";
        await Launcher.LaunchUriAsync(new Uri(url));
        HideUpdate();
    }

    partial void OnIsInitialLoadingChanged(bool value)
    {
        if (!value)
        {
            this.Get<NotificationViewModel>().TryStartCommand.Execute(default);
        }
    }
}
