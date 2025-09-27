// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.Search;

/// <summary>
/// 视频搜索分区详情.
/// </summary>
public sealed partial class VideoSectionDetailControl : VideoSectionDetailControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoSectionDetailControl"/> class.
    /// </summary>
    public VideoSectionDetailControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.ListUpdated += OnListUpdatedAsync;
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        ViewModel.ListUpdated -= OnListUpdatedAsync;
    }

    private async void OnListUpdatedAsync(object? sender, EventArgs e)
    {
        await View.DelayCheckItemsAsync();
    }
}

/// <summary>
/// 视频搜索分区详情基类.
/// </summary>
public abstract class VideoSectionDetailControlBase : LayoutUserControlBase<VideoSearchSectionDetailViewModel>
{
}
