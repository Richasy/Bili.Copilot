// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bilibili.App.Interface.V1;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Content;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;

namespace Richasy.BiliKernel.Services.Media.Core;

internal sealed class ViewHistoryClient
{
    private readonly BiliHttpClient _httpClient;
    private readonly IAuthenticationService _authenticationService;
    private readonly BasicAuthenticator _authenticator;

    public ViewHistoryClient(
        BiliHttpClient httpClient,
        IAuthenticationService authenticationService,
        BasicAuthenticator authenticator)
    {
        _httpClient = httpClient;
        _authenticationService = authenticationService;
        _authenticator = authenticator;
    }

    public async Task<ViewHistoryGroup> GetHistorySetAsync(ViewHistoryTabType tabType, long offset, CancellationToken cancellationToken)
    {
        var business = GetBusiness(tabType);
        var req = new CursorReq
        {
            Business = business,
            Cursor = new Cursor { Max = offset }
        };

        var request = BiliHttpClient.CreateRequest(new Uri(BiliApis.Account.HistoryCursor), req);
        _authenticator.AuthorizeGrpcRequest(request);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var data = await BiliHttpClient.ParseAsync(response, CursorV2Reply.Parser).ConfigureAwait(false);
        var nextOffset = data.Cursor.Max;
        var videos = data.Items.Where(p => p.CardItemCase == CursorItem.CardItemOneofCase.CardUgc)
            .Select(p => p.ToVideoInformation()).ToList().AsReadOnly();
        var episodes = data.Items.Where(p => p.CardItemCase == CursorItem.CardItemOneofCase.CardOgv)
            .Select(p => p.ToEpisodeInformation()).ToList().AsReadOnly();
        var lives = data.Items.Where(p => p.CardItemCase == CursorItem.CardItemOneofCase.CardLive)
            .Select(p => p.ToLiveInformation()).ToList().AsReadOnly();
        var articles = data.Items.Where(p => p.CardItemCase == CursorItem.CardItemOneofCase.CardArticle)
            .Select(p => p.ToArticleInformation()).ToList().AsReadOnly();
        return new ViewHistoryGroup(tabType, videos, episodes, lives, articles, nextOffset);
    }

    public async Task RemoveHistoryItemAsync(string itemId, ViewHistoryTabType tab, CancellationToken cancellationToken)
    {
        var business = GetBusiness(tab);
        var req = new DeleteReq
        {
            HisInfo = new HisInfo
            {
                Business = business,
                Kid = Convert.ToInt64(itemId),
            }
        };

        var request = BiliHttpClient.CreateRequest(new Uri(BiliApis.Account.DeleteHistoryItem), req);
        _authenticator.AuthorizeGrpcRequest(request);
        await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task CleanHistoryAsync(ViewHistoryTabType tab, CancellationToken cancellationToken)
    {
        var business = GetBusiness(tab);
        var req = new ClearReq
        {
            Business = business,
        };

        var request = BiliHttpClient.CreateRequest(new Uri(BiliApis.Account.ClearHistory), req);
        _authenticator.AuthorizeGrpcRequest(request);
        await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> GetIsHistoryEnabledAsync(CancellationToken cancellationToken)
    {
        var request = BiliHttpClient.CreateRequest(System.Net.Http.HttpMethod.Get, new Uri(BiliApis.Account.HistoryRecordOption));
        _authenticator.AuthroizeRestRequest(request, default, new BasicAuthorizeExecutionSettings { ApiType = BiliApiType.Web, RequireCookie = true, ForceNoToken = true });
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync<BiliDataResponse<bool>>(response).ConfigureAwait(false);
        return !responseObj.Data;
    }

    public async Task SetHistoryRecordOptionAsync(bool stopRecord, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "switch", stopRecord.ToString().ToLower() },
        };

        var request = BiliHttpClient.CreateRequest(System.Net.Http.HttpMethod.Post, new Uri(BiliApis.Account.SetHistoryRecordOption));
        _authenticator.AuthroizeRestRequest(request, parameters, new BasicAuthorizeExecutionSettings { ApiType = BiliApiType.Web, NeedCSRF = true, RequireCookie = true, ForceNoToken = true });
        await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    private static string GetBusiness(ViewHistoryTabType tabType)
    {
        return tabType switch
        {
            ViewHistoryTabType.Video => "archive",
            ViewHistoryTabType.Article => "article",
            ViewHistoryTabType.Live => "live",
            ViewHistoryTabType.Pgc => "pgc",
            _ => "all",
        };
    }
}
