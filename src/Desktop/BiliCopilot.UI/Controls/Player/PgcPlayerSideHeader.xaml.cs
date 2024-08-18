// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using BiliCopilot.UI.ViewModels.View;

namespace BiliCopilot.UI.Controls.Player;

/// <summary>
/// PGC播放页侧边栏头部.
/// </summary>
public sealed partial class PgcPlayerSideHeader : PgcPlayerPageControlBase
{
    private long _viewModelChangedToken;
    private PgcPlayerPageViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="PgcPlayerSideHeader"/> class.
    /// </summary>
    public PgcPlayerSideHeader() => InitializeComponent();

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
        ViewModel.SectionInitialized += OnViewModelSectionInitialized;
        InitializeChildPartitions();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.SectionInitialized -= OnViewModelSectionInitialized;
        }

        UnregisterPropertyChangedCallback(ViewModelProperty, _viewModelChangedToken);
        Selector.SelectionChanged -= OnSelectorChanged;
        _viewModel = default;
    }

    private void OnViewModelPropertyChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (_viewModel is not null)
        {
            _viewModel.SectionInitialized -= OnViewModelSectionInitialized;
        }

        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        _viewModel.SectionInitialized += OnViewModelSectionInitialized;
        InitializeChildPartitions();
    }

    private void OnViewModelSectionInitialized(object? sender, EventArgs e)
        => InitializeChildPartitions();

    private void OnSelectorChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {
        if (sender.SelectedItem is null)
        {
            return;
        }

        var item = sender.SelectedItem.Tag as IPlayerSectionDetailViewModel;
        ViewModel.SelectSectionCommand.Execute(item);
    }

    private void InitializeChildPartitions()
    {
        Selector.Items.Clear();
        if (ViewModel.Sections is not null)
        {
            foreach (var item in ViewModel.Sections)
            {
                Selector.Items.Add(new SelectorBarItem()
                {
                    Text = item.Title ?? default,
                    Tag = item,
                });
            }
        }

        Selector.SelectedItem = Selector.Items.FirstOrDefault(p => p.Tag == ViewModel.SelectedSection);
    }
}
