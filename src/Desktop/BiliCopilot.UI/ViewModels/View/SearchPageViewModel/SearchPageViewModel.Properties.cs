// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Search;
using Richasy.BiliKernel.Models.Media;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 搜索页面视图模型.
/// </summary>
public sealed partial class SearchPageViewModel
{
    private readonly ISearchService _service;
    private readonly ILogger<SearchPageViewModel> _logger;

    private string _initialOffset;
    private IReadOnlyList<VideoInformation> _initialVideos;

    [ObservableProperty]
    private string _keyword;

    [ObservableProperty]
    private ISearchSectionDetailViewModel _selectedSection;

    /// <summary>
    /// 详情分区列表.
    /// </summary>
    public ObservableCollection<ISearchSectionDetailViewModel> Sections { get; } = new();
}
