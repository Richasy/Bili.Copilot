﻿// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Models.Media;

namespace Richasy.BiliKernel.Bili.Media;

/// <summary>
/// 动漫服务（包括番剧和国创）.
/// </summary>
public interface IAnimeService
{
    /// <summary>
    /// 获取番剧发布时间线.
    /// </summary>
    Task<(string Title, string Description, IReadOnlyList<TimelineInformation>)> GetBangumiTimelineAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取国创（国内动漫）发布时间线.
    /// </summary>
    Task<(string Title, string Description, IReadOnlyList<TimelineInformation>)> GetDomesticTimelineAsync(CancellationToken cancellationToken = default);
}
