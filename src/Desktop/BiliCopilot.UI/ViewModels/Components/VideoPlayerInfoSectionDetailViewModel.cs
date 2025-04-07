// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.Items;
using BiliCopilot.UI.ViewModels.View;
using CommunityToolkit.Mvvm.Input;
using Richasy.WinUIKernel.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 视频播放页信息区块详情视图模型.
/// </summary>
public sealed partial class VideoPlayerInfoSectionDetailViewModel : ViewModelBase, IPlayerSectionDetailViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoPlayerInfoSectionDetailViewModel"/> class.
    /// </summary>
    public VideoPlayerInfoSectionDetailViewModel(VideoPlayerPageViewModel pageVM)
        => Page = pageVM;

    public VideoPlayerInfoSectionDetailViewModel(VideoSourceViewModel source)
        => Source = source;

    /// <summary>
    /// 视频播放页视图模型.
    /// </summary>
    public VideoPlayerPageViewModel Page { get; }

    /// <summary>
    /// 视频源视图模型.
    /// </summary>
    public VideoSourceViewModel? Source { get; }

    /// <inheritdoc/>
    public string Title { get; } = ResourceToolkit.GetLocalizedString(StringNames.Information);

    [RelayCommand]
    private static Task TryFirstLoadAsync()
        => Task.CompletedTask;
}
