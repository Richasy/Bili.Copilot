// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models.User;
using Richasy.BiliKernel.Services.User.Core;

namespace Richasy.BiliKernel.Services.User;

/// <summary>
/// 消息服务.
/// </summary>
public sealed class MessageService : IMessageService
{
    private readonly MyClient _myClient;

    /// <summary>
    /// 初始化 <see cref="MyProfileService"/> 类的新实例.
    /// </summary>
    public MessageService(
        BiliHttpClient biliHttpClient,
        IAuthenticationService authenticationService,
        IBiliTokenResolver tokenResolver,
        BasicAuthenticator basicAuthenticator)
    {
        _myClient = new MyClient(biliHttpClient, authenticationService, tokenResolver, basicAuthenticator);
    }

    /// <inheritdoc/>
    public Task<(IReadOnlyList<ChatMessage> Messages, long MaxNumber, bool HasMore)> GetChatMessagesAsync(UserProfile user, int messageCount = 100, CancellationToken cancellationToken = default)
    {
        return messageCount <= 0
            ? throw new ArgumentOutOfRangeException(nameof(messageCount))
            : _myClient.GetChatMessagesAsync(user, messageCount, cancellationToken);
    }

    /// <inheritdoc/>
    public Task MarkChatMessagesAsReadAsync(UserProfile user, long maxNumber, CancellationToken cancellationToken = default)
        => _myClient.MarkChatMessagesAsReadAsync(user, maxNumber, cancellationToken);

    /// <inheritdoc/>
    public Task<(IReadOnlyList<ChatSession> Sessions, bool HasNext)> GetChatSessionsAsync(DateTimeOffset? lastMessageTime = null, CancellationToken cancellationToken = default)
        => _myClient.GetChatSessionsAsync(lastMessageTime, cancellationToken);

    /// <inheritdoc/>
    public Task<(IReadOnlyList<NotifyMessage> Messages, long OffsetId, long OffsetTime, bool HasMore)> GetNotifyMessagesAsync(NotifyMessageType type, long offsetId = 0, long offsetTime = 0, CancellationToken cancellationToken = default)
        => _myClient.GetNotifyMessagesAsync(type, offsetId, offsetTime, cancellationToken);

    /// <inheritdoc/>
    public Task<UnreadInformation> GetUnreadInformationAsync(CancellationToken cancellationToken = default)
        => _myClient.GetUnreadInformationAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<ChatMessage> SendChatMessageAsync(string content, UserProfile user, CancellationToken cancellationToken = default)
        => _myClient.SendChatMessageAsync(content, user, cancellationToken);
}
