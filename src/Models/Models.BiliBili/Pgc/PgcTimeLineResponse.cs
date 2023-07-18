// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// PGC时间线响应结果.
/// </summary>
public class PgcTimeLineResponse
{
    /// <summary>
    /// 副标题.
    /// </summary>
    [JsonPropertyName("current_time_text")]
    public string Subtitle { get; set; }

    /// <summary>
    /// 标签页Id.
    /// </summary>
    [JsonPropertyName("data")]
    public List<PgcTimeLineItem> Data { get; set; }

    /// <summary>
    /// 导航标题.
    /// </summary>
    [JsonPropertyName("navigation_title")]
    public string Title { get; set; }
}

/// <summary>
/// 时间轴条目.
/// </summary>
public class PgcTimeLineItem
{
    /// <summary>
    /// 日期.
    /// </summary>
    [JsonPropertyName("date")]
    public string Date { get; set; }

    /// <summary>
    /// 日期时间戳.
    /// </summary>
    [JsonPropertyName("date_ts")]
    public int DateTimeStamp { get; set; }

    /// <summary>
    /// 周几.
    /// </summary>
    [JsonPropertyName("day_of_week")]
    public int DayOfWeek { get; set; }

    /// <summary>
    /// 占位符文本.
    /// </summary>
    [JsonPropertyName("day_update_text")]
    public string HolderText { get; set; }

    /// <summary>
    /// 标签页Id.
    /// </summary>
    [JsonPropertyName("episodes")]
    public List<TimeLineEpisode> Episodes { get; set; }

    /// <summary>
    /// 是否为今天，0-不是，1-是.
    /// </summary>
    [JsonPropertyName("is_today")]
    public int IsToday { get; set; }
}

/// <summary>
/// 时间轴剧集信息.
/// </summary>
public class TimeLineEpisode
{
    /// <summary>
    /// 封面.
    /// </summary>
    [JsonPropertyName("cover")]
    public string Cover { get; set; }

    /// <summary>
    /// 分集Id.
    /// </summary>
    [JsonPropertyName("episode_id")]
    public int EpisodeId { get; set; }

    /// <summary>
    /// 是否关注，0-不关注，1-关注.
    /// </summary>
    [JsonPropertyName("follow")]
    public int IsFollow { get; set; }

    /// <summary>
    /// 发布到第几集.
    /// </summary>
    [JsonPropertyName("pub_index")]
    public string PublishIndex { get; set; }

    /// <summary>
    /// 发布时间.
    /// </summary>
    [JsonPropertyName("pub_time")]
    public string PublishTime { get; set; }

    /// <summary>
    /// 发布时间戳.
    /// </summary>
    [JsonPropertyName("pub_ts")]
    public int PublishTimeStamp { get; set; }

    /// <summary>
    /// 是否已经发布.
    /// </summary>
    [JsonPropertyName("published")]
    public int IsPublished { get; set; }

    /// <summary>
    /// 剧集Id.
    /// </summary>
    [JsonPropertyName("season_id")]
    public int SeasonId { get; set; }

    /// <summary>
    /// 剧集类型.
    /// </summary>
    [JsonPropertyName("season_type")]
    public int SeasonType { get; set; }

    /// <summary>
    /// 矩形封面.
    /// </summary>
    [JsonPropertyName("square_cover")]
    public string SqureCover { get; set; }

    /// <summary>
    /// 标题.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// 网址.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; }
}

