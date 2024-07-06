// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Richasy.BiliKernel.Services.Media.Core;

internal sealed class ViewLaterSetResponse
{
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("list")]
    public IList<ViewLaterVideo>? List { get; set; }
}

internal sealed class ViewLaterVideo
{
    [JsonPropertyName("aid")]
    public long Aid { get; set; }

    [JsonPropertyName("videos")]
    public int? Videos { get; set; }

    [JsonPropertyName("tid")]
    public int? TagId { get; set; }

    [JsonPropertyName("tname")]
    public string? TagName { get; set; }

    [JsonPropertyName("copyright")]
    public int? Copyright { get; set; }

    [JsonPropertyName("pic")]
    public string? Cover { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("pubdate")]
    public long? PublishTime { get; set; }

    [JsonPropertyName("ctime")]
    public long? CreateTime { get; set; }

    [JsonPropertyName("desc")]
    public string? Description { get; set; }

    [JsonPropertyName("state")]
    public int? State { get; set; }

    [JsonPropertyName("duration")]
    public int Duration { get; set; }

    [JsonPropertyName("redirect_url")]
    public string? RedirectUrl { get; set; }

    [JsonPropertyName("owner")]
    public PublisherInfo Owner { get; set; }

    [JsonPropertyName("stat")]
    public VideoStatusInfo Status { get; set; }

    [JsonPropertyName("dynamic")]
    public string Dynamic { get; set; }

    [JsonPropertyName("short_link_v2")]
    public string ShortLink { get; set; }

    [JsonPropertyName("pub_location")]
    public string PublishLocation { get; set; }

    [JsonPropertyName("cid")]
    public long? Cid { get; set; }

    [JsonPropertyName("progress")]
    public int Progress { get; set; }

    [JsonPropertyName("add_at")]
    public long? AddAt { get; set; }

    [JsonPropertyName("bvid")]
    public string Bvid { get; set; }

    [JsonPropertyName("view_text_1")]
    public string? PlayCountText { get; set; }

    [JsonPropertyName("card_type")]
    public int? CardType { get; set; }

    [JsonPropertyName("right_text")]
    public string? DanmakuText { get; set; }

    [JsonPropertyName("arc_state")]
    public int? VideoState { get; set; }

    [JsonPropertyName("pgc_label")]
    public string? PgcLabel { get; set; }

    [JsonPropertyName("show_up")]
    public bool? ShowUp { get; set; }

    [JsonPropertyName("forbid_fav")]
    public bool? ForbidFavorite { get; set; }

    [JsonPropertyName("forbid_sort")]
    public bool? ForbidSort { get; set; }

    [JsonPropertyName("season_title")]
    public string? SeasonTitle { get; set; }

    [JsonPropertyName("long_title")]
    public string? LongTitle { get; set; }

    [JsonPropertyName("index_title")]
    public string? IndexTitle { get; set; }

    [JsonPropertyName("first_frame")]
    public string FirstFrame { get; set; }
}
