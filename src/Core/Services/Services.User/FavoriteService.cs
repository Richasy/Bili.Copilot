// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili;
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
    public Task<(IReadOnlyList<VideoFavoriteFolderGroup> Groups, VideoFavoriteFolderDetail Default)> GetVideoFavoriteGroupsAsync(CancellationToken cancellationToken = default)
        => _myClient.GetVideoFavoriteGroupsAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<(IReadOnlyList<ArticleInformation> Articles, int TotalCount, int? NextPageNumber)> GetArticleFavoritesAsync(int pageNumber = 0, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 0)
        {
            throw new KernelException("页码不能小于0");
        }

        pageNumber++;
        var (list, count) = await _myClient.GetFavoritesArticlesAsync(pageNumber, cancellationToken).ConfigureAwait(false);
        return (list, count, count == 0 ? null : pageNumber);
    }

    /// <inheritdoc/>
    public async Task<(IReadOnlyList<SeasonInformation> Seasons, int TotalCount, int? NextPageNumber)> GetPgcFavoritesAsync(PgcFavoriteType type, PgcFavoriteStatus status, int pageNumber = 0, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 0)
        {
            throw new KernelException("页码不能小于0");
        }

        pageNumber++;
        var url = type == PgcFavoriteType.Anime ? BiliApis.Account.AnimeFavorite : BiliApis.Account.CinemaFavorite;
        var (list, count) = await _myClient.GetFavoritePgcListAsync(url, pageNumber, status, cancellationToken).ConfigureAwait(false);
        return (list, count, count == 0 ? null : pageNumber);
    }

    /// <inheritdoc/>
    public Task<(IReadOnlyList<VideoFavoriteFolder> Folders, IReadOnlyList<string> ContainerIds)> GetPlayingVideoFavoriteFoldersAsync(VideoInformation video, CancellationToken cancellationToken = default)
        => _myClient.GetPlayingVideoFavoriteFoldersAsync(video, cancellationToken);

    /// <inheritdoc/>
    public async Task<(VideoFavoriteFolderDetail Detail, int? NextPageNumber)> GetVideoFavoriteFolderDetailAsync(VideoFavoriteFolder folder, int pageNumber = 0, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 0)
        {
            throw new KernelException("页码不能小于0");
        }

        pageNumber++;
        var data = await _myClient.GetVideoFavoriteFolderDetailAsync(folder, pageNumber, cancellationToken).ConfigureAwait(false);
        return (data, data.Videos?.Count > 0 ? pageNumber : null);
    }

    /// <inheritdoc/>
    public Task MarkPgcAsync(MediaIdentifier season, PgcFavoriteStatus status, CancellationToken cancellationToken = default)
        => _myClient.UpdatePgcFavoriteStatusAsync(season, status, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveArticleAsync(ArticleIdentifier article, CancellationToken cancellationToken = default)
        => _myClient.RemoveFavoriteArticleAsync(article, cancellationToken);

    /// <inheritdoc/>
    public Task RemovePgcAsync(MediaIdentifier season, CancellationToken cancellationToken = default)
        => _myClient.RemoveFavoritePgcAsync(season, cancellationToken);

    /// <inheritdoc/>
    public Task RemoveVideoAsync(VideoFavoriteFolder folder, MediaIdentifier identifier, CancellationToken cancellationToken = default)
        => _myClient.RemoveFavoriteVideoAsync(folder, identifier, cancellationToken);
}
