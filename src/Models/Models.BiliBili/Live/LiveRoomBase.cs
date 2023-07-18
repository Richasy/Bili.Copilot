// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 直播间基类.
/// </summary>
public class LiveRoomBase
{
    /// <summary>
    /// 用户Id.
    /// </summary>
    [JsonPropertyName("uid")]
    public long UserId { get; set; }

    /// <summary>
    /// 直播间封面.
    /// </summary>
    [JsonPropertyName("cover")]
    public string Cover { get; set; }

    /// <summary>
    /// 直播间标题.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// 清晰度描述列表.
    /// </summary>
    [JsonPropertyName("quality_description")]
    public List<LiveQualityDescription> QualityDescriptionList { get; set; }

    /// <summary>
    /// 会话标识符.
    /// </summary>
    [JsonPropertyName("session_id")]
    public string SessionId { get; set; }

    /// <summary>
    /// 分组标识符.
    /// </summary>
    [JsonPropertyName("group_id")]
    public int GroupId { get; set; }

    /// <summary>
    /// 在线观看人数.
    /// </summary>
    [JsonPropertyName("online")]
    public int ViewerCount { get; set; }

    /// <summary>
    /// 播放地址.
    /// </summary>
    [JsonPropertyName("play_url")]
    public string PlayUrl { get; set; }

    /// <summary>
    /// H265播放地址.
    /// </summary>
    [JsonPropertyName("play_url_h265")]
    public string H265PlayUrl { get; set; }

    /// <summary>
    /// 当前清晰度.
    /// </summary>
    [JsonPropertyName("current_quality")]
    public int CurrentQuality { get; set; }

    /// <summary>
    /// 对决的直播间Id.
    /// </summary>
    [JsonPropertyName("pk_id")]
    public int PkId { get; set; }

    /// <summary>
    /// 直播间地址.
    /// </summary>
    [JsonPropertyName("link")]
    public string Link { get; set; }
}

/// <summary>
/// 直播清晰度说明.
/// </summary>
public class LiveQualityDescription
{
    /// <summary>
    /// 清晰度标识.
    /// </summary>
    [JsonPropertyName("qn")]
    public int Quality { get; set; }

    /// <summary>
    /// 清晰度说明.
    /// </summary>
    [JsonPropertyName("desc")]
    public string Description { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is LiveQualityDescription description && Quality == description.Quality;

    /// <inheritdoc/>
    public override int GetHashCode() => -141866058 + Quality.GetHashCode();
}

