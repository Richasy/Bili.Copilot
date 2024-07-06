// Copyright (c) Richasy. All rights reserved.

using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Authenticator;

using Richasy.BiliKernel.Bili.Authorization;


// Copyright (c) Richasy. All rights reserved.

using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;
using Richasy.BiliKernel.Services.Media.Core;

namespace Richasy.BiliKernel.Services.Media;

/// <summary>
/// 观看历史服务.
/// </summary>
public sealed class ViewHistoryService : IViewHistoryService
{
    private readonly ViewHistoryClient _viewHistoryClient;

    /// <summary>
    /// 初始化 <see cref="ViewHistoryService"/> 类的新实例.
    /// </summary>
    public ViewHistoryService(
        BiliHttpClient biliHttpClient,
        IAuthenticationService authenticationService,
        BasicAuthenticator basicAuthenticator)
    {
        _viewHistoryClient = new ViewHistoryClient(biliHttpClient, authenticationService, basicAuthenticator);
    }

    /// <inheritdoc/>
    public Task<ViewHistoryGroup> GetViewHistoryAsync(ViewHistoryTabType tabType, long offset, CancellationToken cancellationToken = default)
        => _viewHistoryClient.GetHistorySetAsync(tabType, offset, cancellationToken);
}
