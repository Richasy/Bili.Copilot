// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;
using Richasy.BiliKernel.Models;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.VideoPartition;

/// <summary>
/// 视频分区主区域头部.
/// </summary>
public sealed partial class VideoPartitionMainHeader : VideoPartitionDetailControlBase
{
    private long _viewModelChangedToken;
    private VideoPartitionDetailViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="VideoPartitionMainHeader"/> class.
    /// </summary>
    public VideoPartitionMainHeader() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new(Bindings.Initialize, Bindings.StopTracking);

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        Selector.SelectionChanged += OnSelectorChanged;
        _viewModelChangedToken = RegisterPropertyChangedCallback(ViewModelProperty, new DependencyPropertyChangedCallback(OnViewModelPropertyChanged));
        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        ViewModel.Initialized += OnViewModelInitialized;
        InitializeChildPartitions();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        Selector.SelectionChanged -= OnSelectorChanged;
        _viewModel = default;
        UnregisterPropertyChangedCallback(ViewModelProperty, _viewModelChangedToken);
        if (ViewModel is not null)
        {
            ViewModel.Initialized -= OnViewModelInitialized;
        }
    }

    private void OnViewModelPropertyChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (_viewModel is not null)
        {
            _viewModel.Initialized -= OnViewModelInitialized;
        }

        _viewModel = ViewModel;
        _viewModel.Initialized += OnViewModelInitialized;
        InitializeChildPartitions();
    }

    private void OnViewModelInitialized(object? sender, EventArgs e)
        => InitializeChildPartitions();

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

    private void OnSortTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Any()
            && e.AddedItems[0] is PartitionVideoSortType sortType
            && sortType != ViewModel.SelectedSortType)
        {
            ViewModel.ChangeSortTypeCommand.Execute(sortType);
        }
    }
}
