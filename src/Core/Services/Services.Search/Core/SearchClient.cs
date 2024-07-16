// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili;
using Richasy.BiliKernel.Content;
using Richasy.BiliKernel.Http;
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

    public async Task<IReadOnlyList<HotSearchItem>> GetTotalHotSearchAsync(CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "limit", "50" },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new Uri(BiliApis.Search.HotSearch));
        _authenticator.AuthroizeRestRequest(request, parameters, new BasicAuthorizeExecutionSettings {  RequireToken = false, ForceNoToken = true, NeedCSRF = true });
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync<BiliDataResponse<HotSearchResponse>>(response).ConfigureAwait(false);
        return responseObj.Data.List.Select(p => p.ToHotSearchItem()).ToList().AsReadOnly()
            ?? throw new KernelException("无法获取到有效的热搜榜单");
    }
}
