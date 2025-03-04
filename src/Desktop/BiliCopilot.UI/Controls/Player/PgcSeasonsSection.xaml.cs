// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.Player;

/// <summary>
/// PGC 播放器页面剧集区域.
/// </summary>
public sealed partial class PgcSeasonsSection : PgcSeasonsSectionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PgcSeasonsSection"/> class.
    /// </summary>
    public PgcSeasonsSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        if (ViewModel.SelectedItem is SeasonItemViewModel part)
        {
            View.Select(ViewModel.Items.ToList().IndexOf(part));
        }
    }

    private void OnSelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
    {
        if (IsLoaded && sender.SelectedItem is SeasonItemViewModel season && season != ViewModel.SelectedItem)
        {
            season.PlayCommand.Execute(default);
        }
    }
}

/// <summary>
/// PGC 播放器页面剧集区域基类.
/// </summary>
public abstract class PgcSeasonsSectionBase : LayoutUserControlBase<PgcPlayerSeasonSectionDetailViewModel>
{
}
