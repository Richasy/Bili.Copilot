// Copyright (c) Bili Copilot. All rights reserved.

using System.Linq;
using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Models.Data.Video;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 合集视图.
/// </summary>
public sealed partial class UgcSeasonView : UgcSeasonViewBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UgcSeasonView"/> class.
    /// </summary>
    public UgcSeasonView() => InitializeComponent();

    private void OnSeasonComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var season = SeasonComboBox.SelectedItem as VideoSeason;
        if (ViewModel.CurrentSeason != season)
        {
            ViewModel.SelectSeasonCommand.Execute(season);
        }
    }

    private async void OnRepeaterLoadedAsync(object sender, RoutedEventArgs e)
    {
        await System.Threading.Tasks.Task.Delay(200);
        if (ViewModel.IsShowUgcSeason)
        {
            var selectedVideo = ViewModel.CurrentSeasonVideos.FirstOrDefault(p => p.IsSelected);
            if (selectedVideo != null)
            {
                var index = ViewModel.CurrentSeasonVideos.IndexOf(selectedVideo);
                if (index > 0)
                {
                    (sender as VerticalRepeaterView).ScrollToItem(selectedVideo);
                }
            }
        }
    }
}

/// <summary>
/// <see cref="UgcSeason"/> 的基类.
/// </summary>
public class UgcSeasonViewBase : ReactiveUserControl<VideoPlayerPageViewModel>
{
}
