// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Models.Media;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 直播分区详情视图模型.
/// </summary>
public sealed partial class LivePartitionDetailViewModel
{
    private readonly ILogger<LivePartitionDetailViewModel> _logger;
    private readonly ILiveDiscoveryService _service;
    private readonly Dictionary<LiveTag, List<LiveItemViewModel>> _childPartitionRoomCache = new();
    private readonly Dictionary<LiveTag, int> _childPartitionOffsetCache = new();

    private bool _preventLoadMore;

    [ObservableProperty]
    private IReadOnlyCollection<LiveTag>? _children;

    [ObservableProperty]
    private bool _isLiveLoading;

    [ObservableProperty]
    private LiveTag _currentTag;

    /// <summary>
    /// 初始化完成.
    /// </summary>
    public event EventHandler Initialized;

    /// <summary>
    /// 直播列表已完成更新.
    /// </summary>
    public event EventHandler LiveListUpdated;

    /// <summary>
    /// 显示的直播间列表.
    /// </summary>
    public ObservableCollection<LiveItemViewModel> Rooms { get; } = new();
}
