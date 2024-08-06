// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Search;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 搜索页面视图模型.
/// </summary>
public sealed partial class SearchPageViewModel
{
    private readonly ISearchService _service;
    private readonly ILogger<SearchPageViewModel> _logger;

    [ObservableProperty]
    private string _keyword;

    [ObservableProperty]
    private bool _isSearching;

    [ObservableProperty]
    private ISearchSectionDetailViewModel _selectedSection;

    /// <summary>
    /// 详情分区列表.
    /// </summary>
    [ObservableProperty]
    private IReadOnlyCollection<ISearchSectionDetailViewModel>? _sections;

    /// <summary>
    /// 分区初始化完成.
    /// </summary>
    public event EventHandler SectionInitialized;
}
