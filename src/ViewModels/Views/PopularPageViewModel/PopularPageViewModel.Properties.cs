// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Video;
using Bili.Copilot.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 流行视频页面的视图模型.
/// </summary>
public sealed partial class PopularPageViewModel
{
    private readonly Dictionary<PopularType, IEnumerable<VideoInformation>> _moduleCaches;
    private readonly Dictionary<string,  IEnumerable<VideoInformation>> _partitionCaches;

    [ObservableProperty]
    private PopularType _currentType;

    [ObservableProperty]
    private string _partitionId;

    [ObservableProperty]
    private bool _isRecommendShown;

    [ObservableProperty]
    private bool _isHotShown;

    [ObservableProperty]
    private bool _isRankShown;

    [ObservableProperty]
    private bool _isInPartition;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private double _navListColumnWidth;

    [ObservableProperty]
    private bool _isInitializing;

    /// <summary>
    /// 实例.
    /// </summary>
    public static PopularPageViewModel Instance { get; } = new();

    /// <summary>
    /// 分区集合.
    /// </summary>
    public ObservableCollection<PartitionItemViewModel> Partitions { get; }
}
