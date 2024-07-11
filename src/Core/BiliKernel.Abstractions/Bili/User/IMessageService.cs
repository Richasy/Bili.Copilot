// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Models.User;

namespace Richasy.BiliKernel.Bili.User;

/// <summary>
/// 消息服务.
/// </summary>
public interface IMessageService
{
    /// <summary>
    /// 获取未读消息数.
    /// </summary>
    Task<UnreadInformation> GetUnreadInformationAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取聊天会话列表.
    /// </summary>
    /// <remarks>
    /// 在第二次请求会话列表时，需要传入上一次请求的最早的一个会话的时间，以便获取更早的会话列表.
    /// </remarks>
    Task<(IReadOnlyList<ChatSession> Sessions, bool HasNext)> GetChatSessionsAsync(DateTimeOffset? lastSessionTime = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取和指定用户的聊天消息（可以指定数目）.
    /// </summary>
    Task<(IReadOnlyList<ChatMessage> Messages,long MaxNumber, bool HasMore)> GetChatMessagesAsync(UserProfile user, int messageCount = 100, CancellationToken cancellationToken = default);

    /// <summary>
    /// 标记聊天消息为已读.
    /// </summary>
    /// <remarks>
    /// 最大消息序号 <paramref name="maxNumber"/> 可以从 <see cref="GetChatMessagesAsync(UserProfile, int, CancellationToken)"/> 方法的返回值中获取.
    /// </remarks>
    Task MarkChatMessagesAsReadAsync(UserProfile user, long maxNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送聊天消息.
    /// </summary>
    Task<ChatMessage> SendChatMessageAsync(string content, UserProfile user, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取通知消息.
    /// </summary>
    Task<(IReadOnlyList<NotifyMessage> Messages, long OffsetId, long OffsetTime, bool HasMore)> GetNotifyMessagesAsync(NotifyMessageType type, long offsetId = 0, long offsetTime = 0, CancellationToken cancellationToken = default);
}
