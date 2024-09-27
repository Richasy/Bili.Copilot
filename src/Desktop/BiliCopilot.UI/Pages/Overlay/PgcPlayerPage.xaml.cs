﻿// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Controls.Core;
using BiliCopilot.UI.Controls.Danmaku;
using BiliCopilot.UI.Controls.Player;
using BiliCopilot.UI.ViewModels.View;
using Microsoft.UI.Xaml.Navigation;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUI.Share.Base;

namespace BiliCopilot.UI.Pages.Overlay;

/// <summary>
/// PGC 播放器页面.
/// </summary>
public sealed partial class PgcPlayerPage : PgcPlayerPageBase
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

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (Frame.Tag is string tag && tag == "PlayerWindow")
        {
            ViewModel.IsSeparatorWindowPlayer = true;
        }

        if (e.Parameter is MediaIdentifier id)
        {
            ViewModel.InitializePageCommand.Execute(id);
        }
    }

    /// <inheritdoc/>
    protected override void OnNavigatedFrom(NavigationEventArgs e)
        => ViewModel.CleanCommand.Execute(default);

    private DanmakuControlBase CreateDanmakuPanel()
        => new VideoDanmakuPanel() { ViewModel = ViewModel.Danmaku };

    private FrameworkElement CreateTransportControl()
    {
        var leftPanel = new PgcPlayerTransportLeftPanel() { ViewModel = ViewModel };
        var danmakuBox = new DanmakuBox() { ViewModel = ViewModel.Danmaku };
        var rightPanel = new StackPanel() { Orientation = Orientation.Horizontal };
        var formatButton = new PgcPlayerFormatButton() { ViewModel = ViewModel };
        var subtitleButton = new SubtitleButton() { ViewModel = ViewModel.Subtitle };
        rightPanel.Children.Add(formatButton);
        rightPanel.Children.Add(subtitleButton);
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
