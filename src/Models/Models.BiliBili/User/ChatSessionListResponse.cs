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
/// 消息列表响应.
/// </summary>
public class ChatMessageResponse
{
    /// <summary>
    /// 消息列表.
    /// </summary>
    [JsonPropertyName("messages")]
    public List<BiliChatMsg> MessageList { get; set; }

    /// <summary>
    /// 是否有更多消息.
    /// </summary>
    [JsonPropertyName("has_more")]
    public int HasMore { get; set; }

    /// <summary>
    /// 最小消息序号.
    /// </summary>
    [JsonPropertyName("min_seqno")]
    public long MinSeqNo { get; set; }

    /// <summary>
    /// 最大消息序号.
    /// </summary>
    [JsonPropertyName("max_seqno")]
    public long MaxSeqNo { get; set; }

    /// <summary>
    /// 表情信息.
    /// </summary>
    [JsonPropertyName("e_infos")]
    public List<ChatEmoteInfo> EmoteInfos { get; set; }
}

/// <summary>
/// 发送消息的响应.
/// </summary>
public class SendMessageResponse
{
    /// <summary>
    /// 消息内容.
    /// </summary>
    [JsonPropertyName("msg_content")]
    public string Content { get; set; }

    /// <summary>
    /// 表情信息.
    /// </summary>
    [JsonPropertyName("e_infos")]
    public List<ChatEmoteInfo> EmoteInfos { get; set; }

    /// <summary>
    /// 消息标识符.
    /// </summary>
    [JsonPropertyName("msg_key")]
    public long Key { get; set; }
}

/// <summary>
/// 表情信息.
/// </summary>
public class ChatEmoteInfo
{
    /// <summary>
    /// 文本.
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; }

    /// <summary>
    /// 地址.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; }
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

    /// <summary>
    /// 消息类型.
    /// </summary>
    [JsonPropertyName("msg_type")]
    public int Type { get; set; }
}
