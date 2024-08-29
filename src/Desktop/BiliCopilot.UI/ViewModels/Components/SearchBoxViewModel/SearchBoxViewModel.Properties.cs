// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Search;
using Richasy.BiliKernel.Models.Search;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 搜索视图模型.
/// </summary>
public sealed partial class SearchBoxViewModel
{
    private readonly ISearchService _searchService;
    private readonly ILogger<SearchBoxViewModel> _logger;
    private readonly List<SearchRecommendItem> _recommendItems = new();
    private CancellationTokenSource? _cancellationTokenSource;

    [ObservableProperty]
    private string _keyword = string.Empty;

    [ObservableProperty]
    private bool _isHotSearchLoading;

    /// <summary>
    /// 搜索建议.
    /// </summary>
    public ObservableCollection<SearchSuggestItem> Suggestion { get; } = new();

    /// <summary>
    /// 热搜列表.
    /// </summary>
    public ObservableCollection<HotSearchItem> HotSearchItems { get; } = new();
}
