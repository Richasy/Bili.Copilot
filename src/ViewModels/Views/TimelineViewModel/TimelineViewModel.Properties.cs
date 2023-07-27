// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using Bili.Copilot.Models.Data.Pgc;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 动漫时间线视图模型.
/// </summary>
public sealed partial class TimelineViewModel
{
    [ObservableProperty]
    private TimelineInformation _selectedTimeline;

    [ObservableProperty]
    private string _errorText;

    [ObservableProperty]
    private bool _isError;

    [ObservableProperty]
    private bool _isReloading;

    /// <summary>
    /// 实例.
    /// </summary>
    public static TimelineViewModel Instance { get; } = new();

    /// <summary>
    /// 时间线集合.
    /// </summary>
    public ObservableCollection<TimelineInformation> TimelineCollection { get; }
}
