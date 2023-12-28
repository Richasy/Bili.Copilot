﻿// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using Bili.Copilot.Models.Data.Community;
using Bili.Copilot.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 直播分区索引视图模型.
/// </summary>
public sealed partial class LivePartitionIndexViewModel
{
    [ObservableProperty]
    private string _errorText;

    [ObservableProperty]
    private bool _isError;

    [ObservableProperty]
    private bool _isReloading;

    /// <summary>
    /// 实例.
    /// </summary>
    public static LivePartitionIndexViewModel Instance { get; } = new();

    /// <summary>
    /// 父级分区集合.
    /// </summary>
    public ObservableCollection<PartitionItemViewModel> ParentPartitions { get; }
}
