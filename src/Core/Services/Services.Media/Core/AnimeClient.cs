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
using Richasy.BiliKernel.Models.Media;
using Richasy.BiliKernel.Services.Media.Core.Models;

namespace Richasy.BiliKernel.Services.Media.Core;

/// <summary>
/// 动漫客户端.
/// </summary>
internal sealed class AnimeClient
{
    private readonly BiliHttpClient _httpClient;
    private readonly BasicAuthenticator _authenticator;

    public AnimeClient(
        BiliHttpClient httpClient,
        BasicAuthenticator authenticator)
    {
        _httpClient = httpClient;
        _authenticator = authenticator;
    }

    public async Task<(string Title, string Description, IReadOnlyList<TimelineInformation>)> GetAnimeTimelineAsync(bool isBangumi, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "type", isBangumi ? "2" : "3" },
            { "filter_type", "0" },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new Uri(BiliApis.Pgc.TimeLine));
        _authenticator.AuthroizeRestRequest(request, parameters, new BasicAuthorizeExecutionSettings { RequireToken = false });
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync<BiliResultResponse<PgcTimeLineResponse>>(response).ConfigureAwait(false);
        var title = responseObj.Result.Title;
        var description = responseObj.Result.Subtitle;
        var timelineItems = responseObj.Result.Data.Select(p => p.ToTimelineInformation()).ToList().AsReadOnly();
        return (title!, description!, timelineItems);
    }
}
