// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Threading.Tasks;
using Bili.Copilot.App.Controls;
using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Constants;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Article;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using Windows.Storage;
using Windows.System;

namespace Bili.Copilot.App.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ReaderPage : ReaderPageBase
{
    private ArticleInformation _article;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReaderPage"/> class.
    /// </summary>
    public ReaderPage()
    {
        InitializeComponent();
        ViewModel = new ReaderPageViewModel();
    }

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is ArticleInformation info)
        {
            _article = info;
        }
    }

    /// <inheritdoc/>
    protected override async void OnPageLoaded()
    {
        ViewModel.InitializeCommand.Execute(_article);
        Environment.SetEnvironmentVariable("WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS", "--disable-web-security");
        await ReaderView.EnsureCoreWebView2Async().AsTask();
        ReaderView.CoreWebView2.Settings.AreDevToolsEnabled = true;
        ReaderView.CoreWebView2.Settings.UserAgent = ServiceConstants.DefaultUserAgentString;
        ReaderView.CoreWebView2.NavigationStarting += OnNavigationStarting;
        ReaderView.CoreWebView2.NavigationCompleted += OnNavigationCompletedAsync;
        ReaderView.CoreWebView2.ContextMenuRequested += OnContextMenuRequested;

        var theme = ActualTheme == ElementTheme.Dark ? 2 : 1;
        var url = ApiConstants.Article.ArticleContent + $"?id={_article.Identifier.Id}&theme={theme}";
        ReaderView.CoreWebView2.Navigate(url);
    }

    private void OnContextMenuRequested(CoreWebView2 sender, CoreWebView2ContextMenuRequestedEventArgs args)
    {
        var menuList = args.MenuItems;
        var def = args.GetDeferral();
        args.Handled = true;
        var menuflyout = new MenuFlyout();
        menuflyout.Closed += (s, e) => def.Complete();
        var aiItem = new MenuFlyoutItem
        {
            Text = ResourceToolkit.GetLocalizedString(StringNames.WordExplain),
            Icon = new FluentIcon() { Symbol = FluentSymbol.SlideTextSparkle },
            Tag = args.ContextMenuTarget.SelectionText,
            IsEnabled = args.ContextMenuTarget.HasSelection && AppViewModel.Instance.IsAISupported,
            MinWidth = 160,
        };
        var webSearchItem = new MenuFlyoutItem
        {
            Text = ResourceToolkit.GetLocalizedString(StringNames.SearchInWeb),
            Icon = new FluentIcon() { Symbol = FluentSymbol.GlobeSearch },
            Tag = args.ContextMenuTarget.SelectionText,
            IsEnabled = args.ContextMenuTarget.HasSelection,
        };
        aiItem.Click += OnWordExplainItemClickAsync;
        webSearchItem.Click += OnSearchInWebItemClickAsync;
        menuflyout.Items.Add(aiItem);
        menuflyout.Items.Add(webSearchItem);
        menuflyout.ShowAt(ReaderView, args.Location);
    }

    private async void OnSearchInWebItemClickAsync(object sender, RoutedEventArgs e)
    {
        var text = (sender as MenuFlyoutItem).Tag as string;
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        var url = $"https://www.bing.com/search?q={Uri.EscapeDataString(text)}";
        await Launcher.LaunchUriAsync(new Uri(url));
    }

    private async void OnWordExplainItemClickAsync(object sender, RoutedEventArgs e)
    {
        var item = (sender as MenuFlyoutItem).Tag as string;
        if (string.IsNullOrEmpty(item))
        {
            return;
        }

        var dialog = new AIFeatureDialog((item, _article.Identifier), AIFeatureType.WordExplain)
        {
            XamlRoot = XamlRoot,
        };
        await dialog.ShowAsync();
    }

    private async void OnNavigationCompletedAsync(CoreWebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
    {
        // 先执行一次清理脚本去除多于内容，再执行一次去除图片遮罩.
        var js = await GetJsAsync();
        await ReaderView.CoreWebView2.ExecuteScriptAsync(js).AsTask();

        await Task.Delay(500);
        await ReaderView.CoreWebView2.ExecuteScriptAsync(js).AsTask();
    }

    private void OnNavigationStarting(CoreWebView2 sender, CoreWebView2NavigationStartingEventArgs args)
    {
        if (!args.IsUserInitiated)
        {
            args.Cancel = true;
        }
    }

    private async Task<string> GetJsAsync()
    {
        var isDark = ActualTheme == ElementTheme.Dark;
        var scrollForeground = isDark ? "gray" : "#aaaaaa";
        var background = isDark ? "#2f3239" : "#faf9fa";
        var jsFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/ReaderClean.js")).AsTask();
        var js = await FileIO.ReadTextAsync(jsFile).AsTask();
        return js
            .Replace("$scroll-foreground$", scrollForeground)
            .Replace("$body-background$", background);
    }

    private void OnNavigationItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        var tag = args.InvokedItemContainer.Tag.ToString();
        var isDetail = tag == "Detail";
        ViewModel.IsDetailShown = isDetail;
        ViewModel.IsCommentShown = !isDetail;
    }
}

/// <summary>
/// 阅读器页面基类.
/// </summary>
public abstract class ReaderPageBase : PageBase<ReaderPageViewModel>
{
}
