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
/// 视频播放器合集视图模型.
/// </summary>
public sealed partial class VideoPlayerSeasonSectionDetailViewModel : ViewModelBase, IPlayerSectionDetailViewModel
{
    /// <summary>
    /// 当前播放页面正在播放的视频Id.
    /// </summary>
    private readonly string _videoId;

    [ObservableProperty]
    private List<VideoSeason> _seasons;

    [ObservableProperty]
    private List<VideoItemViewModel> _items;

    [ObservableProperty]
    private VideoSeason _selectedSeason;

    [ObservableProperty]
    private VideoItemViewModel _selectedItem;

    public VideoPlayerSeasonSectionDetailViewModel(
        VideoSourceViewModel source,
        IList<VideoSeason> seasons,
        string videoId)
    {
        Source = source;
        Seasons = [.. seasons];
        _videoId = videoId;
        var selectedSeason = Seasons.FirstOrDefault(p => p.Videos.Any(q => q.Identifier.Id == _videoId));
        ChangeSeason(selectedSeason);
    }

    /// <inheritdoc/>
    public string Title => ResourceToolkit.GetLocalizedString(StringNames.UgcSeason);

    /// <summary>
    /// 视频源视图模型.
    /// </summary>
    public VideoSourceViewModel? Source { get; }

    [RelayCommand]
    private static Task TryFirstLoadAsync()
        => Task.CompletedTask;

    [RelayCommand]
    private void ChangeSeason(VideoSeason season)
    {
        if (season is null || season == SelectedSeason)
        {
            return;
        }

        SelectedSeason = season;
        Items = season.Videos.Select(p => new VideoItemViewModel(p, Models.Constants.VideoCardStyle.PlayerSeason)).ToList();
        SelectedItem = Items.FirstOrDefault(p => p.Data.Identifier.Id == _videoId);
    }
}
