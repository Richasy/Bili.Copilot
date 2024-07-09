using System;
using System.Collections.Generic;

namespace Richasy.BiliKernel.Models.Media;

/// <summary>
/// 时间线条目信息.
/// </summary>
public sealed class TimelineInformation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TimelineInformation"/> class.
    /// </summary>
    /// <param name="date">发布日期.</param>
    /// <param name="dayOfWeek">周几.</param>
    /// <param name="isToday">是否是今天.</param>
    /// <param name="seasons">剧集列表.</param>
    public TimelineInformation(
        string date,
        int dayOfWeek,
        bool isToday,
        IList<SeasonInformation> seasons = default)
    {
        Date = date;
        DayOfWeek = dayOfWeek;
        IsToday = isToday;
        Seasons = seasons;
    }

    /// <summary>
    /// 发布时间.
    /// </summary>
    public string? Date { get; }

    /// <summary>
    /// 发布时间在周几.
    /// </summary>
    public int DayOfWeek { get; }

    /// <summary>
    /// 是否是今天.
    /// </summary>
    public bool IsToday { get; }

    /// <summary>
    /// 下属剧集.
    /// </summary>
    public IList<SeasonInformation>? Seasons { get; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is TimelineInformation information && Date == information.Date;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Date);
}
