// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 动漫时间线视图模型.
/// </summary>
public sealed partial class AnimeTimelineViewModel
{
    private readonly IEntertainmentDiscoveryService _service;
    private readonly ILogger<AnimeTimelineViewModel> _logger;

    [ObservableProperty]
    private bool _isTimelineLoading;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private TimelineItemViewModel _selectedTimeline;

    [ObservableProperty]
    private ObservableCollection<TimelineItemViewModel> _timelines = new();

    /// <summary>
    /// 时间线加载完成.
    /// </summary>
    public event EventHandler TimelineInitialized;

    /// <inheritdoc/>
    public PgcSectionType SectionType => PgcSectionType.Timeline;
}
