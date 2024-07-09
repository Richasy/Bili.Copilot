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
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;

namespace Richasy.BiliKernel.Services.Media.Core;

internal sealed class LiveDiscoveryClient
{
    private readonly BiliHttpClient _httpClient;
    private readonly BasicAuthenticator _authenticator;
    private readonly IBiliTokenResolver _tokenResolver;

    public LiveDiscoveryClient(
        BiliHttpClient httpClient,
        BasicAuthenticator authenticator,
        IBiliTokenResolver tokenResolver)
    {
        _httpClient = httpClient;
        _authenticator = authenticator;
        _tokenResolver = tokenResolver;
    }

    public async Task<IReadOnlyList<Partition>> GetLivePartitionsAsync(CancellationToken cancellationToken)
    {
        var request = BiliHttpClient.CreateRequest(System.Net.Http.HttpMethod.Get, new Uri(BiliApis.Live.LiveArea));
        _authenticator.AuthroizeRestRequest(request, settings: new BasicAuthorizeExecutionSettings { RequireToken = false });
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync<BiliDataResponse<LiveAreaResponse>>(response).ConfigureAwait(false);
        return responseObj.Data is null || responseObj.Data.List.Count == 0
            ? throw new KernelException("直播分区数据为空")
            : (IReadOnlyList<Partition>)responseObj.Data.List.Select(p => p.ToPartition()).ToList().AsReadOnly();
    }

    public async Task<(IReadOnlyList<LiveInformation> Lives, IReadOnlyList<LiveTag>? Tags, int NextPageNumber)> GetPartitionDetailAsync(Partition partition, LiveTag? tag = default, int pageNumber = 0, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "page", pageNumber.ToString() },
            { "page_size", "40" },
            { "area_id", partition.Id },
            { "parent_area_id", partition.ParentId },
            { "device", "phone" },
        };

        if (tag != null)
        {
            parameters.Add("sort_type", tag.SortType);
        }

        var request = BiliHttpClient.CreateRequest(System.Net.Http.HttpMethod.Get, new Uri(BiliApis.Live.AreaDetail));
        _authenticator.AuthroizeRestRequest(request, parameters, new BasicAuthorizeExecutionSettings { RequireToken = false });
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync<BiliDataResponse<LiveAreaDetailResponse>>(response).ConfigureAwait(false);
        var lives = responseObj.Data.List.Select(p => p.ToLiveInformation()).ToList().AsReadOnly();
        var tags = responseObj.Data.Tags.Select(p => p.ToLiveTag()).ToList().AsReadOnly();
        if (lives.Count == 0)
        {
            throw new KernelException("无法获取到有效的直播间列表，请稍后再试");
        }

        return (lives, tags, pageNumber);
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

        return recommendList.Count == 0
            ? throw new KernelException("直播信息流没有返回数据")
            : ((IReadOnlyList<LiveInformation>? Follow, IReadOnlyList<LiveInformation>? Recommend, int NextPageNumber))(followList, recommendList, pageNumber);
    }
}
