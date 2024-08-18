// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Player;

/// <summary>
/// PGC 播放器页面剧集控件.
/// </summary>
public sealed partial class PgcEpisodesSection : PgcEpisodesSectionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PgcEpisodesSection"/> class.
    /// </summary>
    public PgcEpisodesSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        if (ViewModel.SelectedEpisode is EpisodeItemViewModel part)
        {
            DefaultView.Select(ViewModel.Episodes.ToList().IndexOf(part));
            IndexView.Select(ViewModel.Episodes.ToList().IndexOf(part));
        }

        InitializeLayoutAsync();
    }

    private void OnEpisodeSelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
    {
        if (IsLoaded && sender.SelectedItem is EpisodeItemViewModel episode && episode != ViewModel.SelectedEpisode)
        {
            ViewModel.SelectEpisodeCommand.Execute(episode);
        }
    }

    private async void OnIndexToggledAsync(object sender, RoutedEventArgs e)
    {
        await Task.Delay(100);
        InitializeLayoutAsync();
    }

    private async void InitializeLayoutAsync()
    {
        await Task.Delay(100);
        if (ViewModel.OnlyIndex)
        {
            IndexView.StartBringItemIntoView(ViewModel.Episodes.ToList().IndexOf(ViewModel.SelectedEpisode), new BringIntoViewOptions { VerticalAlignmentRatio = 0.5 });
        }
        else
        {
            DefaultView.StartBringItemIntoView(ViewModel.Episodes.ToList().IndexOf(ViewModel.SelectedEpisode), new BringIntoViewOptions { VerticalAlignmentRatio = 0.5 });
        }
    }
}

/// <summary>
/// PGC 播放器页面剧集控件基类.
/// </summary>
public abstract class PgcEpisodesSectionBase : LayoutUserControlBase<PgcPlayerEpisodeSectionDetailViewModel>
{
}
