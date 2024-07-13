// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;

namespace Richasy.BiliKernel.Bili.User;

/// <summary>
/// 收藏夹服务.
/// </summary>
public interface IFavoriteService
{
    /// <summary>
    /// 获取用户的视频收藏夹分组.
    /// </summary>
    Task<(IReadOnlyList<VideoFavoriteFolderGroup> Groups, VideoFavoriteFolderDetail Default)> GetVideoFavoriteGroupsAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取播放视频时显示的收藏夹信息.
    /// </summary>
    /// <remarks>
    /// 返回值中包含的 ContainerIds 用于标识已包含此视频的收藏夹 ID 列表，可以辅助确认该视频是否已经被收藏.
    /// </remarks>
    Task<(IReadOnlyList<VideoFavoriteFolder> Folders, IReadOnlyList<string> ContainerIds)> GetPlayingVideoFavoriteFoldersAsync(VideoInformation video, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取视频收藏夹详情.
    /// </summary>
    Task<VideoFavoriteFolderDetail> GetVideoFavoriteFolderDetailAsync(VideoFavoriteFolder folder, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取当前用户的追番列表.
    /// </summary>
    Task<(IReadOnlyList<SeasonInformation> Seasons, int TotalCount, int NextPageNumber)> GetPgcFavoritesAsync(PgcFavoriteType type, int status, int pageNumber = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// 将视频从收藏夹中移除.
    /// </summary>
    Task RemoveVideoAsync(VideoFavoriteFolder folder, MediaIdentifier identifier, CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消追番/追剧.
    /// </summary>
    Task RemovePgcAsync(MediaIdentifier season, CancellationToken cancellationToken = default);
}
