// Copyright (c) Richasy. All rights reserved.

using System.Text.Json.Serialization;

namespace Richasy.BiliKernel.Services.User.Core;

/// <summary>
/// 发布者信息.
/// </summary>
internal class PublisherInfo
{
    /// <summary>
    /// 视频发布者的Id.
    /// </summary>
    [JsonPropertyName("mid")]
    public long UserId { get; set; }

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
