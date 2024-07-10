// Copyright (c) Richasy. All rights reserved.

using System;

namespace Richasy.BiliKernel.Models.Moment;

/// <summary>
/// 动态的社区信息.
/// </summary>
public sealed class MomentCommunityInformation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MomentCommunityInformation"/> class.
    /// </summary>
    /// <param name="id">动态 Id.</param>
    /// <param name="likeCount">点赞数.</param>
    /// <param name="commentCount">评论数.</param>
    /// <param name="isLiked">是否已点赞.</param>
    public MomentCommunityInformation(
        string id,
        long? likeCount,
        long? commentCount,
        bool? isLiked = false)
    {
        Id = id;
        LikeCount = likeCount;
        CommentCount = commentCount;
        IsLiked = isLiked;
    }

    /// <summary>
    /// 动态 Id.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// 点赞数.
    /// </summary>
    public long? LikeCount { get; set; }

    /// <summary>
    /// 评论数.
    /// </summary>
    public long? CommentCount { get; }

    /// <summary>
    /// 是否已点赞.
    /// </summary>
    public bool? IsLiked { get; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is MomentCommunityInformation information && Id == information.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
