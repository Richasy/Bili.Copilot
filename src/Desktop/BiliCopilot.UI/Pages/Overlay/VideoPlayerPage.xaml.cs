// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.View;
using Microsoft.UI.Xaml.Navigation;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Pages.Overlay;

/// <summary>
/// 视频播放界面.
/// </summary>
public sealed partial class VideoPlayerPage : VideoPlayerPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoPlayerPage"/> class.
    /// </summary>
    public VideoPlayerPage() => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is VideoInformation video)
        {
            ViewModel.InitializePageCommand.Execute(video.Identifier);
        }
    }

    /// <inheritdoc/>
    protected override void OnNavigatedFrom(NavigationEventArgs e)
        => ViewModel.CleanCommand.Execute(default);
}

/// <summary>
/// 视频播放界面基类.
/// </summary>
public abstract class VideoPlayerPageBase : LayoutPageBase<VideoPlayerPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoPlayerPageBase"/> class.
    /// </summary>
    protected VideoPlayerPageBase() => ViewModel = this.Get<VideoPlayerPageViewModel>();
}
