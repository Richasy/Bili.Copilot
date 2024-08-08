// Copyright (c) Bili Copilot. All rights reserved.

using System.Text.Json;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.Web.WebView2.Core;
using Richasy.BiliKernel.Models.Article;
using Richasy.WinUI.Share.Base;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;

namespace BiliCopilot.UI.Controls.Components;

/// <summary>
/// 文章阅读器.
/// </summary>
public sealed partial class ArticleReader : LayoutUserControlBase
{
    private string? _selectedText;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArticleReader"/> class.
    /// </summary>
    public ArticleReader() => InitializeComponent();

    /// <summary>
    /// 控件加载完成.
    /// </summary>
    public event EventHandler Initialized;

    /// <summary>
    /// 是否已加载好.
    /// </summary>
    public bool IsInitialized { get; private set; }

    /// <summary>
    /// 加载文章内容.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public async Task LoadContentAsync(
        ArticleDetail article,
        string fontFamily = "Segoe UI",
        double fontSize = 16,
        double lineHeight = 1.5)
    {
        var path = Path.Combine(Package.Current.InstalledPath, "Assets/Reader");
        var theme = App.Current.RequestedTheme.ToString();
        var localHtml = await File.ReadAllTextAsync(Path.Combine(path, "ShowPage.html"));
        var localCss = await File.ReadAllTextAsync(Path.Combine(path, $"{theme}.css"));
        localCss = localCss.Replace("$FontFamily$", fontFamily)
            .Replace("$FontSize$", $"{fontSize}")
            .Replace("$LineHeight$", $"{lineHeight}");
        var html = localHtml.Replace("$theme$", theme.ToLower())
            .Replace("$style$", localCss)
            .Replace("$title$", article.Identifier.Title)
            .Replace("$body$", article.HtmlContent);
        MainView.CoreWebView2.NavigateToString(html);
    }

    /// <inheritdoc/>
    protected override async void OnControlLoaded()
    {
        await MainView.EnsureCoreWebView2Async();
        MainView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
        MainView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
        MainView.CoreWebView2.Settings.IsPinchZoomEnabled = false;
        MainView.CoreWebView2.Settings.IsSwipeNavigationEnabled = false;
        MainView.CoreWebView2.Settings.IsZoomControlEnabled = false;
        MainView.CoreWebView2.Settings.AreDevToolsEnabled = false;
        MainView.CoreWebView2.ContextMenuRequested += OnContextMenuRequested;
        IsInitialized = true;
        Initialized?.Invoke(this, EventArgs.Empty);
    }

    private void OnContextMenuRequested(CoreWebView2 sender, CoreWebView2ContextMenuRequestedEventArgs args)
    {
        if (args.ContextMenuTarget.HasSelection)
        {
            args.Handled = true;
            if (!string.IsNullOrEmpty(args.ContextMenuTarget.SelectionText))
            {
                _selectedText = args.ContextMenuTarget.SelectionText;
                TextMenuFlyout.ShowAt(MainView, new FlyoutShowOptions { Position = args.Location });
            }
        }
        else if (args.ContextMenuTarget.Kind == CoreWebView2ContextMenuTargetKind.Image)
        {
            for (var i = args.MenuItems.Count - 1; i >= 0; i--)
            {
                if (args.MenuItems[i].Name == "back"
                    || args.MenuItems[i].Name == "forward"
                    || args.MenuItems[i].Name == "reload"
                    || args.MenuItems[i].Name == "saveAs"
                    || args.MenuItems[i].Name == "share"
                    || args.MenuItems[i].Name == "webCapture"
                    || args.MenuItems[i].Name == "copyImageLocation"
                    || (args.MenuItems[i].Name == "other" && !string.IsNullOrEmpty(args.MenuItems[i].Label)))
                {
                    args.MenuItems.RemoveAt(i);
                }
            }
        }
        else
        {
            args.Handled = true;
        }
    }

    private async void OnMessageReceivedAsync(WebView2 sender, CoreWebView2WebMessageReceivedEventArgs args)
    {
        _ = this;
        var jsonEle = JsonSerializer.Deserialize<JsonElement>(args.WebMessageAsJson);
        if (jsonEle.TryGetProperty("Name", out var nameEle))
        {
            var name = nameEle.GetString();
            var data = jsonEle.GetProperty("Data").GetString();
            switch (name)
            {
                case "LinkClick":
                    _ = await Launcher.LaunchUriAsync(new Uri(data)).AsTask();
                    break;
            }
        }
    }

    private async void OnSearchItemClickAsync(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_selectedText))
        {
            return;
        }

        var uri = new Uri($"https://cn.bing.com/search?q={Uri.EscapeDataString(_selectedText)}");
        await Launcher.LaunchUriAsync(uri).AsTask();
    }

    private void OnCopyItemClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_selectedText))
        {
            return;
        }

        var dp = new DataPackage();
        dp.SetText(_selectedText);
        Clipboard.SetContent(dp);
    }
}
