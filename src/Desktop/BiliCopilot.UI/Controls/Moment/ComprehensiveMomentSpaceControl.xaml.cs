﻿// Copyright (c) Bili Copilot. All rights reserved.

namespace BiliCopilot.UI.Controls.Moment;

/// <summary>
/// 综合动态空间控件.
/// </summary>
public sealed partial class ComprehensiveMomentSpaceControl : UserMomentDetailControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ComprehensiveMomentSpaceControl"/> class.
    /// </summary>
    public ComprehensiveMomentSpaceControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        MomentScrollView.ViewChanged += OnViewChanged;
        MomentScrollView.SizeChanged += OnScrollViewSizeChanged;

        ViewModel.ListUpdated += OnListUpdatedAsync;
        CheckMomentCount();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.ListUpdated -= OnListUpdatedAsync;
        }

        MomentScrollView.ViewChanged -= OnViewChanged;
        MomentScrollView.SizeChanged -= OnScrollViewSizeChanged;
    }

    private async void OnListUpdatedAsync(object? sender, EventArgs e)
    {
        await Task.Delay(500);
        CheckMomentCount();
    }

    private void OnViewChanged(object? sender, ScrollViewerViewChangedEventArgs args)
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            if (MomentScrollView.ExtentHeight - MomentScrollView.ViewportHeight - MomentScrollView.VerticalOffset <= 240)
            {
                ViewModel.LoadItemsCommand.Execute(default);
            }
        });
    }

    private void OnScrollViewSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize.Width > 100)
        {
            CheckMomentCount();
        }
    }

    private void CheckMomentCount()
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            if (MomentScrollView.ScrollableHeight <= 240 && ViewModel is not null)
            {
                ViewModel.LoadItemsCommand.Execute(default);
            }
        });
    }
}
