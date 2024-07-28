// Copyright (c) Bili Copilot. All rights reserved.

using System.Threading;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.View;
using Microsoft.UI.Windowing;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.WinUI.Share.Base;
using WinUIEx;

namespace BiliCopilot.UI.Forms;

/// <summary>
/// 网页登录窗口.
/// </summary>
public sealed partial class WebSignInWindow : WindowBase
{
    private CancellationTokenSource _cancellationTokenSource = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="WebSignInWindow"/> class.
    /// </summary>
    public WebSignInWindow()
    {
        InitializeComponent();
        AppWindow.SetPresenter(AppWindowPresenterKind.CompactOverlay);

        Width = 1200;
        Height = 632;

        Title = ResourceToolkit.GetLocalizedString(StringNames.AppName);

        this.CenterOnScreen();
        this.SetIcon("Assets/logo.ico");
        this.SetTitleBar(TitleBar);
        this.Get<AppViewModel>().Windows.Add(this);

        Closed += OnClosed;
        Activated += OnActivated;
    }

    private void OnActivated(object sender, WindowActivatedEventArgs args)
        => this.Get<StartupPageViewModel>().ExitCommand.Execute(default);

    private void OnClosed(object sender, WindowEventArgs args)
    {
        if (_cancellationTokenSource is not null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = default;
        }

        Activated -= OnActivated;
        Closed -= OnClosed;
        var localToken = this.Get<IBiliTokenResolver>().GetToken();
        if (localToken != null)
        {
            this.Get<AppViewModel>().RestartCommand.Execute(default);
        }
    }

    private async void OnLoadedAsync(object sender, RoutedEventArgs e)
    {
        await MainView.EnsureCoreWebView2Async();
        MainView.CoreWebView2.Navigate("https://passport.bilibili.com/login");
    }

    private async void OnNavigationCompletedAsync(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
    {
        if (sender.CoreWebView2.Source.StartsWith("https://passport.bilibili.com"))
        {
            await sender.CoreWebView2.ExecuteScriptAsync("document.querySelector('.international-footer').remove();document.querySelector('.top-header').remove()");
        }
        else if (sender.CoreWebView2.Source.StartsWith("https://www.bilibili.com"))
        {
            var cookies = await sender.CoreWebView2.CookieManager.GetCookiesAsync(sender.Source.AbsoluteUri);
            var cookieDict = cookies.Select(p => (p.Name, p.Value)).ToDictionary();
            if (cookieDict.Count > 0)
            {
                await this.Get<IAuthenticationService>().SignInAsync(new Richasy.BiliKernel.Bili.AuthorizeExecutionSettings { Cookies = cookieDict }, _cancellationTokenSource.Token).ConfigureAwait(true);
                _cancellationTokenSource = default;
                Close();
            }
        }
    }
}
