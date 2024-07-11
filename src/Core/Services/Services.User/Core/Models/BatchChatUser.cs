// Copyright (c) Richasy. All rights reserved.

using System.Text.Json.Serialization;

namespace Richasy.BiliKernel.Services.User.Core;

/// <summary>
/// 聊天用户.
/// </summary>
internal sealed class BiliChatUser : PublisherInfo
{
    /// <summary>
    /// 等级.
    /// </summary>
    [JsonPropertyName("level")]
    public int? Level { get; set; }

    /// <summary>
    /// 性别.
    /// </summary>
    [JsonPropertyName("sex")]
    public string? Sex { get; set; }

    /// <summary>
    /// 是否为会员.
    /// </summary>
    [JsonPropertyName("vip")]
    public Vip? Vip { get; set; }
}
