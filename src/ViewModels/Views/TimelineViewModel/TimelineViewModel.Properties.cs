// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.ObjectModel;
using Bili.Copilot.Models.Data.Pgc;
using Bili.Copilot.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 动漫时间线视图模型.
/// </summary>
public sealed partial class TimelineViewModel
{
    private static readonly Lazy<TimelineViewModel> _lazyInstance = new(() => new TimelineViewModel());

    [ObservableProperty]
    private string _errorText;

    [ObservableProperty]
    private bool _isError;

    [ObservableProperty]
    private bool _isReloading;

    [ObservableProperty]
    private bool _isSeasonEmpty;

    [ObservableProperty]
    private TimelineItemViewModel _selectedTimeline;

    /// <summary>
    /// 实例.
    /// </summary>
    public static TimelineViewModel Instance => _lazyInstance.Value;

    /// <summary>
    /// 时间线集合.
    /// </summary>
    public ObservableCollection<TimelineItemViewModel> TimelineCollection { get; }

    /// <summary>
    /// 季度集合.
    /// </summary>
    public ObservableCollection<SeasonInformation> SeasonCollection { get; }
}
