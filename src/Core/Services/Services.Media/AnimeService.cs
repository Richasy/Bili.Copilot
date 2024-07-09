// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models.Appearance;
using Richasy.BiliKernel.Models.Media;
using Richasy.BiliKernel.Services.Media.Core;

namespace Richasy.BiliKernel.Services.Media;

/// <summary>
/// 动漫服务.
/// </summary>
public sealed class AnimeService : IAnimeService
{
    private readonly PgcClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnimeService"/> class.
    /// </summary>
    public AnimeService(
        BiliHttpClient httpClient,
        BasicAuthenticator authenticator)
    {
        _client = new PgcClient(httpClient, authenticator);
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<Filter>> GetAnimeFiltersAsync(CancellationToken cancellationToken = default)
        => _client.GetAnimeFiltersAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<(IReadOnlyList<SeasonInformation> Seasons, bool HasNext)> GetAnimeSeasonsWithFiltersAsync(Dictionary<Filter, Condition>? filters = null, int pageNumber = 0, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 0)
        {
            throw new KernelException("页码不能小于0");
        }

        pageNumber++;
        return _client.GetAnimeIndexWithFiltersAsync(filters, pageNumber, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<(string Title, string Description, IReadOnlyList<TimelineInformation>)> GetBangumiTimelineAsync(CancellationToken cancellationToken = default)
        => _client.GetAnimeTimelineAsync(true, cancellationToken);

    /// <inheritdoc/>
    public Task<(string Title, string Description, IReadOnlyList<TimelineInformation>)> GetDomesticTimelineAsync(CancellationToken cancellationToken = default)
        => _client.GetAnimeTimelineAsync(false, cancellationToken);

    /// <inheritdoc/>
    public Task FollowAsync(string seasonId, CancellationToken cancellationToken = default)
        => _client.ToggleFollowAsync(seasonId, true, cancellationToken);

    /// <inheritdoc/>
    public Task UnfollowAsync(string seasonId, CancellationToken cancellationToken = default)
        => _client.ToggleFollowAsync(seasonId, false, cancellationToken);
}
