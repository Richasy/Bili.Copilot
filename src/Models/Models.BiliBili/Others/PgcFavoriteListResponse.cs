// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// PGC收藏夹响应结果.
/// </summary>
public class PgcFavoriteListResponse
{
    /// <summary>
    /// 剧集类型名.
    /// </summary>
    [JsonPropertyName("follow_list")]
    public List<FavoritePgcItem> FollowList { get; set; }

    /// <summary>
    /// 是否还有更多.
    /// </summary>
    [JsonPropertyName("has_next")]
    public int HasMore { get; set; }

    /// <summary>
    /// 总数.
    /// </summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }
}

/// <summary>
/// PGC收藏夹条目.
/// </summary>
public class FavoritePgcItem
{
    /// <summary>
    /// 所属地区.
    /// </summary>
    [JsonPropertyName("areas")]
    public List<PgcArea> Areas { get; set; }

    /// <summary>
    /// 徽章文本.
    /// </summary>
    [JsonPropertyName("badge")]
    public string BadgeText { get; set; }

    /// <summary>
    /// 封面.
    /// </summary>
    [JsonPropertyName("cover")]
    public string Cover { get; set; }

    /// <summary>
    /// 是否正在追.
    /// </summary>
    [JsonPropertyName("follow")]
    public int IsFollow { get; set; }

    /// <summary>
    /// 是否已完结.
    /// </summary>
    [JsonPropertyName("is_finish")]
    public int IsFinish { get; set; }

    /// <summary>
    /// 收藏时间.
    /// </summary>
    [JsonPropertyName("mtime")]
    public int CollectTime { get; set; }

    /// <summary>
    /// 最新章节.
    /// </summary>
    [JsonPropertyName("new_ep")]
    public PgcEpisode NewEpisode { get; set; }

    /// <summary>
    /// 播放历史记录.
    /// </summary>
    [JsonPropertyName("progress")]
    public PgcProgress Progress { get; set; }

    /// <summary>
    /// 剧集的季Id.
    /// </summary>
    [JsonPropertyName("season_id")]
    public int SeasonId { get; set; }

    /// <summary>
    /// 剧集类型名.
    /// </summary>
    [JsonPropertyName("season_type_name")]
    public string SeasonTypeName { get; set; }

    /// <summary>
    /// 剧集类型名.
    /// </summary>
    [JsonPropertyName("series")]
    public PgcSeries Series { get; set; }

    /// <summary>
    /// 剧集类型名.
    /// </summary>
    [JsonPropertyName("square_cover")]
    public string SquareCover { get; set; }

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

