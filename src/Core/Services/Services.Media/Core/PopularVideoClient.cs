// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bilibili.App.Show.V1;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Content;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models.Media;
using Richasy.BiliKernel.Services.Media.Core.Models;

namespace Richasy.BiliKernel.Services.Media.Core;

internal sealed class PopularVideoClient
{
    private readonly BiliHttpClient _httpClient;
    private readonly IBiliTokenResolver _tokenResolver;
    private readonly BasicAuthenticator _authenticator;

    public PopularVideoClient(
        BiliHttpClient httpClient,
        IBiliTokenResolver tokenResolver,
        BasicAuthenticator authenticator)
    {
        _httpClient = httpClient;
        _authenticator = authenticator;
        _tokenResolver = tokenResolver;
    }

    public async Task<IReadOnlyList<VideoInformation>> GetCuratedPlaylistAsync(CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "fresh_idx", "1" },
            { "ps", "10" },
            { "plat", "1" },
            { "feed_version", "CLIENT_SELECTED" },
            { "fresh_type", "0" },
            { "web_location", "bilibili-electron" },
        };

        var request = BiliHttpClient.CreateRequest(System.Net.Http.HttpMethod.Get, new Uri(BiliApis.Home.CuratedPlaylist));
        _authenticator.AuthroizeRestRequest(request, parameters, new BasicAuthorizeExecutionSettings { NeedCSRF = true, RequireCookie = true, ForceNoToken = true });
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync<BiliDataResponse<CuratedPlaylistResponse>>(response).ConfigureAwait(false);
        return responseObj.Data.Items.Select(p => p.ToVideoInformation()).ToList().AsReadOnly();
    }

    public async Task<(IReadOnlyList<VideoInformation> Videos, long Offset)> GetRecommendVideoListAsync(long offset, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "idx", offset.ToString() },
            { "flush", "5" },
            { "column", "4" },
            { "device", "desktop" },
            { "pull", (offset == 0).ToString().ToLower() },
        };

        var request = BiliHttpClient.CreateRequest(System.Net.Http.HttpMethod.Get, new Uri(BiliApis.Home.Recommend));
        _authenticator.AuthroizeRestRequest(request, parameters);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObject = await BiliHttpClient.ParseAsync<BiliDataResponse<RecommendVideoResponse>>(response).ConfigureAwait(false);
        var nextOffset = responseObject.Data.Items.Last().Idx;
        var videos = responseObject.Data.Items.Where(p => p.CardGoto is "av")
            .Select(p => p.ToVideoInformation()).ToList().AsReadOnly();
        return (videos, nextOffset);
    }

    public async Task<(IReadOnlyList<VideoInformation> Videos, long Offset)> GetHotVideoListAsync(long offset, CancellationToken cancellationToken)
    {
        var localToken = _tokenResolver.GetToken();
        var isLogin = localToken is not null;
        var req = new PopularResultReq()
        {
            Idx = offset,
            LoginEvent = isLogin ? 2 : 1,
        };

        var request = BiliHttpClient.CreateRequest(new Uri(BiliApis.Home.PopularGRPC), req);
        _authenticator.AuthorizeGrpcRequest(request, false);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync(response, PopularReply.Parser).ConfigureAwait(false);
        var videos = responseObj.Items
            .Where(p => p.ItemCase == Bilibili.App.Card.V1.Card.ItemOneofCase.SmallCoverV5
                && p.SmallCoverV5 != null
                && p.SmallCoverV5.Base.CardGoto == "av"
                && !p.SmallCoverV5.Base.Uri.Contains("bangumi"));
        var nextOffset = videos.Last().SmallCoverV5.Base.Idx;
        return (videos.Select(p => p.ToVideoInformation()).ToList().AsReadOnly(), nextOffset);
    }

    public async Task<IReadOnlyList<VideoInformation>> GetGlobalRankingListAsync(CancellationToken cancellationToken)
    {
        var req = new RankRegionResultReq
        {
            Rid = 0,
        };
        var request = BiliHttpClient.CreateRequest(new Uri(BiliApis.Home.RankingGRPC), req);
        _authenticator.AuthorizeGrpcRequest(request, false);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync(response, RankListReply.Parser).ConfigureAwait(false);
        return responseObj.Items.Select(p => p.ToVideoInformation()).ToList().AsReadOnly();
    }
}
