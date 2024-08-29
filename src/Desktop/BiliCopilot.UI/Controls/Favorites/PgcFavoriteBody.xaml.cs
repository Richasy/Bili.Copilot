// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Favorites;

/// <summary>
/// PGC收藏主体.
/// </summary>
public sealed partial class PgcFavoriteBody : PgcFavoriteControlBase
{
    private long _viewModelChangedToken;
    private PgcFavoriteSectionDetailViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="PgcFavoriteBody"/> class.
    /// </summary>
    public PgcFavoriteBody() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        SeasonScrollView.ViewChanged += OnViewChanged;
        SeasonScrollView.SizeChanged += OnScrollViewSizeChanged;
        _viewModelChangedToken = RegisterPropertyChangedCallback(ViewModelProperty, new DependencyPropertyChangedCallback(OnViewModelPropertyChanged));
        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        ViewModel.ListUpdated += OnSeasonListUpdatedAsync;
        CheckSeasonCount();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.ListUpdated -= OnSeasonListUpdatedAsync;
        }

        UnregisterPropertyChangedCallback(ViewModelProperty, _viewModelChangedToken);
        SeasonScrollView.ViewChanged -= OnViewChanged;
        SeasonScrollView.SizeChanged -= OnScrollViewSizeChanged;
        _viewModel = default;
    }

    private void OnViewModelPropertyChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (_viewModel is not null)
        {
            _viewModel.ListUpdated -= OnSeasonListUpdatedAsync;
        }

        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        _viewModel.ListUpdated += OnSeasonListUpdatedAsync;
    }

    private async void OnSeasonListUpdatedAsync(object? sender, EventArgs e)
    {
        await Task.Delay(500);
        CheckSeasonCount();
    }

    private void OnViewChanged(ScrollView sender, object args)
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            if (SeasonScrollView.ExtentHeight - SeasonScrollView.ViewportHeight - SeasonScrollView.VerticalOffset <= 40)
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
            if (SeasonScrollView.ScrollableHeight <= 0 && ViewModel is not null)
            {
                ViewModel.LoadItemsCommand.Execute(default);
            }
        });
    }
}
