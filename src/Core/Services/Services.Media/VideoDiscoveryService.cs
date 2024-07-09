// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;
using Richasy.BiliKernel.Services.Media.Core;

namespace Richasy.BiliKernel.Services.Media;

/// <summary>
/// 视频分区服务.
/// </summary>
public sealed class VideoDiscoveryService : IVideoDiscoveryService
{
    private readonly VideoDiscoveryClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="VideoDiscoveryService"/> class.
    /// </summary>
    public VideoDiscoveryService(
        BiliHttpClient httpClient,
        BasicAuthenticator authenticator,
        IBiliTokenResolver tokenResolver)
    {
        _client = new VideoDiscoveryClient(httpClient, authenticator, tokenResolver);
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<Partition>> GetVideoPartitionsAsync(CancellationToken cancellationToken)
        => _client.GetVideoPartitionsAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<IReadOnlyList<VideoInformation>> GetPartitionRankingListAsync(Partition partition, CancellationToken cancellationToken)
        => _client.GetPartitionRankingListAsync(partition, cancellationToken);

    /// <inheritdoc/>
    public Task<(IReadOnlyList<VideoInformation> Videos, long Offset)> GetPartitionRecommendVideoListAsync(Partition partition, long offset = 0, CancellationToken cancellationToken = default)
        => _client.GetPartitionRecommendVideoListAsync(partition, offset, cancellationToken);

    /// <inheritdoc/>
    public Task<(IReadOnlyList<VideoInformation> Videos, long Offset, int NextPageNumber)> GetChildPartitionVideoListAsync(Partition childPartition, long offset = 0, int pageNumber = 0, PartitionVideoSortType sortType = PartitionVideoSortType.Default, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 0)
        {
            throw new KernelException("页码不能小于0");
        }

        pageNumber++;
        return _client.GetChildPartitionVideoListAsync(childPartition, offset, pageNumber, sortType, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<VideoInformation>> GetCuratedPlaylistAsync(CancellationToken cancellationToken = default)
        => _client.GetCuratedPlaylistAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<IReadOnlyList<VideoInformation>> GetGlobalRankingListAsync(CancellationToken cancellationToken = default)
        => _client.GetGlobalRankingListAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<(IReadOnlyList<VideoInformation> Items, long Offset)> GetHotVideoListAsync(long offset = 0, CancellationToken cancellationToken = default)
        => _client.GetHotVideoListAsync(offset, cancellationToken);

    /// <inheritdoc/>
    public Task<(IReadOnlyList<VideoInformation> Items, long Offset)> GetRecommendVideoListAsync(long offset = 0, CancellationToken cancellationToken = default)
        => _client.GetRecommendVideoListAsync(offset, cancellationToken);
}
