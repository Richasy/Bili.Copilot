// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Constants.Bili;

namespace Bili.Copilot.Models.App.Args;

/// <summary>
/// 显示评论详情的事件参数.
/// </summary>
public sealed class ShowCommentEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShowCommentEventArgs"/> class.
    /// </summary>
    /// <param name="type">评论区类型.</param>
    /// <param name="sortType">评论区排序方式.</param>
    /// <param name="sourceId">评论源 Id.</param>
    public ShowCommentEventArgs(
        CommentType type,
        CommentSortType sortType,
        string sourceId,
        string title = default)
    {
        Type = type;
        SortType = sortType;
        SourceId = sourceId;
        Title = title;
    }

    /// <summary>
    /// 评论区类型.
    /// </summary>
    public CommentType Type { get; }

    /// <summary>
    /// 评论区排序方式.
    /// </summary>
    public CommentSortType SortType { get; }

    /// <summary>
    /// 评论源 Id.
    /// </summary>
    public string SourceId { get; }

    /// <summary>
    /// 评论源标题.
    /// </summary>
    public string Title { get; set; }
}

