// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Models.Media;

namespace Richasy.BiliKernel.Bili.Media;

/// <summary>
/// 获取流行视频的服务，包括精选视频/推荐视频/热门视频/排行榜.
/// </summary>
public interface IPopularVideoService
{
    /// <summary>
    /// 获取来自桌面客户端的精选视频播放列表.
    /// </summary>
    Task<IReadOnlyList<VideoInformation>> GetCuratedPlaylistAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取推荐视频列表.
    /// </summary>
    Task<(IReadOnlyList<VideoInformation> Items, long Offset)> GetRecommendVideoListAsync(long offset = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取热门视频列表.
    /// </summary>
    Task<(IReadOnlyList<VideoInformation> Items, long Offset)> GetHotVideoListAsync(long offset = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取全站排行榜.
    /// </summary>
    Task<IReadOnlyList<VideoInformation>> GetGlobalRankingListAsync(CancellationToken cancellationToken = default);
}
