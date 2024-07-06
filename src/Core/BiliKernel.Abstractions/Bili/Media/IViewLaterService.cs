// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;

namespace Richasy.BiliKernel.Bili.Media;

/// <summary>
/// 稍后再看服务.
/// </summary>
public interface IViewLaterService
{
    /// <summary>
    /// 获取稍后再看视频.
    /// </summary>
    Task<(IReadOnlyList<VideoInformation> Videos, int Count)> GetViewLaterSetAsync(int pageNumber = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加视频到稍后再看.
    /// </summary>
    Task AddAsync(string aid, CancellationToken cancellationToken = default);

    /// <summary>
    /// 从稍后再看中移除视频.
    /// </summary>
    Task RemoveAsync(string[] aids, CancellationToken cancellationToken = default);

    /// <summary>
    /// 清空稍后再看视频.
    /// </summary>
    Task CleanAsync(ViewLaterCleanType cleanType, CancellationToken cancellationToken = default);
}
