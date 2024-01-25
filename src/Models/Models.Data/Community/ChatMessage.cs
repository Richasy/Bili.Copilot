﻿// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Constants.Bili;

namespace Bili.Copilot.Models.Data.Community;

/// <summary>
/// 聊天消息.
/// </summary>
public sealed class ChatMessage
{
    /// <summary>
    /// 消息内容.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// 消息时间.
    /// </summary>
    public DateTimeOffset Time { get; set; }

    /// <summary>
    /// 消息 Id.
    /// </summary>
    public long Key { get; set; }

    /// <summary>
    /// 发送者 Id.
    /// </summary>
    public string SenderId { get; set; }

    /// <summary>
    /// 消息类型.
    /// </summary>
    public ChatMessageType Type { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is ChatMessage message && Key == message.Key;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Key);
}
