﻿// Copyright (c) Richasy. All rights reserved.

namespace Richasy.BiliKernel.Models.User;

/// <summary>
/// 未读信息.
/// </summary>
public sealed class UnreadInformation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnreadInformation"/> class.
    /// </summary>
    /// <param name="atCount">提到的次数.</param>
    /// <param name="replyCount">回复的次数.</param>
    /// <param name="likeCount">点赞的次数.</param>
    /// <param name="chatCount">聊天未读消息数.</param>
    public UnreadInformation(
        int atCount,
        int replyCount,
        int likeCount,
        int chatCount)
    {
        AtCount = atCount;
        ReplyCount = replyCount;
        LikeCount = likeCount;
        ChatCount = chatCount;
    }

    /// <summary>
    /// 提到的次数.
    /// </summary>
    public int AtCount { get; }

    /// <summary>
    /// 回复的次数.
    /// </summary>
    public int ReplyCount { get; }

    /// <summary>
    /// 点赞的次数.
    /// </summary>
    public int LikeCount { get; }

    /// <summary>
    /// 对话消息数.
    /// </summary>
    public int ChatCount { get; }

    /// <summary>
    /// 未读消息总数.
    /// </summary>
    public int Total => AtCount + LikeCount + ReplyCount + ChatCount;
}
