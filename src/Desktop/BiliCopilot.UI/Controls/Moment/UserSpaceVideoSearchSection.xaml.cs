﻿// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.Moment;

/// <summary>
/// 用户空间视频搜索区域.
/// </summary>
public sealed partial class UserSpaceVideoSearchSection : UserSpacePageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserSpaceVideoSearchSection"/> class.
    /// </summary>
    public UserSpaceVideoSearchSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.SearchUpdated += OnSearchUpdatedAsync;
        VideoScrollView.ViewChanged += OnViewChanged;
        VideoScrollView.SizeChanged += OnScrollViewSizeChanged;

        CheckVideoCount();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        ViewModel.SearchUpdated -= OnSearchUpdatedAsync;
        VideoScrollView.ViewChanged -= OnViewChanged;
        VideoScrollView.SizeChanged -= OnScrollViewSizeChanged;
    }

    private async void OnSearchUpdatedAsync(object? sender, EventArgs e)
    {
        await Task.Delay(500);
        CheckVideoCount();
    }

    private void OnViewChanged(object? sender, ScrollViewerViewChangedEventArgs args)
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            if (VideoScrollView.ExtentHeight - VideoScrollView.ViewportHeight - VideoScrollView.VerticalOffset <= 240)
            {
                ViewModel.LoadMoreSearchResultCommand.Execute(default);
            }
        });
    }

    private void OnScrollViewSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize.Width > 100)
        {
            CheckVideoCount();
        }
    }

    private void CheckVideoCount()
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            if (VideoScrollView.ScrollableHeight <= 240 && ViewModel is not null)
            {
                ViewModel.LoadMoreSearchResultCommand.Execute(default);
            }
        });
    }
}
