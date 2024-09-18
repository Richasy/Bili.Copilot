// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Controls.Player;

/// <summary>
/// 视频播放列表.
/// </summary>
public sealed partial class VideoPlaylistSection : VideoPlaylistSectionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoPlaylistSection"/> class.
    /// </summary>
    public VideoPlaylistSection() => InitializeComponent();

    /// <inheritdoc/>
    protected override async void OnControlLoaded()
        => await CheckSelectedItemAsync();

    /// <inheritdoc/>
    protected override async void OnViewModelChanged(VideoPlayerPlaylistSectionDetailViewModel? oldValue, VideoPlayerPlaylistSectionDetailViewModel? newValue)
        => await CheckSelectedItemAsync();

    private void OnVideoSelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
    {
        var item = sender.SelectedItem as VideoItemViewModel;
        if (item is not null && ViewModel.SelectedItem != item)
        {
            item.PlayCommand.Execute(default);
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
/// 视频播放列表基类.
/// </summary>
public abstract class VideoPlaylistSectionBase : LayoutUserControlBase<VideoPlayerPlaylistSectionDetailViewModel>
{
}
