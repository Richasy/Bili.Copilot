// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Bili.Copilot.Models.Data.Community;
using Bili.Copilot.Models.Data.Live;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 直播分区详情视图模型.
/// </summary>
public sealed partial class LivePartitionDetailViewModel
{
    private readonly Dictionary<LiveTag, IEnumerable<LiveInformation>> _caches;

    private int _totalCount;

    [ObservableProperty]
    private Partition _originPartition;

    [ObservableProperty]
    private LiveTag _currentTag;

    [ObservableProperty]
    private bool _isEmpty;

    /// <summary>
    /// 请求滚动到顶部.
    /// </summary>
    public event EventHandler RequestScrollToTop;

    /// <summary>
    /// 实例.
    /// </summary>
    public static LivePartitionDetailViewModel Instance { get; } = new();

    /// <summary>
    /// 直播标签集合.
    /// </summary>
    public ObservableCollection<LiveTag> Tags { get; }
}
