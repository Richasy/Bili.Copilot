// Copyright (c) Richasy. All rights reserved.

using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili.Moment;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models.Moment;
using Richasy.BiliKernel.Services.Moment.Core;

namespace Richasy.BiliKernel.Services.Moment;

/// <summary>
/// 动态操作服务.
/// </summary>
public sealed class MomentOperationService : IMomentOperationService
{
    private readonly MomentClient _momentClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="MomentDiscoveryService"/> class.
    /// </summary>
    public MomentOperationService(
        BiliHttpClient httpClient,
        BasicAuthenticator authenticator)
    {
        _momentClient = new MomentClient(httpClient, authenticator);
    }

    /// <inheritdoc/>
    public Task DislikeMomentAsync(MomentInformation moment, CancellationToken cancellationToken = default)
        => _momentClient.LikeMomentAsync(moment, false, cancellationToken);

    /// <inheritdoc/>
    public Task LikeMomentAsync(MomentInformation moment, CancellationToken cancellationToken = default)
        => _momentClient.LikeMomentAsync(moment, true, cancellationToken);

    /// <inheritdoc/>
    public Task MarkUserMomentAsReadAsync(MomentProfile user, string? offset = default, CancellationToken cancellationToken = default)
        => _momentClient.MarkAsReadAsync(user.User.Id, offset, cancellationToken);
}
