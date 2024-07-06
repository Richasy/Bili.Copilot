// Copyright (c) Richasy. All rights reserved.

using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models.User;
using Richasy.BiliKernel.Services.User.Core;

namespace Richasy.BiliKernel.Services.User;

/// <summary>
/// 用户资料服务.
/// </summary>
public sealed class MyProfileService : IMyProfileService
{
    private readonly MyClient _myClient;

    /// <summary>
    /// 初始化 <see cref="MyProfileService"/> 类的新实例.
    /// </summary>
    public MyProfileService(
        BiliHttpClient biliHttpClient,
        IAuthenticationService authenticationService,
        IBiliTokenResolver tokenResolver,
        BasicAuthenticator basicAuthenticator)
    {
        _myClient = new MyClient(biliHttpClient, authenticationService, tokenResolver, basicAuthenticator);
    }

    /// <inheritdoc/>
    public Task<UserDetailProfile> GetMyProfileAsync(CancellationToken cancellationToken = default)
        => _myClient.GetMyInformationAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<UserCommunityInformation> GetMyCommunityInformationAsync(CancellationToken cancellationToken = default)
        => _myClient.GetMyCommunityInformationAsync(cancellationToken);
}
