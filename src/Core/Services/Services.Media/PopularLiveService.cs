// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models.Media;
using Richasy.BiliKernel.Services.Media.Core;

namespace Richasy.BiliKernel.Services.Media;

/// <summary>
/// 直播信息流服务.
/// </summary>
public sealed class PopularLiveService : IPopularLiveService
{
    private readonly PopularLiveClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="PopularLiveService"/> class.
    /// </summary>
    public PopularLiveService(
        BiliHttpClient httpClient,
        IBiliTokenResolver tokenResolver,
        BasicAuthenticator authenticator)
    {
        _client = new PopularLiveClient(httpClient, tokenResolver, authenticator);
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
