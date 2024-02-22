// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Bili.Copilot.Models.Constants.Bili;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 搜索页面视图模型.
/// </summary>
public sealed partial class SearchDetailViewModel
{
    private readonly Dictionary<SearchModuleType, bool> _requestStatusCache;
    private readonly Dictionary<SearchModuleType, IEnumerable<SearchFilterViewModel>> _filters;
    private bool _isRequesting;

    [ObservableProperty]
    private SearchModuleItemViewModel _currentModule;

    [ObservableProperty]
    private bool _isCurrentContentEmpty;

    [ObservableProperty]
    private bool _isCurrentFilterEmpty;

    [ObservableProperty]
    private bool _isVideoModuleShown;

    [ObservableProperty]
    private bool _isAnimeModuleShown;

    [ObservableProperty]
    private bool _isMovieModuleShown;

    [ObservableProperty]
    private bool _isArticleModuleShown;

    [ObservableProperty]
    private bool _isLiveModuleShown;

    [ObservableProperty]
    private bool _isUserModuleShown;

    [ObservableProperty]
    private string _keyword;

    [ObservableProperty]
    private bool _isReloadingModule;

    /// <summary>
    /// 视频列表.
    /// </summary>
    public ObservableCollection<VideoItemViewModel> Videos { get; }

    /// <summary>
    /// 动漫列表.
    /// </summary>
    public ObservableCollection<SeasonItemViewModel> Animes { get; }

    /// <summary>
    /// 影视列表.
    /// </summary>
    public ObservableCollection<SeasonItemViewModel> Movies { get; }

    /// <summary>
    /// 文章列表.
    /// </summary>
    public ObservableCollection<UserItemViewModel> Users { get; }

    /// <summary>
    /// 用户列表.
    /// </summary>
    public ObservableCollection<ArticleItemViewModel> Articles { get; }

    /// <summary>
    /// 直播列表.
    /// </summary>
    public ObservableCollection<LiveItemViewModel> Lives { get; }

    /// <summary>
    /// 当前筛选器.
    /// </summary>
    public ObservableCollection<SearchFilterViewModel> CurrentFilters { get; }

    /// <summary>
    /// 搜索模块列表.
    /// </summary>
    public ObservableCollection<SearchModuleItemViewModel> Modules { get; }
}
