// Copyright (c) Bili Copilot. All rights reserved.

using Richasy.BiliKernel.Models.Media;
using Richasy.WinUI.Share.ViewModels;
using Windows.Globalization;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 时间轴条目视图模型.
/// </summary>
public sealed partial class TimelineItemViewModel : ViewModelBase<TimelineInformation>
{
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
