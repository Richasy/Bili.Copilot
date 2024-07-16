// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Models.Media;
using Richasy.BiliKernel.Models.Search;

namespace Richasy.BiliKernel.Bili.Search;

/// <summary>
/// 搜索服务.
/// </summary>
public interface ISearchService
{
    /// <summary>
    /// 获取完整热搜榜单.
    /// </summary>
    Task<IReadOnlyList<HotSearchItem>> GetTotalHotSearchAsync(int count = 30, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取搜索推荐.
    /// </summary>
    Task<IReadOnlyList<SearchRecommendItem>> GetSearchRecommendsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取综合搜索结果.
    /// </summary>
    /// <remarks>
    /// 综合搜索结果在第一次请求时会返回分区信息，之后的请求需要传入上一次请求返回的 Offset，以获取更多的搜索结果.
    /// </remarks>
    Task<(IReadOnlyList<VideoInformation> Videos, IReadOnlyList<SearchPartition>? Partitions, string? NextOffset)> GetComprehensiveSearchResultAsync(string keyword, string? offset = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取分区搜索结果.
    /// </summary>
    Task<(IReadOnlyList<SearchResultItem> Result, string? Offset)> GetPartitionSearchResultAsync(string keyword, SearchPartition partition, string? offset = default, CancellationToken cancellationToken = default);
}
