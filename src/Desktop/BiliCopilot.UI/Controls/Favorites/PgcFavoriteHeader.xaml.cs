// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.BiliKernel.Models;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Favorites;

/// <summary>
/// PGC收藏头部.
/// </summary>
public sealed partial class PgcFavoriteHeader : PgcFavoriteControlBase
{
    private long _viewModelChangedToken;
    private PgcFavoriteSectionDetailViewModel _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="PgcFavoriteHeader"/> class.
    /// </summary>
    public PgcFavoriteHeader() => InitializeComponent();

    /// <inheritdoc/>
    protected override ControlBindings? ControlBindings => Bindings is null ? null : new(Bindings.Initialize, Bindings.StopTracking);

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        StatusComboBox.SelectionChanged += OnStatusChanged;
        _viewModelChangedToken = RegisterPropertyChangedCallback(ViewModelProperty, new DependencyPropertyChangedCallback(OnViewModelPropertyChanged));
        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        UpdateStatusSelection();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        UnregisterPropertyChangedCallback(ViewModelProperty, _viewModelChangedToken);
        StatusComboBox.SelectionChanged -= OnStatusChanged;
        _viewModel = default;
    }

    private void OnViewModelPropertyChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (ViewModel is null)
        {
            return;
        }

        _viewModel = ViewModel;
        UpdateStatusSelection();
    }

    private void UpdateStatusSelection()
    {
        if (ViewModel is null)
        {
            return;
        }

        if (StatusComboBox.ItemsSource is null)
        {
            StatusComboBox.ItemsSource = ViewModel.StatusList;
        }

        StatusComboBox.SelectedIndex = (int)ViewModel.CurrentStatus - 1;
    }

    private void OnStatusChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsLoaded)
        {
            return;
        }

        var status = (PgcFavoriteStatus)(StatusComboBox.SelectedIndex + 1);
        ViewModel.ChangeStatusCommand.Execute(status);
    }
}
