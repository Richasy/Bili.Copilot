// Copyright (c) Richasy. All rights reserved.

using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Models.User;

namespace Richasy.BiliKernel.Bili.User;

/// <summary>
/// 已登录用户的资料服务.
/// </summary>
public interface IMyProfileService
{
    /// <summary>
    /// 获取已登录用户的资料.
    /// </summary>
    Task<UserDetailProfile> GetMyProfileAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取已登录用户的社区信息.
    /// </summary>
    Task<UserCommunityInformation> GetMyCommunityInformationAsync(CancellationToken cancellationToken = default);
}
