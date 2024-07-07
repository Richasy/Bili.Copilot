﻿// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Authenticator;



// Copyright (c) Richasy. All rights reserved.

using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;
using Richasy.BiliKernel.Services.Media.Core;

namespace Richasy.BiliKernel.Services.Media;

/// <summary>
/// 视频分区服务.
/// </summary>
public sealed class VideoPartitionService : IVideoPartitionService
{
    private readonly VideoPartitionClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="VideoPartitionService"/> class.
    /// </summary>
    public VideoPartitionService(
        BiliHttpClient httpClient,
        BasicAuthenticator authenticator)
    {
        _client = new VideoPartitionClient(httpClient, authenticator);
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<Partition>> GetVideoPartitionsAsync(CancellationToken cancellationToken)
        => _client.GetVideoPartitionsAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<IReadOnlyList<VideoInformation>> GetPartitionRankingListAsync(Partition partition, CancellationToken cancellationToken)
        => _client.GetPartitionRankingListAsync(partition, cancellationToken);
}
