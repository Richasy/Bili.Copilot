// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Richasy.BiliKernel.Services.Media.Core;

internal sealed class PgcIndexResultResponse
{
    [JsonPropertyName("has_next")]
    public int? HasNext { get; set; }

    [JsonPropertyName("list")]
    public IList<PgcIndexItem>? List { get; set; }

    [JsonPropertyName("num")]
    public int? PageNumber { get; set; }

    [JsonPropertyName("size")]
    public int? PageSize { get; set; }

    [JsonPropertyName("total")]
    public int? TotalCount { get; set; }
}

internal sealed class PgcIndexItem
{
    [JsonPropertyName("badge")]
    public string? BadgeText { get; set; }

    [JsonPropertyName("cover")]
    public string? Cover { get; set; }

    [JsonPropertyName("index_show")]
    public string? AdditionalText { get; set; }

    [JsonPropertyName("is_finish")]
    public int? IsFinish { get; set; }

    [JsonPropertyName("link")]
    public string? Link { get; set; }

    [JsonPropertyName("media_id")]
    public long? MediaId { get; set; }

    [JsonPropertyName("season_id")]
    public long? SeasonId { get; set; }

    [JsonPropertyName("season_type")]
    public int SeasonType { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("subTitle")]
    public string? Subtitle { get; set; }

    [JsonPropertyName("first_ep")]
    public PgcIndexFirstEpisode FirstEpisode { get; set; }

    [JsonPropertyName("score")]
    public double? Score { get; set; }
}

internal sealed class PgcIndexFirstEpisode
{
    [JsonPropertyName("cover")]
    public string? Cover { get; set; }

    [JsonPropertyName("ep_id")]
    public long EpisodeId { get; set; }
}
