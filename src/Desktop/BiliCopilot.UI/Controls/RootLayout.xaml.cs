// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.ViewModels.Core;
using Richasy.WinUI.Share.Base;
using Richasy.WinUI.Share.ViewModels;

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
        InitializeSubtitle();
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
    public void PrepareFullPlayerPresenter()
    {
        if (!ViewModel.IsOverlayOpen)
        {
            return;
        }

        MainTitleBar.Visibility = Visibility.Collapsed;
        NavView.IsPaneOpen = false;
        VisualStateManager.GoToState(this, nameof(PlayerState), false);

        if (OverlayFrame.Content is VideoPlayerPage vPage)
        {
            vPage.EnterPlayerHostMode();
        }
        else if (OverlayFrame.Content is PgcPlayerPage pPage)
        {
            pPage.EnterPlayerHostMode();
        }
        else if (OverlayFrame.Content is LivePlayerPage lPage)
        {
            lPage.EnterPlayerHostMode();
        }
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
        NavView.IsPaneOpen = true;
        VisualStateManager.GoToState(this, nameof(NormalState), false);

        if (OverlayFrame.Content is VideoPlayerPage vPage)
        {
            vPage.ExitPlayerHostMode();
        }
        else if (OverlayFrame.Content is PgcPlayerPage pPage)
        {
            pPage.ExitPlayerHostMode();
        }
        else if (OverlayFrame.Content is LivePlayerPage lPage)
        {
            lPage.ExitPlayerHostMode();
        }
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

        if (OverlayFrame.Content is VideoPlayerPage vPage)
        {
            vPage.ViewModel.Player.TogglePlayPauseCommand.Execute(default);
            return true;
        }
        else if (OverlayFrame.Content is PgcPlayerPage pPage)
        {
            pPage.ViewModel.Player.TogglePlayPauseCommand.Execute(default);
            return true;
        }
        else if (OverlayFrame.Content is LivePlayerPage lPage)
        {
            lPage.ViewModel.Player.TogglePlayPauseCommand.Execute(default);
            return true;
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

    private void InitializeSubtitle()
    {
#if LOCAL_DEV
        var subtitles = new List<string>();
        subtitles.Add("🛠️");
#if DEBUG
        subtitles.Add("Debug");
#else
        subtitles.Add("Release");
#endif
#if ARCH_X64
        subtitles.Add("x64");
#elif ARCH_X86
        subtitles.Add("x86");
#elif ARCH_ARM64
        subtitles.Add("ARM64");
#endif
        if (subtitles.Count > 0)
        {
            MainTitleBar.Subtitle = string.Join(" | ", subtitles);
        }
#else
        _ = this;
#endif
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
