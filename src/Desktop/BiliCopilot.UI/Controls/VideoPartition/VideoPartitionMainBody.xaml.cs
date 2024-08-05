// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.VideoPartition;

/// <summary>
/// 视频分区主体.
/// </summary>
public sealed partial class VideoPartitionMainBody : VideoPartitionDetailControlBase
{
    private long _viewModelChangedToken;
    private VideoPartitionDetailViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="VideoPartitionMainBody"/> class.
    /// </summary>
    public VideoPartitionMainBody() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new ControlBindings(Bindings.Initialize, Bindings.StopTracking);

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        VideoScrollView.ViewChanged += OnViewChanged;
        VideoScrollView.SizeChanged += OnScrollViewSizeChanged;
        Selector.SelectionChanged += OnSelectorChanged;
        _viewModelChangedToken = RegisterPropertyChangedCallback(ViewModelProperty, new DependencyPropertyChangedCallback(OnViewModelPropertyChanged));
        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        ViewModel.VideoListUpdated += OnVideoListUpdatedAsync;
        ViewModel.Initialized += OnViewModelInitialized;
        InitializeChildPartitions();
        CheckVideoCount();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.Initialized -= OnViewModelInitialized;
            ViewModel.VideoListUpdated -= OnVideoListUpdatedAsync;
        }

        UnregisterPropertyChangedCallback(ViewModelProperty, _viewModelChangedToken);
        Selector.SelectionChanged -= OnSelectorChanged;
        VideoScrollView.ViewChanged -= OnViewChanged;
        VideoScrollView.SizeChanged -= OnScrollViewSizeChanged;
        _viewModel = default;
    }

    private void OnViewModelPropertyChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (_viewModel is not null)
        {
            _viewModel.Initialized -= OnViewModelInitialized;
            _viewModel.VideoListUpdated -= OnVideoListUpdatedAsync;
        }

        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        _viewModel.VideoListUpdated += OnVideoListUpdatedAsync;
        _viewModel.Initialized += OnViewModelInitialized;
        InitializeChildPartitions();
    }

    private void OnViewModelInitialized(object? sender, EventArgs e)
        => InitializeChildPartitions();

    private async void OnVideoListUpdatedAsync(object? sender, EventArgs e)
    {
        await Task.Delay(500);
        CheckVideoCount();
    }

    private void OnSelectorChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {
        if (sender.SelectedItem is null)
        {
            return;
        }

        var item = sender.SelectedItem.Tag as PartitionViewModel;
        if (item is not null && item != ViewModel.CurrentPartition)
        {
            ViewModel.ChangeChildPartitionCommand.Execute(item);
        }
    }

    private void InitializeChildPartitions()
    {
        Selector.Items.Clear();
        if (ViewModel.Children is not null)
        {
            foreach (var item in ViewModel.Children)
            {
                Selector.Items.Add(new SelectorBarItem()
                {
                    Text = item.Title ?? default,
                    Tag = item,
                });
            }
        }

        Selector.SelectedItem = ViewModel.IsRecommend
            ? Selector.Items.FirstOrDefault()
            : Selector.Items.FirstOrDefault(p => p.Tag == ViewModel.CurrentPartition);
    }

    private void OnViewChanged(ScrollView sender, object args)
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            if (VideoScrollView.ExtentHeight - VideoScrollView.ViewportHeight - VideoScrollView.VerticalOffset <= 40)
            {
                ViewModel.LoadVideosCommand.Execute(default);
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
                ViewModel.LoadVideosCommand.Execute(default);
            }
        });
    }
}
