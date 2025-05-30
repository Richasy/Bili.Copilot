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
    protected override void OnControlLoaded()
        => CheckSelectedItem();

    protected override void OnControlUnloaded()
        => View.ItemsSource = default;

    /// <inheritdoc/>
    protected override void OnViewModelChanged(VideoPlayerSeasonSectionDetailViewModel? oldValue, VideoPlayerSeasonSectionDetailViewModel? newValue)
        => CheckSelectedItem();

    private void OnSeasonChanged(object sender, SelectionChangedEventArgs e)
    {
        var season = SeasonComboBox.SelectedItem as VideoSeason;
        ViewModel.ChangeSeasonCommand.Execute(season);
        CheckSelectedItem();
    }

    private void OnVideoSelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
    {
        var item = sender.SelectedItem as VideoItemViewModel;
        if (item is not null && ViewModel.SelectedItem != item)
        {
            ViewModel.SelectedItem = item;
            ViewModel.Page.InitializePageCommand.Execute(new VideoSnapshot(item.Data));
        }
    }

    private void CheckSelectedItem()
    {
        if (ViewModel.SelectedItem is null)
        {
            return;
        }

        var index = ViewModel.Items.ToList().IndexOf(ViewModel.SelectedItem);
        View.Select(index);
    }
}

/// <summary>
/// 视频播放器合集部分基类.
/// </summary>
public abstract class VideoSeasonSectionBase : LayoutUserControlBase<VideoPlayerSeasonSectionDetailViewModel>
{
}
