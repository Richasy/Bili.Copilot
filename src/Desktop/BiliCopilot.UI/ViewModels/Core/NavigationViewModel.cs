// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Controls;
using BiliCopilot.UI.Forms;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Pages;
using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Richasy.WinUIKernel.Share.Toolkits;
using Richasy.WinUIKernel.Share.ViewModels;
using System.Collections.ObjectModel;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 导航视图模型.
/// </summary>
public sealed partial class NavigationViewModel : ViewModelBase, INavServiceViewModel
{
    private NavigationView _navigationView;
    private MainFrame? _navFrame;
    private OverlayFrame? _overFrame;

    [ObservableProperty]
    private bool _isOverlayOpen;

    /// <summary>
    /// 导航条目列表.
    /// </summary>
    [ObservableProperty]
    private List<AppNavigationItemViewModel> _menuItems;

    [ObservableProperty]
    private bool _isTopNavBarShown;

    [ObservableProperty]
    private bool _isFavoritesPage;

    [ObservableProperty]
    private bool _isHistoryPage;

    [ObservableProperty]
    private bool _isViewLaterPage;

    [ObservableProperty]
    public partial bool IsTemporaryOverlayClosed { get; set; }

    /// <summary>
    /// 底部条目列表.
    /// </summary>
    public ObservableCollection<AppNavigationItemViewModel> FooterItems { get; } = new();

    /// <inheritdoc/>
    public void NavigateTo(Type pageType, object? parameter = null)
    {
        if (_navFrame is null)
        {
            throw new InvalidOperationException("导航框架未初始化.");
        }

        ActiveMainWindow();
        var lastSelectedPage = SettingsToolkit.ReadLocalSetting(SettingNames.LastSelectedFeaturePage, string.Empty);
        if (IsOverlayOpen)
        {
            IsOverlayOpen = false;
            IsTemporaryOverlayClosed = true;
            CheckSubPageStatus();
        }

        if (lastSelectedPage == pageType.FullName && _navFrame.GetCurrentContent() is not null && _navFrame.GetCurrentContent().GetType().FullName == lastSelectedPage)
        {
            if (_navFrame.GetCurrentContent() is ICancelPageViewModel cancelPage)
            {
                cancelPage.CancelLoading();
            }

            return;
        }

        SettingsToolkit.WriteLocalSetting(SettingNames.LastSelectedFeaturePage, pageType.FullName);
        _navFrame.NavigateTo(pageType, parameter);

        var item = MenuItems.Find(p => p.PageKey == pageType.FullName);
        if (item is not null && _navigationView.SelectedItem != item)
        {
            _navigationView.SelectedItem = item;
        }
    }

    public void TryClearOverlayPages()
    {
        if (_overFrame?.GetCurrentContent() is ICancelPageViewModel cancelPage)
        {
            cancelPage.CancelLoading();
        }

        IsOverlayOpen = false;
        IsTemporaryOverlayClosed = false;
        _overFrame?.ClearBackStack();
    }

    /// <inheritdoc/>
    public void NavigateToOver(Type pageType, object? parameter = null)
    {
        if (_overFrame is null)
        {
            throw new InvalidOperationException("导航框架未初始化.");
        }

        if (IsTemporaryOverlayClosed)
        {
            TryClearOverlayPages();
            IsTemporaryOverlayClosed = false;
        }

        ActiveMainWindow();
        if (pageType == typeof(VideoPlayerPage) && _overFrame.GetCurrentContent() is VideoPlayerPage page)
        {
            page.ViewModel.InitializePageCommand.Execute(parameter);
            IsOverlayOpen = true;
            return;
        }
        else if (pageType == typeof(LivePlayerPage) && _overFrame.GetCurrentContent() is LivePlayerPage livePage)
        {
            livePage.ViewModel.InitializePageCommand.Execute(parameter);
            IsOverlayOpen = true;
            return;
        }
        else if (pageType == typeof(PgcPlayerPage) && _overFrame.GetCurrentContent() is PgcPlayerPage pgcPage)
        {
            pgcPage.ViewModel.InitializePageCommand.Execute(parameter);
            IsOverlayOpen = true;
            return;
        }
        else if (pageType == typeof(WebDavPlayerPage) && _overFrame.GetCurrentContent() is WebDavPlayerPage webDavPage)
        {
            webDavPage.ViewModel.InitializeCommand.Execute(parameter);
            IsOverlayOpen = true;
            return;
        }

        _overFrame.NavigateTo(pageType, parameter);
        IsOverlayOpen = true;
        CheckSubPageStatus();

        var searchBox = this.Get<SearchBoxViewModel>();
        if (pageType == typeof(UserSpacePage))
        {
            searchBox.SetExtraRegion("space", ResourceToolkit.GetLocalizedString(StringNames.UserSpace));
        }
        else if (pageType == typeof(HistoryPage))
        {
            searchBox.SetExtraRegion("history", ResourceToolkit.GetLocalizedString(StringNames.ViewHistory));
        }
        else
        {
            searchBox.SetExtraRegion(string.Empty, string.Empty);
            searchBox.Keyword = string.Empty;
        }
    }

    /// <summary>
    /// 执行搜索.
    /// </summary>
    public void Search(string keyword)
    {
        if (_overFrame is not null && _overFrame.GetCurrentContent() is SearchPage page)
        {
            page.ViewModel.SearchCommand.Execute(keyword);
            return;
        }

        NavigateToOver(typeof(SearchPage), keyword);
    }

    /// <summary>
    /// 设置导航条目可见性.
    /// </summary>
    public void SetNavItemVisibility(Type pageType, bool isVisible)
    {
        var item = MenuItems.Find(p => p.PageKey == pageType.FullName);
        item?.IsVisible = isVisible;
    }

