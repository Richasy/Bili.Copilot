// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models.Media;
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

    /// <inheritdoc/>
    public Task<IReadOnlyList<UserGroup>> GetMyFollowUserGroupsAsync(CancellationToken cancellationToken = default)
        => _myClient.GetMyFollowUserGroupsAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<IReadOnlyList<UserCard>> GetMyFollowUserGroupDetailAsync(string groupId, int pageNumber = 0, CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber < 0 ? 0 : pageNumber + 1;
        return _myClient.GetMyFollowUserGroupDetailAsync(groupId, pageNumber, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<(IReadOnlyList<UserCard> Users, int Count)> GetMyFansAsync(int pageNumber = 0, CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber < 0 ? 0 : pageNumber + 1;
        return _myClient.GetMyFansAsync(pageNumber, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<(IReadOnlyList<VideoInformation> Videos, int Count)> GetMyViewLaterAsync(int pageNumber = 0, CancellationToken cancellationToken = default)
    {
        pageNumber = pageNumber < 0 ? 0 : pageNumber;
        return _myClient.GetMyViewLaterAsync(pageNumber, cancellationToken);
    }
}
