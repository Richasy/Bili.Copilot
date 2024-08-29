// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.History;

/// <summary>
/// 直播历史记录区域.
/// </summary>
public sealed partial class LiveHistorySection : LiveHistorySectionBase
{
    private long _viewModelChangedToken;
    private LiveHistorySectionDetailViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="LiveHistorySection"/> class.
    /// </summary>
    public LiveHistorySection() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        LiveScrollView.ViewChanged += OnViewChanged;
        LiveScrollView.SizeChanged += OnScrollViewSizeChanged;
        _viewModelChangedToken = RegisterPropertyChangedCallback(ViewModelProperty, new DependencyPropertyChangedCallback(OnViewModelPropertyChanged));
        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        ViewModel.ListUpdated += OnLiveListUpdatedAsync;
        CheckLiveCount();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.ListUpdated -= OnLiveListUpdatedAsync;
        }

        UnregisterPropertyChangedCallback(ViewModelProperty, _viewModelChangedToken);
        LiveScrollView.ViewChanged -= OnViewChanged;
        LiveScrollView.SizeChanged -= OnScrollViewSizeChanged;
        _viewModel = default;
    }

    private void OnViewModelPropertyChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (_viewModel is not null)
        {
            _viewModel.ListUpdated -= OnLiveListUpdatedAsync;
        }

        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        _viewModel.ListUpdated += OnLiveListUpdatedAsync;
    }

    private async void OnLiveListUpdatedAsync(object? sender, EventArgs e)
    {
        await Task.Delay(500);
        CheckLiveCount();
    }

    private void OnViewChanged(ScrollView sender, object args)
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            if (LiveScrollView.ExtentHeight - LiveScrollView.ViewportHeight - LiveScrollView.VerticalOffset <= 40)
            {
                ViewModel.LoadItemsCommand.Execute(default);
            }
        });
    }

    private void OnScrollViewSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize.Width > 100 && ViewModel is not null)
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
                ViewModel.LoadItemsCommand.Execute(default);
            }
        });
    }
}

/// <summary>
/// 直播历史记录区域基类.
/// </summary>
public abstract class LiveHistorySectionBase : LayoutUserControlBase<LiveHistorySectionDetailViewModel>
{
}
