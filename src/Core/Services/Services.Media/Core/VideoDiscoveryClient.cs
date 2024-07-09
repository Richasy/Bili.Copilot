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
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;
using Richasy.BiliKernel.Services.Media.Core.Models;

namespace Richasy.BiliKernel.Services.Media.Core;

internal sealed class VideoDiscoveryClient
{
    private readonly BiliHttpClient _httpClient;
    private readonly BasicAuthenticator _authenticator;
    private readonly IBiliTokenResolver _tokenResolver;

    public VideoDiscoveryClient(
        BiliHttpClient httpClient,
        BasicAuthenticator authenticator,
        IBiliTokenResolver tokenResolver)
    {
        _httpClient = httpClient;
        _authenticator = authenticator;
        _tokenResolver = tokenResolver;
    }

    public async Task<IReadOnlyList<Partition>> GetVideoPartitionsAsync(CancellationToken cancellationToken)
    {
        var request = BiliHttpClient.CreateRequest(System.Net.Http.HttpMethod.Get, new Uri(BiliApis.Partition.PartitionIndex));
        _authenticator.AuthroizeRestRequest(request, settings: new BasicAuthorizeExecutionSettings { RequireToken = false });
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync<BiliDataResponse<List<VideoPartition>>>(response).ConfigureAwait(false);
        var items = responseObj.Data.Where(p => p.IsNeedToShow()).Select(p => p.ToPartition()).ToList();
        return items.Count == 0
            ? throw new KernelException("视频分区数据为空")
            : (IReadOnlyList<Partition>)items.AsReadOnly();
    }

    public async Task<IReadOnlyList<VideoInformation>> GetPartitionRankingListAsync(Partition partition, CancellationToken cancellationToken)
    {
        var req = new RankRegionResultReq
        {
            Rid = Convert.ToInt32(partition.Id),
        };
        var request = BiliHttpClient.CreateRequest(new Uri(BiliApis.Home.RankingGRPC), req);
        _authenticator.AuthorizeGrpcRequest(request, false);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync(response, RankListReply.Parser).ConfigureAwait(false);
        return responseObj.Items is null || responseObj.Items.Count == 0
            ? throw new KernelException("无法获取到有效的视频列表，请稍后再试")
            : (IReadOnlyList<VideoInformation>)responseObj.Items.Select(p => p.ToVideoInformation()).ToList().AsReadOnly();
    }

    public async Task<(IReadOnlyList<VideoInformation> Videos, long Offset)> GetPartitionRecommendVideoListAsync(Partition partition, long offset, CancellationToken cancellationToken)
    {
        var isOffset = offset > 0;
        var url = isOffset ? BiliApis.Partition.SubPartitionRecommendOffset : BiliApis.Partition.SubPartitionRecommend;
        var parameters = new Dictionary<string, string>
        {
            { "rid", partition.Id },
            { "pull", "0" },
        };

        if (isOffset)
        {
            parameters.Add("ctime", offset.ToString());
        }

        var request = BiliHttpClient.CreateRequest(System.Net.Http.HttpMethod.Get, new Uri(url));
        _authenticator.AuthroizeRestRequest(request, parameters, new BasicAuthorizeExecutionSettings { RequireToken = false });
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var data = (await BiliHttpClient.ParseAsync<BiliDataResponse<SubPartition>>(response).ConfigureAwait(false)).Data;
        var videos = data.NewVideos
            .Concat(data.RecommendVideos ?? new List<PartitionVideo>())
            .Distinct()
            .Select(p => p.ToVideoInformation());

        var offsetId = data.BottomOffsetId;
        return videos.Count() == 0
            ? throw new KernelException("无法获取到有效的视频列表，请稍后再试")
            : ((IReadOnlyList<VideoInformation> Videos, long Offset))(videos.ToList().AsReadOnly(), offsetId);
    }

    public async Task<(IReadOnlyList<VideoInformation> Videos, long Offset, int NextPageNumber)> GetChildPartitionVideoListAsync(Partition partition, long offset, int pageNumber, PartitionVideoSortType sort, CancellationToken cancellationToken)
    {
        var isDefaultOrder = sort == PartitionVideoSortType.Default;
        var isOffset = offset > 0;
        var url = isDefaultOrder
                ? isOffset ? BiliApis.Partition.SubPartitionNormalOffset : BiliApis.Partition.SubPartitionNormal
                : BiliApis.Partition.SubPartitionOrderOffset;
        var parameters = new Dictionary<string, string>
        {
            { "rid", partition.Id },
            { "pull", "0" },
        };

        if (isOffset)
        {
            parameters.Add("ctime", offset.ToString());
        }

        if (!isDefaultOrder)
        {
            var sortStr = sort switch
            {
                PartitionVideoSortType.Newest => "senddate",
                PartitionVideoSortType.Play => "view",
                PartitionVideoSortType.Danmaku => "danmaku",
                PartitionVideoSortType.Favorite => "favorite",
                _ => default,
            };

            parameters.Add("order", sortStr);
            parameters.Add("pn", pageNumber.ToString());
            parameters.Add("ps", "30");
        }

        var request = BiliHttpClient.CreateRequest(System.Net.Http.HttpMethod.Get, new Uri(url));
        _authenticator.AuthroizeRestRequest(request, parameters, new BasicAuthorizeExecutionSettings { RequireToken = false });
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        SubPartition data;
        if (!isDefaultOrder)
        {
            var videoList = (await BiliHttpClient.ParseAsync<BiliDataResponse<List<PartitionVideo>>>(response).ConfigureAwait(false)).Data;
            data = new SubPartition
            {
                NewVideos = videoList,
            };
        }
        else
        {
            data = (await BiliHttpClient.ParseAsync<BiliDataResponse<SubPartition>>(response).ConfigureAwait(false)).Data;
        }

        var videos = data.NewVideos
            .Concat(data.RecommendVideos ?? new List<PartitionVideo>())
            .Distinct()
            .Select(p => p.ToVideoInformation());

        var offsetId = data.BottomOffsetId;
        var nextPageNumber = !isDefaultOrder ? pageNumber : 1;

        return videos.Count() == 0
            ? throw new KernelException("无法获取到有效的视频列表，请稍后再试")
            : ((IReadOnlyList<VideoInformation> Videos, long Offset, int NextPageNumber))(videos.ToList().AsReadOnly(), offsetId, nextPageNumber);
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
        return responseObj.Data is null || responseObj.Data.Items is null || responseObj.Data.Items.Count == 0
            ? throw new KernelException("精选视频数据为空")
            : (IReadOnlyList<VideoInformation>)responseObj.Data.Items.Select(p => p.ToVideoInformation()).ToList().AsReadOnly();
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
        return videos.Count == 0
            ? throw new KernelException("推荐视频数据为空")
            : ((IReadOnlyList<VideoInformation> Videos, long Offset))(videos, nextOffset);
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
        return videos.Count() == 0
            ? throw new KernelException("热门视频数据为空")
            : ((IReadOnlyList<VideoInformation> Videos, long Offset))(videos.Select(p => p.ToVideoInformation()).ToList().AsReadOnly(), nextOffset);
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
        return responseObj.Items is null || responseObj.Items.Count == 0
            ? throw new KernelException("全站排行榜数据为空")
            : (IReadOnlyList<VideoInformation>)responseObj.Items.Select(p => p.ToVideoInformation()).ToList().AsReadOnly();
    }
}
