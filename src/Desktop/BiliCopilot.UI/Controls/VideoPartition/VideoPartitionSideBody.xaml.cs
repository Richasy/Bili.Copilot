// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using BiliCopilot.UI.ViewModels.View;
using Richasy.WinUI.Share.Base;

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

    private void OnPartitionSelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
    {
        var item = sender.SelectedItem as VideoPartitionViewModel;
        _ = this;
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
