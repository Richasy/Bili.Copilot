// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using BiliCopilot.UI.ViewModels.View;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.VideoPartition;

/// <summary>
/// 视频分区侧边导航栏主体.
/// </summary>
public sealed partial class VideoPartitionSideBody : VideoPartitionSideBodyBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoPartitionSideBody"/> class.
    /// </summary>
    public VideoPartitionSideBody() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.PartitionInitialized += OnPartitionInitialized;
        CheckPartitionSelection();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        PartitionView.ItemsSource = default;
        ViewModel.PartitionInitialized -= OnPartitionInitialized;
    }

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
/// 视频分区页面侧边栏主体的基类.
/// </summary>
public abstract class VideoPartitionSideBodyBase : LayoutUserControlBase<VideoPartitionPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoPartitionSideBodyBase"/> class.
    /// </summary>
    protected VideoPartitionSideBodyBase() => ViewModel = this.Get<VideoPartitionPageViewModel>();
}
