// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;

namespace Richasy.BiliKernel.Models.User;

/// <summary>
/// 消息信息.
/// </summary>
public sealed class NotifyMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotifyMessage"/> class.
    /// </summary>
    /// <param name="id">该消息标识符.</param>
    /// <param name="type">消息类型.</param>
    /// <param name="users">用户列表.</param>
    /// <param name="publishTime">发布时间.</param>
    /// <param name="subtitle">副标题.</param>
    /// <param name="message">消息内容.</param>
    /// <param name="sourceContent">源内容.</param>
    /// <param name="sourceId">源内容标识符.</param>
    /// <param name="properties">键值对属性.</param>
    public NotifyMessage(
        string id,
        NotifyMessageType type,
        IReadOnlyList<UserProfile> users,
        DateTimeOffset publishTime,
        string? subtitle,
        string? message,
        string? sourceContent,
        string? sourceId,
        Dictionary<string, string>? properties = default)
    {
        Type = type;
        Users = users;
        PublishTime = publishTime;
        Subtitle = subtitle;
        Message = message;
        SourceContent = sourceContent;
        SourceId = sourceId;
        Id = id;
        Properties = properties;
    }

    /// <summary>
    /// 消息类型.
    /// </summary>
    public NotifyMessageType? Type { get; }


    /// <summary>
    /// 关联的用户列表.
    /// </summary>
    public IReadOnlyList<UserProfile> Users { get; }

    /// <summary>
    /// 发布时间.
    /// </summary>
    public DateTimeOffset? PublishTime { get; }

    /// <summary>
    /// 副标题.
    /// </summary>
    public string? Subtitle { get; }

    /// <summary>
    /// 消息内容.
    /// </summary>
    public string? Message { get; }

    /// <summary>
    /// 源内容，比如是通过哪个动态或者视频做的评论.
    /// </summary>
    public string? SourceContent { get; }

    /// <summary>
    /// 源内容的标识符.
    /// </summary>
    public string? SourceId { get; }

    /// <summary>
    /// 该消息的标识符.
    /// </summary>
    public string? Id { get; }

    /// <summary>
    /// 附加的键值对属性.
    /// </summary>
    public Dictionary<string, string>? Properties { get; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is NotifyMessage information && Id == information.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
