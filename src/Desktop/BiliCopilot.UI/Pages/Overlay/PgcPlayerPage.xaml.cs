// Copyright (c) Bili Copilot. All rights reserved.

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
    public PgcPlayerPage() => InitializeComponent();

    /// <summary>
    /// 进入播放器主持模式.
    /// </summary>
    public void EnterPlayerHostMode()
    {
        VisualStateManager.GoToState(this, "PlayerHostState", false);
        ViewModel?.Danmaku?.Redraw();
    }

    /// <summary>
    /// 退出播放器主持模式.
    /// </summary>
    public void ExitPlayerHostMode()
    {
        VisualStateManager.GoToState(this, "DefaultState", false);
        ViewModel?.Danmaku?.Redraw();
    }

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is MediaIdentifier id)
        {
            ViewModel.InitializePageCommand.Execute(id);
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
        ViewModel.PlayerWidth = ViewModel.Player.IsFullScreen ? ActualWidth : e.NewSize.Width;

        if (ViewModel.Player.IsFullScreen)
        {
            ViewModel.PlayerHeight = ActualHeight;
        }

        // 播放器不能超出容器高度.
        PlayerContainer.MaxHeight = ViewModel.Player.IsFullScreen ? ActualHeight : VerticalHolderContainer.ActualHeight;
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
