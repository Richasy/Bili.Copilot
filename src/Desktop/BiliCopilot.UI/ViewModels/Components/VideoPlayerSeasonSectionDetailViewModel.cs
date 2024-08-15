// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUI.Share.ViewModels;

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
    private IReadOnlyCollection<VideoSeason> _seasons;

    [ObservableProperty]
    private IReadOnlyCollection<VideoItemViewModel> _items;

    [ObservableProperty]
    private VideoSeason _selectedSeason;

    [ObservableProperty]
    private VideoItemViewModel _selectedItem;

    /// <summary>
    /// Initializes a new instance of the <see cref="VideoPlayerSeasonSectionDetailViewModel"/> class.
    /// </summary>
    public VideoPlayerSeasonSectionDetailViewModel(
        IList<VideoSeason> seasons,
        string videoId)
    {
        Seasons = seasons.ToList();
        _videoId = videoId;
        var selectedSeason = Seasons.FirstOrDefault(p => p.Videos.Any(q => q.Identifier.Id == _videoId));
        ChangeSeason(selectedSeason);
    }

    /// <inheritdoc/>
    public string Title => ResourceToolkit.GetLocalizedString(StringNames.UgcSeason);

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
