// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Controls.Core;
using BiliCopilot.UI.Controls.Danmaku;
using BiliCopilot.UI.ViewModels.View;
using Microsoft.UI.Xaml.Navigation;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUIKernel.Share.Base;

namespace BiliCopilot.UI.Pages.Overlay;

/// <summary>
/// 直播播放页面.
/// </summary>
public sealed partial class LivePlayerPage : LivePlayerPageBase, IParameterPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LivePlayerPage"/> class.
    /// </summary>
    public LivePlayerPage()
    {
        InitializeComponent();
        BiliPlayer.InjectDanmakuControlFunc(CreateDanmakuControl);
        BiliPlayer.InjectTransportControlFunc(CreateTransportControl);
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
        if (parameter is MediaIdentifier live)
        {
            ViewModel.InitializePageCommand.Execute(live);
        }
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

    private DanmakuControlBase CreateDanmakuControl()
        => new LiveDanmakuPanel { ViewModel = ViewModel.Danmaku };

    private FrameworkElement CreateTransportControl()
        => new LiveTransportControl { MaxWidth = 640, Margin = new Thickness(12), HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Bottom, ViewModel = ViewModel };
}

/// <summary>
/// 直播播放页面基类.
/// </summary>
public abstract class LivePlayerPageBase : LayoutPageBase<LivePlayerPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LivePlayerPageBase"/> class.
    /// </summary>
    protected LivePlayerPageBase() => ViewModel = this.Get<LivePlayerPageViewModel>();
}
