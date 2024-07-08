// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Content;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models.Media;

namespace Richasy.BiliKernel.Services.Media.Core;

internal sealed class PopularLiveClient
{
    private readonly BiliHttpClient _httpClient;
    private readonly IBiliTokenResolver _tokenResolver;
    private readonly BasicAuthenticator _authenticator;

    public PopularLiveClient(
        BiliHttpClient httpClient,
        IBiliTokenResolver tokenResolver,
        BasicAuthenticator authenticator)
    {
        _httpClient = httpClient;
        _authenticator = authenticator;
        _tokenResolver = tokenResolver;
    }

    public async Task<(IReadOnlyList<LiveInformation>? Follow, IReadOnlyList<LiveInformation>? Recommend, int NextPageNumber)> GetFeedAsync(int pageNumber, CancellationToken cancellationToken)
    {
        var localToken = _tokenResolver.GetToken();
        var isLogin = localToken is not null;
        var parameters = new Dictionary<string, string>
        {
            { "page", pageNumber.ToString() },
            { "relation_page", pageNumber.ToString() },
            { "scale", "2" },
            { "login_event", isLogin ? "2" : "1" },
            { "device", "phone" },
        };

        var request = BiliHttpClient.CreateRequest(System.Net.Http.HttpMethod.Get, new Uri(BiliApis.Live.LiveFeed));
        _authenticator.AuthroizeRestRequest(request, parameters);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync<BiliDataResponse<LiveFeedResponse>>(response).ConfigureAwait(false);
        var followList = responseObj.Data.CardList
            .Where(p => p.CardType.Contains("idol"))
            .SelectMany(p => p.CardData?.FollowList?.List)
            .Select(p => p.ToLiveInformation()).ToList().AsReadOnly();
        var recommendList = responseObj.Data.CardList
            .Where(p => p.CardType.Contains("small_card") && p.CardData?.LiveCard != null)
            .Select(p => p.CardData.LiveCard.ToLiveInformation()).ToList().AsReadOnly();
        return (followList, recommendList, pageNumber);
    }
}
