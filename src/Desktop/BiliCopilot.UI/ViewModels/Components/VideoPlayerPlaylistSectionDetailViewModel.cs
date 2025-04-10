// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.Items;
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

    public VideoPlayerPlaylistSectionDetailViewModel(
        VideoSourceViewModel source,
        IList<VideoInformation> list,
        string videoId)
    {
        _videoId = videoId;
        Source = source;
        Items = list.Select(p => new VideoItemViewModel(p, VideoCardStyle.PlayerPlaylist)).ToList();
        SelectedItem = Items.FirstOrDefault(p => p.Data.Identifier.Id == _videoId);
    }

    /// <inheritdoc/>
    public string Title => ResourceToolkit.GetLocalizedString(StringNames.Playlist);

    /// <summary>
    /// 视频源视图模型.
    /// </summary>
    public VideoSourceViewModel? Source { get; }

    [RelayCommand]
    private static Task TryFirstLoadAsync()
        => Task.CompletedTask;
}
