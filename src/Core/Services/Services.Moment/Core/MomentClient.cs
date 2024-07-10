// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bilibili.App.Dynamic.V2;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models.Moment;
using Richasy.BiliKernel.Models.User;

namespace Richasy.BiliKernel.Services.Moment.Core;

internal sealed class MomentClient
{
    private readonly BiliHttpClient _httpClient;
    private readonly BasicAuthenticator _authenticator;

    public MomentClient(
        BiliHttpClient httpClient,
        BasicAuthenticator authenticator)
    {
        _httpClient = httpClient;
        _authenticator = authenticator;
    }

    public async Task<(IReadOnlyList<MomentInformation> Moments, string Offset, bool HasMore)> GetUserMomentsAsync(UserProfile user, string? offset = default, CancellationToken cancellationToken = default)
    {
        var req = new DynSpaceReq
        {
            From = "space",
            HostUid = Convert.ToInt64(user.Id),
            HistoryOffset = offset ?? string.Empty,
        };

        var request = BiliHttpClient.CreateRequest(new Uri(BiliApis.Community.DynamicSpace), req);
        _authenticator.AuthorizeGrpcRequest(request, false);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync(response, DynSpaceRsp.Parser).ConfigureAwait(false);
        var moments = responseObj.List.Select(p => p.ToMomentInformation()).ToList().AsReadOnly();
        return moments.Count == 0
            ? throw new KernelException("没有获取到动态信息，请稍后重试")
            : ((IReadOnlyList<MomentInformation> Moments, string Offset, bool HasMore))(moments, responseObj.HistoryOffset, responseObj.HasMore);
    }
}
