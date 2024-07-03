// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Content;
using Richasy.BiliKernel.Http;

namespace Richasy.BiliKernel.Authorizers.Tv.Core;

internal sealed class TvAuthorizeClient
{
    private readonly string _localId;
    private readonly BiliHttpClient _httpClient;
    private readonly ILocalBiliCookiesResolver _localCookiesResolver;
    private readonly ILocalBiliTokenResolver _localTokenResolver;
    private readonly BasicAuthenticator _basicAuthenticator;

    public TvAuthorizeClient(
        BiliHttpClient httpClient,
        ILocalBiliCookiesResolver localCookiesResolver,
        ILocalBiliTokenResolver localTokenResolver,
        BasicAuthenticator basicAuthenticator)
    {
        _localId = Guid.NewGuid().ToString("N");
        _httpClient = httpClient;
        _localCookiesResolver = localCookiesResolver;
        _localTokenResolver = localTokenResolver;
        _basicAuthenticator = basicAuthenticator;
    }

    public async Task<TvQrCode> GetQRCodeAsync(CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "local_id", _localId },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Post, new Uri(BiliApis.Passport.QRCode));
        _basicAuthenticator.AuthroizeRequest(request, parameters, new BasicAuthorizeExecutionSettings() { ForceNoToken = true });
        var response = await _httpClient.SendAsync(request, cancellationToken);
        var responseObj = await BiliHttpClient.ParseAsync<BiliDataResponse<TvQrCode>>(response);
        return responseObj.Data;
    }
}
