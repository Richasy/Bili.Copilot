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
    private TimelineItemViewModel _selectedTimeline;

    /// <summary>
    /// 时间线加载完成.
    /// </summary>
    public event EventHandler TimelineInitialized;

    /// <inheritdoc/>
    public AnimeSectionType SectionType => AnimeSectionType.Timeline;

    /// <summary>
    /// 时间线.
    /// </summary>
    public ObservableCollection<TimelineItemViewModel> Timelines { get; } = new();
}
