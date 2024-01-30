// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.App.Forms;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;

namespace Bili.Copilot.App.Pages;

/// <summary>
/// 网页登录页面.
/// </summary>
public sealed partial class WebSignInPage : PageBase
{
    private bool _isVerify;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebSignInPage"/> class.
    /// </summary>
    public WebSignInPage()
    {
        InitializeComponent();
    }

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        var currentWindow = CoreViewModel.ActivatedWindow;
        if (currentWindow is MainWindow)
        {
            _isVerify = true;
            BackButton.Visibility = Visibility.Collapsed;
        }
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
                if (_isVerify)
                {
                    SettingsToolkit.WriteLocalSetting(Models.Constants.App.SettingNames.IsWebSignIn, true);
                    CoreViewModel.BackCommand.Execute(default);
                    return;
                }

                MainView.Visibility = Visibility.Collapsed;
                LoadingOverlay.IsOpen = true;
                LoadingOverlay.Text = ResourceToolkit.GetLocalizedString(Models.Constants.App.StringNames.Verifying);

                try
                {
                    await AuthorizeProvider.Instance.SignInWithCookieAsync(cookieDict);
                    MainView.CoreWebView2.NavigateToString("<p>你好哔哩.</p>");
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
