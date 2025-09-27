// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.History;

/// <summary>
/// 文章历史记录区域.
/// </summary>
public sealed partial class ArticleHistorySection : ArticleHistorySectionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArticleHistorySection"/> class.
    /// </summary>
    public ArticleHistorySection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.ListUpdated -= OnArticleListUpdatedAsync;
        }
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(ArticleHistorySectionDetailViewModel? oldValue, ArticleHistorySectionDetailViewModel? newValue)
    {
        if (oldValue is not null)
        {
            oldValue.ListUpdated -= OnArticleListUpdatedAsync;
        }

        if (newValue is null)
        {
            return;
        }

        newValue.ListUpdated += OnArticleListUpdatedAsync;
    }

    private async void OnArticleListUpdatedAsync(object? sender, EventArgs e)
    {
        await View.DelayCheckItemsAsync();
    }
}

/// <summary>
/// 文章历史记录区域基类.
/// </summary>
public abstract class ArticleHistorySectionBase : LayoutUserControlBase<ArticleHistorySectionDetailViewModel>
{
}
