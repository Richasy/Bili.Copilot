// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.History;

/// <summary>
/// 视频历史记录区域.
/// </summary>
public sealed partial class VideoHistorySection : VideoHistorySectionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoHistorySection"/> class.
    /// </summary>
    public VideoHistorySection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.ListUpdated -= OnVideoListUpdatedAsync;
        }
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(VideoHistorySectionDetailViewModel? oldValue, VideoHistorySectionDetailViewModel? newValue)
    {
        if (oldValue is not null)
        {
            oldValue.ListUpdated -= OnVideoListUpdatedAsync;
        }

        if (newValue is null)
        {
            return;
        }

        newValue.ListUpdated += OnVideoListUpdatedAsync;
    }

    private async void OnVideoListUpdatedAsync(object? sender, EventArgs e)
    {
        await View.DelayCheckItemsAsync();
    }
}

/// <summary>
/// 视频历史记录区域基类.
/// </summary>
public abstract class VideoHistorySectionBase : LayoutUserControlBase<VideoHistorySectionDetailViewModel>
{
}
