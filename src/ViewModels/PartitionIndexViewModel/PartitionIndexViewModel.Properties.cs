// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using Bili.Copilot.Models.Data.Community;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 分区索引视图模型.
/// </summary>
public sealed partial class PartitionIndexViewModel
{
    private bool _isInitialized;

    [ObservableProperty]
    private bool _isInitializing;

    /// <summary>
    /// 实例.
    /// </summary>
    public static PartitionIndexViewModel Instance { get; } = new();

    /// <summary>
    /// 分区集合.
    /// </summary>
    public ObservableCollection<Partition> Partitions { get; }
}
