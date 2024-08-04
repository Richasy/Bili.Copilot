﻿// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.BiliKernel.Models;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.VideoPartition;

/// <summary>
/// 视频分区主区域头部.
/// </summary>
public sealed partial class VideoPartitionMainHeader : VideoPartitionDetailControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoPartitionMainHeader"/> class.
    /// </summary>
    public VideoPartitionMainHeader() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new(Bindings.Initialize, Bindings.StopTracking);

    private void OnSortTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Any()
            && e.AddedItems[0] is PartitionVideoSortType sortType
            && sortType != ViewModel.SelectedSortType)
        {
            ViewModel.ChangeSortTypeCommand.Execute(sortType);
        }
    }
}
