// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    /// <summary>
    /// Initializes a new instance of the <see cref="VideoPlayerPlaylistSectionDetailViewModel"/> class.
    /// </summary>
    public VideoPlayerPlaylistSectionDetailViewModel(
        VideoConnectorViewModel page,
        IList<MediaSnapshot> list,
        string videoId)
    {
        _videoId = videoId;
        Page = page;
        Items = [.. list.Select(p => new VideoItemViewModel(p.Video, VideoCardStyle.PlayerPlaylist, playAction: Play))];
        foreach (var item in Items)
        {
            item.IsSelected = item.Data.Identifier.Id == videoId;
        }
    }

    /// <inheritdoc/>
    public string Title => ResourceToolkit.GetLocalizedString(StringNames.Playlist);

    /// <summary>
    /// 页面视图模型.
    /// </summary>
    public VideoConnectorViewModel Page { get; }

    [RelayCommand]
    private static Task TryFirstLoadAsync()
        => Task.CompletedTask;

    private void Play(VideoItemViewModel vm)
    {
        if (vm.Data.Identifier.Id == _videoId)
        {
            return;
        }

        var snapshot = new MediaSnapshot(vm.Data, Page.IsPrivatePlay)
        {
            Playlist = Items.ConvertAll(p => new MediaSnapshot(p.Data)),
        };

        Page.Parent.InitializeCommand.Execute(snapshot);
    }
}
