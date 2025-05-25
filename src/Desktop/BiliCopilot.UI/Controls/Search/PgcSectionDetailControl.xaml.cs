// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.Search;

/// <summary>
/// PGC搜索分区详情控件.
/// </summary>
public sealed partial class PgcSectionDetailControl : PgcSectionDetailControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PgcSectionDetailControl"/> class.
    /// </summary>
    public PgcSectionDetailControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.ListUpdated += OnListUpdatedAsync;
        PgcScrollView.ViewChanged += OnViewChanged;
        PgcScrollView.SizeChanged += OnScrollViewSizeChanged;

        CheckPgcCount();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        SeasonRepeater.ItemsSource = null;
        ViewModel.ListUpdated -= OnListUpdatedAsync;
        PgcScrollView.ViewChanged -= OnViewChanged;
        PgcScrollView.SizeChanged -= OnScrollViewSizeChanged;
    }

    private async void OnListUpdatedAsync(object? sender, EventArgs e)
    {
        await Task.Delay(500);
        CheckPgcCount();
    }

    private void OnViewChanged(object? sender, ScrollViewerViewChangedEventArgs args)
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            if (PgcScrollView.ExtentHeight - PgcScrollView.ViewportHeight - PgcScrollView.VerticalOffset <= 240)
            {
                ViewModel.LoadItemsCommand.Execute(default);
            }
        });
    }

    private void OnScrollViewSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize.Width > 100)
        {
            CheckPgcCount();
        }
    }

    private void CheckPgcCount()
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            if (PgcScrollView.ScrollableHeight <= 240 && ViewModel is not null)
            {
                ViewModel.LoadItemsCommand.Execute(default);
            }
        });
    }
}

/// <summary>
/// PGC搜索分区详情控件基类.
/// </summary>
public abstract class PgcSectionDetailControlBase : LayoutUserControlBase<PgcSearchSectionDetailViewModel>
{
}
