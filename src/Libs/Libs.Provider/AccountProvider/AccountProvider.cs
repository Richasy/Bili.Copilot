// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Adapter;
using Bili.Copilot.Models.BiliBili;
using Bili.Copilot.Models.BiliBili.User;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Authorize;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Constants.Community;
using Bili.Copilot.Models.Data.Community;
using Bili.Copilot.Models.Data.User;
using Bili.Copilot.Models.Data.Video;
using Bilibili.App.Interface.V1;
using static Bili.Copilot.Models.App.Constants.ApiConstants;
using static Bili.Copilot.Models.App.Constants.ServiceConstants;

namespace Bili.Copilot.Libs.Provider;

/// <summary>
/// 提供已登录用户的数据操作.
/// </summary>
public sealed partial class AccountProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AccountProvider"/> class.
    /// </summary>
    /// <param name="httpProvider">网络操作工具.</param>
    /// <param name="userAdapter">用户适配器.</param>
    /// <param name="communityAdapter">社区数据适配器.</param>
    /// <param name="videoAdapter">视频数据适配器.</param>
    private AccountProvider()
    {
        _messageOffsetCache = new Dictionary<MessageType, MessageCursor>();
        _relationOffsetCache = new Dictionary<RelationType, int>();
        _myFollowOffsetCache = new Dictionary<string, int>();
        _spaceVideoOffsets = new Dictionary<string, string>();
        _spaceSearchPageNumbers = new Dictionary<string, int>();
        ResetViewLaterStatus();
        ResetHistoryStatus();
    }

    /// <summary>
    /// 删除指定的历史记录条目.
    /// </summary>
    /// <param name="itemId">条目Id.</param>
    /// <param name="tabSign">标签信息，默认是 <c>archive</c>，表示视频..</param>
    /// <returns>删除是否成功.</returns>
    public static async Task<bool> RemoveHistoryItemAsync(string itemId, string tabSign = "archive")
    {
        var req = new DeleteReq
        {
            HisInfo = new HisInfo
            {
                Business = tabSign,
                Kid = System.Convert.ToInt64(itemId),
            },
        };

        var request = await HttpProvider.GetRequestMessageAsync(Account.DeleteHistoryItem, req, true);
        _ = await HttpProvider.Instance.SendAsync(request);
        return true;
    }

    /// <summary>
    /// 清空历史记录.
    /// </summary>
    /// <param name="tabSign">标签信息，默认是 <c>archive</c>，表示视频..</param>
    /// <returns>清空是否成功.</returns>
    public static async Task<bool> ClearHistoryAsync(string tabSign = "archive")
    {
        var req = new ClearReq() { Business = tabSign };
        var request = await HttpProvider.GetRequestMessageAsync(Account.ClearHistory, req, true);
        _ = await HttpProvider.Instance.SendAsync(request);
        return true;
    }

    /// <summary>
    /// 关注/取消关注用户.
    /// </summary>
    /// <param name="userId">用户Id.</param>
    /// <param name="isFollow">是否关注.</param>
    /// <returns>关注是否成功.</returns>
    public static async Task<bool> ModifyUserRelationAsync(string userId, bool isFollow)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.Fid, userId },
            { Query.ReSrc, "21" },
            { Query.ActionSlim, isFollow ? "1" : "2" },
        };

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Post, Account.ModifyRelation, queryParameters, needToken: true);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse>(response);

        return result.IsSuccess();
    }

    /// <summary>
    /// 清空稍后再看列表.
    /// </summary>
    /// <returns>清除结果.</returns>
    public static async Task<bool> ClearViewLaterAsync()
    {
        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Post, Account.ViewLaterClear, clientType: RequestClientType.IOS, needToken: true);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse>(response);
        return result.IsSuccess();
    }

    /// <summary>
    /// 将视频添加到稍后再看.
    /// </summary>
    /// <param name="videoId">视频Id.</param>
    /// <returns>添加的结果.</returns>
    public static async Task<bool> AddVideoToViewLaterAsync(string videoId)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.Aid, videoId },
        };
        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Post, Account.ViewLaterAdd, queryParameters, RequestClientType.IOS, needToken: true);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse>(response);
        return result.IsSuccess();
    }

    /// <summary>
    /// 将视频从稍后再看中移除.
    /// </summary>
    /// <param name="videoIds">需要移除的视频Id列表.</param>
    /// <returns>移除结果.</returns>
    public static async Task<bool> RemoveVideoFromViewLaterAsync(params string[] videoIds)
    {
        var ids = string.Join(",", videoIds);
        var queryParameters = new Dictionary<string, string>
        {
            { Query.Aid, ids },
        };
        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Post, Account.ViewLaterDelete, queryParameters, RequestClientType.IOS, needToken: true);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse>(response);
        return result.IsSuccess();
    }

    /// <summary>
    /// 获取未读消息.
    /// </summary>
    /// <returns>未读消息.</returns>
    public static async Task<UnreadInformation> GetUnreadMessageAsync()
    {
        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Account.MessageUnread, null, RequestClientType.IOS, true);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse<UnreadMessage>>(response);
        return CommunityAdapter.ConvertToUnreadInformation(result.Data);
    }

    /// <summary>
    /// 查询我与某用户之间的关系（是否关注）.
    /// </summary>
    /// <param name="targetUserId">目标用户Id.</param>
    /// <returns>是否关注.</returns>
    public static async Task<UserRelationStatus> GetRelationAsync(string targetUserId)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.Fid, targetUserId.ToString() },
        };

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Account.Relation, queryParameters, RequestClientType.IOS, true);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse<UserRelationResponse>>(response);
        return result.Data.Type switch
        {
            2 => UserRelationStatus.Following,
            6 => UserRelationStatus.Friends,
            _ => UserRelationStatus.Unfollow,
        };
    }

    /// <summary>
    /// 获取我的关注分组.
    /// </summary>
    /// <returns>分组列表.</returns>
    public static async Task<IEnumerable<FollowGroup>> GetMyFollowingGroupsAsync()
    {
        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Account.MyFollowingTags, null, RequestClientType.IOS, true);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse<List<RelatedTag>>>(response);
        return result.Data.Select(CommunityAdapter.ConvertToFollowGroup);
    }

    /// <summary>
    /// 获取聊天消息.
    /// </summary>
    /// <param name="userId">用户 Id.</param>
    /// <returns>消息列表.</returns>
    public static async Task<ChatMessageView> GetChatMessagesAsync(string userId)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { "sender_device_id", "1" },
            { "talker_id", userId },
            { "session_type", "1" },
            { "size", "100" },
            { "wts", DateTimeOffset.Now.ToUnixTimeSeconds().ToString() },
        };

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Account.ChatMessages, queryParameters);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse<ChatMessageResponse>>(response);
        var view = CommunityAdapter.ConvertToChatMessageView(result.Data);
        return view;
    }

    /// <summary>
    /// 获取已登录用户的个人资料.
    /// </summary>
    /// <returns>个人资料.</returns>
    public async Task<AccountInformation> GetMyInformationAsync()
    {
        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Account.MyInfo, clientType: RequestClientType.IOS, needToken: true);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse<MyInfo>>(response);
        UserId = result.Data.Mid;
        return UserAdapter.ConvertToAccountInformation(result.Data, AvatarSize.Size48);
    }

    /// <summary>
    /// 获取我的基本数据.
    /// </summary>
    /// <returns>个人数据.</returns>
    public async Task<UserCommunityInformation> GetMyCommunityInformationAsync()
    {
        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Account.Mine, clientType: RequestClientType.IOS, needToken: true);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse<Mine>>(response);
        UserId = result.Data.Mid;
        return CommunityAdapter.ConvertToUserCommunityInformation(result.Data);
    }

    /// <summary>
    /// 获取我的历史记录信息.
    /// </summary>
    /// <param name="tabSign">标签信息，默认是 <c>archive</c>，表示视频.</param>
    /// <returns>历史记录.</returns>
    public async Task<VideoHistoryView> GetMyHistorySetAsync(string tabSign = "archive")
    {
        var req = new CursorV2Req
        {
            Business = tabSign,
            Cursor = _historyCursor,
        };

        var request = await HttpProvider.GetRequestMessageAsync(Account.HistoryCursor, req, true);
        var response = await HttpProvider.Instance.SendAsync(request);
        var data = await HttpProvider.ParseAsync(response, CursorV2Reply.Parser);
        _historyCursor = data.Cursor;
        return VideoAdapter.ConvertToVideoHistoryView(data);
    }

    /// <summary>
    /// 获取用户主页信息.
    /// </summary>
    /// <param name="userId">用户Id.</param>
    /// <returns><see cref="UserSpaceResponse"/>.</returns>
    public async Task<UserSpaceView> GetUserSpaceInformationAsync(string userId)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.VMid, userId },
        };

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Account.Space, queryParameters, forceNoToken: true);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse<UserSpaceResponse>>(response);
        var accInfo = UserAdapter.ConvertToAccountInformation(result.Data.User, AvatarSize.Size96);
        var videoSet = VideoAdapter.ConvertToVideoSet(result.Data.VideoSet);
        if (videoSet.Items.Any())
        {
            _spaceVideoOffsets[userId] = videoSet.Items.Last().Identifier.Id;
        }

        return new UserSpaceView(accInfo, videoSet);
    }

    /// <summary>
    /// 获取用户空间的视频集.
    /// </summary>
    /// <param name="userId">用户Id.</param>
    /// <returns>视频集.</returns>
    public async Task<VideoSet> GetUserSpaceVideoSetAsync(string userId)
    {
        var offset = _spaceVideoOffsets.GetValueOrDefault(userId) ?? string.Empty;
        var queryParameters = new Dictionary<string, string>
        {
            { Query.VMid, userId },
            { Query.Aid, offset },
            { Query.Order, "pubdate" },
        };

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Account.VideoCursor, queryParameters, forceNoToken: true);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse<UserSpaceVideoSet>>(response);
        var data = VideoAdapter.ConvertToVideoSet(result.Data);
        if (data.Items.Any())
        {
            _spaceVideoOffsets[userId] = data.Items.Last().Identifier.Id;
        }

        return data;
    }

    /// <summary>
    /// 获取指定用户的粉丝或关注列表.
    /// </summary>
    /// <param name="userId">用户 Id.</param>
    /// <param name="type">关系类型.</param>
    /// <returns>粉丝或关注列表.</returns>
    public async Task<RelationView> GetUserFansOrFollowsAsync(string userId, RelationType type)
    {
        var hasCache = _relationOffsetCache.TryGetValue(type, out var page);
        if (!hasCache)
        {
            page = 1;
        }

        var queryParameters = new Dictionary<string, string>
        {
            { Query.VMid, userId },
            { Query.PageNumber, page.ToString() },
        };

        var uri = type == RelationType.Fans
                ? Account.Fans
                : Account.Follows;

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, uri, queryParameters, needToken: true);
        var response = await HttpProvider.Instance.SendAsync(request);
        var parsed = await HttpProvider.ParseAsync<ServerResponse<RelatedUserResponse>>(response);
        ResetRelationStatus(type);
        page++;
        _relationOffsetCache.Add(type, page);
        return UserAdapter.ConvertToRelationView(parsed.Data);
    }

    /// <summary>
    /// 获取稍后再看列表.
    /// </summary>
    /// <returns>稍后再看视频列表.</returns>
    public async Task<VideoSet> GetViewLaterListAsync()
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.PageNumber, _viewLaterPageNumber.ToString() },
            { Query.PageSizeSlim, "40" },
        };
        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Account.ViewLaterList, queryParameters, RequestClientType.IOS, needToken: true);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse<ViewLaterResponse>>(response);
        _viewLaterPageNumber++;
        return VideoAdapter.ConvertToVideoSet(result.Data);
    }

    /// <summary>
    /// 获取指定类型的消息列表.
    /// </summary>
    /// <param name="type">消息类型.</param>
    /// <returns>指定类型的消息响应结果.</returns>
    public async Task<MessageView> GetMyMessagesAsync(MessageType type)
    {
        var id = 0L;
        var time = 0L;
        if (_messageOffsetCache.TryGetValue(type, out var cache))
        {
            id = cache.Id;
            time = cache.Time;
        }

        var data = await GetMessageInternalAsync(type, id, time);
        return data;
    }

    /// <summary>
    /// 搜索用户空间的视频.
    /// </summary>
    /// <param name="userId">用户Id.</param>
    /// <param name="keyword">关键词.</param>
    /// <returns>视频集.</returns>
    public async Task<VideoSet> SearchUserSpaceVideoAsync(string userId, string keyword)
    {
        var pn = _spaceSearchPageNumbers.GetValueOrDefault(userId);
        var req = new SearchArchiveReq
        {
            Mid = System.Convert.ToInt64(userId),
            Keyword = keyword,
            Pn = pn,
            Ps = 20,
        };

        var request = await HttpProvider.GetRequestMessageAsync(Account.SpaceVideoSearch, req, false);
        var response = await HttpProvider.Instance.SendAsync(request);
        var data = await HttpProvider.ParseAsync(response, SearchArchiveReply.Parser);
        _spaceSearchPageNumbers[userId] = pn + 1;
        var videoSet = VideoAdapter.ConvertToVideoSet(data);
        foreach (var item in videoSet.Items)
        {
            item.Publisher = null;
        }

        return videoSet;
    }

    /// <summary>
    /// 获取我的关注分组详情.
    /// </summary>
    /// <param name="tagId">分组Id.</param>
    /// <returns>用户列表.</returns>
    public async Task<IEnumerable<AccountInformation>> GetMyFollowingGroupDetailAsync(string tagId)
    {
        if (UserId <= 0)
        {
            throw new System.InvalidOperationException("未登录");
        }

        if (!_myFollowOffsetCache.TryGetValue(tagId, out var page))
        {
            page = 1;
        }

        var queryParameters = new Dictionary<string, string>
        {
            { Query.TagId, tagId.ToString() },
            { Query.PageNumber, page.ToString() },
            { Query.MyId, UserId.ToString() },
        };

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Account.MyFollowingTagDetail, queryParameters, RequestClientType.IOS, true);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse<List<RelatedUser>>>(response);
        page++;
        _ = _myFollowOffsetCache.Remove(tagId);
        _myFollowOffsetCache.Add(tagId, page);
        result.Data.ForEach(p => p.Attribute = 2);
        return result.Data.Select(p => UserAdapter.ConvertToAccountInformation(p));
    }

    /// <summary>
    /// 获取聊天会话列表.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public async Task<ChatSessionListView> GetChatSessionsAsync()
    {
        var queryParameters = new Dictionary<string, string>
        {
            { "session_type", "1" },
            { "group_fold", "1" },
            { "sort_rule", "2" },
            { "end_ts", _chatSessionOffset.ToString() },
            { "wts", DateTimeOffset.Now.ToUnixTimeSeconds().ToString() },
        };

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Account.ChatSessions, queryParameters);
        var response = await HttpProvider.Instance.SendAsync(request);
        var sessionResult = await HttpProvider.ParseAsync<ServerResponse<ChatSessionListResponse>>(response);
        _chatSessionOffset = sessionResult.Data.SessionList.LastOrDefault()?.SessionTimestamp ?? 0;
        var sessions = sessionResult.Data.SessionList.Where(p => p.SystemMessageType == 0).ToList();
        var mockSessionResult = new ChatSessionListResponse { HasMore = sessionResult.Data.HasMore, SessionList = sessions };
        var userIds = mockSessionResult.SessionList.Select(p => p.TalkerId).Distinct().ToList();
        var userQueryParams = new Dictionary<string, string>
        {
            { "uids", string.Join(',', userIds) },
        };
        var userRequest = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Account.BatchUserInfo, userQueryParams, RequestClientType.Web);
        var userResponse = await HttpProvider.Instance.SendAsync(userRequest);
        var userResult = await HttpProvider.ParseAsync<ServerResponse<List<BiliChatUser>>>(userResponse);
        var view = CommunityAdapter.ConvertToChatSessionListView(mockSessionResult, userResult.Data);
        return view;
    }

    /// <summary>
    /// 清除消息的请求状态.
    /// </summary>
    public void ClearMessageStatus()
        => _messageOffsetCache.Clear();

    /// <summary>
    /// 重置稍后再看的请求状态.
    /// </summary>
    public void ResetViewLaterStatus()
        => _viewLaterPageNumber = 0;

    /// <summary>
    /// 重置历史记录请求状态.
    /// </summary>
    public void ResetHistoryStatus()
        => _historyCursor = new Cursor { Max = 0 };

    /// <summary>
    /// 重置关系请求状态.
    /// </summary>
    /// <param name="type">关系类型.</param>
    public void ResetRelationStatus(RelationType type)
        => _relationOffsetCache.Remove(type);

    /// <summary>
    /// 重置空间视频请求状态.
    /// </summary>
    public void ResetSpaceVideoStatus(string userId)
        => _spaceVideoOffsets.Remove(userId);

    /// <summary>
    /// 重置空间搜索请求状态.
    /// </summary>
    public void ResetSpaceSearchStatus(string userId)
        => _spaceSearchPageNumbers[userId] = 1;

    /// <summary>
    /// 重置我的关注请求状态.
    /// </summary>
    /// <param name="groupId">分组 Id.</param>
    public void ResetMyFollowStatus(string groupId)
        => _myFollowOffsetCache.Remove(groupId);

    /// <summary>
    /// 重置聊天会话列表请求状态.
    /// </summary>
    public void ResetChatSessionOffset()
        => _chatSessionOffset = 0;

    /// <summary>
    /// 清除我的关注请求状态.
    /// </summary>
    public void ClearMyFollowStatus()
        => _myFollowOffsetCache.Clear();
}
