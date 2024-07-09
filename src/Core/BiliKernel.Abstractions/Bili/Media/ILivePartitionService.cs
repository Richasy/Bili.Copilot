﻿// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;

namespace Richasy.BiliKernel.Bili.Media;

/// <summary>
/// 直播分区服务.
/// </summary>
public interface ILivePartitionService
{
    /// <summary>
    /// 获取直播分区.
    /// </summary>
    Task<IReadOnlyList<Partition>> GetLivePartitionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取分区直播列表.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>该响应在第一次请求时无需传递标签参数，标签列表会在第一次请求时返回.</item>
    /// <item>如果不传入标签参数，返回的就是该分区下的推荐直播间列表.</item>
    /// <item>标签列表仅在无标签传入，且 pageNumebr 为 0 时有值.</item>
    /// </list>
    /// </remarks>
    Task<(IReadOnlyList<LiveInformation> Lives, IReadOnlyList<LiveTag>? Tags, int NextPageNumber)> GetPartitionLiveListAsync(Partition partion, LiveTag? tag = default, int pageNumber = 0, CancellationToken cancellationToken = default);
}
