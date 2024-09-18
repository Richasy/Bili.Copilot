// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models;
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

    /// <summary>
    /// 进入播放器主持模式.
    /// </summary>
    public void EnterPlayerHostMode()
        => VisualStateManager.GoToState(this, "PlayerHostState", false);

    /// <summary>
    /// 退出播放器主持模式.
    /// </summary>
    public void ExitPlayerHostMode()
        => VisualStateManager.GoToState(this, "DefaultState", false);

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (Frame.Tag is string tag && tag == "PlayerWindow")
        {
            ViewModel.IsSeparatorWindowPlayer = true;
        }

        if (e.Parameter is VideoSnapshot video)
        {
            ViewModel.InitializePageCommand.Execute(video);
        }
        else if (e.Parameter is (IList<VideoInformation> list, VideoSnapshot v))
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
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, CheckPlayerSize);
    }

    private void OnPlayContainerSizeChanged(object sender, SizeChangedEventArgs e)
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, CheckPlayerSize);
    }

    private void CheckPlayerSize()
    {
        ViewModel.PlayerWidth = ViewModel.Player.IsFullScreen ? ActualWidth : PlayerContainer.ActualWidth;

        if (ViewModel.Player.IsFullScreen || ViewModel.Player.IsFullWindow || ViewModel.Player.IsCompactOverlay)
        {
            PlayerContainer.MaxHeight = ActualHeight;
            ViewModel.PlayerHeight = ActualHeight;
        }
        else
        {
            PlayerContainer.MaxHeight = VerticalHolderContainer.ActualHeight;
        }
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
