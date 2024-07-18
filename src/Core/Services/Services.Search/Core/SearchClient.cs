// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Bilibili.Polymer.App.Search.V1;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili;
using Richasy.BiliKernel.Content;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models.Media;
using Richasy.BiliKernel.Models.Search;

namespace Richasy.BiliKernel.Services.Search.Core;

internal sealed class SearchClient
{
    private readonly BiliHttpClient _httpClient;
    private readonly BasicAuthenticator _authenticator;

    public SearchClient(
        BiliHttpClient httpClient,
        BasicAuthenticator authenticator)
    {
        _httpClient = httpClient;
        _authenticator = authenticator;
    }

    public async Task<IReadOnlyList<HotSearchItem>> GetTotalHotSearchAsync(int count, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "limit", count.ToString() },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new Uri(BiliApis.Search.HotSearch));
        _authenticator.AuthroizeRestRequest(request, parameters, new BasicAuthorizeExecutionSettings { RequireToken = false, ForceNoToken = true, NeedCSRF = true });
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync<BiliDataResponse<HotSearchResponse>>(response).ConfigureAwait(false);
        return responseObj.Data.List.Select(p => p.ToHotSearchItem()).ToList().AsReadOnly()
            ?? throw new KernelException("无法获取到有效的热搜榜单");
    }

    public async Task<IReadOnlyList<SearchRecommendItem>> GetSearchRecommendsAsync(CancellationToken cancellationToken)
    {
        var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new Uri(BiliApis.Search.RecommendSearch));
        _authenticator.AuthroizeRestRequest(request, default, new BasicAuthorizeExecutionSettings { RequireToken = false });
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync<BiliDataResponse<SearchRecommendResponse>>(response).ConfigureAwait(false);
        return responseObj.Data.List.Select(p => p.ToSearchRecommendItem()).ToList().AsReadOnly()
            ?? throw new KernelException("无法获取到有效的搜索推荐");
    }

    public async Task<(IReadOnlyList<VideoInformation>, IReadOnlyList<SearchPartition>?, string?)> GetComprehensiveSearchResultAsync(string keyword, string? offset, CancellationToken cancellationToken)
    {
        var req = new SearchAllRequest
        {
            Keyword = keyword,
            Pagination = new Bilibili.Pagination.Pagination
            {
                Next = offset ?? string.Empty,
                PageSize = 20,
            }
        };

        var request = BiliHttpClient.CreateRequest(new Uri(BiliApis.Search.SearchAll), req);
        _authenticator.AuthorizeGrpcRequest(request, false);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync(response, SearchAllResponse.Parser).ConfigureAwait(false);
        var videos = responseObj.Item.Where(p => p.Av is not null).Select(p => p.ToVideoInformation()).ToList().AsReadOnly();
        var partitions = responseObj.Nav?.Select(p => p.ToSearchPartition()).ToList().AsReadOnly();
        return (videos, partitions, responseObj.Pagination?.Next);
    }

    public async Task<(IReadOnlyList<SearchResultItem>, string?)> GetPartitionSearchResultAsync(string keyword, SearchPartition partition, string? offset, CancellationToken cancellationToken)
    {
        var req = new SearchByTypeRequest
        {
            Keyword = keyword,
            Type = partition.Id,
            Pagination = new Bilibili.Pagination.Pagination
            {
                Next = offset ?? string.Empty,
                PageSize = 20,
            }
        };

        var request = BiliHttpClient.CreateRequest(new Uri(BiliApis.Search.SearchByType), req);
        _authenticator.AuthorizeGrpcRequest(request, false);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync(response, SearchByTypeResponse.Parser).ConfigureAwait(false);
        var results = responseObj.Items.Select(p => p.ToSearchResultItem()).Where(p => !p.IsInvalid()).ToList().AsReadOnly();
        return (results, responseObj.Pagination?.Next);
    }
}
