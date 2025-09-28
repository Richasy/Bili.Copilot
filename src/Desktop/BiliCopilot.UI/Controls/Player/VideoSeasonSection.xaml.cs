// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Controls.Player;

/// <summary>
/// 视频播放器合集部分.
/// </summary>
public sealed partial class VideoSeasonSection : VideoSeasonSectionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoSeasonSection"/> class.
    /// </summary>
    public VideoSeasonSection() => InitializeComponent();

    private async void OnSeasonChanged(object sender, SelectionChangedEventArgs e)
    {
        var season = SeasonComboBox.SelectedItem as VideoSeason;
        ViewModel.ChangeSeasonCommand.Execute(season);
        await CheckSelectedItemAsync();
    }

    private async Task CheckSelectedItemAsync()
    {
        await Task.Delay(200);
        var selectedItem = ViewModel.Items.Find(p => p.IsSelected);
        if (selectedItem is null)
        {
            return;
        }

        var index = ViewModel.Items.ToList().IndexOf(selectedItem);
        var offset = 86 * index;
        var actualOffset = offset - View.ViewportHeight;
        if (actualOffset > 0)
        {
            View.ScrollTo(0, actualOffset + (View.ViewportHeight / 2));
        }
    }
}

/// <summary>
/// 视频播放器合集部分基类.
/// </summary>
public abstract class VideoSeasonSectionBase : LayoutUserControlBase<VideoPlayerSeasonSectionDetailViewModel>
{
}
