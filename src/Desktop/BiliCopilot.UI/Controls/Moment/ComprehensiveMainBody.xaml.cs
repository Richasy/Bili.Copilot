// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Moment;

/// <summary>
/// 综合动态分区主体.
/// </summary>
public sealed partial class ComprehensiveMainBody : MomentUperSectionControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ComprehensiveMainBody"/> class.
    /// </summary>
    public ComprehensiveMainBody() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        MomentScrollView.ViewChanged += OnViewChanged;
        MomentScrollView.SizeChanged += OnScrollViewSizeChanged;

        if (ViewModel is null)
        {
            return;
        }

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

    /// <inheritdoc/>
    protected override void OnViewModelChanged(MomentUperSectionViewModel? oldValue, MomentUperSectionViewModel? newValue)
    {
        if (oldValue is not null)
        {
            oldValue.ListUpdated -= OnListUpdatedAsync;
        }

        if (newValue is null)
        {
            return;
        }

        newValue.ListUpdated += OnListUpdatedAsync;
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
                ViewModel?.LoadItemsCommand.Execute(default);
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
                ViewModel?.LoadItemsCommand.Execute(default);
            }
        });
    }
}
