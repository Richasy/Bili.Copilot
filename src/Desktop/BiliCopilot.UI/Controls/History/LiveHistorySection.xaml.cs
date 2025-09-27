// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.History;

/// <summary>
/// 直播历史记录区域.
/// </summary>
public sealed partial class LiveHistorySection : LiveHistorySectionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LiveHistorySection"/> class.
    /// </summary>
    public LiveHistorySection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.ListUpdated -= OnLiveListUpdatedAsync;
        }
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(LiveHistorySectionDetailViewModel? oldValue, LiveHistorySectionDetailViewModel? newValue)
    {
        if (oldValue is not null)
        {
            oldValue.ListUpdated -= OnLiveListUpdatedAsync;
        }

        if (newValue is null)
        {
            return;
        }

        newValue.ListUpdated += OnLiveListUpdatedAsync;
    }

    private async void OnLiveListUpdatedAsync(object? sender, EventArgs e)
    {
        await View.DelayCheckItemsAsync();
    }
}

/// <summary>
/// 直播历史记录区域基类.
/// </summary>
public abstract class LiveHistorySectionBase : LayoutUserControlBase<LiveHistorySectionDetailViewModel>
{
}
