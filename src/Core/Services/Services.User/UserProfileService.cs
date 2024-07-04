// Copyright (c) Richasy. All rights reserved.

using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili;
using Richasy.BiliKernel.Bili.Authorization;


// Copyright (c) Richasy. All rights reserved.

using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models.User;
using Richasy.BiliKernel.Services.User.Core;

namespace Richasy.BiliKernel.Services.User;

/// <summary>
/// 用户资料服务.
/// </summary>
public sealed class UserProfileService : IUserProfileService
{
    private readonly MyClient _myClient;

    /// <summary>
    /// 初始化 <see cref="UserProfileService"/> 类的新实例.
    /// </summary>
    public UserProfileService(
        BiliHttpClient biliHttpClient,
        IAuthenticationService authenticationService,
        BasicAuthenticator basicAuthenticator)
    {
        _myClient = new MyClient(biliHttpClient, authenticationService, basicAuthenticator);
    }

    /// <inheritdoc/>
    public Task<UserDetailProfile> GetMyProfileAsync(UserExecutionSettings? executionSettings = null, CancellationToken cancellationToken = default)
        => _myClient.GetMyInformationAsync(cancellationToken);
}
