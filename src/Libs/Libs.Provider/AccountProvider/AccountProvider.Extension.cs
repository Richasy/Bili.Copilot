// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Adapter;
using Bili.Copilot.Models.BiliBili;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Authorize;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.Community;
using Bilibili.App.Interface.V1;
using static Bili.Copilot.Models.App.Constants.ApiConstants;
using static Bili.Copilot.Models.App.Constants.ServiceConstants;

namespace Bili.Copilot.Libs.Provider;

/// <summary>
/// 提供已登录用户相关的数据操作.
/// </summary>
public sealed partial class AccountProvider
{
    private readonly HttpProvider _httpProvider;
    private readonly Dictionary<MessageType, MessageCursor> _messageOffsetCache;
    private readonly Dictionary<RelationType, int> _relationOffsetCache;
    private readonly Dictionary<string, int> _myFollowOffsetCache;
    private int _viewLaterPageNumber;
    private string _spaceVideoOffset;
    private int _spaceSearchPageNumber;
    private Cursor _historyCursor;

    /// <summary>
    /// 实例.
    /// </summary>
    public static AccountProvider Instance { get; } = new AccountProvider();

    /// <summary>
    /// 已登录的用户Id.
    /// </summary>
    public long UserId { get; set; }

    private async Task<MessageView> GetMessageInternalAsync(MessageType type, long id, long time)
    {
        var timeName = string.Empty;
        var url = string.Empty;

        switch (type)
        {
            case MessageType.Like:
                timeName = Query.LikeTime;
                url = Account.MessageLike;
                break;
            case MessageType.At:
                timeName = Query.AtTime;
                url = Account.MessageAt;
                break;
            case MessageType.Reply:
                timeName = Query.ReplyTime;
                url = Account.MessageReply;
                break;
            default:
                break;
        }

        var queryParameters = new Dictionary<string, string>
        {
            { Query.Id, id.ToString() },
            { timeName, time.ToString() },
        };

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, url, queryParameters, RequestClientType.IOS, true);
        var response = await HttpProvider.Instance.SendAsync(request);

        MessageView data = null;
        MessageCursor cursor = null;
        if (type == MessageType.Reply)
        {
            var parsed = await HttpProvider.ParseAsync<ServerResponse<ReplyMessageResponse>>(response);
            cursor = parsed.Data.Cursor;
            data = CommunityAdapter.ConvertToMessageView(parsed.Data);
        }
        else if (type == MessageType.Like)
        {
            var parsed = await HttpProvider.ParseAsync<ServerResponse<LikeMessageResponse>>(response);
            cursor = parsed.Data.Total.Cursor;
            data = CommunityAdapter.ConvertToMessageView(parsed.Data);
        }
        else if (type == MessageType.At)
        {
            var parsed = await HttpProvider.ParseAsync<ServerResponse<AtMessageResponse>>(response);
            cursor = parsed.Data.Cursor;
            data = CommunityAdapter.ConvertToMessageView(parsed.Data);
        }

        _ = _messageOffsetCache.Remove(type);
        _messageOffsetCache.Add(type, cursor);
        return data;
    }
}
