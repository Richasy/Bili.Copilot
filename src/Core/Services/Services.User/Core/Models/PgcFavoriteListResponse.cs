// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Richasy.BiliKernel.Services.User.Core;

/// <summary>
/// PGC收藏夹响应结果.
/// </summary>
internal sealed class PgcFavoriteListResponse
{
    /// <summary>
    /// 剧集类型名.
    /// </summary>
    [JsonPropertyName("follow_list")]
    public IList<FavoritePgcItem>? FollowList { get; set; }

    /// <summary>
    /// 是否还有更多.
    /// </summary>
    [JsonPropertyName("has_next")]
    public int? HasMore { get; set; }

    /// <summary>
    /// 总数.
    /// </summary>
    [JsonPropertyName("total")]
    public int? Total { get; set; }
}

/// <summary>
/// PGC内容所属地区.
/// </summary>
internal sealed class PgcArea
{
    /// <summary>
    /// 地区Id.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 属地名称.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

/// <summary>
/// PGC收藏夹条目.
/// </summary>
internal sealed class FavoritePgcItem
{
    /// <summary>
    /// 所属地区.
    /// </summary>
    [JsonPropertyName("areas")]
    public IList<PgcArea> Areas { get; set; }

    /// <summary>
    /// 徽章文本.
    /// </summary>
    [JsonPropertyName("badge")]
    public string? BadgeText { get; set; }

    /// <summary>
    /// 封面.
    /// </summary>
    [JsonPropertyName("cover")]
    public string? Cover { get; set; }

    /// <summary>
    /// 是否正在追.
    /// </summary>
    [JsonPropertyName("follow")]
    public int? IsFollow { get; set; }

    /// <summary>
    /// 是否已完结.
    /// </summary>
    [JsonPropertyName("is_finish")]
    public int? IsFinish { get; set; }

    /// <summary>
    /// 收藏时间.
    /// </summary>
    [JsonPropertyName("mtime")]
    public long? CollectTime { get; set; }

    /// <summary>
    /// 最新章节.
    /// </summary>
    [JsonPropertyName("new_ep")]
    public PgcEpisode? NewEpisode { get; set; }

    /// <summary>
    /// 播放历史记录.
    /// </summary>
    [JsonPropertyName("progress")]
    public PgcProgress? Progress { get; set; }

    /// <summary>
    /// 剧集的季Id.
    /// </summary>
    [JsonPropertyName("season_id")]
    public int? SeasonId { get; set; }

    /// <summary>
    /// 剧集类型名.
    /// </summary>
    [JsonPropertyName("season_type_name")]
    public string? SeasonTypeName { get; set; }

    /// <summary>
    /// 剧集类型名.
    /// </summary>
    [JsonPropertyName("series")]
    public PgcSeries? Series { get; set; }

    /// <summary>
    /// 剧集类型名.
    /// </summary>
    [JsonPropertyName("square_cover")]
    public string? SquareCover { get; set; }

    /// <summary>
    /// 标题.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// 网址.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

/// <summary>
/// PGC内容历史记录.
/// </summary>
internal sealed class PgcProgress
{
    /// <summary>
    /// 最后一次播放的单集Id.
    /// </summary>
    [JsonPropertyName("last_ep_id")]
    public long LastEpisodeId { get; set; }

    /// <summary>
    /// 最后一次播放的单集索引.
    /// </summary>
    [JsonPropertyName("last_ep_index")]
    public string? LastEpisodeIndex { get; set; }

    /// <summary>
    /// 播放进度（秒）.
    /// </summary>
    [JsonPropertyName("last_time")]
    public long LastTime { get; set; }
}

/// <summary>
/// PGC系列.
/// </summary>
internal sealed class PgcSeries
{
    /// <summary>
    /// 系列Id.
    /// </summary>
    [JsonPropertyName("series_id")]
    public int Id { get; set; }

    /// <summary>
    /// 标题.
    /// </summary>
    [JsonPropertyName("series_title")]
    public string? Title { get; set; }
}

/// <summary>
/// 剧集信息.
/// </summary>
internal sealed class PgcEpisode
{
    /// <summary>
    /// 剧集封面.
    /// </summary>
    [JsonPropertyName("cover")]
    public string? Cover { get; set; }

    /// <summary>
    /// 剧集Id.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; set; }

    /// <summary>
    /// 显示内容.
    /// </summary>
    [JsonPropertyName("index_show")]
    public string? DisplayText { get; set; }
}
