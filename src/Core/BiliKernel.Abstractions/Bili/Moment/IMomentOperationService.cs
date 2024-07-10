// Copyright (c) Richasy. All rights reserved.

using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Models.Moment;

namespace Richasy.BiliKernel.Bili.Moment;

/// <summary>
/// 动态操作服务，用于对动态进行操作，包括点赞，标记等.
/// </summary>
public interface IMomentOperationService
{
    /// <summary>
    /// 给动态点赞.
    /// </summary>
    Task LikeMomentAsync(MomentInformation moment, CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消对动态的点赞.
    /// </summary>
    Task DislikeMomentAsync(MomentInformation moment, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 将用户动态标记为已读.
    /// </summary>
    /// <remarks>
    /// 在请求综合动态或者用户空间动态时，会包含偏移值 <paramref name="offset"/>，用于标记动态的已读状态.
    /// </remarks>
    Task MarkUserMomentAsReadAsync(MomentProfile user, string offset, CancellationToken cancellationToken = default);
}
