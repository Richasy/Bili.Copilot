// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Models;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 文章历史记录详情视图模型.
/// </summary>
public sealed partial class ArticleHistorySectionDetailViewModel
{
    private readonly IViewHistoryService _service;
    private readonly ILogger<ArticleHistorySectionDetailViewModel> _logger;

    private bool _isPreventLoadMore;
    private long _offset;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isLoading;

    /// <summary>
    /// 列表已完成更新.
    /// </summary>
    public event EventHandler ListUpdated;

    /// <inheritdoc/>
    public ViewHistoryTabType Type => ViewHistoryTabType.Article;

    /// <summary>
    /// 条目列表.
    /// </summary>
    public ObservableCollection<ArticleItemViewModel> Items { get; } = new();
}
