// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Models.App.Args;
using Microsoft.Web.WebView2.Core;
using Windows.Storage;

namespace Bili.Copilot.App.Pages;

/// <summary>
/// 网页播放器界面.
/// </summary>
public sealed partial class WebPlayerPage : PageBase
{
    private const string UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/118.0.0.0 Safari/537.36 Edg/118.0.2048.1";
    private string _url;
    private Window _attachedWindow;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebPlayerPage"/> class.
    /// </summary>
    public WebPlayerPage()
    {
        InitializeComponent();
    }

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is PlayerPageNavigateEventArgs args)
        {
            if (args.Snapshot != null)
            {
                _url = args.Snapshot.WebLink;
                _attachedWindow = args.AttachedWindow as Window;
            }
        }
    }

    /// <inheritdoc/>
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        try
        {
            MainView.NavigateToString(string.Empty);
        }
        catch (Exception)
        {
        }
    }

    /// <inheritdoc/>
    protected override async void OnPageLoaded()
    {
        LoadingOverlay.IsOpen = true;

        // 取消自动静音限制.
        Environment.SetEnvironmentVariable("WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS", "--autoplay-policy=no-user-gesture-required --enable-features=PlatformHEVCEncoderSupport --enable-features=HardwareMediaDecoding --enable-features=PlatformEncryptedDolbyVision");
        await MainView.EnsureCoreWebView2Async();
        MainView.CoreWebView2.Settings.IsStatusBarEnabled = false;
        MainView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
        MainView.CoreWebView2.Settings.AreDevToolsEnabled = false;
        MainView.CoreWebView2.Settings.UserAgent = UserAgent;
        MainView.CoreWebView2.ContainsFullScreenElementChanged += OnContainsFullScreenElementChanged;
        MainView.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
        MainView.CoreWebView2.WebResourceRequested += OnWebResourceRequested;
        MainView.CoreWebView2.NavigationStarting += OnNavigationStarting;
        if (!string.IsNullOrEmpty(_url))
        {
            MainView.CoreWebView2.Navigate(_url);
        }
    }

    private void OnNavigationStarting(CoreWebView2 sender, CoreWebView2NavigationStartingEventArgs args)
        => args.RequestHeaders.SetHeader("User-Agent", UserAgent);

    private void OnWebResourceRequested(CoreWebView2 sender, CoreWebView2WebResourceRequestedEventArgs args)
        => args.Request.Headers.SetHeader("User-Agent", UserAgent);

    private void OnContainsFullScreenElementChanged(CoreWebView2 sender, object args)
    {
        if (sender.ContainsFullScreenElement)
        {
            _attachedWindow.AppWindow.SetPresenter(Microsoft.UI.Windowing.AppWindowPresenterKind.FullScreen);
        }
        else
        {
            _attachedWindow.AppWindow.SetPresenter(Microsoft.UI.Windowing.AppWindowPresenterKind.Overlapped);
        }
    }

    private async void OnNavigationCompletedAsync(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
    {
        LoadingOverlay.IsOpen = false;

        var cookies = await sender.CoreWebView2.CookieManager.GetCookiesAsync(sender.Source.AbsoluteUri);
        var cookieDict = cookies.Select(p => (p.Name, p.Value)).ToDictionary();
        if (cookieDict.ContainsKey("bili_jct"))
        {
            AuthorizeProvider.SaveCookies(cookieDict);
        }

        // 注入 js.
        var cleanFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/WebPlayer.js"));
        var cleanContent = await FileIO.ReadTextAsync(cleanFile);
        await MainView.CoreWebView2.ExecuteScriptAsync(cleanContent);
    }
}
