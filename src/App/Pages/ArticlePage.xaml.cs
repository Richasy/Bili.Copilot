// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.Community;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Pages;

/// <summary>
/// 专栏文章页面.
/// </summary>
public sealed partial class ArticlePage : ArticlePageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArticlePage"/> class.
    /// </summary>
    public ArticlePage()
    {
        InitializeComponent();
        ViewModel = ArticlePageViewModel.Instance;
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        CoreViewModel.IsBackButtonShown = false;
        ViewModel.InitializeCommand.Execute(default);
        ViewModel.RequestScrollToTop += OnRequestScrollToTop;
    }

    /// <inheritdoc/>
    protected override void OnPageUnloaded()
        => ViewModel.RequestScrollToTop -= OnRequestScrollToTop;

    private void OnRequestScrollToTop(object sender, EventArgs e)
        => ContentScrollViewer.ChangeView(default, 0, default, true);

    private void OnPartitionNavigationViewItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        var item = (Partition)args.InvokedItem;
        ViewModel.SelectPartitionCommand.Execute(item);
    }

    private void OnArticleViewIncrementalTriggered(object sender, EventArgs e)
        => ViewModel.IncrementalCommand.Execute(default);

    private void OnArticleSortComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ArticleSortComboBox.SelectedItem is ArticleSortType type
            && ViewModel.SortType != type)
        {
            ViewModel.SortType = type;
            _ = ContentScrollViewer.ChangeView(default, 0, default, true);
            ViewModel.ReloadCommand.Execute(default);
        }
    }
}

/// <summary>
/// <see cref="ArticlePage"/> 的基类.
/// </summary>
public abstract class ArticlePageBase : PageBase<ArticlePageViewModel>
{
}
