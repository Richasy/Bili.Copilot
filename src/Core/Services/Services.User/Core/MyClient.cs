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
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Article;
using Richasy.BiliKernel.Models.Media;
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

    public async Task<(IReadOnlyList<VideoFavoriteFolderGroup>, VideoFavoriteFolderDetail)> GetVideoFavoriteGroupsAsync(CancellationToken cancellationToken)
    {
        var localToken = _tokenResolver.GetToken();
        var userId = localToken.UserId.ToString();
        var parameters = new Dictionary<string, string>
        {
            { "up_mid", userId },
        };

        var galleryResponse = await GetAsync<BiliDataResponse<VideoFavoriteGalleryResponse>>(BiliApis.Account.VideoFavoriteGallery, parameters, cancellationToken).ConfigureAwait(false);

        parameters = new Dictionary<string, string>
        {
            { "up_mid", userId },
            { "type", "2" },
        };
        var defaultFolderResponse = await GetAsync<BiliDataResponse<FavoriteDetailListResponse>>(BiliApis.Account.FavoriteList, parameters, cancellationToken).ConfigureAwait(false);

        parameters = new Dictionary<string, string>
        {
            { "up_mid", userId },
            { "ps", "20" },
            { "pn", "1" },
        };
        var folderRequest = BiliHttpClient.CreateRequest(HttpMethod.Get, new Uri(BiliApis.Account.CollectList));
        _authenticator.AuthroizeRestRequest(folderRequest, parameters, new BasicAuthorizeExecutionSettings() { ApiType = BiliApiType.Web, RequireCookie = true, NeedRID = true, ForceNoToken = true, });
        var folderResponse = _httpClient.SendAsync(folderRequest, cancellationToken).ConfigureAwait(false);
        var folderResponseObj = await BiliHttpClient.ParseAsync<BiliDataResponse<FavoriteDetailListResponse>>(await folderResponse).ConfigureAwait(false);
        var defaultFolder = galleryResponse.Data.DefaultFavoriteList.ToVideoFavoriteFolderDetail();
        var mineCreateFav = galleryResponse.Data.FavoriteFolderList.FirstOrDefault(ff => ff.Id == 1);

        if (mineCreateFav is not null)
        {
            mineCreateFav.MediaList.List = defaultFolderResponse.Data.List.Where(fm => fm.Title != "默认收藏夹").ToList();
        }

        if (folderResponseObj.Data?.Count > 0)
        {
            var myCollectFav = galleryResponse.Data.FavoriteFolderList.FirstOrDefault(f => f.Id == 2);
            myCollectFav.MediaList.List = folderResponseObj.Data.List;
        }

        var favoriteSets = galleryResponse.Data.FavoriteFolderList?
            .Where(p => p.Id != 3)
            .Select(p => p.ToVideoFavoriteFolderGroup());
        return (favoriteSets.ToList().AsReadOnly(), defaultFolder);
    }

    public async Task<(IReadOnlyList<ArticleInformation>, int)> GetFavoritesArticlesAsync(int pageNumber, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "pn", pageNumber.ToString() },
            { "ps", "40" },
        };

        var responseObj = await GetAsync<BiliDataResponse<ArticleFavoriteListResponse>>(BiliApis.Account.ArticleFavorite, parameters, cancellationToken).ConfigureAwait(false);
        var articles = responseObj.Data?.Items.Select(p => p.ToArticleInformation()).ToList().AsReadOnly()
            ?? throw new KernelException("无法获取用户收藏的文章数据");
        return (articles, responseObj.Data.Count);
    }

    public async Task<(IReadOnlyList<SeasonInformation>, int)> GetFavoritePgcListAsync(string url, int pageNumber, PgcFavoriteStatus status, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "pn", pageNumber.ToString() },
            { "ps", "20" },
            { "status", ((int)status).ToString() },
        };

        var responseObj = await GetAsync<BiliResultResponse<PgcFavoriteListResponse>>(url, parameters, cancellationToken).ConfigureAwait(false);
        var seasons = responseObj.Result.FollowList.Select(p => p.ToSeasonInformation()).ToList().AsReadOnly();
        return (seasons, responseObj.Result.Total ?? 0);
    }

    public async Task<(IReadOnlyList<VideoFavoriteFolder> Folders, IReadOnlyList<string> ContainerIds)> GetPlayingVideoFavoriteFoldersAsync(VideoInformation video, CancellationToken cancellationToken)
    {
        var localToken = _tokenResolver.GetToken();
        var myId = localToken.UserId.ToString();
        var parameters = new Dictionary<string, string>
        {
            { "up_mid", myId },
            { "type", "2" },
            { "rid", video.Identifier.Id },
        };

        var responseObj = await GetAsync<BiliDataResponse<FavoriteListResponse>>(BiliApis.Account.FavoriteList, parameters, cancellationToken).ConfigureAwait(false);
        var items = responseObj.Data.List.Select(p => p.ToVideoFavoriteFolder());
        var ids = responseObj.Data.List.Where(p => p.FavoriteState == 1).Select(p => p.Id.ToString()).ToList().AsReadOnly();
        return (items.ToList().AsReadOnly(), ids);
    }

    public async Task<VideoFavoriteFolderDetail> GetVideoFavoriteFolderDetailAsync(VideoFavoriteFolder folder, int pageNumber, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "media_id", folder.Id },
            { "ps", "20" },
            { "pn", pageNumber.ToString() },
        };

        var responseObj = await GetAsync<BiliDataResponse<VideoFavoriteListResponse>>(BiliApis.Account.VideoFavoriteDelta, parameters, cancellationToken).ConfigureAwait(false);
        return responseObj.Data.ToVideoFavoriteFolderDetail();
    }

    public async Task UpdatePgcFavoriteStatusAsync(MediaIdentifier identifier, PgcFavoriteStatus status, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "season_id", identifier.Id },
            { "status", ((int)status).ToString() },
        };

        await _authenticationService.EnsureTokenAsync(cancellationToken).ConfigureAwait(false);
        var request = BiliHttpClient.CreateRequest(HttpMethod.Post, new Uri(BiliApis.Account.UpdatePgcStatus));
        _authenticator.AuthroizeRestRequest(request, parameters);
        await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task RemoveFavoriteVideoAsync(VideoFavoriteFolder folder, MediaIdentifier identifier, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "media_id", folder.Id },
            { "resources", $"{identifier.Id}:2" },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Post, new Uri(BiliApis.Account.UnFavoriteVideo));
        _authenticator.AuthroizeRestRequest(request, parameters);
        await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task RemoveFavoritePgcAsync(MediaIdentifier identifier, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "season_id", identifier.Id },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Post, new Uri(BiliApis.Account.UnFavoritePgc));
        _authenticator.AuthroizeRestRequest(request, parameters);
        await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task RemoveFavoriteArticleAsync(ArticleIdentifier article, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "id", article.Id },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Post, new Uri(BiliApis.Account.UnFavoriteArticle));
        _authenticator.AuthroizeRestRequest(request, parameters);
        await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
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
