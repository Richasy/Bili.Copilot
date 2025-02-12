// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUIKernel.Share.ViewModels;
using Windows.Globalization;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 时间轴条目视图模型.
/// </summary>
public sealed partial class TimelineItemViewModel : ViewModelBase<TimelineInformation>
{
    [ObservableProperty]
    private List<SeasonItemViewModel>? _items;

    [ObservableProperty]
    private bool _isEmpty;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimelineItemViewModel"/> class.
    /// </summary>
    public TimelineItemViewModel(TimelineInformation info)
        : base(info)
    {
        var firstLan = ApplicationLanguages.Languages[0];
        var culture = new System.Globalization.CultureInfo(firstLan);
        var targetDay = DateTimeOffset.FromUnixTimeSeconds(info.TimeStamp).ToLocalTime();
        DayOfWeek = targetDay.ToString("ddd", culture);
        Date = info.Date;
        Count = info.Seasons.Count;
        Items = info.Seasons.Select(p => new SeasonItemViewModel(p, Models.Constants.SeasonCardStyle.Timeline)).ToList() ?? default;
        IsEmpty = Items is null || Items.Count == 0;
    }

    /// <summary>
    /// 星期几.
    /// </summary>
    public string DayOfWeek { get; }

    /// <summary>
    /// 日期.
    /// </summary>
    public string Date { get; }

    /// <summary>
    /// 剧集个数.
    /// </summary>
    public int Count { get; }
}
