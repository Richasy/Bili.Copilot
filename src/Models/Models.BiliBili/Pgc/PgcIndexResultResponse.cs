// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// PGC索引筛选结果响应.
/// </summary>
public class PgcIndexResultResponse
{
    /// <summary>
    /// 是否还有下一页.
    /// </summary>
    [JsonPropertyName("has_next")]
    public int HasNext { get; set; }

    /// <summary>
    /// 结果.
    /// </summary>
    [JsonPropertyName("list")]
    public List<PgcIndexItem> List { get; set; }

    /// <summary>
    /// 当前页码.
    /// </summary>
    [JsonPropertyName("num")]
    public int PageNumber { get; set; }

    /// <summary>
    /// 每页条目数.
    /// </summary>
    [JsonPropertyName("size")]
    public int PageSize { get; set; }

    /// <summary>
    /// 总数.
    /// </summary>
    [JsonPropertyName("total")]
    public int TotalCount { get; set; }
}

/// <summary>
/// PGC索引条目.
/// </summary>
public class PgcIndexItem
{
    /// <summary>
    /// 徽章文本.
    /// </summary>
    [JsonPropertyName("badge")]
    public string BadgeText { get; set; }

    /// <summary>
    /// 筛选条件.
    /// </summary>
    [JsonPropertyName("cover")]
    public string Cover { get; set; }

    /// <summary>
    /// 显示附加文本.
    /// </summary>
    [JsonPropertyName("index_show")]
    public string AdditionalText { get; set; }

    /// <summary>
    /// 是否完结.
    /// </summary>
    [JsonPropertyName("is_finish")]
    public int IsFinish { get; set; }

    /// <summary>
    /// 网址.
    /// </summary>
    [JsonPropertyName("link")]
    public string Link { get; set; }

    /// <summary>
    /// 媒体Id.
    /// </summary>
    [JsonPropertyName("media_id")]
    public int MediaId { get; set; }

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
    /// 标题.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// 排序显示文本.
    /// </summary>
    [JsonPropertyName("order")]
    public string OrderText { get; set; }
}

