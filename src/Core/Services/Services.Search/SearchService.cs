// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili.Search;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models.Media;
using Richasy.BiliKernel.Models.Search;
using Richasy.BiliKernel.Services.Search.Core;

namespace Richasy.BiliKernel.Services.Search;

/// <summary>
/// 搜索服务.
/// </summary>
public sealed class SearchService : ISearchService
{
    private readonly SearchClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchService"/> class.
    /// </summary>
    public SearchService(
        BiliHttpClient httpClient,
        BasicAuthenticator authenticator)
    {
        _client = new SearchClient(httpClient, authenticator);
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<HotSearchItem>> GetTotalHotSearchAsync(int count = 30, CancellationToken cancellationToken = default)
        => count <= 0 ? throw new KernelException("热搜榜单数量必须大于0") : _client.GetTotalHotSearchAsync(count, cancellationToken);

    /// <inheritdoc/>
    public Task<IReadOnlyList<SearchRecommendItem>> GetSearchRecommendsAsync(CancellationToken cancellationToken = default)
        => _client.GetSearchRecommendsAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<(IReadOnlyList<VideoInformation> Videos, IReadOnlyList<SearchPartition>? Partitions, string? NextOffset)> GetComprehensiveSearchResultAsync(string keyword, string? offset = null, CancellationToken cancellationToken = default)
    {
        return string.IsNullOrWhiteSpace(keyword)
            ? throw new KernelException("搜索关键字不能为空")
            : _client.GetComprehensiveSearchResultAsync(keyword, offset, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<(IReadOnlyList<SearchResultItem> Result, string? Offset)> GetPartitionSearchResultAsync(string keyword, SearchPartition partition, string? offset = null, CancellationToken cancellationToken = default)
    {
        return string.IsNullOrWhiteSpace(keyword)
            ? throw new KernelException("搜索关键字不能为空")
            : _client.GetPartitionSearchResultAsync(keyword, partition, offset, cancellationToken);
    }
}
