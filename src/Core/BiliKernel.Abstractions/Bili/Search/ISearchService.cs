// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Models.Search;

namespace Richasy.BiliKernel.Bili.Search;

/// <summary>
/// 搜索服务.
/// </summary>
public interface ISearchService
{
    /// <summary>
    /// 获取完整热搜榜单.
    /// </summary>
    Task<IReadOnlyList<HotSearchItem>> GetTotalHotSearchAsync(CancellationToken cancellationToken = default);
}
