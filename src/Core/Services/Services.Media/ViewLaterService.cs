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
        IBiliTokenResolver tokenResolver,
        BasicAuthenticator basicAuthenticator)
    {
        _viewLaterClient = new ViewLaterClient(biliHttpClient, authenticationService, tokenResolver, basicAuthenticator);
    }

    /// <inheritdoc/>
    public Task<(IReadOnlyList<VideoInformation> Videos, int Count)> GetViewLaterSetAsync(int pageNumber = 0, CancellationToken cancellationToken = default)
        => _viewLaterClient.GetMyViewLaterAsync(pageNumber, cancellationToken);
}
