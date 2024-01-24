// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili.User;

/// <summary>
/// 会话列表响应.
/// </summary>
public class ChatSessionListResponse
{
    /// <summary>
    /// 会话列表.
    /// </summary>
    [JsonPropertyName("session_list")]
    public List<BiliChatSession> SessionList { get; set; }

    /// <summary>
    /// 是否有更多消息.
    /// </summary>
    [JsonPropertyName("has_more")]
    public int HasMore { get; set; }
}

/// <summary>
/// 会话.
/// </summary>
public class BiliChatSession
{
    /// <summary>
    /// 发起者 Id.
    /// </summary>
    [JsonPropertyName("talker_id")]
    public long TalkerId { get; set; }

    /// <summary>
    /// 会话类型.
    /// </summary>
    [JsonPropertyName("session_type")]
    public int SessionType { get; set; }

    /// <summary>
    /// 是否关注（0：未关注，1：已关注）.
    /// </summary>
    [JsonPropertyName("is_follow")]
    public int IsFollow { get; set; }

    /// <summary>
    /// 是否免打扰（0：否，1：是）.
    /// </summary>
    [JsonPropertyName("is_dnd")]
    public int DoNotDisturb { get; set; }

    /// <summary>
    /// 会话时间戳.
    /// </summary>
    [JsonPropertyName("session_ts")]
    public long SessionTimestamp { get; set; }

    /// <summary>
    /// 未读消息数.
    /// </summary>
    [JsonPropertyName("unread_count")]
    public int UnreadCount { get; set; }

    /// <summary>
    /// 最后一条消息.
    /// </summary>
    [JsonPropertyName("last_msg")]
    public BiliChatMsg LastMessage { get; set; }

    /// <summary>
    /// 系统消息类型（如果是用户消息则为0）.
    /// </summary>
    [JsonPropertyName("system_msg_type")]
    public int SystemMessageType { get; set; }
}

/// <summary>
/// 聊天消息.
/// </summary>
public class BiliChatMsg
{
    /// <summary>
    /// 发送方 Id.
    /// </summary>
    [JsonPropertyName("sender_uid")]
    public long SenderUid { get; set; }

    /// <summary>
    /// 接收方 Id.
    /// </summary>
    [JsonPropertyName("receiver_id")]
    public int ReceiverId { get; set; }

    /// <summary>
    /// 内容.
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; }

    /// <summary>
    /// 时间戳.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }

    /// <summary>
    /// 消息标识符.
    /// </summary>
    [JsonPropertyName("msg_key")]
    public long Key { get; set; }
}
