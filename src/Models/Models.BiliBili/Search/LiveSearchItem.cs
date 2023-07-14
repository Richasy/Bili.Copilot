// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 直播搜索条目.
/// </summary>
public class LiveSearchItem : SearchItemBase
{
    /// <summary>
    /// 主播名.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// 直播间Id.
    /// </summary>
    [JsonPropertyName("roomid")]
    public int RoomId { get; set; }

    /// <summary>
    /// 用户Id.
    /// </summary>
    [JsonPropertyName("mid")]
    public long UserId { get; set; }

    /// <summary>
    /// 类型.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>
    /// 关注数.
    /// </summary>
    [JsonPropertyName("attentions")]
    public int FollowerCount { get; set; }

    /// <summary>
    /// 直播状态.
    /// </summary>
    [JsonPropertyName("live_status")]
    public int LiveStatus { get; set; }

    /// <summary>
    /// 直播间标签.
    /// </summary>
    [JsonPropertyName("tags")]
    public string Tags { get; set; }

    /// <summary>
    /// 直播分区.
    /// </summary>
    [JsonPropertyName("region")]
    public int Region { get; set; }

    /// <summary>
    /// 观看人数.
    /// </summary>
    [JsonPropertyName("online")]
    public int ViewerCount { get; set; }

    /// <summary>
    /// 直播分区名称.
    /// </summary>
    [JsonPropertyName("area_v2_name")]
    public string AreaName { get; set; }

    /// <summary>
    /// 徽章文本.
    /// </summary>
    [JsonPropertyName("badge")]
    public string BadgeText { get; set; }

    /// <summary>
    /// 直播网址.
    /// </summary>
    [JsonPropertyName("live_link")]
    public string LiveLink { get; set; }
}

