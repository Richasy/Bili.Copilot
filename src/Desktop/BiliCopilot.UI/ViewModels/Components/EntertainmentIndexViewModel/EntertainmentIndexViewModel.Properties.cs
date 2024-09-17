// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 娱乐索引视图模型.
/// </summary>
public sealed partial class EntertainmentIndexViewModel
{
    private readonly IEntertainmentDiscoveryService _service;
    private readonly ILogger<EntertainmentIndexViewModel> _logger;
    private int _pageNumer;

    [ObservableProperty]
    private List<IndexFilterViewModel>? _filters;

    [ObservableProperty]
    private bool _isFilterLoading;

    [ObservableProperty]
    private bool _isSeasonLoading;

    /// <summary>
    /// 条目已更新.
    /// </summary>
    public event EventHandler ItemsUpdated;

    /// <inheritdoc/>
    public PgcSectionType SectionType { get; }

    /// <summary>
    /// 剧集列表.
    /// </summary>
    public ObservableCollection<SeasonItemViewModel> Items { get; } = new();
}
