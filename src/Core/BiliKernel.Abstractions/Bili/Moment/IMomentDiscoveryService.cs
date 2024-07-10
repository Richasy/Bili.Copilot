// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Models.Moment;
using Richasy.BiliKernel.Models.User;

namespace Richasy.BiliKernel.Bili.Moment;

/// <summary>
/// 动态发现服务，用于获取用户动态.
/// </summary>
public interface IMomentDiscoveryService
{
    /// <summary>
    /// 获取用户动态.
    /// </summary>
    Task<(IReadOnlyList<MomentInformation> Moments, string Offset, bool HasMore)> GetUserMomentsAsync(UserProfile user, string? offset = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取综合动态.
    /// </summary>
    Task<MomentView> GetComprehensiveMomentsAsync(string? offset = default, string? baseline = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取视频动态.
    /// </summary>
    Task<MomentView> GetVideoMomentsAsync(string? offset = default, string? baseline = default, CancellationToken cancellationToken = default);
}
