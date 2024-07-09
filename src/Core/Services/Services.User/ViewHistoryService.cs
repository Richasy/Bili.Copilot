// Copyright (c) Richasy. All rights reserved.

using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Article;
using Richasy.BiliKernel.Models.Media;
using Richasy.BiliKernel.Services.User.Core;

namespace Richasy.BiliKernel.Services.User;

/// <summary>
/// 观看历史服务.
/// </summary>
public sealed class ViewHistoryService : IViewHistoryService
{
    private readonly ViewHistoryClient _viewHistoryClient;

    /// <summary>
    /// 初始化 <see cref="ViewHistoryService"/> 类的新实例.
    /// </summary>
    public ViewHistoryService(
        BiliHttpClient biliHttpClient,
        IAuthenticationService authenticationService,
        BasicAuthenticator basicAuthenticator)
    {
        _viewHistoryClient = new ViewHistoryClient(biliHttpClient, authenticationService, basicAuthenticator);
    }

    /// <inheritdoc/>
    public Task CleanHistoryAsync(ViewHistoryTabType tabType, CancellationToken cancellationToken = default)
        => _viewHistoryClient.CleanHistoryAsync(tabType, cancellationToken);

    /// <inheritdoc/>
    public Task<ViewHistoryGroup> GetViewHistoryAsync(ViewHistoryTabType tabType, long offset, CancellationToken cancellationToken = default)
        => _viewHistoryClient.GetHistorySetAsync(tabType, offset, cancellationToken);
    
    /// <inheritdoc/>
    public Task RemoveArticleHistoryItemAsync(ArticleInformation article, CancellationToken cancellationToken = default)
        => _viewHistoryClient.RemoveHistoryItemAsync(article.Identifier.Id, ViewHistoryTabType.Article, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveLiveHistoryItemAsync(LiveInformation live, CancellationToken cancellationToken = default)
        => _viewHistoryClient.RemoveHistoryItemAsync(live.Identifier.Id, ViewHistoryTabType.Live, cancellationToken);

    /// <inheritdoc/>
    public Task RemovePgcHistoryItemAsync(EpisodeInformation episode, CancellationToken cancellationToken = default)
        => _viewHistoryClient.RemoveHistoryItemAsync(episode.GetExtensionIfNotNull<string>(EpisodeExtensionDataId.SeasonId), ViewHistoryTabType.Pgc, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveVideoHistoryItemAsync(VideoInformation video, CancellationToken cancellationToken = default)
        => _viewHistoryClient.RemoveHistoryItemAsync(video.Identifier.Id, ViewHistoryTabType.Video, cancellationToken);

    /// <inheritdoc/>
    public Task<bool> IsHistoryEnabledAsync(CancellationToken cancellationToken = default)
        => _viewHistoryClient.GetIsHistoryEnabledAsync(cancellationToken);

    /// <inheritdoc/>
    public Task ResumeHistoryRecordingAsync(CancellationToken cancellationToken = default)
        => _viewHistoryClient.SetHistoryRecordOptionAsync(false, cancellationToken);

    /// <inheritdoc/>
    public Task StopHistoryRecordingAsync(CancellationToken cancellationToken = default)
        => _viewHistoryClient.SetHistoryRecordOptionAsync(true, cancellationToken);
}
