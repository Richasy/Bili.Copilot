// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.LivePartition;

/// <summary>
/// 直播推荐主体.
/// </summary>
public sealed partial class LiveRecommendMainBody : LivePartitionPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LiveRecommendMainBody"/> class.
    /// </summary>
    public LiveRecommendMainBody() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.RecommendUpdated += OnLiveListUpdatedAsync;
        LiveScrollView.ViewChanged += OnViewChanged;
        LiveScrollView.SizeChanged += OnScrollViewSizeChanged;

        CheckLiveCount();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        ViewModel.RecommendUpdated -= OnLiveListUpdatedAsync;
        LiveScrollView.ViewChanged -= OnViewChanged;
        LiveScrollView.SizeChanged -= OnScrollViewSizeChanged;
    }

    private async void OnLiveListUpdatedAsync(object? sender, EventArgs e)
    {
        await Task.Delay(500);
        CheckLiveCount();
    }

    private void OnViewChanged(object? sender, ScrollViewerViewChangedEventArgs args)
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            if (LiveScrollView.ExtentHeight - LiveScrollView.ViewportHeight - LiveScrollView.VerticalOffset <= 40)
            {
                ViewModel.LoadRecommendRoomsCommand.Execute(default);
            }
        });
    }

    private void OnScrollViewSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize.Width > 100)
        {
            CheckLiveCount();
        }
    }

    private void CheckLiveCount()
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            if (LiveScrollView.ScrollableHeight <= 0 && ViewModel is not null)
            {
                ViewModel.LoadRecommendRoomsCommand.Execute(default);
            }
        });
    }
}
