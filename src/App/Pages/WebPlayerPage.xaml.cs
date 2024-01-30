// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Models.App.Args;
using Microsoft.Web.WebView2.Core;
using Windows.Storage;

namespace Bili.Copilot.App.Pages;

/// <summary>
/// 网页播放器界面.
/// </summary>
public sealed partial class WebPlayerPage : PageBase
{
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
    protected override async void OnPageLoaded()
    {
        LoadingOverlay.IsOpen = true;

        // 取消自动静音限制.
        Environment.SetEnvironmentVariable("WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS", "--autoplay-policy=no-user-gesture-required");
        await MainView.EnsureCoreWebView2Async();
        MainView.CoreWebView2.Settings.IsStatusBarEnabled = false;
        MainView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
        MainView.CoreWebView2.Settings.AreDevToolsEnabled = true;
        MainView.CoreWebView2.ContainsFullScreenElementChanged += OnContainsFullScreenElementChanged;
        if (!string.IsNullOrEmpty(_url))
        {
            MainView.CoreWebView2.Navigate(_url);
        }
    }

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

        // 注入 js.
        var cleanFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/WebPlayer.js"));
        var cleanContent = await FileIO.ReadTextAsync(cleanFile);
        await MainView.CoreWebView2.ExecuteScriptAsync(cleanContent);
    }
}
