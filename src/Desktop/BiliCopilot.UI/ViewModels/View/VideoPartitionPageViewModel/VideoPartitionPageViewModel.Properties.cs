﻿// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Models;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 视频分区页面视图模型.
/// </summary>
public sealed partial class VideoPartitionPageViewModel
{
    private readonly IVideoDiscoveryService _service;
    private readonly ILogger<VideoPartitionPageViewModel> _logger;
    private readonly Dictionary<Partition, VideoPartitionDetailViewModel> _partitionCache = new();

    [ObservableProperty]
    private double _navColumnWidth;

    [ObservableProperty]
    private bool _isNavColumnManualHide;

    [ObservableProperty]
    private bool _isPartitionLoading;

    [ObservableProperty]
    private VideoPartitionDetailViewModel? _selectedPartition;

    /// <summary>
    /// 区块加载完成.
    /// </summary>
    public event EventHandler PartitionInitialized;

    /// <summary>
    /// 主分区.
    /// </summary>
    public ObservableCollectionEx<VideoPartitionViewModel> Partitions { get; } = new();
}
