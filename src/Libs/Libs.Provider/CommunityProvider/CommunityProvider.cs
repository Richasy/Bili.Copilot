// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Adapter;
using Bili.Copilot.Models.BiliBili;
using Bili.Copilot.Models.Constants.Authorize;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.Community;
using Bili.Copilot.Models.Data.Dynamic;
using Bilibili.App.Dynamic.V2;
using Bilibili.Main.Community.Reply.V1;
using static Bili.Copilot.Models.App.Constants.ApiConstants;
using static Bili.Copilot.Models.App.Constants.ServiceConstants;

namespace Bili.Copilot.Libs.Provider;

/// <summary>
/// 社区交互数据处理.
/// </summary>
public partial class CommunityProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommunityProvider"/> class.
    /// </summary>
    private CommunityProvider()
    {
        ResetMainCommentsStatus();
        ResetDetailCommentsStatus();
        ResetVideoDynamicStatus();
        ResetComprehensiveDynamicStatus();
    }

    /// <summary>
    /// 给评论点赞/取消点赞.
    /// </summary>
    /// <param name="isLike">是否点赞.</param>
    /// <param name="replyId">评论Id.</param>
    /// <param name="targetId">目标评论区Id.</param>
    /// <param name="type">评论区类型.</param>
    /// <returns>结果.</returns>
    public static async Task<bool> LikeCommentAsync(bool isLike, string replyId, string targetId, CommentType type)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.Oid, targetId.ToString() },
            { Query.ActionFull, isLike ? "1" : "0" },
            { Query.ReplyId, replyId.ToString() },
            { Query.Type, ((int)type).ToString() },
        };

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Post, Community.LikeReply, queryParameters, RequestClientType.IOS, true);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse>(response);
        return result.IsSuccess();
    }

    /// <summary>
    /// 点赞/取消点赞动态.
    /// </summary>
    /// <param name="dynamicId">动态Id.</param>
    /// <param name="isLike">是否点赞.</param>
    /// <param name="userId">用户Id.</param>
    /// <param name="rid">扩展数据标识.</param>
    /// <returns>是否操作成功.</returns>
    public static async Task<bool> LikeDynamicAsync(string dynamicId, bool isLike, string userId, string rid)
    {
        var req = new DynThumbReq
        {
            Type = isLike ? ThumbType.Thumb : ThumbType.Cancel,
            DynId = dynamicId.ToString(),
            Rid = rid,
            Uid = Convert.ToInt64(userId),
        };

        try
        {
            var request = await HttpProvider.GetRequestMessageAsync(Community.LikeDynamic, req, true);
            _ = await HttpProvider.Instance.SendAsync(request);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 添加评论.
    /// </summary>
    /// <param name="message">评论内容.</param>
    /// <param name="targetId">评论区Id.</param>
    /// <param name="type">评论区类型.</param>
    /// <param name="rootId">根评论Id.</param>
    /// <param name="parentId">正在回复的评论Id.</param>
    /// <returns>发布结果.</returns>
    public static async Task<bool> AddCommentAsync(string message, string targetId, CommentType type, string rootId, string parentId)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.MessageFull, message },
            { Query.Oid, targetId.ToString() },
            { Query.Type, ((int)type).ToString() },
            { Query.PlatformSlim, "3" },
            { Query.Root, rootId },
            { Query.Parent, parentId },
        };

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Post, Community.AddReply, queryParameters, RequestClientType.IOS, true);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse>(response);
        return result.IsSuccess();
    }

    /// <summary>
    /// 获取单层评论详情列表.
    /// </summary>
    /// <param name="targetId">目标评论区Id.</param>
    /// <param name="type">评论区类型.</param>
    /// <param name="sort">排序方式.</param>
    /// <param name="rootId">根评论Id.</param>
    /// <returns>评论列表响应.</returns>
    public async Task<CommentView> GetCommentsAsync(string targetId, CommentType type, CommentSortType sort, string rootId)
    {
        _detailCommentCursor.Mode = sort == CommentSortType.Time
            ? Mode.MainListTime
            : Mode.MainListHot;
        var req = new DetailListReq
        {
            Scene = DetailListScene.Reply,
            Cursor = _detailCommentCursor,
            Oid = Convert.ToInt64(targetId),
            Root = Convert.ToInt64(rootId),
            Type = (int)type,
        };

        var request = await HttpProvider.GetRequestMessageAsync(Community.ReplyDetailList, req);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync(response, DetailListReply.Parser);
        var cursor = result.Cursor;
        _detailCommentCursor = new CursorReq
        {
            Mode = cursor.Mode,
            Next = cursor.Next,
            Prev = 0,
        };

        var view = CommentAdapter.ConvertToCommentView(result, targetId);
        foreach (var item in view.Comments)
        {
            item.CommentId = targetId;
            item.CommentType = type;
        }

        return view;
    }

    /// <summary>
    /// 获取评论列表.
    /// </summary>
    /// <param name="targetId">目标Id.</param>
    /// <param name="type">评论区类型.</param>
    /// <param name="sort">排序方式.</param>
    /// <returns>评论列表响应.</returns>
    public async Task<CommentView> GetCommentsAsync(string targetId, CommentType type, CommentSortType sort)
    {
        _mainCommentCursor.Mode = sort == CommentSortType.Time
            ? Mode.MainListTime
            : Mode.MainListHot;
        var req = new MainListReq
        {
            Cursor = _mainCommentCursor,
            Oid = Convert.ToInt64(targetId),
            Type = (int)type,
            Rpid = 0,
        };

        var request = await HttpProvider.GetRequestMessageAsync(Community.ReplyMainList, req);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync(response, MainListReply.Parser);
        var cursor = result.Cursor;
        _mainCommentCursor = new CursorReq
        {
            Mode = cursor.Mode,
            Next = cursor.Next,
            Prev = 0,
        };
        var view = CommentAdapter.ConvertToCommentView(result, targetId);
        foreach (var item in view.Comments)
        {
            item.CommentId = targetId;
            item.CommentType = type;
        }

        if (view.TopComment != null)
        {
            view.TopComment.CommentId = targetId;
            view.TopComment.CommentType = type;
        }

        return view;
    }

    /// <summary>
    /// 获取综合动态列表.
    /// </summary>
    /// <returns>综合动态响应.</returns>
    public async Task<DynamicView> GetDynamicComprehensiveListAsync()
    {
        var type = string.IsNullOrEmpty(_comprehensiveDynamicOffset.Offset) ? Refresh.New : Refresh.History;
        var req = new DynAllReq
        {
            RefreshType = type,
            LocalTime = 8,
            Offset = _comprehensiveDynamicOffset.Offset,
            UpdateBaseline = _comprehensiveDynamicOffset.Baseline,
        };

        var request = await HttpProvider.GetRequestMessageAsync(Community.DynamicAll, req, true);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync(response, DynAllReply.Parser);
        _comprehensiveDynamicOffset = new(result.DynamicList.HistoryOffset, result.DynamicList.UpdateBaseline);
        return DynamicAdapter.ConvertToDynamicView(result);
    }

    /// <summary>
    /// 获取视频动态列表.
    /// </summary>
    /// <returns>视频动态响应.</returns>
    public async Task<DynamicView> GetDynamicVideoListAsync()
    {
        var type = string.IsNullOrEmpty(_videoDynamicOffset.Offset) ? Refresh.New : Refresh.History;
        var req = new DynVideoReq
        {
            RefreshType = type,
            LocalTime = 8,
            Offset = _videoDynamicOffset.Offset,
            UpdateBaseline = _videoDynamicOffset.Baseline,
        };

        var request = await HttpProvider.GetRequestMessageAsync(Community.DynamicVideo, req, true);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync(response, DynVideoReply.Parser);
        _videoDynamicOffset = new(result.DynamicList.HistoryOffset, result.DynamicList.UpdateBaseline);
        return DynamicAdapter.ConvertToDynamicView(result);
    }

    /// <summary>
    /// 重置视频动态请求状态.
    /// </summary>
    public void ResetVideoDynamicStatus()
        => _videoDynamicOffset = new(string.Empty, string.Empty);

    /// <summary>
    /// 重置综合动态请求状态.
    /// </summary>
    public void ResetComprehensiveDynamicStatus()
        => _comprehensiveDynamicOffset = new(string.Empty, string.Empty);

    /// <summary>
    /// 清除评论区请求状态.
    /// </summary>
    public void ResetMainCommentsStatus()
    {
        _mainCommentCursor = new CursorReq
        {
            Mode = Mode.Default,
            Next = 0,
            Prev = 0,
        };
    }

    /// <summary>
    /// 清除评论详情请求状态.
    /// </summary>
    public void ResetDetailCommentsStatus()
    {
        _detailCommentCursor = new CursorReq
        {
            Mode = Mode.Default,
            Next = 0,
            Prev = 0,
        };
    }
}
