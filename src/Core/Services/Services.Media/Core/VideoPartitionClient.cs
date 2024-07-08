// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bilibili.App.Show.V1;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili;
using Richasy.BiliKernel.Content;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;

namespace Richasy.BiliKernel.Services.Media.Core;

internal sealed class VideoPartitionClient
{
    private readonly BiliHttpClient _httpClient;
    private readonly BasicAuthenticator _authenticator;

    public VideoPartitionClient(
        BiliHttpClient httpClient,
        BasicAuthenticator authenticator)
    {
        _httpClient = httpClient;
        _authenticator = authenticator;
    }

    public async Task<IReadOnlyList<Partition>> GetVideoPartitionsAsync(CancellationToken cancellationToken)
    {
        var request = BiliHttpClient.CreateRequest(System.Net.Http.HttpMethod.Get, new Uri(BiliApis.Partition.PartitionIndex));
        _authenticator.AuthroizeRestRequest(request);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync<BiliDataResponse<List<VideoPartition>>>(response).ConfigureAwait(false);
        var items = responseObj.Data.Where(p => p.IsNeedToShow()).Select(p => p.ToPartition()).ToList();
        return items.AsReadOnly();
    }

    public async Task<IReadOnlyList<VideoInformation>> GetPartitionRankingListAsync(Partition partition, CancellationToken cancellationToken)
    {
        var req = new RankRegionResultReq
        {
            Rid = Convert.ToInt32(partition.Id),
        };
        var request = BiliHttpClient.CreateRequest(new Uri(BiliApis.Home.RankingGRPC), req);
        _authenticator.AuthorizeGrpcRequest(request);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync(response, RankListReply.Parser).ConfigureAwait(false);
        return responseObj.Items.Select(p => p.ToVideoInformation()).ToList().AsReadOnly();
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
        _authenticator.AuthroizeRestRequest(request, parameters);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var data = (await BiliHttpClient.ParseAsync<BiliDataResponse<SubPartition>>(response).ConfigureAwait(false)).Data;
        var videos = data.NewVideos
            .Concat(data.RecommendVideos ?? new List<PartitionVideo>())
            .Distinct()
            .Select(p => p.ToVideoInformation());

        var offsetId = data.BottomOffsetId;
        return (videos.ToList().AsReadOnly(), offsetId);
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
        _authenticator.AuthroizeRestRequest(request, parameters);
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
        var nextPageNumber = !isDefaultOrder ? pageNumber + 1 : 1;
        return (videos.ToList().AsReadOnly(), offsetId, nextPageNumber);
    }
}
