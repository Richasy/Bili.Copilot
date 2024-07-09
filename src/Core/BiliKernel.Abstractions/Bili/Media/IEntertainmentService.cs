// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Appearance;
using Richasy.BiliKernel.Models.Media;

namespace Richasy.BiliKernel.Bili.Media;

/// <summary>
/// 电影/电视剧/纪录片服务.
/// </summary>
public interface IEntertainmentService
{
    /// <summary>
    /// 获取影视作品索引筛选条件.
    /// </summary>
    Task<IReadOnlyList<Filter>> GetEntertainmentFiltersAsync(EntertainmentType type, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据筛选条件，获取影视作品索引.
    /// </summary>
    Task<(IReadOnlyList<SeasonInformation> Seasons, bool HasNext)> GetEntertainmentSeasonsWithFiltersAsync(EntertainmentType type, Dictionary<Filter, Condition>? filters = default, int pageNumber = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// 追剧.
    /// </summary>
    Task FollowAsync(string seasonId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消追剧.
    /// </summary>
    Task UnfollowAsync(string seasonId, CancellationToken cancellationToken = default);
}
