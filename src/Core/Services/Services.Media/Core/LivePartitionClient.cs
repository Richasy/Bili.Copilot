// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili;
using Richasy.BiliKernel.Content;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;

namespace Richasy.BiliKernel.Services.Media.Core;

internal sealed class LivePartitionClient
{
    private readonly BiliHttpClient _httpClient;
    private readonly BasicAuthenticator _authenticator;

    public LivePartitionClient(
        BiliHttpClient httpClient,
        BasicAuthenticator authenticator)
    {
        _httpClient = httpClient;
        _authenticator = authenticator;
    }

    public async Task<IReadOnlyList<Partition>> GetLivePartitionsAsync(CancellationToken cancellationToken)
    {
        var request = BiliHttpClient.CreateRequest(System.Net.Http.HttpMethod.Get, new Uri(BiliApis.Live.LiveArea));
        _authenticator.AuthroizeRestRequest(request);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync<BiliDataResponse<LiveAreaResponse>>(response).ConfigureAwait(false);
        return responseObj.Data.List.Select(p => p.ToPartition()).ToList().AsReadOnly()
            ?? throw new KernelException("直播分区数据为空");
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
        _authenticator.AuthroizeRestRequest(request, parameters);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync<BiliDataResponse<LiveAreaDetailResponse>>(response).ConfigureAwait(false);
        var lives = responseObj.Data.List.Select(p => p.ToLiveInformation()).ToList().AsReadOnly();
        var tags = responseObj.Data.Tags.Select(p => p.ToLiveTag()).ToList().AsReadOnly();
        return (lives, tags, pageNumber);
    }
}
