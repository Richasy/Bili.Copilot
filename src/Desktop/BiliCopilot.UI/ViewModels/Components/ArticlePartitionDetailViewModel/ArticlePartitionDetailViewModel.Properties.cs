// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Article;
using Richasy.BiliKernel.Models;
using System.Collections.ObjectModel;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 专栏文章分区详情视图模型.
/// </summary>
public sealed partial class ArticlePartitionDetailViewModel
{
    private readonly ILogger<ArticlePartitionDetailViewModel> _logger;
    private readonly IArticleDiscoveryService _service;
    private readonly Dictionary<string, List<ArticleItemViewModel>> _childPartitionArticleCache = new();
    private readonly Dictionary<string, int> _childPartitionOffsetCache = new();
    private readonly bool _isRecommendPartition;
    private readonly bool _isHotPartition;

    private List<ArticleItemViewModel> _recommendCache;
    private int _recommendOffset;
    private bool _preventLoadMore;

    [ObservableProperty]
    private List<ArticleSortType> _sortTypes;

    [ObservableProperty]
    private List<PartitionViewModel> _children;

    [ObservableProperty]
    private ArticleSortType _selectedSortType;

    [ObservableProperty]
    private bool _isRecommend;

    [ObservableProperty]
    private bool _isArticleLoading;

    [ObservableProperty]
    private PartitionViewModel _currentPartition;

    /// <summary>
    /// 初始化完成.
    /// </summary>
    public event EventHandler Initialized;

    /// <summary>
    /// 文章列表已完成更新.
    /// </summary>
    public event EventHandler ArticleListUpdated;

    /// <summary>
    /// 显示的文章列表.
    /// </summary>
    public ObservableCollection<ArticleItemViewModel> Articles { get; } = new();
}
