// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;

namespace Richasy.BiliKernel.Models.User;

/// <summary>
/// 聊天会话.
/// </summary>
public sealed class ChatSession
{
    /// <summary>
    /// 用户信息.
    /// </summary>
    public UserProfile User { get; set; }

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
    public DateTimeOffset Time { get; set; }

    /// <summary>
    /// 未读消息数.
    /// </summary>
    public int UnreadCount { get; set; }

    /// <summary>
    /// 最后一条消息内容.
    /// </summary>
    public string? LastMessage { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ChatSession session && EqualityComparer<UserProfile>.Default.Equals(User, session.User);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(User);
}
