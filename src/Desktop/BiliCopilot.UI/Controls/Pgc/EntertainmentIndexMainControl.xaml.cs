// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Pgc;

/// <summary>
/// 娱乐索引主体.
/// </summary>
public sealed partial class EntertainmentIndexMainControl : EntertainmentIndexControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EntertainmentIndexMainControl"/> class.
    /// </summary>
    public EntertainmentIndexMainControl() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        SeasonScrollView.ViewChanged += OnViewChanged;
        SeasonScrollView.SizeChanged += OnScrollViewSizeChanged;

        if (ViewModel is null)
        {
            return;
        }

        CheckSeasonCount();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.ItemsUpdated -= OnItemsUpdatedAsync;
        }

        SeasonScrollView.ViewChanged -= OnViewChanged;
        SeasonScrollView.SizeChanged -= OnScrollViewSizeChanged;
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(EntertainmentIndexViewModel? oldValue, EntertainmentIndexViewModel? newValue)
    {
        if (oldValue is not null)
        {
            oldValue.ItemsUpdated -= OnItemsUpdatedAsync;
        }

        if (newValue is null)
        {
            return;
        }

        SeasonScrollView?.ChangeView(0, 0, default, true);
        newValue.ItemsUpdated += OnItemsUpdatedAsync;
    }

    private async void OnItemsUpdatedAsync(object? sender, EventArgs e)
    {
        await Task.Delay(500);
        CheckSeasonCount();
    }

    private void OnViewChanged(object? sender, ScrollViewerViewChangedEventArgs args)
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            if (SeasonScrollView.ExtentHeight - SeasonScrollView.ViewportHeight - SeasonScrollView.VerticalOffset <= 40)
            {
                ViewModel.RequestIndexCommand.Execute(default);
            }
        });
    }

    private void OnScrollViewSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize.Width > 100)
        {
            CheckSeasonCount();
        }
    }

    private void CheckSeasonCount()
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            if (SeasonScrollView.ScrollableHeight <= 0 && ViewModel is not null)
            {
                ViewModel.RequestIndexCommand.Execute(default);
            }
        });
    }
}
