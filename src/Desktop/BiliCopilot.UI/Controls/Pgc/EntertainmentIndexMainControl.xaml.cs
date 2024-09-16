// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Pgc;

/// <summary>
/// 娱乐索引主体.
/// </summary>
public sealed partial class EntertainmentIndexMainControl : EntertainmentIndexControlBase
{
    private long _viewModelChangedToken;
    private EntertainmentIndexViewModel _viewModel;

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

        _viewModelChangedToken = RegisterPropertyChangedCallback(ViewModelProperty, new DependencyPropertyChangedCallback(OnViewModelPropertyChanged));
        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        ViewModel.ItemsUpdated += OnItemsUpdatedAsync;
        CheckSeasonCount();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.ItemsUpdated -= OnItemsUpdatedAsync;
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
            _viewModel.ItemsUpdated -= OnItemsUpdatedAsync;
        }

        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        SeasonScrollView.ChangeView(0, 0, default, true);
        _viewModel.ItemsUpdated += OnItemsUpdatedAsync;
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
