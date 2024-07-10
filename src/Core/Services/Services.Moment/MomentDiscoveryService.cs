// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili.Moment;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models.Moment;
using Richasy.BiliKernel.Models.User;
using Richasy.BiliKernel.Services.Moment.Core;

namespace Richasy.BiliKernel.Services.Moment;

/// <summary>
/// 动态发现服务.
/// </summary>
public sealed class MomentDiscoveryService : IMomentDiscoveryService
{
    private readonly MomentClient _momentClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="MomentDiscoveryService"/> class.
    /// </summary>
    public MomentDiscoveryService(
        BiliHttpClient httpClient,
        BasicAuthenticator authenticator)
    {
        _momentClient = new MomentClient(httpClient, authenticator);
    }

    /// <inheritdoc/>
    public Task<(IReadOnlyList<MomentInformation> Moments, string Offset, bool HasMore)> GetUserMomentsAsync(UserProfile user, string? offset = null, CancellationToken cancellationToken = default)
        => _momentClient.GetUserMomentsAsync(user, offset, cancellationToken);

    /// <inheritdoc/>
    public Task<MomentView> GetComprehensiveMomentsAsync(string? offset = null, string? baseline = null, CancellationToken cancellationToken = default)
        => _momentClient.GetComprehensiveMomentsAsync(offset, baseline, cancellationToken);

    /// <inheritdoc/>
    public Task<MomentView> GetVideoMomentsAsync(string? offset = null, string? baseline = null, CancellationToken cancellationToken = default)
        => _momentClient.GetVideoMomentsAsync(offset, baseline, cancellationToken);
}
