// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// PGCʱ������Ӧ���.
/// </summary>
public class PgcTimeLineResponse
{
    /// <summary>
    /// ������.
    /// </summary>
    [JsonPropertyName("current_time_text")]
    public string Subtitle { get; set; }

    /// <summary>
    /// ��ǩҳId.
    /// </summary>
    [JsonPropertyName("data")]
    public List<PgcTimeLineItem> Data { get; set; }

    /// <summary>
    /// ��������.
    /// </summary>
    [JsonPropertyName("navigation_title")]
    public string Title { get; set; }
}

/// <summary>
/// ʱ������Ŀ.
/// </summary>
public class PgcTimeLineItem
{
    /// <summary>
    /// ����.
    /// </summary>
    [JsonPropertyName("date")]
    public string Date { get; set; }

    /// <summary>
    /// ����ʱ���.
    /// </summary>
    [JsonPropertyName("date_ts")]
    public int DateTimeStamp { get; set; }

    /// <summary>
    /// �ܼ�.
    /// </summary>
    [JsonPropertyName("day_of_week")]
    public int DayOfWeek { get; set; }

    /// <summary>
    /// ռλ���ı�.
    /// </summary>
    [JsonPropertyName("day_update_text")]
    public string HolderText { get; set; }

    /// <summary>
    /// ��ǩҳId.
    /// </summary>
    [JsonPropertyName("episodes")]
    public List<TimeLineEpisode> Episodes { get; set; }

    /// <summary>
    /// �Ƿ�Ϊ���죬0-���ǣ�1-��.
    /// </summary>
    [JsonPropertyName("is_today")]
    public int IsToday { get; set; }
}

/// <summary>
/// ʱ����缯��Ϣ.
/// </summary>
public class TimeLineEpisode
{
    /// <summary>
    /// ����.
    /// </summary>
    [JsonPropertyName("cover")]
    public string Cover { get; set; }

    /// <summary>
    /// �ּ�Id.
    /// </summary>
    [JsonPropertyName("episode_id")]
    public int EpisodeId { get; set; }

    /// <summary>
    /// �Ƿ��ע��0-����ע��1-��ע.
    /// </summary>
    [JsonPropertyName("follow")]
    public int IsFollow { get; set; }

    /// <summary>
    /// �������ڼ���.
    /// </summary>
    [JsonPropertyName("pub_index")]
    public string PublishIndex { get; set; }

    /// <summary>
    /// ����ʱ��.
    /// </summary>
    [JsonPropertyName("pub_time")]
    public string PublishTime { get; set; }

    /// <summary>
    /// ����ʱ���.
    /// </summary>
    [JsonPropertyName("pub_ts")]
    public int PublishTimeStamp { get; set; }

    /// <summary>
    /// �Ƿ��Ѿ�����.
    /// </summary>
    [JsonPropertyName("published")]
    public int IsPublished { get; set; }

    /// <summary>
    /// �缯Id.
    /// </summary>
    [JsonPropertyName("season_id")]
    public int SeasonId { get; set; }

    /// <summary>
    /// �缯����.
    /// </summary>
    [JsonPropertyName("season_type")]
    public int SeasonType { get; set; }

    /// <summary>
    /// ���η���.
    /// </summary>
    [JsonPropertyName("square_cover")]
    public string SqureCover { get; set; }

    /// <summary>
    /// ����.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// ��ַ.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; }
}