    /// <summary>
    /// 在指定区域执行搜索.
    /// </summary>
    public void SearchInRegion(string keyword)
    {
        if (_overFrame.GetCurrentContent() is UserSpacePage usp)
        {
            usp.ViewModel.SearchCommand.Execute(keyword);
        }
        else if (_overFrame.GetCurrentContent() is HistoryPage hp)
        {
            hp.ViewModel.SearchCommand.Execute(keyword);
        }
    }

    /// <summary>
    /// 尝试返回.
    /// </summary>
    public void Back()
    {
        if (_overFrame is not null)
        {
            if (_overFrame.GetCurrentContent() is ICancelPageViewModel cancelPage)
            {
                cancelPage.CancelLoading();
            }

            _overFrame.GoBack();
            if (_overFrame.GetBackStack().Count == 0)
            {
                IsOverlayOpen = false;
                _navFrame?.Focus(FocusState.Programmatic);
            }
        }

        CheckSubPageStatus();
    }

    /// <summary>
    /// 初始化导航视图模型.
    /// </summary>
    public void Initialize(NavigationView view, MainFrame navFrame, OverlayFrame overFrame)
    {
        if (_navFrame is not null && _overFrame is not null)
        {
            return;
        }

        _navFrame = navFrame;
        _overFrame = overFrame;
        _navigationView = view;
        IsTopNavBarShown = SettingsToolkit.ReadLocalSetting(SettingNames.IsTopNavBarShown, true);

        MenuItems = [.. GetMenuItems()];

        foreach (var item in GetFooterItems())
        {
            FooterItems.Add(item);
        }
    }

    [RelayCommand]
    private void CheckWebDavItem()
    {
        var isWebDavEnabled = SettingsToolkit.ReadLocalSetting(SettingNames.IsWebDavEnabled, false);
        var exist = FooterItems.Any(p => p.PageKey == typeof(WebDavPage).FullName);
        if (isWebDavEnabled && !exist)
        {
            FooterItems.Insert(0, GetItem<WebDavPage>(StringNames.WebDav, FluentIcons.Common.Symbol.CloudDatabase));
        }
        else if (!isWebDavEnabled && exist)
        {
            FooterItems.Remove(FooterItems.First(p => p.PageKey == typeof(WebDavPage).FullName));
        }
    }

    private IReadOnlyList<AppNavigationItemViewModel> GetMenuItems()
    {
        var lastSelectedPage = SettingsToolkit.ReadLocalSetting(SettingNames.LastSelectedFeaturePage, typeof(PopularPage).FullName);
        var list = new List<AppNavigationItemViewModel>
        {
            GetItem<PopularPage>(StringNames.PopularSlim, FluentIcons.Common.Symbol.Rocket),
            GetItem<MomentPage>(StringNames.DynamicFeed, FluentIcons.Common.Symbol.DesignIdeas),
            GetItem<VideoPartitionPage>(StringNames.Video, FluentIcons.Common.Symbol.VideoClip),
            GetItem<LivePartitionPage>(StringNames.Live, FluentIcons.Common.Symbol.VideoChat),
            GetItem<AnimePage>(StringNames.Anime, FluentIcons.Common.Symbol.Dust),
            GetItem<CinemaPage>(StringNames.Cinema, FluentIcons.Common.Symbol.FilmstripPlay),
            GetItem<ArticlePartitionPage>(StringNames.Article, FluentIcons.Common.Symbol.DocumentBulletList),
        };

        foreach (var item in list)
        {
            item.IsSelected = item.PageKey == lastSelectedPage && item.IsVisible;
        }

        if (!list.Any(p => p.IsSelected) && list.Any(p => p.IsVisible))
        {
            list.First(p => p.IsVisible).IsSelected = true;
        }

        return list;
    }

    private IReadOnlyList<AppNavigationItemViewModel> GetFooterItems()
    {
        var list = new List<AppNavigationItemViewModel>
        {
            // GetItem<DownloadPage>(StringNames.Download, FluentIcons.Common.Symbol.CloudArrowDown),
            GetItem<MessagePage>(StringNames.Message, FluentIcons.Common.Symbol.Chat),
            GetItem<SettingsPage>(StringNames.Settings, FluentIcons.Common.Symbol.Settings),
        };

        var isWebDavEnabled = SettingsToolkit.ReadLocalSetting(SettingNames.IsWebDavEnabled, false);
        if (isWebDavEnabled)
        {
            list.Insert(0, GetItem<WebDavPage>(StringNames.WebDav, FluentIcons.Common.Symbol.CloudDatabase));
        }

        return list;
    }

    private AppNavigationItemViewModel GetItem<TPage>(StringNames title, FluentIcons.Common.Symbol symbol, bool isSelected = false)
        where TPage : Page
    {
        var isVisible = this.Get<ISettingsToolkit>().ReadLocalSetting($"Is{typeof(TPage).Name}Visible", true);
        return new AppNavigationItemViewModel(this, typeof(TPage), ResourceToolkit.GetLocalizedString(title), symbol, isSelected, isVisible: isVisible);
    }

    private void ActiveMainWindow()
        => this.Get<AppViewModel>().Windows.Find(p => p is MainWindow)?.Activate();

    private void CheckSubPageStatus()
    {
        var overPageType = _overFrame.GetCurrentContent()?.GetType();
        IsFavoritesPage = IsOverlayOpen && overPageType == typeof(FavoritesPage);
        IsHistoryPage = IsOverlayOpen && overPageType == typeof(HistoryPage);
        IsViewLaterPage = IsOverlayOpen && overPageType == typeof(ViewLaterPage);
    }
}
