﻿// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Data.Appearance;
using Bili.Copilot.Models.Data.User;

namespace Bili.Copilot.Models.Data.Video;

/// <summary>
/// 收藏夹信息.
/// </summary>
public sealed class VideoFavoriteFolder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoFavoriteFolder"/> class.
    /// </summary>
    /// <param name="id">收藏夹 Id.</param>
    /// <param name="title">标题.</param>
    /// <param name="cover">封面.</param>
    /// <param name="user">用户信息.</param>
    /// <param name="description">收藏夹描述.</param>
    /// <param name="totalCount">收藏夹下内容总数.</param>
    public VideoFavoriteFolder(
        string id,
        string title,
        Image cover,
        UserProfile user,
        string description,
        int totalCount)
    {
        Id = id;
        Title = title;
        Cover = cover;
        User = user;
        Description = description;
        TotalCount = totalCount;
    }

    /// <summary>
    /// 收藏夹 Id.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// 收藏夹标题.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// 收藏夹封面.
    /// </summary>
    public Image Cover { get; }

    /// <summary>
    /// 创建收藏夹的用户信息.
    /// </summary>
    public UserProfile User { get; }

    /// <summary>
    /// 收藏夹描述.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// 收藏夹下的内容总数.
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// 是否为合集.
    /// </summary>
    public bool IsUgcSeason { get; set; }

    /// <summary>
    /// 合集视频 Id.
    /// </summary>
    public string SeasonVideoId { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is VideoFavoriteFolder folder && Id == folder.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => Id.GetHashCode();
}
