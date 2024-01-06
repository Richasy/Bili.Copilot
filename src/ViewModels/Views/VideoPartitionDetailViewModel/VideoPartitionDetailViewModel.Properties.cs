// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.Community;
using Bili.Copilot.Models.Data.Video;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 分区详情视图模型.
/// </summary>
public sealed partial class VideoPartitionDetailViewModel
{
    private readonly Dictionary<Partition, IEnumerable<VideoInformation>> _caches;

    [ObservableProperty]
    private Partition _originPartition;

    [ObservableProperty]
    private Partition _currentSubPartition;

    [ObservableProperty]
    private Partition _currentRecommendPartition;

    [ObservableProperty]
    private VideoSortType _sortType;

    [ObservableProperty]
    private bool _isShowBanner;

    [ObservableProperty]
    private bool _isRecommendShown;

    [ObservableProperty]
    private bool _isSubPartitionShown;

    [ObservableProperty]
    private VideoPartitionDetailType _detailType;

    [ObservableProperty]
    private bool _isEmpty;

    /// <summary>
    /// 实例.
    /// </summary>
    public static VideoPartitionDetailViewModel Instance { get; } = new();

    /// <summary>
    /// 横幅集合.
    /// </summary>
    public ObservableCollection<BannerViewModel> Banners { get; }

    /// <summary>
    /// 子分区集合.
    /// </summary>
    public ObservableCollection<Partition> SubPartitions { get; }

    /// <summary>
    /// 排序方式集合.
    /// </summary>
    public ObservableCollection<VideoSortType> SortTypes { get; }
}
