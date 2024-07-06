// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Content;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models.Media;

namespace Richasy.BiliKernel.Services.Media.Core;

internal sealed class ViewLaterClient
{
    private readonly BiliHttpClient _httpClient;
    private readonly IAuthenticationService _authenticationService;
    private readonly IBiliTokenResolver _tokenResolver;
    private readonly BasicAuthenticator _authenticator;

    public ViewLaterClient(
        BiliHttpClient httpClient,
        IAuthenticationService authenticationService,
        IBiliTokenResolver tokenResolver,
        BasicAuthenticator basicAuthenticator)
    {
        _httpClient = httpClient;
        _authenticationService = authenticationService;
        _tokenResolver = tokenResolver;
        _authenticator = basicAuthenticator;
    }

    public async Task<(IReadOnlyList<VideoInformation> Videos, int Count)> GetMyViewLaterAsync(int page = 0, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "pn", page.ToString() },
            { "ps", "40" },
        };

        var responseObj = await GetAsync<BiliDataResponse<ViewLaterSetResponse>>(BiliApis.Account.ViewLaterList, parameters, cancellationToken).ConfigureAwait(false);
        var videos = responseObj.Data?.List?.Select(p => p.ToVideoInformation()).ToList().AsReadOnly()
            ?? throw new KernelException("无法获取稍后再看视频数据");
        return (videos, responseObj.Data.Count);
    }

    private async Task<T> GetAsync<T>(string url, Dictionary<string, string>? paramters = default, CancellationToken cancellationToken = default)
    {
        await _authenticationService.EnsureTokenAsync(cancellationToken).ConfigureAwait(false);
        var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new Uri(url));
        _authenticator.AuthroizeRestRequest(request, paramters);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        return await BiliHttpClient.ParseAsync<T>(response).ConfigureAwait(false);
    }
}
