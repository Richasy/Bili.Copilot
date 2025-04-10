// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.ViewModels.Core;
using Richasy.WinUIKernel.Share.Base;
using Richasy.WinUIKernel.Share.ViewModels;

namespace BiliCopilot.UI.Controls;

/// <summary>
/// 根布局，用于包裹整个应用程序的布局.
/// </summary>
public sealed partial class RootLayout : RootLayoutBase
{
    private readonly AppViewModel _appViewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="RootLayout"/> class.
    /// </summary>
    public RootLayout()
    {
        InitializeComponent();
        _appViewModel = this.Get<AppViewModel>();
    }

    /// <inheritdoc/>
    protected override ControlBindings ControlBindings => Bindings is null ? null : new(Bindings.Initialize, Bindings.StopTracking);

    /// <summary>
    /// 获取主标题栏.
    /// </summary>
    /// <returns><see cref="AppTitleBar"/>.</returns>
    public AppTitleBar GetMainTitleBar() => MainTitleBar;

    /// <summary>
    /// 准备隐藏除播放器外的其它控件.
    /// </summary>
    public void PrepareFullPlayerPresenter(PlayerDisplayMode mode)
    {
        if (!ViewModel.IsOverlayOpen)
        {
            return;
        }

        MainTitleBar.Visibility = Visibility.Collapsed;
        this.Get<AppViewModel>().ActivatedWindow.AppWindow.TitleBar.PreferredHeightOption = Microsoft.UI.Windowing.TitleBarHeightOption.Standard;
        NavView.IsPaneOpen = false;
        VisualStateManager.GoToState(NavView, "PaneCollapsed", false);
        VisualStateManager.GoToState(this, nameof(PlayerState), false);

        if (OverlayFrame.Content is WebDavPlayerPage wPage)
        {
            SecondaryTitleBar.Title = wPage.ViewModel.Title;
            wPage.EnterPlayerHostMode();
        }

        MainTitleBar.DelayUpdateAsync(100);
        SecondaryTitleBar.DelayUpdateAsync(500);
    }

    /// <summary>
    /// 播放器解除占用状态，显示其它控件.
    /// </summary>
    public void ExitFullPlayerPresenter()
    {
        if (!ViewModel.IsOverlayOpen)
        {
            return;
        }

        MainTitleBar.Visibility = Visibility.Visible;
        SecondaryTitleBar.Visibility = Visibility.Collapsed;
        SecondaryTitleBar.IsBackButtonVisible = false;
        this.Get<AppViewModel>().ActivatedWindow.AppWindow.TitleBar.PreferredHeightOption = Microsoft.UI.Windowing.TitleBarHeightOption.Tall;
        NavView.IsPaneOpen = true;
        VisualStateManager.GoToState(NavView, "PaneVisible", false);
        VisualStateManager.GoToState(this, nameof(NormalState), false);

        if (OverlayFrame.Content is WebDavPlayerPage wPage)
        {
            wPage.ExitPlayerHostMode();
        }

        MainTitleBar.DelayUpdateAsync(500);
        SecondaryTitleBar.DelayUpdateAsync(100);
    }

    /// <summary>
    /// 是否处于独占播放模式.
    /// </summary>
    /// <returns>结果.</returns>
    public bool TryBackToDefaultIfPlayerHostMode()
    {
        if (MainTitleBar.Visibility == Visibility.Visible)
        {
            return false;
        }

        var isHandled = false;
        if (OverlayFrame.Content is WebDavPlayerPage wPage)
        {
            wPage.ViewModel.Player.BackToDefaultModeCommand.Execute(default);
            isHandled = true;
        }

        return isHandled;
    }

    /// <summary>
    /// 尝试在播放器中切换播放暂停状态.
    /// </summary>
    /// <returns>是否处理.</returns>
    public bool TryTogglePlayPauseIfInPlayer()
    {
        if (!ViewModel.IsOverlayOpen)
        {
            return false;
        }

        if (OverlayFrame.Content is WebDavPlayerPage wPage)
        {
            return wPage.ViewModel.Player.TryTogglePlayPause();
        }

        return false;
    }

    /// <summary>
    /// 尝试标记右箭头按下时间.
    /// </summary>
    /// <returns>是否处理.</returns>
    public bool TryMarkRightArrowPressedTime()
    {
        if (!ViewModel.IsOverlayOpen)
        {
            return false;
        }

        return false;
    }

    /// <summary>
    /// 尝试取消右箭头按下.
    /// </summary>
    /// <returns>是否处理.</returns>
    public bool TryCancelRightArrow()
    {
        if (!ViewModel.IsOverlayOpen)
        {
            return false;
        }

        return false;
    }

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.Initialize(MainFrame, OverlayFrame);
        var selectedItem = ViewModel.MenuItems.FirstOrDefault(p => p.IsSelected);
        if (selectedItem is not null)
        {
            NavView.SelectedItem = selectedItem;
            selectedItem.NavigateCommand.Execute(default);
        }
    }

    private void OnNavViewBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        => OnBackRequested(default, default);

    private void OnNavViewItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        _ = this;
        var item = args.InvokedItemContainer as AppNavigationViewItem;
        var context = item?.Tag as AppNavigationItemViewModel;
        context?.NavigateCommand.Execute(default);
    }

    private void OnBackRequested(object sender, EventArgs e)
        => ViewModel.Back();

    private void OnUpdateActionButtonClick(TeachingTip sender, object args)
        => _appViewModel.ShowUpdateCommand.Execute(default);

    private void OnUpdateCloseButtonClick(TeachingTip sender, object args)
        => _appViewModel.HideUpdateCommand.Execute(default);

    private void OnFavoriteButtonClick(object sender, EventArgs e)
        => ViewModel.NavigateToOver(typeof(FavoritesPage), default);

    private void OnViewLaterButtonClick(object sender, EventArgs e)
        => ViewModel.NavigateToOver(typeof(ViewLaterPage), default);

    private void OnHistoryButtonClick(object sender, EventArgs e)
        => ViewModel.NavigateToOver(typeof(HistoryPage), default);

    private void OnVideoBackRequested(object sender, EventArgs e)
        => TryBackToDefaultIfPlayerHostMode();
}

/// <summary>
/// 根布局基类.
/// </summary>
public abstract class RootLayoutBase : LayoutUserControlBase<NavigationViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RootLayoutBase"/> class.
    /// </summary>
    protected RootLayoutBase() => ViewModel = this.Get<NavigationViewModel>();
}
