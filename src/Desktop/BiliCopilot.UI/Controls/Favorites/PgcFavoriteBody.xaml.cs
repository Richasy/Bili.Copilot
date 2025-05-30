// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;

namespace BiliCopilot.UI.Controls.Favorites;

/// <summary>
/// PGC收藏主体.
/// </summary>
public sealed partial class PgcFavoriteBody : PgcFavoriteControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PgcFavoriteBody"/> class.
    /// </summary>
    public PgcFavoriteBody() => InitializeComponent();

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
            ViewModel.ListUpdated -= OnSeasonListUpdatedAsync;
        }

        SeasonRepeater.ItemsSource = null;
        SeasonScrollView.ViewChanged -= OnViewChanged;
        SeasonScrollView.SizeChanged -= OnScrollViewSizeChanged;
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(PgcFavoriteSectionDetailViewModel? oldValue, PgcFavoriteSectionDetailViewModel? newValue)
    {
        if (oldValue is not null)
        {
            oldValue.ListUpdated -= OnSeasonListUpdatedAsync;
        }

        if (newValue is null)
        {
            return;
        }

        newValue.ListUpdated += OnSeasonListUpdatedAsync;
    }

    private async void OnSeasonListUpdatedAsync(object? sender, EventArgs e)
    {
        await Task.Delay(500);
        CheckSeasonCount();
    }

    private void OnViewChanged(object? sender, ScrollViewerViewChangedEventArgs args)
    {
        Richasy.WinUIKernel.Share.WinUIKernelShareExtensions.IsCardAnimationEnabled = !args.IsIntermediate;
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            if (SeasonScrollView.ExtentHeight - SeasonScrollView.ViewportHeight - SeasonScrollView.VerticalOffset <= 240)
            {
                ViewModel.LoadItemsCommand.Execute(default);
            }
        });
    }

    private void OnScrollViewSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize.Width > 100 && ViewModel is not null)
        {
            CheckSeasonCount();
        }
    }

    private void CheckSeasonCount()
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            if (SeasonScrollView.ScrollableHeight <= 240 && ViewModel is not null)
            {
                ViewModel.LoadItemsCommand.Execute(default);
            }
        });
    }
}
