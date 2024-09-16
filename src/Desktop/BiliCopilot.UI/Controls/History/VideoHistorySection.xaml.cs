// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.History;

/// <summary>
/// 视频历史记录区域.
/// </summary>
public sealed partial class VideoHistorySection : VideoHistorySectionBase
{
    private long _viewModelChangedToken;
    private VideoHistorySectionDetailViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="VideoHistorySection"/> class.
    /// </summary>
    public VideoHistorySection() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        VideoScrollView.ViewChanged += OnViewChanged;
        VideoScrollView.SizeChanged += OnScrollViewSizeChanged;
        _viewModelChangedToken = RegisterPropertyChangedCallback(ViewModelProperty, new DependencyPropertyChangedCallback(OnViewModelPropertyChanged));
        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        ViewModel.ListUpdated += OnVideoListUpdatedAsync;
        CheckVideoCount();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.ListUpdated -= OnVideoListUpdatedAsync;
        }

        UnregisterPropertyChangedCallback(ViewModelProperty, _viewModelChangedToken);
        VideoScrollView.ViewChanged -= OnViewChanged;
        VideoScrollView.SizeChanged -= OnScrollViewSizeChanged;
        _viewModel = default;
    }

    private void OnViewModelPropertyChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (_viewModel is not null)
        {
            _viewModel.ListUpdated -= OnVideoListUpdatedAsync;
        }

        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        _viewModel.ListUpdated += OnVideoListUpdatedAsync;
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
            if (VideoScrollView.ExtentHeight - VideoScrollView.ViewportHeight - VideoScrollView.VerticalOffset <= 40)
            {
                ViewModel.LoadItemsCommand.Execute(default);
            }
        });
    }

    private void OnScrollViewSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize.Width > 100 && ViewModel is not null)
        {
            CheckVideoCount();
        }
    }

    private void CheckVideoCount()
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            if (VideoScrollView.ScrollableHeight <= 0 && ViewModel is not null)
            {
                ViewModel.LoadItemsCommand.Execute(default);
            }
        });
    }
}

/// <summary>
/// 视频历史记录区域基类.
/// </summary>
public abstract class VideoHistorySectionBase : LayoutUserControlBase<VideoHistorySectionDetailViewModel>
{
}
