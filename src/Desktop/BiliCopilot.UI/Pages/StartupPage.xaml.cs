// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Forms;
using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.View;
using Richasy.WinUIKernel.Share.Base;
using Windows.System;

namespace BiliCopilot.UI.Pages;

/// <summary>
/// 初始登录页面.
/// </summary>
public sealed partial class StartupPage : StartupPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StartupPage"/> class.
    /// </summary>
    public StartupPage()
    {
        InitializeComponent();
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        ViewModel.InitializeCommand.Execute(QRCodeImage);
    }

    private void OnWebSignInButtonClick(object sender, RoutedEventArgs e)
    {
        var window = this.Get<AppViewModel>().Windows.FirstOrDefault(p => p is WebSignInWindow);
        if (window is not null)
        {
            window.Activate();
        }
        else
        {
            new WebSignInWindow().Activate();
        }
    }

    private async void OnRepoButtonClickAsync(object sender, RoutedEventArgs e)
        => await Launcher.LaunchUriAsync(new Uri("https://github.com/Richasy/Bili.Copilot"));

    private async void OnBiliButtonClickAsync(object sender, RoutedEventArgs e)
        => await Launcher.LaunchUriAsync(new Uri("https://space.bilibili.com/5992670"));

    private void OnRefreshQRButtonClick(object sender, RoutedEventArgs e)
        => ViewModel.ReloadQRCodeCommand.Execute(default);
}

/// <summary>
/// 初始登录页面基类.
/// </summary>
public abstract class StartupPageBase : LayoutPageBase<StartupPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StartupPageBase"/> class.
    /// </summary>
    public StartupPageBase() => ViewModel = this.Get<StartupPageViewModel>();
}
