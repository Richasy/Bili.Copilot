// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.Article;
using Bili.Copilot.Models.Data.Community;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 专栏文章页面的视图模型.
/// </summary>
public sealed partial class ArticlePageViewModel
{
    private readonly Dictionary<Partition, IEnumerable<ArticleInformation>> _caches;

    [ObservableProperty]
    private Partition _currentPartition;

    [ObservableProperty]
    private ArticleSortType _sortType;

    [ObservableProperty]
    private bool _isRecommendPartition;

    /// <summary>
    /// 请求滚动到顶部.
    /// </summary>
    public event EventHandler RequestScrollToTop;

    /// <summary>
    /// 实例.
    /// </summary>
    public static ArticlePageViewModel Instance { get; } = new();

    /// <summary>
    /// 分区集合.
    /// </summary>
    public ObservableCollection<Partition> Partitions { get; }

    /// <summary>
    /// 排序方式.
    /// </summary>
    public ObservableCollection<ArticleSortType> SortTypes { get; }
}
