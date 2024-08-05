// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Forms;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.WinUI.Share.ViewModels;

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
    private void Launch()
    {
        if (_tokenResolver.GetToken() is not null)
        {
            IsInitialLoading = true;
            new MainWindow().Activate();
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
}
