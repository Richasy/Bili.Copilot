// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models;
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
        VideoConnectorViewModel page,
        IList<VideoInformation> items)
    {
        Page = page;
        Items = [.. items.Select(p => new VideoItemViewModel(p, VideoCardStyle.PlayerRecommend, playAction: Play))];
    }

    /// <inheritdoc/>
    public string Title { get; } = ResourceToolkit.GetLocalizedString(StringNames.Recommend);

    public VideoConnectorViewModel Page { get; init; }

    [RelayCommand]
    private static Task TryFirstLoadAsync()
        => Task.CompletedTask;

    private void Play(VideoItemViewModel vm)
    {
        var snapshot = new MediaSnapshot(vm.Data, Page.IsPrivatePlay);
        Page.Parent.InitializeCommand.Execute(snapshot);
    }
}
