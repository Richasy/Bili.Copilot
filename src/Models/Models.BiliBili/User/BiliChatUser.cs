// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili.User;

/// <summary>
/// 聊天用户.
/// </summary>
public class BiliChatUser : PublisherInfo
{
    /// <summary>
    /// 等级.
    /// </summary>
    [JsonPropertyName("level")]
    public int Level { get; set; }

    /// <summary>
    /// 性别.
    /// </summary>
    [JsonPropertyName("sex")]
    public string Sex { get; set; }

    /// <summary>
    /// 是否为会员.
    /// </summary>
    [JsonPropertyName("vip")]
    public Vip Vip { get; set; }
}
