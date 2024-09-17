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
/// 视频播放器推荐区域详情视图模型.
/// </summary>
public sealed partial class VideoPlayerRecommendSectionDetailViewModel : ViewModelBase, IPlayerSectionDetailViewModel
{
    [ObservableProperty]
    private List<VideoItemViewModel> _items;

    /// <summary>
    /// Initializes a new instance of the <see cref="VideoPlayerRecommendSectionDetailViewModel"/> class.
    /// </summary>
    public VideoPlayerRecommendSectionDetailViewModel(
        IList<VideoInformation> items)
    {
        Items = items.Select(p => new VideoItemViewModel(p, VideoCardStyle.PlayerRecommend)).ToList();
    }

    /// <inheritdoc/>
    public string Title { get; } = ResourceToolkit.GetLocalizedString(StringNames.Recommend);

    [RelayCommand]
    private static Task TryFirstLoadAsync()
        => Task.CompletedTask;
}
