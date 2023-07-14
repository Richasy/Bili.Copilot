// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 发布者信息.
/// </summary>
public class PublisherInfo
{
    /// <summary>
    /// 视频发布者的Id.
    /// </summary>
    [JsonPropertyName("mid")]
    public long Mid { get; set; }

    /// <summary>
    /// 视频发布者.
    /// </summary>
    [JsonPropertyName("name")]
    public string Publisher { get; set; }

    /// <summary>
    /// 视频发布者的头像.
    /// </summary>
    [JsonPropertyName("face")]
    public string PublisherAvatar { get; set; }
}

