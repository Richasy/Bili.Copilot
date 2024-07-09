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
/// 直播分区服务.
/// </summary>
public sealed class LiveDiscoveryService : ILiveDiscoveryService
{
    private readonly LiveDiscoveryClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="LiveDiscoveryService"/> class.
    /// </summary>
    public LiveDiscoveryService(
        BiliHttpClient httpClient,
        BasicAuthenticator authenticator,
        IBiliTokenResolver tokenResolver)
    {
        _client = new LiveDiscoveryClient(httpClient, authenticator, tokenResolver);
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<Partition>> GetLivePartitionsAsync(CancellationToken cancellationToken = default)
        => _client.GetLivePartitionsAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<(IReadOnlyList<LiveInformation> Lives, IReadOnlyList<LiveTag>? Tags, int NextPageNumber)> GetPartitionLiveListAsync(Partition partion, LiveTag? tag = null, int pageNumber = 0, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 0)
        {
            throw new KernelException("页码不能小于0");
        }

        pageNumber++;
        return _client.GetPartitionDetailAsync(partion, tag, pageNumber, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<(IReadOnlyList<LiveInformation>? Follows, IReadOnlyList<LiveInformation>? Recommend, int NextPageNumber)> GetFeedAsync(int pageNumber = 0, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 0)
        {
            throw new KernelException("页码不能小于0");
        }

        pageNumber++;
        return _client.GetFeedAsync(pageNumber, cancellationToken);
    }
}
