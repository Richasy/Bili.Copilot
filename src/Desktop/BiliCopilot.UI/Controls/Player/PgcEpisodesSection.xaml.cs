// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.WinUIKernel.Share.Base;

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
        InitializeLayoutAsync();
    }

    private async void OnIndexToggledAsync(object sender, RoutedEventArgs e)
    {
        await Task.Delay(100);
        InitializeLayoutAsync();
    }

    private async void InitializeLayoutAsync()
    {
        await Task.Delay(100);
        var selectedItem = ViewModel.Episodes.Find(p => p.IsSelected);
        if (selectedItem is null)
        {
            return;
        }

        var index = ViewModel.Episodes.ToList().IndexOf(selectedItem);
        var actualOffset = 0d;
        if (ViewModel.OnlyIndex)
        {
            var offset = 40 * (index / 7);
            actualOffset = offset - IndexView.ViewportHeight;

            if (actualOffset > 0)
            {
                IndexView.ScrollTo(0, actualOffset);
            }
        }
        else
        {
            var offset = 86 * index;
            actualOffset = offset - DefaultView.ViewportHeight;

            if (actualOffset > 0)
            {
                DefaultView.ScrollTo(0, actualOffset + (DefaultView.ViewportHeight / 2));
            }
        }
    }
}

/// <summary>
/// PGC 播放器页面剧集控件基类.
/// </summary>
public abstract class PgcEpisodesSectionBase : LayoutUserControlBase<PgcPlayerEpisodeSectionDetailViewModel>
{
}
