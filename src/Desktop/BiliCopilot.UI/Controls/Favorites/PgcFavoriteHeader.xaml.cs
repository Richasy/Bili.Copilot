// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.BiliKernel.Models;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.Favorites;

/// <summary>
/// PGC收藏头部.
/// </summary>
public sealed partial class PgcFavoriteHeader : PgcFavoriteControlBase
{
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
        if (ViewModel is null)
        {
            return;
        }

        UpdateStatusSelectionAsync();
    }

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
        => StatusComboBox.SelectionChanged -= OnStatusChanged;

    /// <inheritdoc/>
    protected override void OnViewModelChanged(PgcFavoriteSectionDetailViewModel? oldValue, PgcFavoriteSectionDetailViewModel? newValue)
    {
        if (newValue is null)
        {
            return;
        }

        UpdateStatusSelectionAsync();
    }

    private async void UpdateStatusSelectionAsync()
    {
        if (ViewModel is null)
        {
            return;
        }

        StatusComboBox.SelectedIndex = -1;
        await Task.Delay(500);
        StatusComboBox.SelectedIndex = (int)ViewModel.CurrentStatus - 1;
    }

    private void OnStatusChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsLoaded || StatusComboBox.SelectedIndex == -1)
        {
            return;
        }

        var status = (PgcFavoriteStatus)(StatusComboBox.SelectedIndex + 1);
        ViewModel.ChangeStatusCommand.Execute(status);
    }
}
