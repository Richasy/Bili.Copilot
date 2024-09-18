// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Search;
using Richasy.BiliKernel.Models;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 视频搜索分区详情视图模型.
/// </summary>
public sealed partial class VideoSearchSectionDetailViewModel
{
    private readonly ISearchService _service;
    private readonly ILogger<VideoSearchSectionDetailViewModel> _logger;

    private bool _canRequest;
    private string _offset;
    private string _keyword;
    private bool _isPreventLoadMore;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private ComprehensiveSearchSortType _sort;

    [ObservableProperty]
    private List<ComprehensiveSearchSortType> _sorts;

    /// <summary>
    /// 列表已完成更新.
    /// </summary>
    public event EventHandler ListUpdated;

    /// <inheritdoc/>
    public SearchSectionType SectionType => SearchSectionType.Video;

    /// <summary>
    /// 条目列表.
    /// </summary>
    public ObservableCollection<VideoItemViewModel> Items { get; } = new();
}
