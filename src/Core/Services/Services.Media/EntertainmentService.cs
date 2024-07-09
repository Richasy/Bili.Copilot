﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Appearance;
using Richasy.BiliKernel.Models.Media;
using Richasy.BiliKernel.Services.Media.Core;

namespace Richasy.BiliKernel.Services.Media;

/// <summary>
/// 电影/电视剧/纪录片服务.
/// </summary>
public sealed class EntertainmentService : IEntertainmentService
{
    private readonly PgcClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnimeService"/> class.
    /// </summary>
    public EntertainmentService(
        BiliHttpClient httpClient,
        BasicAuthenticator authenticator)
    {
        _client = new PgcClient(httpClient, authenticator);
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<Filter>> GetEntertainmentFiltersAsync(EntertainmentType type, CancellationToken cancellationToken = default)
        => _client.GetEntertainmentFiltersAsync(type, cancellationToken);

    /// <inheritdoc/>
    public Task<(IReadOnlyList<SeasonInformation> Seasons, bool HasNext)> GetEntertainmentSeasonsWithFiltersAsync(EntertainmentType type, Dictionary<Filter, Condition>? filters = null, int pageNumber = 0, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 0)
        {
            throw new KernelException("页码不能小于0");
        }

        pageNumber++;
        return _client.GetEntertainmentIndexWithFiltersAsync(type, filters, pageNumber, cancellationToken);
    }

    /// <inheritdoc/>
    public Task FollowAsync(string seasonId, CancellationToken cancellationToken = default)
        => _client.ToggleFollowAsync(seasonId, true, cancellationToken);

    /// <inheritdoc/>
    public Task UnfollowAsync(string seasonId, CancellationToken cancellationToken = default)
        => _client.ToggleFollowAsync(seasonId, false, cancellationToken);
}
