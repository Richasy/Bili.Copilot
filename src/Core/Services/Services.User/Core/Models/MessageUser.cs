// Copyright (c) Richasy. All rights reserved.

using System.Text.Json.Serialization;

namespace Richasy.BiliKernel.Services.User.Core;

/// <summary>
/// 发出消息的用户.
/// </summary>
internal sealed class MessageUser
{
    /// <summary>
    /// 用户ID.
    /// </summary>
    [JsonPropertyName("mid")]
    public long UserId { get; set; }

    /// <summary>
    /// 是否为粉丝，0-不是，1-是.
    /// </summary>
    [JsonPropertyName("fans")]
    public int IsFans { get; set; }

    /// <summary>
    /// 用户名.
    /// </summary>
    [JsonPropertyName("nickname")]
    public string UserName { get; set; }

    /// <summary>
    /// 头像.
    /// </summary>
    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }

    /// <summary>
    /// 是否关注了该用户.
    /// </summary>
    [JsonPropertyName("follow")]
    public bool IsFollow { get; set; }
}
