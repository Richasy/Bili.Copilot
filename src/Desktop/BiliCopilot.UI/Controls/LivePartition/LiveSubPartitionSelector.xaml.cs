// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.BiliKernel.Models.Media;

namespace BiliCopilot.UI.Controls.LivePartition;

/// <summary>
/// 直播子分区选择器.
/// </summary>
public sealed partial class LiveSubPartitionSelector : LiveSubPartitionControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LiveSubPartitionSelector"/> class.
    /// </summary>
    public LiveSubPartitionSelector() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        Selector.SelectionChanged += OnSelectorChanged;
        if (ViewModel is null)
        {
            return;
        }

        InitializeTags();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        Selector.SelectionChanged -= OnSelectorChanged;
        if (ViewModel is not null)
        {
            ViewModel.Initialized -= OnViewModelInitialized;
        }
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(LivePartitionDetailViewModel? oldValue, LivePartitionDetailViewModel? newValue)
    {
        if (oldValue is not null)
        {
            oldValue.Initialized -= OnViewModelInitialized;
        }

        if (newValue is null)
        {
            return;
        }

        newValue.Initialized += OnViewModelInitialized;
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
