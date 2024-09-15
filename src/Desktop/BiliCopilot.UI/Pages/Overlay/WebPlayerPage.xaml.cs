// Copyright (c) Bili Copilot. All rights reserved.

using System.IO.Compression;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using Richasy.WinUI.Share.Base;
using Windows.Storage;

namespace BiliCopilot.UI.Pages.Overlay;

/// <summary>
/// 网页播放器.
/// </summary>
public sealed partial class WebPlayerPage : LayoutPageBase
{
    private const string BewlyVersion = "0.30.2";
    private const string UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/118.0.0.0 Safari/537.36 Edg/118.0.2048.1";
    private string _url;
    private bool _isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebPlayerPage"/> class.
    /// </summary>
    public WebPlayerPage()
    {
        InitializeComponent();
        NavigationCacheMode = NavigationCacheMode.Enabled;
    }

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is string url)
        {
            _url = url;
            ShowLoading();
            if (_isInitialized)
            {
                MainView.CoreWebView2.Navigate(_url);
            }
        }
    }

    /// <inheritdoc/>
    protected override void OnNavigatedFrom(NavigationEventArgs e)
        => MainView?.NavigateToString(string.Empty);

    /// <inheritdoc/>
    protected override async void OnPageLoaded()
    {
        ShowLoading();

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
        _isInitialized = true;
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
        var appVM = this.Get<AppViewModel>();
        if (sender.ContainsFullScreenElement)
        {
            appVM.MakeCurrentWindowEnterFullScreenCommand.Execute(default);
        }
        else
        {
            appVM.MakeCurrentWindowExitFullScreenCommand.Execute(default);
        }
    }

    private async void OnNavigationCompletedAsync(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
    {
        HideLoading();
        await Task.CompletedTask;
    }

    private void ShowLoading()
    {
        LoadingShimmer.Visibility = Visibility.Visible;
        LoadingShimmer.IsActive = true;
    }

    private void HideLoading()
    {
        LoadingShimmer.Visibility = Visibility.Collapsed;
        LoadingShimmer.IsActive = false;
    }

    private async Task TryLoadExtensionAsync()
    {
        var localFolder = ApplicationData.Current.LocalFolder;
        var extensionFolder = await localFolder.CreateFolderAsync("Extensions", CreationCollisionOption.OpenIfExists);
        var bewlyFolderPath = Path.Combine(extensionFolder.Path, "BewlyBewly");
        var localVersion = SettingsToolkit.ReadLocalSetting(SettingNames.BewlyExtensionVersion, string.Empty);
        if (localVersion != BewlyVersion)
        {
            if (!Directory.Exists(bewlyFolderPath))
            {
                Directory.CreateDirectory(bewlyFolderPath);
                var bewlyZipFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/BewlyBewly.zip"));

                // Unzip file and copy to folder.
                await Task.Run(() =>
                {
                    ZipFile.ExtractToDirectory(bewlyZipFile.Path, bewlyFolderPath);
                });
            }

            await MainView.CoreWebView2.Profile.AddBrowserExtensionAsync(bewlyFolderPath);
        }
    }

    private async void OnCoreInitializedAsync(WebView2 sender, CoreWebView2InitializedEventArgs args)
    {
        await TryLoadExtensionAsync();
    }
}
