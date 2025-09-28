// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;

namespace BiliCopilot.UI.Controls.Favorites;

/// <summary>
/// PGC收藏主体.
/// </summary>
public sealed partial class PgcFavoriteBody : PgcFavoriteControlBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PgcFavoriteBody"/> class.
    /// </summary>
    public PgcFavoriteBody() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlUnloaded()
    {
        if (ViewModel is not null)
        {
            ViewModel.ListUpdated -= OnSeasonListUpdatedAsync;
        }
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(PgcFavoriteSectionDetailViewModel? oldValue, PgcFavoriteSectionDetailViewModel? newValue)
    {
        if (oldValue is not null)
        {
            oldValue.ListUpdated -= OnSeasonListUpdatedAsync;
        }

        if (newValue is null)
        {
            return;
        }

        newValue.ListUpdated += OnSeasonListUpdatedAsync;
    }

    private async void OnSeasonListUpdatedAsync(object? sender, EventArgs e)
    {
        await View.DelayCheckItemsAsync();
    }
}
