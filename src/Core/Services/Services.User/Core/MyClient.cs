// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Content;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models.User;

namespace Richasy.BiliKernel.Services.User.Core;

internal sealed class MyClient
{
    private readonly BiliHttpClient _httpClient;
    private readonly IAuthenticationService _authenticationService;
    private readonly IBiliTokenResolver _tokenResolver;
    private readonly BasicAuthenticator _authenticator;

    public MyClient(
        BiliHttpClient httpClient,
        IAuthenticationService authenticationService,
        IBiliTokenResolver tokenResolver,
        BasicAuthenticator basicAuthenticator)
    {
        _httpClient = httpClient;
        _authenticationService = authenticationService;
        _tokenResolver = tokenResolver;
        _authenticator = basicAuthenticator;
    }

    public async Task<UserDetailProfile> GetMyInformationAsync(CancellationToken cancellationToken)
    {
        var responseObj = await GetAsync<BiliDataResponse<MyInfo>>(BiliApis.Account.MyInfo, cancellationToken: cancellationToken).ConfigureAwait(false);
        var info = responseObj.Data;
        return info == null || string.IsNullOrEmpty(info.Name)
            ? throw new KernelException("返回的用户数据为空")
            : info.ToUserDetailProfile(48d);
    }

    public async Task<UserCommunityInformation> GetMyCommunityInformationAsync(CancellationToken cancellationToken)
    {
        var responseObj = await GetAsync<BiliDataResponse<Mine>>(BiliApis.Account.Mine, cancellationToken: cancellationToken).ConfigureAwait(false);
        var mine = responseObj.Data;
        return mine == null || string.IsNullOrEmpty(mine.Name)
            ? throw new KernelException("无法获取用户社区数据")
            : mine.ToUserCommunityInformation();
    }

    public async Task<IReadOnlyList<UserGroup>> GetMyFollowUserGroupsAsync(CancellationToken cancellationToken)
    {
        var responseObj = await GetAsync<BiliDataResponse<List<RelatedTag>>>(BiliApis.Account.MyFollowingTags, cancellationToken: cancellationToken).ConfigureAwait(false);
        return responseObj.Data?.Select(p => p.ToUserGroup()).ToList().AsReadOnly()
            ?? throw new KernelException("无法获取用户关注的分组数据");
    }

    public async Task<IReadOnlyList<UserCard>> GetMyFollowUserGroupDetailAsync(string groupId, int page = 1, CancellationToken cancellationToken = default)
    {
        var localToken = _tokenResolver.GetToken();
        var parameters = new Dictionary<string, string>
        {
            { "tagid", groupId },
            { "pn", page.ToString() },
            { "mid", localToken?.UserId.ToString() ?? string.Empty },
        };

        var responseObj = await GetAsync<BiliDataResponse<List<RelatedUser>>>(BiliApis.Account.MyFollowingTagDetail, parameters, cancellationToken).ConfigureAwait(false);
        var users = responseObj.Data?.Select(p => p.ToUserCard()).ToList().AsReadOnly()
            ?? throw new KernelException("无法获取用户关注的分组详情数据");
        foreach (var user in users)
        {
            user.Community.Relation = UserRelationStatus.Following;
        }

        return users;
    }

    public async Task<(IReadOnlyList<UserCard> Users, int Count)> GetMyFansAsync(int page = 1, CancellationToken cancellationToken = default)
    {
        var localToken = _tokenResolver.GetToken();
        var parameters = new Dictionary<string, string>
        {
            { "pn", page.ToString() },
            { "vmid", localToken.UserId.ToString() },
        };

        var responseObj = await GetAsync<BiliDataResponse<RelatedUserResponse>>(BiliApis.Account.Fans, parameters, cancellationToken).ConfigureAwait(false);
        var users = responseObj.Data?.UserList?.Select(p => p.ToUserCard()).ToList().AsReadOnly()
            ?? throw new KernelException("无法获取用户粉丝数据");
        return (users, responseObj.Data.TotalCount);
    }

