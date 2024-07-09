// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Richasy.BiliKernel.Services.Media.Core.Models;

internal sealed class PgcTimeLineResponse
{
    [JsonPropertyName("current_time_text")]
    public string? Subtitle { get; set; }

    [JsonPropertyName("data")]
    public IList<PgcTimeLineItem>? Data { get; set; }

    [JsonPropertyName("navigation_title")]
    public string? Title { get; set; }
}

internal sealed class PgcTimeLineItem
{
    [JsonPropertyName("date")]
    public string Date { get; set; }

    [JsonPropertyName("date_ts")]
    public long DateTimeStamp { get; set; }

    [JsonPropertyName("day_of_week")]
    public int DayOfWeek { get; set; }

    [JsonPropertyName("day_update_text")]
    public string HolderText { get; set; }

    [JsonPropertyName("episodes")]
    public IList<TimeLineEpisode>? Episodes { get; set; }

    [JsonPropertyName("is_today")]
    public int IsToday { get; set; }
}

internal sealed class TimeLineEpisode
{
    [JsonPropertyName("cover")]
    public string Cover { get; set; }

    [JsonPropertyName("episode_id")]
    public long? EpisodeId { get; set; }

    [JsonPropertyName("follow")]
    public int IsFollow { get; set; }

    [JsonPropertyName("pub_index")]
    public string PublishIndex { get; set; }

    [JsonPropertyName("pub_time")]
    public string PublishTime { get; set; }

    [JsonPropertyName("pub_ts")]
    public long PublishTimeStamp { get; set; }

    [JsonPropertyName("published")]
    public int IsPublished { get; set; }

    [JsonPropertyName("season_id")]
    public long SeasonId { get; set; }

    [JsonPropertyName("season_type")]
    public int SeasonType { get; set; }

    [JsonPropertyName("square_cover")]
    public string? SqureCover { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
