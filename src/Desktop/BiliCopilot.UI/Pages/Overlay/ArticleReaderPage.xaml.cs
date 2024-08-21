// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.View;
using Microsoft.UI.Xaml.Navigation;
using Richasy.BiliKernel.Models.Article;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Pages.Overlay;

/// <summary>
/// 文章阅读页面.
/// </summary>
public sealed partial class ArticleReaderPage : ArticleReaderPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArticleReaderPage"/> class.
    /// </summary>
    public ArticleReaderPage()
    {
        InitializeComponent();
        NavigationCacheMode = NavigationCacheMode.Required;
    }

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is ArticleIdentifier article)
        {
            ViewModel.InitializeCommand.Execute(article);
            Reader.Initialized += OnInitializedAsync;
            ViewModel.ArticleInitialized += OnInitializedAsync;
        }
    }

    /// <inheritdoc/>
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        Reader.ClearContent();
        Reader.Initialized -= OnInitializedAsync;
        ViewModel.ArticleInitialized -= OnInitializedAsync;
    }

    private async void OnInitializedAsync(object? sender, EventArgs e)
    {
        if (ViewModel.Content is not null && Reader.IsInitialized)
        {
            await Reader.LoadContentAsync(ViewModel.Content);
        }
    }
}

/// <summary>
/// 文章阅读页面基类.
/// </summary>
public abstract class ArticleReaderPageBase : LayoutPageBase<ArticleReaderPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArticleReaderPageBase"/> class.
    /// </summary>
    protected ArticleReaderPageBase() => ViewModel = this.Get<ArticleReaderPageViewModel>();
}
