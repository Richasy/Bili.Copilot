// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using Richasy.WinUIKernel.Share.Base;

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

    private async Task CheckSelectedItemAsync()
    {
        await Task.Delay(200);
        var selectedItem = ViewModel.Items.Find(p => p.IsSelected);
        if (selectedItem is null)
        {
            return;
        }

        var index = ViewModel.Items.ToList().IndexOf(selectedItem);
        var offset = 98 * index;
        var actualOffset = offset - View.ViewportHeight;
        if (actualOffset > 0)
        {
            View.ScrollTo(0, actualOffset);
        }
    }
}

/// <summary>
/// 视频播放列表基类.
/// </summary>
public abstract class VideoPlaylistSectionBase : LayoutUserControlBase<VideoPlayerPlaylistSectionDetailViewModel>
{
}
