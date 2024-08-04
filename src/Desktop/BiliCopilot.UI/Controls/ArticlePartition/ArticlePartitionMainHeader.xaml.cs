// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.BiliKernel.Models;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.ArticlePartition;

/// <summary>
/// 文章分区头部.
/// </summary>
public sealed partial class ArticlePartitionMainHeader : ArticlePartitionDetailControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArticlePartitionMainHeader"/> class.
    /// </summary>
    public ArticlePartitionMainHeader() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new(Bindings.Initialize, Bindings.StopTracking);

    private void OnSortTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Any()
            && e.AddedItems[0] is ArticleSortType sortType
            && sortType != ViewModel.SelectedSortType)
        {
            ViewModel.ChangeSortTypeCommand.Execute(sortType);
        }
    }
}
