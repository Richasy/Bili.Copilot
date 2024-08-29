// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using BiliCopilot.UI.ViewModels.View;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Article;

/// <summary>
/// 文章分区侧边栏.
/// </summary>
public sealed partial class ArticlePartitionSideBody : ArticlePartitionSideBodyBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArticlePartitionSideBody"/> class.
    /// </summary>
    public ArticlePartitionSideBody() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.PartitionInitialized += OnPartitionInitialized;
        CheckPartitionSelection();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
        => ViewModel.PartitionInitialized -= OnPartitionInitialized;

    private void OnPartitionInitialized(object? sender, EventArgs e)
        => CheckPartitionSelection();

    private void OnPartitionSelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
    {
        var item = sender.SelectedItem as PartitionViewModel;
        ViewModel.SelectPartitionCommand.Execute(item);
    }

    private void CheckPartitionSelection()
    {
        if (ViewModel.SelectedPartition is not null)
        {
            var partition = ViewModel.Partitions.FirstOrDefault(p => p.Equals(ViewModel.SelectedPartition.Data));
            PartitionView.Select(ViewModel.Partitions.IndexOf(partition));
        }
    }
}

/// <summary>
/// 文章分区侧边栏基类.
/// </summary>
public abstract class ArticlePartitionSideBodyBase : LayoutUserControlBase<ArticlePartitionPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArticlePartitionSideBodyBase"/> class.
    /// </summary>
    protected ArticlePartitionSideBodyBase() => ViewModel = this.Get<ArticlePartitionPageViewModel>();
}
