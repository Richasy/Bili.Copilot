// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models.Media;
using Richasy.BiliKernel.Services.Media.Core;

namespace Richasy.BiliKernel.Services.Media;

/// <summary>
/// 动漫服务.
/// </summary>
public sealed class AnimeService : IAnimeService
{
    private readonly AnimeClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnimeService"/> class.
    /// </summary>
    public AnimeService(
        BiliHttpClient httpClient,
        BasicAuthenticator authenticator)
    {
        _client = new AnimeClient(httpClient, authenticator);
    }

    /// <inheritdoc/>
    public Task<(string Title, string Description, IReadOnlyList<TimelineInformation>)> GetBangumiTimelineAsync(CancellationToken cancellationToken = default)
        => _client.GetAnimeTimelineAsync(true, cancellationToken);

    /// <inheritdoc/>
    public Task<(string Title, string Description, IReadOnlyList<TimelineInformation>)> GetDomesticTimelineAsync(CancellationToken cancellationToken = default)
        => _client.GetAnimeTimelineAsync(false, cancellationToken);
}
