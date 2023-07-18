// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 直播源横幅.
/// </summary>
public class LiveFeedBanner : LiveFeedExtraCardBase
{
    /// <summary>
    /// 内容.
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; }

    /// <summary>
    /// 会话标识符.
    /// </summary>
    [JsonPropertyName("session_id")]
    public string SessionId { get; set; }
}

/// <summary>
/// 直播源横幅列表.
/// </summary>
public class LiveFeedBannerList
{
    /// <summary>
    /// 列表数据.
    /// </summary>
    [JsonPropertyName("list")]
    public List<LiveFeedBanner> List { get; set; }
}

