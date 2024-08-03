// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 直播分区页面视图模型.
/// </summary>
public sealed partial class LivePartitionPageViewModel
{
    private readonly ILiveDiscoveryService _service;
    private readonly ILogger<LivePartitionPageViewModel> _logger;
    private readonly Dictionary<PartitionViewModel, LivePartitionDetailViewModel> _partitionCache = new();
    private readonly Dictionary<string, string> _partitionSelectedCache = new();

    private int _recommendOffset;

    [ObservableProperty]
    private double _subNavColumnWidth;

    [ObservableProperty]
    private bool _isSubNavEnabled;

    [ObservableProperty]
    private IReadOnlyCollection<PartitionViewModel>? _subPartitions;

    [ObservableProperty]
    private bool _isSectionLoading;

    [ObservableProperty]
    private bool _isRecommendLoading;

    [ObservableProperty]
    private PartitionViewModel _selectedMainSection;

    [ObservableProperty]
    private PartitionViewModel _selectedSubSection;

    [ObservableProperty]
    private LivePartitionDetailViewModel _selectedDetail;

    /// <summary>
    /// 分区加载完成.
    /// </summary>
    public event EventHandler SectionInitialized;

    /// <summary>
    /// 主分区变更.
    /// </summary>
    public event EventHandler MainSectionChanged;

    /// <summary>
    /// 推荐直播间更新.
    /// </summary>
    public event EventHandler RecommendUpdated;

    /// <summary>
    /// 直播间主分区.
    /// </summary>
    public ObservableCollection<PartitionViewModel> MainSections { get; } = new();

    /// <summary>
    /// 推荐直播间.
    /// </summary>
    public ObservableCollection<LiveItemViewModel> RecommendRooms { get; } = new();

    /// <summary>
    /// 关注的直播间.
    /// </summary>
    public ObservableCollection<LiveItemViewModel> FollowRooms { get; } = new();
}
