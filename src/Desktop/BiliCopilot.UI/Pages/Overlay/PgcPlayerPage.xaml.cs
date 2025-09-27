// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Controls.Core;
using BiliCopilot.UI.Controls.Danmaku;
using BiliCopilot.UI.Controls.Player;
using BiliCopilot.UI.ViewModels.View;
using Microsoft.UI.Xaml.Navigation;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Pages.Overlay;

/// <summary>
/// PGC 播放器页面.
/// </summary>
public sealed partial class PgcPlayerPage : PgcPlayerPageBase, IParameterPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PgcPlayerPage"/> class.
    /// </summary>
    public PgcPlayerPage()
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

    public void SetParameter(object? parameter)
    {
        if (parameter is MediaIdentifier id)
        {
            ViewModel.InitializePageCommand.Execute(id);
        }
    }

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
    }

    /// <inheritdoc/>
    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        => ViewModel.CleanCommand.Execute(default);

    private DanmakuControlBase CreateDanmakuPanel()
        => new VideoDanmakuPanel();

    private FrameworkElement CreateTransportControl()
    {
        var leftPanel = new PgcPlayerTransportLeftPanel() { ViewModel = ViewModel };
        //var danmakuBox = new DanmakuBox() { ViewModel = ViewModel.Danmaku };
        var rightPanel = new StackPanel() { Orientation = Orientation.Horizontal };
        var formatButton = new PgcPlayerFormatButton() { ViewModel = ViewModel };
        var subtitleButton = new SubtitleButton() { ViewModel = ViewModel.Subtitle };
        rightPanel.Children.Add(formatButton);
        rightPanel.Children.Add(subtitleButton);
        //danmakuBox.InputGotFocus += OnDanmakuInputGotFocus;
        //danmakuBox.InputLostFocus += OnDanmakuInputLostFocus;
        return new VideoTransportControl()
        {
            LeftContent = leftPanel,
            RightContent = rightPanel,
            //MiddleContent = danmakuBox,
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
/// PGC 播放器页面基类.
/// </summary>
public abstract class PgcPlayerPageBase : LayoutPageBase<PgcPlayerPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PgcPlayerPageBase"/> class.
    /// </summary>
    protected PgcPlayerPageBase() => ViewModel = this.Get<PgcPlayerPageViewModel>();
}
