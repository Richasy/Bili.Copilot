﻿// Copyright (c) Richasy. All rights reserved.

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
using Richasy.BiliKernel.Models;
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
        if (timelineItems.Count == 0)
        {
            var name = isBangumi ? "番剧" : "国创";
            throw new KernelException($"{name}时间线数据为空");
        }

        return (title!, description!, timelineItems);
    }

    public Task<IReadOnlyList<Filter>> GetEntertainmentFiltersAsync(EntertainmentType type, CancellationToken cancellationToken)
    {
        var parameters = type == EntertainmentType.Anime ? GetAnimeIndexParameters() : GetEntertainmentIndexParameters(type);
        return GetPgcIndexFiltersAsync(parameters, cancellationToken);
    }

    public Task<(IReadOnlyList<SeasonInformation> Seasons, bool HasNext)> GetEntertainmentIndexWithFiltersAsync(EntertainmentType type, Dictionary<Filter, Condition>? filters, int page, CancellationToken cancellationToken)
    {
        var parameters = type == EntertainmentType.Anime ? GetAnimeIndexParameters() : GetEntertainmentIndexParameters(type);
        return GetPgcSeasonsWithFiltersAsync(parameters, filters, page, cancellationToken);
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

    private async Task<IReadOnlyList<Filter>> GetPgcIndexFiltersAsync(Dictionary<string, string> parameters, CancellationToken cancellationToken)
    {
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

        return filters.Count == 0 ? throw new KernelException("筛选条件为空") : (IReadOnlyList<Filter>)filters.AsReadOnly();
    }

    private async Task<(IReadOnlyList<SeasonInformation> Seasons, bool HasNext)> GetPgcSeasonsWithFiltersAsync(Dictionary<string, string> parameters, Dictionary<Filter, Condition>? filters, int page, CancellationToken cancellationToken)
    {
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
        return seasons.Count == 0
            ? throw new KernelException("无法获取到有效的剧集列表，请检查参数或稍后重试")
            : ((IReadOnlyList<SeasonInformation> Seasons, bool HasNext))(seasons, responseObj.Data.HasNext != 0);
    }

    private static Dictionary<string, string> GetAnimeIndexParameters()
    {
        return new Dictionary<string, string>
        {
            { "season_type", "1" },
            { "type", "0" }
        };
    }

    private static Dictionary<string, string> GetEntertainmentIndexParameters(EntertainmentType type)
    {
        var indexType = type switch
        {
            EntertainmentType.Movie => "2",
            EntertainmentType.TV => "5",
            EntertainmentType.Documentary => "3",
            _ => throw new NotSupportedException()
        };

        return new Dictionary<string, string>
        {
            { "index_type", indexType },
            { "type", "0" }
        };
    }
}
