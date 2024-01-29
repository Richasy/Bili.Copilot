// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;

namespace Bili.Copilot.App.Pages;

/// <summary>
/// 网页登录页面.
/// </summary>
public sealed partial class WebSignInPage : PageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebSignInPage"/> class.
    /// </summary>
    public WebSignInPage()
    {
        InitializeComponent();
    }

    /// <inheritdoc/>
    protected override async void OnPageLoaded()
    {
        LoadingOverlay.IsOpen = true;
        await MainView.EnsureCoreWebView2Async();
        MainView.CoreWebView2.Navigate("https://passport.bilibili.com/login");
    }

    private async void OnNavigationCompletedAsync(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
    {
        LoadingOverlay.IsOpen = false;
        SettingsToolkit.DeleteLocalSetting(Models.Constants.App.SettingNames.LocalCookie);
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
                MainView.Visibility = Visibility.Collapsed;
                LoadingOverlay.IsOpen = true;
                LoadingOverlay.Text = ResourceToolkit.GetLocalizedString(Models.Constants.App.StringNames.Verifying);

                try
                {
                    await AuthorizeProvider.Instance.SignInWithCookieAsync(cookieDict);
                    CoreViewModel.BackCommand.Execute(default);
                }
                catch (Exception)
                {
                    MainView.Visibility = Visibility.Visible;
                }

                LoadingOverlay.IsOpen = false;
            }
        }
    }

    private void OnBackButtonClick(object sender, RoutedEventArgs e)
        => CoreViewModel.BackCommand.Execute(default);
}
