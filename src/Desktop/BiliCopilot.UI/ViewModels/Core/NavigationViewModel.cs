// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using BiliCopilot.UI.Forms;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Pages;
using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Components;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Navigation;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 导航视图模型.
/// </summary>
public sealed partial class NavigationViewModel : ViewModelBase, INavServiceViewModel
{
    private Frame? _navFrame;
    private Frame? _overFrame;

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
            _overFrame.Navigate(typeof(Page));
            _overFrame.BackStack.Clear();

            if (pageType.FullName == lastSelectedPage)
            {
                return;
            }
        }

        if (lastSelectedPage == pageType.FullName && _navFrame.Content is not null && _navFrame.Content.GetType().FullName == lastSelectedPage)
        {
            return;
        }

        SettingsToolkit.WriteLocalSetting(SettingNames.LastSelectedFeaturePage, pageType.FullName);
        _navFrame.Navigate(pageType, parameter, new Microsoft.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());
    }

    /// <inheritdoc/>
    public void NavigateToOver(Type pageType, object? parameter = null)
    {
        if (_overFrame is null)
        {
            throw new InvalidOperationException("导航框架未初始化.");
        }

        if (_overFrame.BackStack.Count > 0)
        {
            _overFrame.BackStack.Clear();
        }

        ActiveMainWindow();
        if (pageType == typeof(VideoPlayerPage) && _overFrame.Content is VideoPlayerPage page)
        {
            page.ViewModel.InitializePageCommand.Execute(parameter);
            IsOverlayOpen = true;
            return;
        }
        else if (pageType == typeof(LivePlayerPage) && _overFrame.Content is LivePlayerPage livePage)
        {
            livePage.ViewModel.InitializePageCommand.Execute(parameter);
            IsOverlayOpen = true;
            return;
        }
        else if (pageType == typeof(PgcPlayerPage) && _overFrame.Content is PgcPlayerPage pgcPage)
        {
            pgcPage.ViewModel.InitializePageCommand.Execute(parameter);
            IsOverlayOpen = true;
            return;
        }
        else if (pageType == typeof(WebDavPlayerPage) && _overFrame.Content is WebDavPlayerPage webDavPage)
        {
            webDavPage.ViewModel.InitializeCommand.Execute(parameter);
            IsOverlayOpen = true;
            return;
        }

        _overFrame.Navigate(pageType, parameter, new Microsoft.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());
        IsOverlayOpen = true;
        CheckSubPageStatus();
    }

    /// <summary>
    /// 执行搜索.
    /// </summary>
    public void Search(string keyword)
    {
        if (_overFrame is not null && _overFrame.Content is SearchPage page)
        {
            page.ViewModel.SearchCommand.Execute(keyword);
            return;
        }

        NavigateToOver(typeof(SearchPage), keyword);
    }

    /// <summary>
    /// 在指定区域执行搜索.
    /// </summary>
    public void SearchInRegion(string keyword)
    {
        if (_overFrame.Content is UserSpacePage usp)
        {
            usp.ViewModel.SearchCommand.Execute(keyword);
        }
        else if (_overFrame.Content is HistoryPage hp)
        {
            hp.ViewModel.SearchCommand.Execute(keyword);
        }
    }

    /// <summary>
    /// 尝试返回.
    /// </summary>
    public void Back()
    {
        for (var i = _overFrame.BackStack.Count - 1; i >= 0; i--)
        {
            if (_overFrame.BackStack[i].SourcePageType.FullName == typeof(Page).FullName)
            {
                _overFrame.BackStack.RemoveAt(i);
            }
        }

        if (_overFrame.CanGoBack)
        {
            _overFrame.GoBack();
        }
        else
        {
            _overFrame.Navigate(typeof(Page));
            _overFrame.BackStack.Clear();
            _overFrame.Content = default;
            IsOverlayOpen = false;
            _navFrame.Focus(FocusState.Programmatic);
        }

        CheckSubPageStatus();
    }

    /// <summary>
    /// 初始化导航视图模型.
    /// </summary>
    public void Initialize(Frame navFrame, Frame overFrame)
    {
        if (_navFrame is not null && _overFrame is not null)
        {
            return;
        }

        _navFrame = navFrame;
        _overFrame = overFrame;
        _overFrame.Navigated += OnOverlayNavigated;
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
            item.IsSelected = item.PageKey == lastSelectedPage;
        }

        if (!list.Any(p => p.IsSelected))
        {
            list[0].IsSelected = true;
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
        => new AppNavigationItemViewModel(this, typeof(TPage), ResourceToolkit.GetLocalizedString(title), symbol, isSelected);

    private void ActiveMainWindow()
        => this.Get<AppViewModel>().Windows.Find(p => p is MainWindow)?.Activate();

    private void OnOverlayNavigated(object sender, NavigationEventArgs e)
    {
        var searchBox = this.Get<SearchBoxViewModel>();
        if (e.SourcePageType == typeof(UserSpacePage))
        {
            searchBox.SetExtraRegion("space", ResourceToolkit.GetLocalizedString(StringNames.UserSpace));
        }
        else if (e.SourcePageType == typeof(HistoryPage))
        {
            searchBox.SetExtraRegion("history", ResourceToolkit.GetLocalizedString(StringNames.ViewHistory));
        }
        else
        {
            searchBox.SetExtraRegion(string.Empty, string.Empty);
            searchBox.Keyword = string.Empty;
        }
    }

    private void CheckSubPageStatus()
    {
        var overPageType = _overFrame.Content?.GetType();
        IsFavoritesPage = overPageType == typeof(FavoritesPage);
        IsHistoryPage = overPageType == typeof(HistoryPage);
        IsViewLaterPage = overPageType == typeof(ViewLaterPage);
    }
}
