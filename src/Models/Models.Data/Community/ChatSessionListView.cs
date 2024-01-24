// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.Data.Community;

/// <summary>
/// 聊天会话视图.
/// </summary>
public sealed class ChatSessionListView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatSessionListView"/> class.
    /// </summary>
    public ChatSessionListView(List<ChatSession> sessions, bool hasMore)
    {
        Sessions = sessions;
        HasMore = hasMore;
    }

    /// <summary>
    /// 会话列表.
    /// </summary>
    public List<ChatSession> Sessions { get; set; }

    /// <summary>
    /// 是否还有更多.
    /// </summary>
    public bool HasMore { get; set; }
}
