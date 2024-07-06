// Copyright (c) Richasy. All rights reserved.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bilibili.App.Interface.V1;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili;
using Richasy.BiliKernel.Bili.Authorization;
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
        var business = tabType switch
        {
            ViewHistoryTabType.Video => "archive",
            ViewHistoryTabType.Article => "article",
            ViewHistoryTabType.Live => "live",
            _ => "all",
        };

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
}
