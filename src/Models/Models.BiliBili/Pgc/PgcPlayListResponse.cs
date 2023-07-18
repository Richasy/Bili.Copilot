// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// PGC播放列表响应.
/// </summary>
public class PgcPlayListResponse
{
    /// <summary>
    /// Id.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 剧集列表.
    /// </summary>
    [JsonPropertyName("seasons")]
    public List<PgcPlayListSeason> Seasons { get; set; }

    /// <summary>
    /// 说明文本.
    /// </summary>
    [JsonPropertyName("summary")]
    public string Description { get; set; }

    /// <summary>
    /// 播放列表标题.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// 总数.
    /// </summary>
    [JsonPropertyName("total")]
    public string Total { get; set; }
}

/// <summary>
/// PGC播放列表剧集条目.
/// </summary>
public class PgcPlayListSeason
{
    /// <summary>
    /// 演员.
    /// </summary>
    [JsonPropertyName("actors")]
    public string Actors { get; set; }

    /// <summary>
    /// 模块子项列表.
    /// </summary>
    [JsonPropertyName("badge")]
    public string BadgeText { get; set; }

    /// <summary>
    /// 封面.
    /// </summary>
    [JsonPropertyName("cover")]
    public string Cover { get; set; }

    /// <summary>
    /// 简介.
    /// </summary>
    [JsonPropertyName("evaluate")]
    public string Description { get; set; }

    /// <summary>
    /// 链接.
    /// </summary>
    [JsonPropertyName("link")]
    public string Link { get; set; }

    /// <summary>
    /// 媒体链接.
    /// </summary>
    [JsonPropertyName("media_id")]
    public int MediaId { get; set; }

    /// <summary>
    /// 最新剧集.
    /// </summary>
    [JsonPropertyName("new_ep")]
    public PgcEpisode NewEpisode { get; set; }

    /// <summary>
    /// 评分.
    /// </summary>
    [JsonPropertyName("rating")]
    public PgcRating Rating { get; set; }

    /// <summary>
    /// 剧集Id.
    /// </summary>
    [JsonPropertyName("seasonId")]
    public int SeasonId { get; set; }

    /// <summary>
    /// 剧集类型.
    /// </summary>
    [JsonPropertyName("seasonType")]
    public int SeasonType { get; set; }

    /// <summary>
    /// 用户交互信息.
    /// </summary>
    [JsonPropertyName("stat")]
    public PgcPlayListItemStat Stat { get; set; }

    /// <summary>
    /// 标签.
    /// </summary>
    [JsonPropertyName("styles")]
    public string Styles { get; set; }

    /// <summary>
    /// 副标题.
    /// </summary>
    [JsonPropertyName("subtitle")]
    public string Subtitle { get; set; }

    /// <summary>
    /// 标题.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }
}

/// <summary>
/// PGC播放列表条目用户交互信息.
/// </summary>
public class PgcPlayListItemStat
{
    /// <summary>
    /// 弹幕数.
    /// </summary>
    [JsonPropertyName("danmakus")]
    public int DanmakuCount { get; set; }

    /// <summary>
    /// 收藏数.
    /// </summary>
    [JsonPropertyName("favorites")]
    public int FavoriteCount { get; set; }

    /// <summary>
    /// 播放数.
    /// </summary>
    [JsonPropertyName("views")]
    public int PlayCount { get; set; }
}

