// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// PGC 分集视图.
/// </summary>
public sealed partial class PgcEpisodeView : PgcEpisodeViewBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PgcEpisodeView"/> class.
    /// </summary>
    public PgcEpisodeView() => InitializeComponent();

    private async void OnEpisodeItemClickAsync(object sender, RoutedEventArgs e)
    {
        var card = sender as CardPanel;
        var data = card.DataContext as EpisodeItemViewModel;
        if (!data.Data.Equals(ViewModel.CurrentEpisode))
        {
            ViewModel.ChangeEpisodeCommand.Execute(data.Data);
        }
        else
        {
            data.IsSelected = false;
            await Task.Delay(100);
            data.IsSelected = true;
        }
    }

    private void OnEpisodeRepeaterLoaded(object sender, RoutedEventArgs e)
        => RelocateSelectedItem();

    private void RelocateSelectedItem()
    {
        var vm = ViewModel.Episodes.FirstOrDefault(p => p.IsSelected);
        if (vm != null)
        {
            var index = ViewModel.Episodes.IndexOf(vm);
            if (index >= 0)
            {
                EpisodeRepeater.ScrollToItem(vm);
                if (ViewModel.IsOnlyShowIndex)
                {
                    var ele = IndexRepeater.GetOrCreateElement(index);
                    ele?.StartBringIntoView(new BringIntoViewOptions { VerticalAlignmentRatio = 0f });
                }
            }
        }
    }
}

/// <summary>
/// <see cref="PgcEpisodeView"/> 的基类.
/// </summary>
public abstract class PgcEpisodeViewBase : ReactiveUserControl<PgcPlayerPageViewModel>
{
}
