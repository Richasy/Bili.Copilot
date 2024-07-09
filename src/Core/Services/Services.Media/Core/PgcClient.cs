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
using Richasy.BiliKernel.Models.Appearance;
using Richasy.BiliKernel.Models.Media;
using Richasy.BiliKernel.Services.Media.Core.Models;

namespace Richasy.BiliKernel.Services.Media.Core;

/// <summary>
/// 动漫客户端.
/// </summary>
internal sealed class PgcClient
{
    private readonly BiliHttpClient _httpClient;
    private readonly BasicAuthenticator _authenticator;

    public PgcClient(
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

    public async Task<IReadOnlyList<Filter>> GetAnimeFiltersAsync(CancellationToken cancellationToken)
    {
        var parameters = GetAnimeIndexParameters();
        var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new Uri(BiliApis.Pgc.IndexCondition));
        _authenticator.AuthroizeRestRequest(request, parameters, new BasicAuthorizeExecutionSettings { RequireToken = false });
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync<BiliDataResponse<PgcIndexConditionResponse>>(response).ConfigureAwait(false);
        var filters = responseObj.Data.FilterList.Select(p => p.ToFilter()).ToList();
        if (responseObj.Data?.OrderList?.Count > 0)
        {
            var orderFilter = new Filter("排序方式", "order", responseObj.Data.OrderList.Select(p => new Condition(p.Name, p.Field)).ToList());
            filters.Insert(0, orderFilter);
        }

        return filters.AsReadOnly();
    }

    public async Task<(IReadOnlyList<SeasonInformation> Seasons, bool HasNext)> GetAnimeIndexWithFiltersAsync(Dictionary<Filter, Condition>? filters, int page, CancellationToken cancellationToken)
    {
        var parameters = GetAnimeIndexParameters();
        parameters.Add("pagesize", "30");
        parameters.Add("page", page.ToString());
        if (filters != null)
        {
            foreach (var item in filters)
            {
                parameters.Add(item.Key.Id, item.Value.Id);
            }
        }

        var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new Uri(BiliApis.Pgc.IndexResult));
        _authenticator.AuthroizeRestRequest(request, parameters, new BasicAuthorizeExecutionSettings { RequireToken = false });
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync<BiliDataResponse<PgcIndexResultResponse>>(response).ConfigureAwait(false);
        var seasons = responseObj.Data.List.Select(p => p.ToSeasonInformation()).ToList().AsReadOnly();
        return (seasons, responseObj.Data.HasNext != 0);
    }

    public async Task ToggleFollowAsync(string seasonId, bool isFollow, CancellationToken cancellationToken)
    {
        var paramters = new Dictionary<string, string>
        {
            { "season_id", seasonId }
        };

        var url = isFollow ? BiliApis.Pgc.Follow : BiliApis.Pgc.Unfollow;
        var request = BiliHttpClient.CreateRequest(HttpMethod.Post, new Uri(url));
        _authenticator.AuthroizeRestRequest(request, paramters);
        await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    private static Dictionary<string, string> GetAnimeIndexParameters()
    {
        return new Dictionary<string, string>
        {
            { "season_type", "1" },
            { "type", "0" }
        };
    }
}
