// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Pages;
using BiliCopilot.UI.Toolkits;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 导航视图模型.
/// </summary>
public sealed partial class NavigationViewModel : ViewModelBase, INavServiceViewModel
{
    private Frame? _navFrame;

    /// <summary>
    /// 导航条目列表.
    /// </summary>
    [ObservableProperty]
    private IReadOnlyCollection<AppNavigationItemViewModel> _menuItems;

    /// <summary>
    /// 底部条目列表.
    /// </summary>
    [ObservableProperty]
    private IReadOnlyCollection<AppNavigationItemViewModel> _footerItems;

    /// <inheritdoc/>
    public void NavigateTo(string pageKey, object? parameter = null)
    {
        if (_navFrame is null)
        {
            throw new InvalidOperationException("导航框架未初始化.");
        }

        SettingsToolkit.WriteLocalSetting(SettingNames.LastSelectedFeaturePage, pageKey);
        var pageType = Type.GetType(pageKey)
            ?? throw new InvalidOperationException("无法找到页面.");
        _navFrame.Navigate(pageType, parameter, new Microsoft.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());
    }

    [RelayCommand]
    private void Initialize(Frame navFrame)
    {
        if (_navFrame is not null)
        {
            return;
        }

        _navFrame = navFrame;
        MenuItems = [.. GetMenuItems()];
        FooterItems = [.. GetFooterItems()];
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
        return new List<AppNavigationItemViewModel>
        {
            GetItem<DownloadPage>(StringNames.Download, FluentIcons.Common.Symbol.CloudArrowDown),
            GetItem<MessagePage>(StringNames.Message, FluentIcons.Common.Symbol.Chat),
            GetItem<SettingsPage>(StringNames.Settings, FluentIcons.Common.Symbol.Settings),
        };
    }

    private AppNavigationItemViewModel GetItem<TPage>(StringNames title, FluentIcons.Common.Symbol symbol, bool isSelected = false)
        where TPage : Page
        => new AppNavigationItemViewModel(this, typeof(TPage).FullName, ResourceToolkit.GetLocalizedString(title), symbol, isSelected);
}
