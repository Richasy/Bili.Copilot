// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Forms;
using BiliCopilot.UI.Models.Constants;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Mpv.Core;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.WinUI.Share.Base;
using Richasy.WinUI.Share.ViewModels;
using Windows.Storage;
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
    private static async Task InitializeMpvAsync()
    {
        var libFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/libmpv/libmpv-2.dll")).AsTask();
        var libPath = libFile.Path;
        Resolver.SetCustomMpvPath(libPath);
    }

    [RelayCommand]
    private static void Restart()
    {
        Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().UnregisterKey();
        Microsoft.Windows.AppLifecycle.AppInstance.Restart(default);
    }

    [RelayCommand]
    private void Launch()
    {
        if (_tokenResolver.GetToken() is not null)
        {
            IsInitialLoading = true;
            new MainWindow().Activate();
            InitializeMpvCommand.Execute(default);
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
        window.SetWindowPresenter(Microsoft.UI.Windowing.AppWindowPresenterKind.FullScreen);
        if (window is IPlayerHostWindow hostWindow)
        {
            hostWindow.EnterPlayerHostMode();
        }
    }

    [RelayCommand]
    private void MakeCurrentWindowExitFullScreen()
    {
        var window = ActivatedWindow;
        window.SetWindowPresenter(Microsoft.UI.Windowing.AppWindowPresenterKind.Default);
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
        window.SetWindowPresenter(Microsoft.UI.Windowing.AppWindowPresenterKind.CompactOverlay);
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
        window.SetWindowPresenter(Microsoft.UI.Windowing.AppWindowPresenterKind.Default);
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
}
