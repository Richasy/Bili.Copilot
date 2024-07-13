// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;

namespace Richasy.BiliKernel.Models.Media;

/// <summary>
/// 视频收藏夹分组.
/// </summary>
public sealed class VideoFavoriteFolderGroup
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoFavoriteFolderGroup"/> class.
    /// </summary>
    /// <param name="id">分组 Id.</param>
    /// <param name="title">标题.</param>
    /// <param name="isMine">是否由本人创建.</param>
    /// <param name="folders">收藏夹列表.</param>
    /// <param name="totalCount">视频总数.</param>
    public VideoFavoriteFolderGroup(
        string id,
        string title,
        bool isMine,
        IList<VideoFavoriteFolder> folders,
        int totalCount)
    {
        Id = id;
        Title = title;
        IsMine = isMine;
        Folders = folders;
        TotalCount = totalCount;
    }

    /// <summary>
    /// 分组 Id.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// 标题.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// 是否由本人创建.
    /// </summary>
    public bool IsMine { get; }

    /// <summary>
    /// 收藏集.
    /// </summary>
    public IList<VideoFavoriteFolder> Folders { get; }

    /// <summary>
    /// 视频总数.
    /// </summary>
    public int TotalCount { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is VideoFavoriteFolderGroup group && Id == group.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
