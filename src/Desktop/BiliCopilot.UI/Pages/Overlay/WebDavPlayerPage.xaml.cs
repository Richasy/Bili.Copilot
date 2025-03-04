// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Controls.Core;
using BiliCopilot.UI.Controls.Player;
using BiliCopilot.UI.ViewModels.View;
using Microsoft.UI.Xaml.Navigation;
using Richasy.WinUIKernel.Share.Base;
using WebDav;

namespace BiliCopilot.UI.Pages.Overlay;

/// <summary>
/// WebDAV 播放器页面.
/// </summary>
public sealed partial class WebDavPlayerPage : WebDavPlayerPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebDavPlayerPage"/> class.
    /// </summary>
    public WebDavPlayerPage()
    {
        InitializeComponent();
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

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (Frame.Tag is string tag && tag == "PlayerWindow")
        {
            ViewModel.IsSeparatorWindowPlayer = true;
        }

        if (e.Parameter is WebDavResource video)
        {
            ViewModel.InitializeCommand.Execute(video);
        }
        else if (e.Parameter is (IList<WebDavResource> list, WebDavResource v))
        {
            ViewModel.InjectPlaylist(list);
            ViewModel.InitializeCommand.Execute(v);
        }
    }

    /// <inheritdoc/>
    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        => ViewModel.CleanCommand.Execute(default);

    private FrameworkElement CreateTransportControl()
    {
        var nextBtn = new WebDavNextButton { ViewModel = ViewModel };
        return new VideoTransportControl
        {
            MaxWidth = 800,
            Margin = new Thickness(12),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Bottom,
            ViewModel = ViewModel.Player,
            LeftContent = nextBtn,
        };
    }
}

/// <summary>
/// WebDAV 播放器页面基类.
/// </summary>
public abstract class WebDavPlayerPageBase : LayoutPageBase<WebDavPlayerPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebDavPlayerPageBase"/> class.
    /// </summary>
    protected WebDavPlayerPageBase() => ViewModel = this.Get<WebDavPlayerPageViewModel>();
}
