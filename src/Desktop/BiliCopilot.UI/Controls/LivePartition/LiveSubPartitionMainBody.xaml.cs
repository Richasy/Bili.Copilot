// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.LivePartition;

/// <summary>
/// 直播子分区详情.
/// </summary>
public sealed partial class LiveSubPartitionMainBody : LiveSubPartitionControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LiveSubPartitionMainBody"/> class.
    /// </summary>
    public LiveSubPartitionMainBody() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.LiveListUpdated -= OnLiveListUpdatedAsync;
        }
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(LivePartitionDetailViewModel? oldValue, LivePartitionDetailViewModel? newValue)
    {
        if (oldValue is not null)
        {
            oldValue.LiveListUpdated -= OnLiveListUpdatedAsync;
        }

        if (newValue is null)
        {
            return;
        }

        newValue.LiveListUpdated += OnLiveListUpdatedAsync;
    }

    private async void OnLiveListUpdatedAsync(object? sender, EventArgs e)
    {
        await View.DelayCheckItemsAsync();
    }
}

/// <summary>
/// 直播子分区详情基类.
/// </summary>
public abstract class LiveSubPartitionControlBase : LayoutUserControlBase<LivePartitionDetailViewModel>
{
}
