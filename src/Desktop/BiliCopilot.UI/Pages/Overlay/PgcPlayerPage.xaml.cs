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

    /// <inheritdoc/>
    protected override void OnPageLoaded()
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, CheckPlayerSize);
    }

    private void OnPlayContainerSizeChanged(object sender, SizeChangedEventArgs e)
    {
        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, CheckPlayerSize);
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
/// PGC 播放器页面基类.
/// </summary>
public abstract class PgcPlayerPageBase : LayoutPageBase<PgcPlayerPageViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PgcPlayerPageBase"/> class.
    /// </summary>
    protected PgcPlayerPageBase() => ViewModel = this.Get<PgcPlayerPageViewModel>();
}
