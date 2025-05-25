// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.Search;

/// <summary>
/// 文章搜索分区详情控件.
/// </summary>
public sealed partial class ArticleSectionDetailControl : ArticleSectionDetailControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArticleSectionDetailControl"/> class.
    /// </summary>
    public ArticleSectionDetailControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.ListUpdated += OnListUpdatedAsync;
        ArticleScrollView.ViewChanged += OnViewChanged;
        ArticleScrollView.SizeChanged += OnScrollViewSizeChanged;

        CheckArticleCount();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        ArticleRepeater.ItemsSource = null;
        ViewModel.ListUpdated -= OnListUpdatedAsync;
        ArticleScrollView.ViewChanged -= OnViewChanged;
        ArticleScrollView.SizeChanged -= OnScrollViewSizeChanged;
    }

    private async void OnListUpdatedAsync(object? sender, EventArgs e)
    {
        await Task.Delay(500);
        CheckArticleCount();
    }

    private void OnViewChanged(object? sender, ScrollViewerViewChangedEventArgs args)
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            if (ArticleScrollView.ExtentHeight - ArticleScrollView.ViewportHeight - ArticleScrollView.VerticalOffset <= 240)
            {
                ViewModel.LoadItemsCommand.Execute(default);
            }
        });
    }

    private void OnScrollViewSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize.Width > 100)
        {
            CheckArticleCount();
        }
    }

    private void CheckArticleCount()
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            if (ArticleScrollView.ScrollableHeight <= 240 && ViewModel is not null)
            {
                ViewModel.LoadItemsCommand.Execute(default);
            }
        });
    }
}

/// <summary>
/// 文章搜索分区详情控件基类.
/// </summary>
public abstract class ArticleSectionDetailControlBase : LayoutUserControlBase<ArticleSearchSectionDetailViewModel>
{
}
