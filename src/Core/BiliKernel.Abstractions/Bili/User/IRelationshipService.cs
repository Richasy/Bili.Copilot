// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Models.User;

namespace Richasy.BiliKernel.Bili.User;

/// <summary>
/// 用户关系服务，用于处理关注、粉丝等关系操作.
/// </summary>
public interface IRelationshipService
{
    /// <summary>
    /// 获取已登录用户关注的分组.
    /// </summary>
    Task<IReadOnlyList<UserGroup>> GetMyFollowUserGroupsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取已登录用户关注的分组的详细信息.
    /// </summary>
    Task<IReadOnlyList<UserCard>> GetMyFollowUserGroupDetailAsync(string groupId, int pageNumber = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取已登录用户的关注者.
    /// </summary>
    Task<(IReadOnlyList<UserCard> Users, int Count)> GetMyFansAsync(int pageNumber = 0, CancellationToken cancellationToken = default);
}
