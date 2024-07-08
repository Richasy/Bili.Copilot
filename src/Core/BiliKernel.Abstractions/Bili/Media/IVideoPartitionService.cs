// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;

namespace Richasy.BiliKernel.Bili.Media;

/// <summary>
/// 视频分区服务.
/// </summary>
public interface IVideoPartitionService
{
    /// <summary>
    /// 获取视频分区.
    /// </summary>
    Task<IReadOnlyList<Partition>> GetVideoPartitionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取分区排行榜视频.（仅限主分区，子分区请求会失败）.
    /// </summary>
    Task<IReadOnlyList<VideoInformation>> GetPartitionRankingListAsync(Partition partition, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取分区推荐视频.（仅限主分区，子分区请求会失败）.
    /// </summary>
    Task<(IReadOnlyList<VideoInformation> Videos, long Offset)> GetPartitionRecommendVideoListAsync(Partition partition, long offset = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取子分区视频列表.（仅限主分区下的子分区）.
    /// </summary>
    Task<(IReadOnlyList<VideoInformation> Videos, long Offset, int NextPageNumber)> GetChildPartitionVideoListAsync(Partition childPartition, long offset = 0, int pageNumber = 0, PartitionVideoSortType sortType = PartitionVideoSortType.Default, CancellationToken cancellationToken = default);
}
