// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Search;
using Richasy.BiliKernel.Models.Search;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 直播搜索分区详情视图模型.
/// </summary>
public sealed partial class LiveSearchSectionDetailViewModel
{
    private readonly ISearchService _service;
    private readonly ILogger<LiveSearchSectionDetailViewModel> _logger;

    private bool _canRequest;
    private string _offset;
    private string _keyword;
    private SearchPartition _partition;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isLoading;

    /// <summary>
    /// 列表已完成更新.
    /// </summary>
    public event EventHandler ListUpdated;

    /// <inheritdoc/>
    public SearchSectionType SectionType => SearchSectionType.Live;

    /// <summary>
    /// 条目列表.
    /// </summary>
    public ObservableCollection<LiveItemViewModel> Items { get; } = new();
}
