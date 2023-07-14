// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 未读消息情况.
/// </summary>
public class UnreadMessage
{
    /// <summary>
    /// @我的.
    /// </summary>
    [JsonPropertyName("at")]
    public int At { get; set; }

    /// <summary>
    /// 聊天消息.
    /// </summary>
    [JsonPropertyName("chat")]
    public int Chat { get; set; }

    /// <summary>
    /// 点赞.
    /// </summary>
    [JsonPropertyName("like")]
    public int Like { get; set; }

    /// <summary>
    /// 回复.
    /// </summary>
    [JsonPropertyName("reply")]
    public int Reply { get; set; }

    /// <summary>
    /// 系统消息.
    /// </summary>
    [JsonPropertyName("sys_msg")]
    public int SystemMessage { get; set; }
}

