// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// PGC搜索条目.
/// </summary>
public class PgcSearchItem : SearchItemBase
{
    /// <summary>
    /// 发布时间.
    /// </summary>
    [JsonPropertyName("ptime")]
    public int PublishTime { get; set; }

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
    /// 剧集类型名.
    /// </summary>
    [JsonPropertyName("season_type_name")]
    public string SeasonTypeName { get; set; }

    /// <summary>
    /// 说明文本.
    /// </summary>
    [JsonPropertyName("label")]
    public string Label { get; set; }

    /// <summary>
    /// 副标题.
    /// </summary>
    [JsonPropertyName("styles")]
    public string SubTitle { get; set; }

    /// <summary>
    /// 声优.
    /// </summary>
    [JsonPropertyName("cv")]
    public string CV { get; set; }

    /// <summary>
    /// 所属区域.
    /// </summary>
    [JsonPropertyName("area")]
    public string Area { get; set; }

    /// <summary>
    /// 参与人员.
    /// </summary>
    [JsonPropertyName("staff")]
    public string Staff { get; set; }

    /// <summary>
    /// 徽章文本.
    /// </summary>
    [JsonPropertyName("badge")]
    public string BadgeText { get; set; }

    /// <summary>
    /// 是否已追，0-未追，1-已追.
    /// </summary>
    [JsonPropertyName("is_atten")]
    public int IsFollow { get; set; }

    /// <summary>
    /// 评分.
    /// </summary>
    [JsonPropertyName("rating")]
    public double Rating { get; set; }

    /// <summary>
    /// 投票人数.
    /// </summary>
    [JsonPropertyName("vote")]
    public double VoteNumber { get; set; }
}

