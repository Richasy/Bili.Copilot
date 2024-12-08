// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Controls.Core;
using BiliCopilot.UI.Controls.Danmaku;
using BiliCopilot.UI.Controls.Player;
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
    public VideoPlayerPage()
    {
        InitializeComponent();
        BiliPlayer.InjectDanmakuControlFunc(CreateDanmakuPanel);
        BiliPlayer.InjectTransportControlFunc(CreateTransportControl);
        BiliPlayer.InjectSubtitleControlFunc(CreateSubtitleControl);
    }

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

    /// <summary>
    /// 标记右箭头按下.
    /// </summary>
    /// <returns>是否成功.</returns>
    public bool MarkRightArrowPressed()
    {
        if (ViewModel.Player.IsPaused)
        {
            return false;
        }

        BiliPlayer.MarkRightArrowKeyPressedTime();
        return true;
    }

    /// <summary>
    /// 取消右箭头按下.
    /// </summary>
    /// <returns>是否成功.</returns>
    public bool CancelRightArrow()
    {
        if (ViewModel.Player.IsPaused)
        {
            return false;
        }

        BiliPlayer.CancelRightArrowKey();
        return true;
    }

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
    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        => ViewModel.CleanCommand.Execute(default);

    private DanmakuControlBase CreateDanmakuPanel()
        => new VideoDanmakuPanel() { ViewModel = ViewModel.Danmaku };

    private FrameworkElement CreateTransportControl()
    {
        var leftPanel = new VideoPlayerTransportLeftPanel() { ViewModel = ViewModel };
        var danmakuBox = new DanmakuBox() { ViewModel = ViewModel.Danmaku };
        var rightPanel = new StackPanel() { Orientation = Orientation.Horizontal };
        var formatButton = new VideoPlayerFormatButton() { ViewModel = ViewModel };
        var subtitleButton = new SubtitleButton() { ViewModel = ViewModel.Subtitle };
        rightPanel.Children.Add(formatButton);
        rightPanel.Children.Add(subtitleButton);
        danmakuBox.InputGotFocus += OnDanmakuInputGotFocus;
        danmakuBox.InputLostFocus += OnDanmakuInputLostFocus;
        return new VideoTransportControl()
        {
            LeftContent = leftPanel,
            RightContent = rightPanel,
            MiddleContent = danmakuBox,
            ViewModel = ViewModel.Player,
            VerticalAlignment = VerticalAlignment.Bottom,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Margin = new Thickness(12),
            MaxWidth = 800,
        };
    }

    private void OnDanmakuInputGotFocus(object? sender, EventArgs e)
        => ViewModel.Player.IsDanmakuInputFocused = true;

    private void OnDanmakuInputLostFocus(object? sender, EventArgs e)
        => ViewModel.Player.IsDanmakuInputFocused = false;

    private SubtitlePresenter CreateSubtitleControl()
    {
        return new SubtitlePresenter
        {
            Margin = new Thickness(0, 0, 0, 16),
            ViewModel = ViewModel.Subtitle,
        };
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
