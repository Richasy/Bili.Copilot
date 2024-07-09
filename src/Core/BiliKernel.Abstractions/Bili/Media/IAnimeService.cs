// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Models.Appearance;
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

    /// <summary>
    /// 获取动漫（包括国创和番剧）的索引筛选条件.
    /// </summary>
    Task<IReadOnlyList<Filter>> GetAnimeFiltersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据筛选条件，获取番剧剧集索引.
    /// </summary>
    Task<(IReadOnlyList<SeasonInformation> Seasons, bool HasNext)> GetAnimeSeasonsWithFiltersAsync(Dictionary<Filter, Condition>? filters = default, int pageNumber = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// 追番.
    /// </summary>
    Task FollowAsync(string seasonId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消追番.
    /// </summary>
    Task UnfollowAsync(string seasonId, CancellationToken cancellationToken = default);
}
