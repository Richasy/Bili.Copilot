// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;

namespace BiliCopilot.UI.Controls.Article;

/// <summary>
/// 文章分区主体.
/// </summary>
public sealed partial class ArticlePartitionMainBody : ArticlePartitionDetailControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArticlePartitionMainBody"/> class.
    /// </summary>
    public ArticlePartitionMainBody() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.ArticleListUpdated -= OnArticleListUpdatedAsync;
        }
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(ArticlePartitionDetailViewModel? oldValue, ArticlePartitionDetailViewModel? newValue)
    {
        if (oldValue is not null)
        {
            oldValue.ArticleListUpdated -= OnArticleListUpdatedAsync;
        }

        if (newValue is null)
        {
            return;
        }

        newValue.ArticleListUpdated += OnArticleListUpdatedAsync;
    }

    private async void OnArticleListUpdatedAsync(object? sender, EventArgs e)
    {
        await View.DelayCheckItemsAsync();
    }
}
