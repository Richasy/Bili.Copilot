// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Search;
using Richasy.BiliKernel.Bili.User;
using System.Collections.ObjectModel;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 历史记录页面视图模型.
/// </summary>
public sealed partial class HistoryPageViewModel
{
    private readonly IViewHistoryService _service;
    private readonly ISearchService _searchService;
    private readonly ILogger<HistoryPageViewModel> _logger;
    private int _searchPn;
    private bool _preventLoadMoreSearch;
    private CancellationTokenSource _searchCancellationTokenSource;

    [ObservableProperty]
    private IHistorySectionDetailViewModel _selectedSection;

    [ObservableProperty]
    private List<IHistorySectionDetailViewModel> _sections;

    [ObservableProperty]
    private bool _isSearching;

    [ObservableProperty]
    private bool _isSearchMode;

    [ObservableProperty]
    private string _searchKeyword;

    [ObservableProperty]
    private bool _isSearchEmpty;

    /// <summary>
    /// 初始化完成.
    /// </summary>
    public event EventHandler SectionInitialized;

    /// <summary>
    /// 搜索更新.
    /// </summary>
    public event EventHandler SearchUpdated;

    /// <summary>
    /// 搜索视频结果.
    /// </summary>
    public ObservableCollection<VideoItemViewModel> SearchVideos { get; } = new();
}
