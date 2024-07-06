// Copyright (c) Richasy. All rights reserved.

using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Article;
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

    /// <summary>
    /// 移除视频观看历史项.
    /// </summary>
    Task RemoveVideoHistoryItemAsync(VideoInformation video, CancellationToken cancellationToken = default);

    /// <summary>
    /// 移除PGC观看历史项（动漫/电影/电视剧）.
    /// </summary>
    Task RemovePgcHistoryItemAsync(EpisodeInformation episode, CancellationToken cancellationToken = default);

    /// <summary>
    /// 移除直播观看历史项.
    /// </summary>
    Task RemoveLiveHistoryItemAsync(LiveInformation live, CancellationToken cancellationToken = default);

    /// <summary>
    /// 移除文章观看历史项.
    /// </summary>
    Task RemoveArticleHistoryItemAsync(ArticleInformation article, CancellationToken cancellationToken = default);

    /// <summary>
    /// 清空观看历史.
    /// </summary>
    Task CleanHistoryAsync(ViewHistoryTabType tabType, CancellationToken cancellationToken = default);

    /// <summary>
    /// 是否启用了观看历史记录.
    /// </summary>
    Task<bool> IsHistoryEnabledAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 停止记录观看历史.
    /// </summary>
    Task StopHistoryRecordingAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 恢复记录观看历史.
    /// </summary>
    Task ResumeHistoryRecordingAsync(CancellationToken cancellationToken = default);
}