    public async Task ModifyRelationshipAsync(string userId, bool isFollow, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "fid", userId },
            { "act", isFollow ? "1" : "2" },
        };

        await _authenticationService.EnsureTokenAsync(cancellationToken).ConfigureAwait(false);
        var request = BiliHttpClient.CreateRequest(HttpMethod.Post, new Uri(BiliApis.Account.ModifyRelation));
        _authenticator.AuthroizeRestRequest(request, parameters);
        await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task<UserRelationStatus> GetRelationshipAsync(string userId, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "fid", userId },
        };

        var responseObj = await GetAsync<BiliDataResponse<UserRelationResponse>>(BiliApis.Account.Relation, parameters, cancellationToken).ConfigureAwait(false);
        return responseObj.Data?.ToUserRelationStatus()
            ?? throw new KernelException("无法获取用户关系数据");
    }

    public async Task<UnreadInformation> GetUnreadInformationAsync(CancellationToken cancellationToken)
    {
        var responseObj = await GetAsync<BiliDataResponse<UnreadMessage>>(BiliApis.Account.MessageUnread, cancellationToken: cancellationToken).ConfigureAwait(false);
        return new(responseObj.Data.At, responseObj.Data.Reply, responseObj.Data.Like, responseObj.Data.Chat);
    }

    public async Task<(IReadOnlyList<ChatMessage> Messages, long MaxNumber, bool HasMore)> GetChatMessagesAsync(UserProfile user, int messageCount, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "sender_device_id", "1" },
            { "talker_id", user.Id },
            { "session_type", "1" },
            { "size", messageCount.ToString() },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new Uri(BiliApis.Account.ChatMessages));
        _authenticator.AuthroizeRestRequest(request, parameters, new BasicAuthorizeExecutionSettings { ForceNoToken = true, NeedRID = true, RequireCookie = true });
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync<BiliDataResponse<ChatMessageResponse>>(response).ConfigureAwait(false);
        var messages = responseObj.Data?.MessageList.Select(p => p.ToChatMessage(responseObj.Data.EmoteInfos)).ToList().AsReadOnly()
            ?? throw new KernelException("无法获取用户聊天消息数据");
        return (messages, responseObj.Data.MaxSeqNo, responseObj.Data.HasMore == 1);
    }

    public async Task MarkChatMessagesAsReadAsync(UserProfile user, long maxSeqNo, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "talker_id", user.Id },
            { "session_type", "1" },
            { "ack_seqno", maxSeqNo.ToString() },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Post, new Uri(BiliApis.Account.ChatUpdate));
        _authenticator.AuthroizeRestRequest(request, parameters, new BasicAuthorizeExecutionSettings { ForceNoToken = true, NeedRID = true, RequireCookie = true });
        await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task<ChatMessage> SendChatMessageAsync(string content, UserProfile user, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.Now.ToUnixTimeSeconds();
        var localToken = _tokenResolver.GetToken();
        var parameters = new Dictionary<string, string>
        {
            { "msg[msg_type]", "1" },
            { "msg[content]", JsonSerializer.Serialize(new { content }) },
            { "msg[sender_uid]", localToken.UserId.ToString() },
            { "msg[receiver_id]", user.Id },
            { "msg[receiver_type]", "1" },
            { "msg[receiver_device_id]", "1" },
            { "msg[timestamp]", now.ToString() },
            { "msg[msg_status]", "0" },
            { "msg[dev_id]", "0" },
            { "msg[new_face_version]", "1" },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Post, new Uri(BiliApis.Account.SendMessage));
        _authenticator.AuthroizeRestRequest(request, parameters, new BasicAuthorizeExecutionSettings { ForceNoToken = true, NeedRID = true, RequireCookie = true, NeedCSRF = true, });
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync<BiliDataResponse<SendMessageResponse>>(response).ConfigureAwait(false);
        var msg = responseObj.Data?.ToChatMessage()
            ?? throw new KernelException("无法发送聊天消息");
        msg.Time = DateTimeOffset.FromUnixTimeSeconds(now).ToLocalTime();
        msg.SenderId = localToken.UserId.ToString();
        return msg;
    }

    public async Task<(IReadOnlyList<ChatSession> Sessions, bool HasNext)> GetChatSessionsAsync(DateTimeOffset? lastSessionTime, CancellationToken cancellationToken)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { "session_type", "1" },
            { "group_fold", "1" },
            { "sort_rule", "2" },
            { "end_ts", (lastSessionTime?.ToUnixTimeSeconds() ?? 0).ToString() },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new Uri(BiliApis.Account.ChatSessions));
        _authenticator.AuthroizeRestRequest(request, queryParameters, new BasicAuthorizeExecutionSettings { ForceNoToken = true, NeedRID = true, RequireCookie = true });
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync<BiliDataResponse<ChatSessionListResponse>>(response).ConfigureAwait(false);
        var sessions = responseObj.Data.SessionList.Where(p => p.SystemMessageType == 0 && p.LastMessage != null).ToList();

        // 批量获取会话的用户信息
        var userIds = sessions.Select(p => p.TalkerId.ToString()).Distinct().ToList();
        var users = await BatchGetUsersAsync(userIds, cancellationToken).ConfigureAwait(false);

        var chatSessions = sessions.Where(p => users.Any(j => j.Id == p.TalkerId.ToString())).Select(p =>
        {
            var user = users.FirstOrDefault(q => q.Id == p.TalkerId.ToString());
            return p.ToChatSession(user);
        }).ToList().AsReadOnly();
        return (chatSessions, responseObj.Data.HasMore == 1);
    }

    public async Task<IReadOnlyList<UserProfile>> BatchGetUsersAsync(IEnumerable<string> userIds, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "uids", string.Join(",", userIds) },
        };

        var userRequest = BiliHttpClient.CreateRequest(HttpMethod.Get, new Uri(BiliApis.Account.BatchUserInfo));
        _authenticator.AuthroizeRestRequest(userRequest, parameters, new BasicAuthorizeExecutionSettings { ForceNoToken = true, NeedRID = true, RequireCookie = true, ApiType = BiliKernel.Models.BiliApiType.Web });
        var userResponse = await _httpClient.SendAsync(userRequest, cancellationToken).ConfigureAwait(false);
        var userResponseObj = await BiliHttpClient.ParseAsync<BiliDataResponse<List<BiliChatUser>>>(userResponse).ConfigureAwait(false);
        return userResponseObj.Data?.Select(p => p.ToUserProfile()).ToList().AsReadOnly()
            ?? throw new KernelException("无法获取用户信息");
    }

    public async Task<(IReadOnlyList<NotifyMessage> Messages, long OffsetId, long OffsetTime, bool HasMore)> GetNotifyMessagesAsync(NotifyMessageType type, long offsetId, long offsetTime, CancellationToken cancellationToken)
    {
        var timeName = type.ToString().ToLower() + "_time";
        var url = type switch
        {
            NotifyMessageType.Like => BiliApis.Account.MessageLike,
            NotifyMessageType.At => BiliApis.Account.MessageAt,
            NotifyMessageType.Reply => BiliApis.Account.MessageReply,
            _ => string.Empty,
        };

        var parameters = new Dictionary<string, string>
        {
            { "id", offsetId.ToString() },
            { timeName, offsetTime.ToString() },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new Uri(url));
        _authenticator.AuthroizeRestRequest(request, parameters);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        MessageCursor cursor = default;
        IReadOnlyList<NotifyMessage> messages = default;
        if (type == NotifyMessageType.Like)
        {
            var responseObj = await BiliHttpClient.ParseAsync<BiliDataResponse<LikeMessageResponse>>(response).ConfigureAwait(false);
            cursor = responseObj.Data.Total.Cursor;
            messages = responseObj.Data.ToNotifyMessages();
        }
        else if (type == NotifyMessageType.Reply)
        {
            var responseObj = await BiliHttpClient.ParseAsync<BiliDataResponse<ReplyMessageResponse>>(response).ConfigureAwait(false);
            cursor = responseObj.Data.Cursor;
            messages = responseObj.Data.ToNotifyMessages();
        }
        else if (type == NotifyMessageType.At)
        {
            var responseObj = await BiliHttpClient.ParseAsync<BiliDataResponse<AtMessageResponse>>(response).ConfigureAwait(false);
            cursor = responseObj.Data.Cursor;
            messages = responseObj.Data.ToNotifyMessages();
        }

        return messages is null || messages.Count == 0
            ? throw new KernelException($"无法获取 {type} 消息")
            : ((IReadOnlyList<NotifyMessage> Messages, long OffsetId, long OffsetTime, bool HasMore))(messages!, cursor.Id, cursor.Time, !cursor.IsEnd);
    }

    private async Task<T> GetAsync<T>(string url, Dictionary<string, string>? paramters = default, CancellationToken cancellationToken = default)
    {
        await _authenticationService.EnsureTokenAsync(cancellationToken).ConfigureAwait(false);
        var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new Uri(url));
        _authenticator.AuthroizeRestRequest(request, paramters);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        return await BiliHttpClient.ParseAsync<T>(response).ConfigureAwait(false);
    }
}
