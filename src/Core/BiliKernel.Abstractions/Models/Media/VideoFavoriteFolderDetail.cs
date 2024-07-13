// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;

namespace Richasy.BiliKernel.Models.Media;

/// <summary>
/// 默认收藏夹信息.
/// </summary>
public sealed class VideoFavoriteFolderDetail
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoFavoriteFolderDetail"/> class.
    /// </summary>
    public VideoFavoriteFolderDetail(
        VideoFavoriteFolder folder,
        IList<VideoInformation>? videos,
        int? totalCount)
    {
        Folder = folder;
        Videos = videos;
        TotalCount = totalCount ?? videos?.Count ?? 0;
    }

    /// <summary>
    /// 默认收藏夹信息.
    /// </summary>
    public VideoFavoriteFolder Folder { get; }

    /// <summary>
    /// 默认收藏视频列表.
    /// </summary>
    public IList<VideoInformation>? Videos { get; }

    /// <summary>
    /// 视频总数.
    /// </summary>
    public int TotalCount { get; }
}
