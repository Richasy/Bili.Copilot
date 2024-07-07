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
    /// 获取分区排行榜视频.
    /// </summary>
    Task<IReadOnlyList<VideoInformation>> GetPartitionRankingListAsync(Partition partition, CancellationToken cancellationToken = default);
}
