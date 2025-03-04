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
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new(Bindings.Initialize, Bindings.StopTracking);

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        LiveScrollView.ViewChanged += OnViewChanged;
        LiveScrollView.SizeChanged += OnScrollViewSizeChanged;
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        LiveScrollView.ViewChanged -= OnViewChanged;
        LiveScrollView.SizeChanged -= OnScrollViewSizeChanged;
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
        await Task.Delay(500);
        CheckRoomCount();
    }

    private void OnViewChanged(object? sender, ScrollViewerViewChangedEventArgs args)
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            if (LiveScrollView.ExtentHeight - LiveScrollView.ViewportHeight - LiveScrollView.VerticalOffset <= 240)
            {
                ViewModel.LoadRoomsCommand.Execute(default);
            }
        });
    }

    private void OnScrollViewSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize.Width > 100 && ViewModel is not null)
        {
            CheckRoomCount();
        }
    }

    private void CheckRoomCount()
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            if (LiveScrollView.ScrollableHeight <= 240 && ViewModel is not null)
            {
                ViewModel.LoadRoomsCommand.Execute(default);
            }
        });
    }
}

/// <summary>
/// 直播子分区详情基类.
/// </summary>
public abstract class LiveSubPartitionControlBase : LayoutUserControlBase<LivePartitionDetailViewModel>
{
}
