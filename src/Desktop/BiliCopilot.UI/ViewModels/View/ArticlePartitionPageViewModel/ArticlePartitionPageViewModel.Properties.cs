// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Article;
using Richasy.BiliKernel.Models;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 专栏文章分区页面视图模型.
/// </summary>
public sealed partial class ArticlePartitionPageViewModel
{
    private readonly IArticleDiscoveryService _service;
    private readonly ILogger<ArticlePartitionPageViewModel> _logger;
    private readonly Dictionary<Partition, ArticlePartitionDetailViewModel> _partitionCache = new();

    [ObservableProperty]
    private bool _isPartitionLoading;

    [ObservableProperty]
    private ArticlePartitionDetailViewModel? _selectedPartition;

    /// <summary>
    /// 区块加载完成.
    /// </summary>
    public event EventHandler PartitionInitialized;

    /// <summary>
    /// 主分区.
    /// </summary>
    public ObservableCollection<PartitionViewModel> Partitions { get; } = new();
}
