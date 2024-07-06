// Copyright (c) Richasy. All rights reserved.

using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;

namespace Richasy.BiliKernel.Bili.Media;

/// <summary>
/// 观看历史服务.
/// </summary>
public interface IViewHistoryService
{
    /// <summary>
    /// 获取观看历史.
    /// </summary>
    Task<ViewHistoryGroup> GetViewHistoryAsync(ViewHistoryTabType tabType, long offset = 0, CancellationToken cancellationToken = default);
}
