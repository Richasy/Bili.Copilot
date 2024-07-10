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

    public async Task<MomentView> GetComprehensiveMomentsAsync(string? offset = default, string? baseline = default, CancellationToken cancellationToken = default)
    {
        var type = string.IsNullOrEmpty(offset) ? Refresh.New : Refresh.History;
        var req = new DynAllReq
        {
            RefreshType = type,
            Offset = offset ?? string.Empty,
            UpdateBaseline = baseline ?? string.Empty,
        };

        var request = BiliHttpClient.CreateRequest(new Uri(BiliApis.Community.DynamicAll), req);
        _authenticator.AuthorizeGrpcRequest(request);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync(response, DynAllReply.Parser).ConfigureAwait(false);
        var moments = responseObj.DynamicList.List.Select(p => p.ToMomentInformation()).ToList().AsReadOnly();
        var nextOffset = responseObj.DynamicList?.HistoryOffset;
        var nextBaseline = responseObj.DynamicList?.UpdateBaseline;
        var ups = responseObj.UpList?.List.Select(p => p.ToMomentProfile()).ToList().AsReadOnly();
        var hasMore = responseObj.DynamicList?.HasMore;
        return new MomentView(moments, ups, nextOffset, nextBaseline, hasMore);
    }

    public async Task<MomentView> GetVideoMomentsAsync(string? offset = default, string? baseline = default, CancellationToken cancellationToken = default)
    {
        var type = string.IsNullOrEmpty(offset) ? Refresh.New : Refresh.History;
        var req = new DynVideoReq
        {
            RefreshType = type,
            Offset = offset ?? string.Empty,
            UpdateBaseline = baseline ?? string.Empty,
        };

        var request = BiliHttpClient.CreateRequest(new Uri(BiliApis.Community.DynamicVideo), req);
        _authenticator.AuthorizeGrpcRequest(request);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync(response, DynVideoReply.Parser).ConfigureAwait(false);
        var moments = responseObj.DynamicList.List.Where(p =>
            p.CardType is DynamicType.Av
            or DynamicType.Pgc
            or DynamicType.UgcSeason)
            .Select(p => p.ToMomentInformation())
            .ToList()
            .AsReadOnly();
        var nextOffset = responseObj.DynamicList?.HistoryOffset;
        var nextBaseline = responseObj.DynamicList?.UpdateBaseline;
        var ups = responseObj.VideoUpList?.List.Select(p => p.ToMomentProfile()).ToList().AsReadOnly();
        var hasMore = responseObj.DynamicList?.HasMore;
        return new MomentView(moments, ups, nextOffset, nextBaseline, hasMore);
    }

    public async Task LikeMomentAsync(MomentInformation moment, bool isLike, CancellationToken cancellationToken)
    {
        var req = new DynThumbReq
        {
            Type = isLike ? ThumbType.Thumb: ThumbType.Cancel,
            DynId = Convert.ToInt64(moment.Id),
            Uid = Convert.ToInt64(moment.User.Id),
        };

        var request = BiliHttpClient.CreateRequest(new Uri(BiliApis.Community.LikeDynamic), req);
        _authenticator.AuthorizeGrpcRequest(request);
        await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task MarkAsReadAsync(string userId, string? offset, CancellationToken cancellationToken)
    {
        var req = new DynAllUpdOffsetReq
        {
            HostUid = Convert.ToInt64(userId),
            ReadOffset = offset ?? string.Empty,
        };

        var request = BiliHttpClient.CreateRequest(new Uri(BiliApis.Community.DynamicSpaceMarkRead), req);
        _authenticator.AuthorizeGrpcRequest(request);
        await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
