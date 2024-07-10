// Copyright (c) Richasy. All rights reserved.

using System;
using Richasy.BiliKernel.Models.Appearance;
using Richasy.BiliKernel.Models.User;

namespace Richasy.BiliKernel.Models.Moment;

/// <summary>
/// 动态信息.
/// </summary>
public sealed class MomentInformation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MomentInformation"/> class.
    /// </summary>
    /// <param name="id">动态 Id.</param>
    /// <param name="user">发布者.</param>
    /// <param name="tip">注解提示.</param>
    /// <param name="communityInformation">社区信息.</param>
    /// <param name="replyType">评论区类型.</param>
    /// <param name="replyId">评论区 Id.</param>
    /// <param name="dynamicType">动态类型.</param>
    /// <param name="data">主内容.</param>
    /// <param name="description">动态文本内容.</param>
    public MomentInformation(
        string id,
        UserProfile user,
        string? tip = default,
        MomentCommunityInformation? communityInformation = default,
        CommentType? replyType = default,
        string? replyId = default,
        MomentItemType? dynamicType = default,
        object? data = default,
        EmoteText? description = default)
    {
        Id = id;
        User = user;
        Tip = tip;
        CommunityInformation = communityInformation;
        CommentType = replyType;
        CommentId = replyId;
        MomentType = dynamicType;
        Data = data;
        Description = description;
    }

    /// <summary>
    /// 动态 Id.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// 动态发布者.
    /// </summary>
    public UserProfile? User { get; }

    /// <summary>
    /// 提示文本，用来标识发布时间.
    /// </summary>
    public string? Tip { get; }

    /// <summary>
    /// 动态的文本内容.
    /// </summary>
    public EmoteText? Description { get; }

    /// <summary>
    /// 动态的社区信息.
    /// </summary>
    public MomentCommunityInformation? CommunityInformation { get; }

    /// <summary>
    /// 评论区类型.
    /// </summary>
    public CommentType? CommentType { get; }

    /// <summary>
    /// 评论区Id.
    /// </summary>
    public string? CommentId { get; }

    /// <summary>
    /// 动态类型.
    /// </summary>
    public MomentItemType? MomentType { get; }

    /// <summary>
    /// 动态主内容，根据 <see cref="MomentType"/> 的不同而变化.
    /// </summary>
    public object? Data { get; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is MomentInformation information && Id == information.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
