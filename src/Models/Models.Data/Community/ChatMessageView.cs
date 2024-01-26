// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.Data.Community;

/// <summary>
/// 聊天消息视图.
/// </summary>
public sealed class ChatMessageView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatMessageView"/> class.
    /// </summary>
    public ChatMessageView(List<ChatMessage> messages, bool hasMore)
    {
        Messages = messages;
        HasMore = hasMore;
    }

    /// <summary>
    /// 消息列表.
    /// </summary>
    public List<ChatMessage> Messages { get; set; }

    /// <summary>
    /// 是否有更多消息.
    /// </summary>
    public bool HasMore { get; set; }
}
