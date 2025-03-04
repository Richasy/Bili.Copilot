// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;
using BiliCopilot.UI.ViewModels.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUIKernel.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 视频播放器播放列表部分.
/// </summary>
public sealed partial class VideoPlayerPlaylistSectionDetailViewModel : ViewModelBase, IPlayerSectionDetailViewModel
{
    private readonly string _videoId;

    [ObservableProperty]
    private List<VideoItemViewModel> _items;

    [ObservableProperty]
    private VideoItemViewModel _selectedItem;

    /// <summary>
    /// Initializes a new instance of the <see cref="VideoPlayerPlaylistSectionDetailViewModel"/> class.
    /// </summary>
    public VideoPlayerPlaylistSectionDetailViewModel(
        VideoPlayerPageViewModel page,
        IList<VideoInformation> list,
        string videoId)
    {
        _videoId = videoId;
        Page = page;
        Items = list.Select(p => new VideoItemViewModel(p, VideoCardStyle.PlayerPlaylist)).ToList();
        SelectedItem = Items.FirstOrDefault(p => p.Data.Identifier.Id == _videoId);
    }

    /// <inheritdoc/>
    public string Title => ResourceToolkit.GetLocalizedString(StringNames.Playlist);

    /// <summary>
    /// 页面视图模型.
    /// </summary>
    public VideoPlayerPageViewModel Page { get; }

    [RelayCommand]
    private static Task TryFirstLoadAsync()
        => Task.CompletedTask;
}
