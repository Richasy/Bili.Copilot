// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 发出消息的用户.
/// </summary>
public class MessageUser
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
    public string Avatar { get; set; }

    /// <summary>
    /// 是否关注了该用户.
    /// </summary>
    [JsonPropertyName("follow")]
    public bool IsFollow { get; set; }
}

