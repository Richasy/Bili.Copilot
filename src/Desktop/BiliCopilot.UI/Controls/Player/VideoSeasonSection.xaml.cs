// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models;
using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;
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

    /// <inheritdoc/>
    protected override async void OnControlLoaded()
        => await CheckSelectedItemAsync();

    /// <inheritdoc/>
    protected override async void OnViewModelChanged(VideoPlayerSeasonSectionDetailViewModel? oldValue, VideoPlayerSeasonSectionDetailViewModel? newValue)
        => await CheckSelectedItemAsync();

    private async void OnSeasonChangedAsync(object sender, SelectionChangedEventArgs e)
    {
        var season = SeasonComboBox.SelectedItem as VideoSeason;
        ViewModel.ChangeSeasonCommand.Execute(season);
        await CheckSelectedItemAsync();
    }

    private async void OnVideoSelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
    {
        var item = sender.SelectedItem as VideoItemViewModel;
        if (item is not null && ViewModel.SelectedItem != item)
        {
            ViewModel.SelectedItem = item;
            if (ViewModel.Source != null)
            {
                ViewModel.Source.InjectSnapshot(new VideoSnapshot(item.Data));
                await ViewModel.Source.InitializeAsync();
            }
            else if (ViewModel.Page != null)
            {
                ViewModel.Page.InitializePageCommand.Execute(new VideoSnapshot(item.Data));
            }
        }
    }

    private async Task CheckSelectedItemAsync()
    {
        await Task.Delay(100);
        if (ViewModel.SelectedItem is null)
        {
            return;
        }

        var index = ViewModel.Items.ToList().IndexOf(ViewModel.SelectedItem);
        View.Select(index);
        View.StartBringItemIntoView(index, new BringIntoViewOptions { VerticalAlignmentRatio = 0.5 });
    }
}

/// <summary>
/// 视频播放器合集部分基类.
/// </summary>
public abstract class VideoSeasonSectionBase : LayoutUserControlBase<VideoPlayerSeasonSectionDetailViewModel>
{
}
