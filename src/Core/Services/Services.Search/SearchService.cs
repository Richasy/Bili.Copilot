// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili.Search;
using Richasy.BiliKernel.Http;
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
    public Task<IReadOnlyList<HotSearchItem>> GetTotalHotSearchAsync(CancellationToken cancellationToken = default)
        => _client.GetTotalHotSearchAsync(cancellationToken);
}
