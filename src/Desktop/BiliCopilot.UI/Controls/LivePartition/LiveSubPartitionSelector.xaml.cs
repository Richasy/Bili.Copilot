// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.BiliKernel.Models.Media;

namespace BiliCopilot.UI.Controls.LivePartition;

/// <summary>
/// 直播子分区选择器.
/// </summary>
public sealed partial class LiveSubPartitionSelector : LiveSubPartitionControlBase
{
    private long _viewModelChangedToken;
    private LivePartitionDetailViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="LiveSubPartitionSelector"/> class.
    /// </summary>
    public LiveSubPartitionSelector() => InitializeComponent();

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
        InitializeTags();
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

        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        ViewModel.Initialized += OnViewModelInitialized;
        InitializeTags();
    }

    private void OnViewModelInitialized(object? sender, EventArgs e)
        => InitializeTags();

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
}
