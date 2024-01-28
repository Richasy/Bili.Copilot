// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Adapter;
using Bili.Copilot.Models.BiliBili;
using Bili.Copilot.Models.Constants.Authorize;
using Bili.Copilot.Models.Data.Article;
using Bili.Copilot.Models.Data.Pgc;
using Bili.Copilot.Models.Data.Video;
using static Bili.Copilot.Models.App.Constants.ApiConstants;
using static Bili.Copilot.Models.App.Constants.ServiceConstants;

namespace Bili.Copilot.Libs.Provider;

/// <summary>
/// 收藏夹相关服务提供工具.
/// </summary>
public sealed partial class FavoriteProvider
{
    private FavoriteProvider()
    {
    }

    /// <summary>
    /// 更新收藏的PGC内容状态.
    /// </summary>
    /// <param name="seasonId">PGC剧集Id.</param>
    /// <param name="status">状态代码.</param>
    /// <returns>是否更新成功.</returns>
    public static async Task<bool> UpdateFavoritePgcStatusAsync(string seasonId, int status)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.SeasonId, seasonId },
            { Query.Status, status.ToString() },
            { Query.Device, "phone" },
        };

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Post, Account.UpdatePgcStatus, queryParameters, RequestClientType.IOS, needToken: true);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse>(response);
        return result.Message == "success";
    }

    /// <summary>
    /// 取消关注收藏夹.
    /// </summary>
    /// <param name="favoriteId">收藏夹Id.</param>
    /// <param name="isMe">是否是登录用户创建的收藏夹.</param>
    /// <returns>结果.</returns>
    public static async Task<bool> RemoveFavoriteFolderAsync(string favoriteId, bool isMe)
    {
        var queryParameters = new Dictionary<string, string>();
        string uri;
        if (isMe)
        {
            uri = Account.DeleteFavoriteFolder;
            queryParameters.Add(Query.MediaIds, favoriteId);
        }
        else
        {
            uri = Account.UnFavoriteFolder;
            queryParameters.Add(Query.MediaId, favoriteId);
        }

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Post, uri, queryParameters, RequestClientType.IOS, true);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse>(response);
        return result.IsSuccess();
    }

    /// <summary>
    /// 取消视频收藏.
    /// </summary>
    /// <param name="favoriteId">收藏夹Id.</param>
    /// <param name="videoId">视频Id.</param>
    /// <returns>结果.</returns>
    public static async Task<bool> RemoveFavoriteVideoAsync(string favoriteId, string videoId)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.MediaId, favoriteId },
            { Query.Resources, $"{videoId}:2" },
        };

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Post, Account.UnFavoriteVideo, queryParameters, RequestClientType.IOS, true);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse>(response);
        return result.IsSuccess();
    }

    /// <summary>
    /// 取消番剧/影视收藏.
    /// </summary>
    /// <param name="seasonId">剧集Id.</param>
    /// <returns>结果.</returns>
    public static async Task<bool> RemoveFavoritePgcAsync(string seasonId)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.SeasonId, seasonId },
        };

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Post, Account.UnFavoritePgc, queryParameters, RequestClientType.IOS, true);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse>(response);
        return result.IsSuccess();
    }

    /// <summary>
    /// 取消文章收藏.
    /// </summary>
    /// <param name="articleId">文章Id.</param>
    /// <returns>结果.</returns>
    public static async Task<bool> RemoveFavoriteArticleAsync(string articleId)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.Id, articleId },
        };

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Post, Account.UnFavoriteArticle, queryParameters, RequestClientType.IOS, true);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse>(response);
        return result.IsSuccess();
    }

    /// <summary>
    /// 获取用户的收藏夹列表（限于播放视频时）.
    /// </summary>
    /// <param name="userId">用户Id.</param>
    /// <param name="videoId">待查询的视频Id.</param>
    /// <returns>收藏夹列表以及包含该视频的收藏夹 Id 集合.</returns>
    public static async Task<(VideoFavoriteSet, IEnumerable<string>)> GetCurrentPlayerFavoriteListAsync(string userId, string videoId)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.UpId, userId.ToString() },
            { Query.Type, "2" },
            { Query.PartitionId, videoId.ToString() },
        };

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Account.FavoriteList, queryParameters, needToken: true);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse<FavoriteListResponse>>(response);
        var data = result.Data;
        var count = data.Count;
        var items = data.List.Select(FavoriteAdapter.ConvertToVideoFavoriteFolder);
        var ids = data.List.Where(p => p.FavoriteState == 1).Select(p => p.Id.ToString());
        var favoriteSet = new VideoFavoriteSet(items, count);
        return (favoriteSet, ids);
    }

    /// <summary>
    /// 获取用户的视频收藏分组列表.
    /// </summary>
    /// <param name="userId">用户Id.</param>
    /// <returns>响应结果.</returns>
    public async Task<VideoFavoriteView> GetVideoFavoriteViewAsync(string userId)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.UpId, userId.ToString() },
        };

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Account.VideoFavoriteGallery, queryParameters, RequestClientType.IOS, needToken: true);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse<VideoFavoriteGalleryResponse>>(response);
        _videoCollectFolderPageNumber = 1;
        _videoCreatedFolderPageNumber = 1;

        queryParameters = new Dictionary<string, string>
        {
            { Query.UpId, userId.ToString() },
            { Query.Type, "2" },
        };
        request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Account.FavoriteList, queryParameters, needToken: true);
        response = await HttpProvider.Instance.SendAsync(request);
        var resultList = await HttpProvider.ParseAsync<ServerResponse<FavoriteDetailListResponse>>(response);
        var listData = resultList.Data;

        queryParameters = new Dictionary<string, string>
        {
            { Query.UpId, userId.ToString() },
            { Query.PageSizeSlim, "20" },
            { Query.PageNumber, _videoCollectFolderPageNumber.ToString() },
            { Query.Platform, "web" },
        };
        request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Account.CollectList, queryParameters, needCookie: true, needRid: true);
        response = await HttpProvider.Instance.SendAsync(request);
        var collectList = await HttpProvider.ParseAsync<ServerResponse<FavoriteDetailListResponse>>(response);

        return FavoriteAdapter.ConvertToVideoFavoriteView(result.Data, listData, collectList.Data);
    }

    /// <summary>
    /// 获取视频收藏夹详情.
    /// </summary>
    /// <param name="folderId">收藏夹Id.</param>
    /// <returns>视频收藏夹响应.</returns>
    public async Task<VideoFavoriteFolderDetail> GetVideoFavoriteFolderDetailAsync(string folderId)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.MediaId, folderId },
            { Query.PageSizeSlim, "20" },
            { Query.PageNumber, _videoFolderDetailPageNumber.ToString() },
        };

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Account.VideoFavoriteDelta, queryParameters, RequestClientType.IOS, needToken: true);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse<VideoFavoriteListResponse>>(response);
        _videoFolderDetailPageNumber++;
        return FavoriteAdapter.ConvertToVideoFavoriteFolderDetail(result.Data);
    }

    /// <summary>
    /// 获取指定用户的视频收藏夹分组详情.
    /// </summary>
    /// <param name="userId">用户Id.</param>
    /// <param name="isCreated">是否是该用户创建的收藏夹.</param>
    /// <returns>视频收藏夹列表响应.</returns>
    /// <remarks>
    /// 对于视频收藏夹分组来说，只有两种，一种是用户创建的，一种是用户收集的.
    /// </remarks>
    public async Task<VideoFavoriteSet> GetVideoFavoriteFolderListAsync(string userId, bool isCreated)
    {
        var pn = isCreated ? _videoCreatedFolderPageNumber : _videoCollectFolderPageNumber;
        var queryParameters = new Dictionary<string, string>
        {
            { Query.UpId, userId.ToString() },
            { Query.PageSizeSlim, "20" },
            { Query.PageNumber, pn.ToString() },
        };

        var url = isCreated ? Account.CreatedVideoFavoriteFolderDelta : Account.CollectedVideoFavoriteFolderDelta;
        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, url, queryParameters, needToken: true);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse<FavoriteMediaList>>(response);
        if (isCreated)
        {
            _videoCreatedFolderPageNumber++;
        }
        else
        {
            _videoCollectFolderPageNumber++;
        }

        var count = result.Data.Count;
        var favorites = result.Data.List.Select(FavoriteAdapter.ConvertToVideoFavoriteFolder);
        return new VideoFavoriteSet(favorites, count);
    }

    /// <summary>
    /// 获取追番列表.
    /// </summary>
    /// <param name="status">状态.</param>
    /// <returns>追番列表响应.</returns>
    public async Task<SeasonSet> GetFavoriteAnimeListAsync(int status)
    {
        var data = await GetPgcFavoriteListInternalAsync(Account.AnimeFavorite, _animeFolderPageNumber, status);
        _animeFolderPageNumber++;
        return data;
    }

    /// <summary>
    /// 获取追剧列表.
    /// </summary>
    /// <param name="status">状态.</param>
    /// <returns>追剧列表响应.</returns>
    public async Task<SeasonSet> GetFavoriteCinemaListAsync(int status)
    {
        var data = await GetPgcFavoriteListInternalAsync(Account.CinemaFavorite, _cinemaFolderPageNumber, status);
        _cinemaFolderPageNumber++;
        return data;
    }

    /// <summary>
    /// 获取收藏文章列表.
    /// </summary>
    /// <returns>收藏文章列表响应.</returns>
    public async Task<ArticleSet> GetFavortieArticleListAsync()
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.PageNumber, _articleFolderPageNumber.ToString() },
            { Query.PageSizeSlim, "20" },
        };

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Account.ArticleFavorite, queryParameters, needToken: true);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse<ArticleFavoriteListResponse>>(response);
        return ArticleAdapter.ConvertToArticleSet(result.Data);
    }

    /// <summary>
    /// 重置视频收藏夹详情的请求状态.
    /// </summary>
    public void ResetVideoFolderDetailStatus()
        => _videoFolderDetailPageNumber = 1;

    /// <summary>
    /// 重置动漫收藏请求状态.
    /// </summary>
    public void ResetAnimeStatus()
        => _animeFolderPageNumber = 1;

    /// <summary>
    /// 重置影视收藏请求状态.
    /// </summary>
    public void ResetCinemaStatus()
        => _cinemaFolderPageNumber = 1;

    /// <summary>
    /// 重置文章收藏请求状态.
    /// </summary>
    public void ResetArticleStatus()
        => _articleFolderPageNumber = 1;
}
