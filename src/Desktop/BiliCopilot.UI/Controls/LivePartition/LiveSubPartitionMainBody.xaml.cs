// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.LivePartition;

/// <summary>
/// 直播子分区详情.
/// </summary>
public sealed partial class LiveSubPartitionMainBody : LiveSubPartitionMainBodyBase
{
    private long _viewModelChangedToken;
    private LivePartitionDetailViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="LiveSubPartitionMainBody"/> class.
    /// </summary>
    public LiveSubPartitionMainBody()
    {
        InitializeComponent();
    }

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new(Bindings.Initialize, Bindings.StopTracking);

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        Selector.SelectionChanged += OnSelectorChanged;
        LiveScrollView.ViewChanged += OnViewChanged;
        LiveScrollView.SizeChanged += OnScrollViewSizeChanged;
        _viewModelChangedToken = RegisterPropertyChangedCallback(ViewModelProperty, new DependencyPropertyChangedCallback(OnViewModelPropertyChanged));
        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        ViewModel.Initialized += OnViewModelInitialized;
        ViewModel.LiveListUpdated += OnLiveListUpdatedAsync;
        InitializeTags();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        Selector.SelectionChanged -= OnSelectorChanged;
        LiveScrollView.ViewChanged -= OnViewChanged;
        LiveScrollView.SizeChanged -= OnScrollViewSizeChanged;
        _viewModel = default;
        UnregisterPropertyChangedCallback(ViewModelProperty, _viewModelChangedToken);
        if (ViewModel is not null)
        {
            ViewModel.Initialized -= OnViewModelInitialized;
            ViewModel.LiveListUpdated -= OnLiveListUpdatedAsync;
        }
    }

    private void OnViewModelPropertyChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (_viewModel is not null)
        {
            _viewModel.Initialized -= OnViewModelInitialized;
            _viewModel.LiveListUpdated -= OnLiveListUpdatedAsync;
        }

        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        ViewModel.Initialized += OnViewModelInitialized;
        ViewModel.LiveListUpdated += OnLiveListUpdatedAsync;
        InitializeTags();
    }

    private void OnViewModelInitialized(object? sender, EventArgs e)
        => InitializeTags();

    private async void OnLiveListUpdatedAsync(object? sender, EventArgs e)
    {
        await Task.Delay(500);
        CheckRoomCount();
    }

    private void OnSelectorChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {
        if (sender.SelectedItem is null)
        {
            return;
        }

        var item = sender.SelectedItem.Tag as LiveTag;
        if (item is not null && item != ViewModel.CurrentTag)
        {
            ViewModel.ChangeChildPartitionCommand.Execute(item);
        }
    }

    private void InitializeTags()
    {
        Selector.Items.Clear();
        if (ViewModel.Children is not null)
        {
            foreach (var item in ViewModel.Children)
            {
                Selector.Items.Add(new SelectorBarItem()
                {
                    Text = item.Name ?? default,
                    Tag = item,
                });
            }
        }

        Selector.SelectedItem = Selector.Items.FirstOrDefault(p => p.Tag == ViewModel.CurrentTag);
    }

    private void OnViewChanged(ScrollView sender, object args)
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            if (LiveScrollView.ExtentHeight - LiveScrollView.ViewportHeight - LiveScrollView.VerticalOffset <= 40)
            {
                ViewModel.LoadRoomsCommand.Execute(default);
            }
        });
    }

    private void OnScrollViewSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize.Width > 100 && ViewModel is not null)
        {
            CheckRoomCount();
        }
    }

    private void CheckRoomCount()
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            if (LiveScrollView.ScrollableHeight <= 0 && ViewModel is not null)
            {
                ViewModel.LoadRoomsCommand.Execute(default);
            }
        });
    }
}

/// <summary>
/// 直播子分区详情基类.
/// </summary>
public abstract class LiveSubPartitionMainBodyBase : LayoutUserControlBase<LivePartitionDetailViewModel>
{
}
