// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Linq;
using Bili.Copilot.Models.Data.Community;
using Bilibili.Main.Community.Reply.V1;

namespace Bili.Copilot.Libs.Adapter;

/// <summary>
/// 评论数据适配器.
/// </summary>
public static class CommentAdapter
{
    /// <summary>
    /// 将回复内容 <see cref="ReplyInfo"/> 转换为评论信息.
    /// </summary>
    /// <param name="info">回复内容.</param>
    /// <returns><see cref="CommentInformation"/>.</returns>
    public static CommentInformation ConvertToCommentInformation(ReplyInfo info)
    {
        var id = info.Id.ToString();
        var rootId = info.Root.ToString();
        var isTop = info.ReplyControl.IsAdminTop || info.ReplyControl.IsUpTop;
        var publishTime = DateTimeOffset.FromUnixTimeSeconds(info.Ctime);
        var user = UserAdapter.ConvertToAccountInformation(info.Member);
        var communityInfo = new CommentCommunityInformation(id, info.Like, Convert.ToInt32(info.Count), info.ReplyControl.Action == 1);
        var content = ImageAdapter.ConvertToEmoteText(info.Content);
        return new CommentInformation(id, content, rootId, isTop, user, publishTime, communityInfo);
    }

    /// <summary>
    /// 将主评论响应 <see cref="MainListReply"/> 转换为评论视图.
    /// </summary>
    /// <param name="reply">主评论响应.</param>
    /// <param name="targetId">评论区Id.</param>
    /// <returns><see cref="CommentView"/>.</returns>
    public static CommentView ConvertToCommentView(MainListReply reply, string targetId)
    {
        var comments = reply.Replies.Select(ConvertToCommentInformation).ToList();
        var top = reply.UpTop ?? reply.VoteTop;
        var topComment = top == null
            ? null
            : ConvertToCommentInformation(top);
        var isEnd = reply.Cursor.IsEnd;
        return new CommentView(comments, targetId, topComment, isEnd);
    }

    /// <summary>
    /// 将二级评论响应 <see cref="DetailListReply"/> 转换为评论视图.
    /// </summary>
    /// <param name="reply">二级评论响应.</param>
    /// <param name="targetId">目标评论Id.</param>
    /// <returns><see cref="CommentView"/>.</returns>
    public static CommentView ConvertToCommentView(DetailListReply reply, string targetId)
    {
        var comments = reply.Root.Replies.Select(ConvertToCommentInformation).ToList();
        var topComment = ConvertToCommentInformation(reply.Root);
        var isEnd = reply.Cursor.IsEnd;
        return new CommentView(comments, targetId, topComment, isEnd);
    }
}
