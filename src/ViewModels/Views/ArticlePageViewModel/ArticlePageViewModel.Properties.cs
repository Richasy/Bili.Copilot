// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.Article;
using Bili.Copilot.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 专栏文章页面的视图模型.
/// </summary>
public sealed partial class ArticlePageViewModel
{
    private static readonly Lazy<ArticlePageViewModel> _lazyInstance = new Lazy<ArticlePageViewModel>(() => new ArticlePageViewModel());
    private readonly Dictionary<PartitionItemViewModel, IEnumerable<ArticleInformation>> _caches;

    [ObservableProperty]
    private PartitionItemViewModel _currentPartition;

    [ObservableProperty]
    private ArticleSortType _sortType;

    [ObservableProperty]
    private bool _isRecommendPartition;

    [ObservableProperty]
    private double _navListColumnWidth;

    /// <summary>
    /// 请求滚动到顶部.
    /// </summary>
    public event EventHandler RequestScrollToTop;

    /// <summary>
    /// 实例.
    /// </summary>
    public static ArticlePageViewModel Instance => _lazyInstance.Value;

    /// <summary>
    /// 分区集合.
    /// </summary>
    public ObservableCollection<PartitionItemViewModel> Partitions { get; }

    /// <summary>
    /// 排序方式.
    /// </summary>
    public ObservableCollection<ArticleSortType> SortTypes { get; }
}
