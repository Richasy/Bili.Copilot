﻿// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Controls.ViewLater;

/// <summary>
/// 稍后再看主体.
/// </summary>
public sealed partial class ViewLaterBody : ViewLaterPageControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ViewLaterBody"/> class.
    /// </summary>
    public ViewLaterBody() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.ListUpdated += OnVideoListUpdatedAsync;
        VideoScrollView.ViewChanged += OnViewChanged;
        VideoScrollView.SizeChanged += OnScrollViewSizeChanged;

        CheckVideoCount();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        VideoRepeater.ItemsSource = null;
        ViewModel.ListUpdated -= OnVideoListUpdatedAsync;
        VideoScrollView.ViewChanged -= OnViewChanged;
        VideoScrollView.SizeChanged -= OnScrollViewSizeChanged;
    }

    private async void OnVideoListUpdatedAsync(object? sender, EventArgs e)
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
                ViewModel.LoadVideosCommand.Execute(default);
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
                ViewModel.LoadVideosCommand.Execute(default);
            }
        });
    }
}
