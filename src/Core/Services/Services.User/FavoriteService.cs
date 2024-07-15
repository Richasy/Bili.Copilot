// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
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
/// 收藏夹服务.
/// </summary>
public sealed class FavoriteService : IFavoriteService
{
    private readonly MyClient _myClient;

    /// <summary>
    /// 初始化 <see cref="FavoriteService"/> 类的新实例.
    /// </summary>
    public FavoriteService(
        BiliHttpClient biliHttpClient,
        IAuthenticationService authenticationService,
        IBiliTokenResolver tokenResolver,
        BasicAuthenticator basicAuthenticator)
    {
        _myClient = new MyClient(biliHttpClient, authenticationService, tokenResolver, basicAuthenticator);
    }

    /// <inheritdoc/>
    public Task<(IReadOnlyList<VideoFavoriteFolderGroup> Groups, VideoFavoriteFolderDetail Default)> GetVideoFavoriteGroupsAsync(string userId, CancellationToken cancellationToken = default)
        => _myClient.GetVideoFavoriteGroupsAsync(userId, cancellationToken);

    /// <inheritdoc/>
    public Task<(IReadOnlyList<ArticleInformation> Articles, int TotalCount, int? NextPageNumber)> GetArticleFavoritesAsync(int pageNumber = 0, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<(IReadOnlyList<SeasonInformation> Seasons, int TotalCount, int? NextPageNumber)> GetPgcFavoritesAsync(PgcFavoriteType type, PgcFavoriteStatus status, int pageNumber = 0, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<(IReadOnlyList<VideoFavoriteFolder> Folders, IReadOnlyList<string> ContainerIds)> GetPlayingVideoFavoriteFoldersAsync(VideoInformation video, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<VideoFavoriteFolderDetail> GetVideoFavoriteFolderDetailAsync(VideoFavoriteFolder folder, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task MarkPgcAsync(MediaIdentifier season, PgcFavoriteStatus status, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task RemoveArticleAsync(ArticleIdentifier article, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task RemovePgcAsync(MediaIdentifier season, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task RemoveVideoAsync(VideoFavoriteFolder folder, MediaIdentifier identifier, CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
