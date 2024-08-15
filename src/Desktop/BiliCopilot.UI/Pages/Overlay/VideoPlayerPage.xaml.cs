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
            ViewModel.InitializePageCommand.Execute(video);
        }
        else if (e.Parameter is(IList<VideoInformation> list, VideoInformation v))
        {
            ViewModel.InjectPlaylist(list);
            ViewModel.InitializePageCommand.Execute(v);
        }
    }

    /// <inheritdoc/>
    protected override void OnNavigatedFrom(NavigationEventArgs e)
        => ViewModel.CleanCommand.Execute(default);

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            ViewModel.PlayerWidth = PlayerContainer.ActualWidth;
        });
    }

    private void OnPlayContainerSizeChanged(object sender, SizeChangedEventArgs e)
    {
        ViewModel.PlayerWidth = e.NewSize.Width;

        // 播放器不能超出容器高度.
        PlayerContainer.MaxHeight = VerticalHolderContainer.ActualHeight;
    }
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
