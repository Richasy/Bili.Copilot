// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;
using Richasy.BiliKernel.Services.User.Core;

namespace Richasy.BiliKernel.Services.User;

/// <summary>
/// 稍后再看服务.
/// </summary>
public sealed class ViewLaterService : IViewLaterService
{
    private readonly ViewLaterClient _viewLaterClient;

    /// <summary>
    /// 初始化 <see cref="ViewLaterService"/> 类的新实例.
    /// </summary>
    public ViewLaterService(
        BiliHttpClient biliHttpClient,
        IAuthenticationService authenticationService,
        BasicAuthenticator basicAuthenticator)
    {
        _viewLaterClient = new ViewLaterClient(biliHttpClient, authenticationService, basicAuthenticator);
    }

    /// <inheritdoc/>
    public Task<(IReadOnlyList<VideoInformation> Videos, int Count)> GetViewLaterSetAsync(int pageNumber = 0, CancellationToken cancellationToken = default)
        => _viewLaterClient.GetMyViewLaterAsync(pageNumber, cancellationToken);

    /// <inheritdoc/>
    public Task AddAsync(string aid, CancellationToken cancellationToken = default)
        => _viewLaterClient.AddAsync(aid, cancellationToken);

    /// <inheritdoc/>
    public Task CleanAsync(ViewLaterCleanType cleanType, CancellationToken cancellationToken = default)
        => _viewLaterClient.CleanAsync(cleanType, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveAsync(string[] aids, CancellationToken cancellationToken = default)
        => _viewLaterClient.RemoveAsync(aids, cancellationToken);
}
