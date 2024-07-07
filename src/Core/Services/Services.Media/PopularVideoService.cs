// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili.Authorization;


// Copyright (c) Richasy. All rights reserved.

using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models.Media;
using Richasy.BiliKernel.Services.Media.Core;

namespace Richasy.BiliKernel.Services.Media;

/// <summary>
/// 获取流行视频的服务，包括精选视频/推荐视频/热门视频/排行榜.
/// </summary>
public sealed class PopularVideoService : IPopularVideoService
{
    private readonly PopularVideoClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="PopularVideoService"/> class.
    /// </summary>
    public PopularVideoService(
        BiliHttpClient httpClient,
        IBiliTokenResolver tokenResolver,
        BasicAuthenticator authenticator)
    {
        _client = new PopularVideoClient(httpClient, tokenResolver, authenticator);
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
