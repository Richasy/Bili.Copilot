// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Data.User;

namespace Bili.Copilot.Models.Data.Community;

/// <summary>
/// 聊天会话.
/// </summary>
public sealed class ChatSession
{
    /// <summary>
    /// 用户信息.
    /// </summary>
    public ContactProfile Profile { get; set; }

    /// <summary>
    /// 是否为关注对象.
    /// </summary>
    public bool IsFollow { get; set; }

    /// <summary>
    /// 是否免打扰.
    /// </summary>
    public bool DoNotDisturb { get; set; }

    /// <summary>
    /// 会话时间戳.
    /// </summary>
    public long Timestamp { get; set; }

    /// <summary>
    /// 未读消息数.
    /// </summary>
    public int UnreadCount { get; set; }

    /// <summary>
    /// 最后一条消息内容.
    /// </summary>
    public string LastMessage { get; set; }
}
