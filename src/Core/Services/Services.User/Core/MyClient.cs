// Copyright (c) Richasy. All rights reserved.

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Content;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models.User;

namespace Richasy.BiliKernel.Services.User.Core;

internal sealed class MyClient
{
    private readonly BiliHttpClient _httpClient;
    private readonly IAuthenticationService _authenticationService;
    private readonly BasicAuthenticator _authenticator;

    public MyClient(
        BiliHttpClient httpClient,
        IAuthenticationService authenticationService,
        BasicAuthenticator basicAuthenticator)
    {
        _httpClient = httpClient;
        _authenticationService = authenticationService;
        _authenticator = basicAuthenticator;
    }

    public async Task<UserDetailProfile> GetMyInformationAsync(CancellationToken cancellationToken)
    {
        await _authenticationService.EnsureTokenAsync(cancellationToken).ConfigureAwait(false);
        var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new Uri(BiliApis.Account.MyInfo));
        _authenticator.AuthroizeRequest(request);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync<BiliDataResponse<MyInfo>>(response).ConfigureAwait(false);
        var info = responseObj.Data;
        return info == null || string.IsNullOrEmpty(info.Name)
            ? throw new KernelException("返回的用户数据为空")
            : info.ToUserDetailProfile(48d);
    }
}
