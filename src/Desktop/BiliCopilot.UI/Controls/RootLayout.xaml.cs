// Copyright (c) Bili Copilot. All rights reserved.

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

    /// <summary>
    /// 获取主标题栏.
    /// </summary>
    /// <returns><see cref="AppTitleBar"/>.</returns>
    public AppTitleBar GetMainTitleBar() => MainTitleBar;

    public void TryFocusSearchBox()
        => SearchBox.Focus(FocusState.Programmatic);

    /// <inheritdoc/>
    protected override void OnControlLoaded()
    {
        ViewModel.Initialize(NavView, MainFrame, OverlayFrame);
        var selectedItem = ViewModel.MenuItems.FirstOrDefault(p => p.IsSelected);
        if (selectedItem is not null)
        {
            NavView.SelectedItem = selectedItem;
            selectedItem.NavigateCommand.Execute(default);
        }
    }

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
