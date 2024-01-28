﻿// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.BiliBili;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Video;

namespace Bili.Copilot.Libs.Adapter;

/// <summary>
/// 收藏夹数据适配器.
/// </summary>
public static class FavoriteAdapter
{
    /// <summary>
    /// 将收藏夹列表详情 <see cref="FavoriteListDetail"/> 转换为收藏夹信息.
    /// </summary>
    /// <param name="detail">收藏夹列表详情.</param>
    /// <returns><see cref="Models.Data.Community.VideoFavoriteFolder"/>.</returns>
    public static VideoFavoriteFolder ConvertToVideoFavoriteFolder(FavoriteListDetail detail)
    {
        var id = detail.Id.ToString();
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(detail.Title);
        var cover = string.IsNullOrEmpty(detail.Cover)
            ? null
            : ImageAdapter.ConvertToImage(detail.Cover, 160, 120);
        var user = string.IsNullOrEmpty(detail.Publisher?.Publisher)
            ? null
            : UserAdapter.ConvertToRoleProfile(detail.Publisher, AvatarSize.Size48).User;
        var desc = TextToolkit.ConvertToTraditionalChineseIfNeeded(detail.Description);
        var count = detail.MediaCount;

        var folder = new VideoFavoriteFolder(id, title, cover, user, desc, count);
        if (detail.Type == 21)
        {
            folder.IsUgcSeason = true;
            folder.SeasonVideoId = detail.Link.Replace("bilibili://video/", string.Empty);
        }

        return folder;
    }

    /// <summary>
    /// 将收藏夹元数据 <see cref="FavoriteMeta"/> 转换为收藏夹信息.
    /// </summary>
    /// <param name="meta">收藏夹元数据.</param>
    /// <returns><see cref="Models.Data.Community.VideoFavoriteFolder"/>.</returns>
    public static VideoFavoriteFolder ConvertToVideoFavoriteFolder(FavoriteMeta meta)
    {
        var id = meta.Id.ToString();
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(meta.Title);
        var count = meta.MediaCount;

        return new VideoFavoriteFolder(id, title, default, default, default, count);
    }

    /// <summary>
    /// 将视频收藏夹列表响应 <see cref="VideoFavoriteListResponse"/> 转换为视频收藏夹详情.
    /// </summary>
    /// <param name="response">视频收藏夹列表响应.</param>
    /// <returns><see cref="VideoFavoriteFolderDetail"/>.</returns>
    public static VideoFavoriteFolderDetail ConvertToVideoFavoriteFolderDetail(VideoFavoriteListResponse response)
    {
        var folder = ConvertToVideoFavoriteFolder(response.Detail ?? response.Information);
        var videos = response.Medias?.Select(VideoAdapter.ConvertToVideoInformation) ?? new List<VideoInformation>();
        var videoSet = new VideoSet(videos, folder.TotalCount);
        return new VideoFavoriteFolderDetail(folder, videoSet);
    }

    /// <summary>
    /// 将收藏夹组 <see cref="FavoriteFolder"/> 转换为收藏夹分组.
    /// </summary>
    /// <param name="folder">收藏夹组.</param>
    /// <returns><see cref="VideoFavoriteFolderGroup"/>.</returns>
    public static VideoFavoriteFolderGroup ConvertToVideoFavoriteFolderGroup(FavoriteFolder folder)
    {
        var id = folder.Id;
        var name = TextToolkit.ConvertToTraditionalChineseIfNeeded(folder.Name);
        var isMine = id == 1;
        var folders = folder.MediaList?.List?.Select(ConvertToVideoFavoriteFolder) ?? new List<VideoFavoriteFolder>();
        var set = new VideoFavoriteSet(folders, folder.MediaList?.Count ?? 0);
        return new VideoFavoriteFolderGroup(id.ToString(), name, isMine, set);
    }

    /// <summary>
    /// 将视频收藏夹概览响应 <see cref="VideoFavoriteGalleryResponse"/> 转换为视频收藏视图.
    /// </summary>
    /// <param name="response">视频收藏夹概览响应.</param>
    /// <returns><see cref="VideoFavoriteView"/>.</returns>
    public static VideoFavoriteView ConvertToVideoFavoriteView(VideoFavoriteGalleryResponse response)
    {
        var defaultFolder = ConvertToVideoFavoriteFolderDetail(response.DefaultFavoriteList);

        // 过滤稍后再看的内容，稍后再看列表的Id为3.
        var favoriteSets = response.FavoriteFolderList?
            .Where(p => p.Id != 3)
            .Select(ConvertToVideoFavoriteFolderGroup);
        return new VideoFavoriteView(favoriteSets, defaultFolder);
    }

    /// <summary>
    /// 将视频收藏夹概览响应 <see cref="VideoFavoriteGalleryResponse"/> 转换为视频收藏视图.
    /// </summary>
    /// <param name="response">视频收藏夹概览响应.</param>
    /// <returns><see cref="VideoFavoriteView"/>.</returns>
    public static VideoFavoriteView ConvertToVideoFavoriteView(VideoFavoriteGalleryResponse response, FavoriteDetailListResponse listData, FavoriteDetailListResponse collectData)
    {
        var defaultFolder = ConvertToVideoFavoriteFolderDetail(response.DefaultFavoriteList);

        var mineCreateFav = response.FavoriteFolderList.Find(ff => ff.Id == 1);
        mineCreateFav.MediaList.List = listData.List.Where(fm => fm.Title != "默认收藏夹").ToList();

        if (collectData != null && collectData.Count > 0)
        {
            var myCollectFav = response.FavoriteFolderList.Find(f => f.Id == 2);
            myCollectFav.MediaList.List = collectData.List.ToList();
        }

        // 过滤稍后再看的内容，稍后再看列表的Id为3.
        var favoriteSets = response.FavoriteFolderList?
            .Where(p => p.Id != 3)
            .Select(ConvertToVideoFavoriteFolderGroup);

        return new VideoFavoriteView(favoriteSets, defaultFolder);
    }

    /// <summary>
    /// 将收藏夹元数据转为收藏夹详情.
    /// </summary>
    /// <param name="favoriteMeta">收藏夹元数据.</param>
    /// <returns>收藏夹详情.</returns>
    public static FavoriteListDetail ConvertToVideoFavoriteListDetail(FavoriteMeta favoriteMeta)
    {
        var favoriteListDetail = new FavoriteListDetail();
        favoriteListDetail.Title = favoriteMeta.Title;
        favoriteListDetail.Id = favoriteMeta.Id;
        favoriteListDetail.OriginId = favoriteMeta.FolderId;
        favoriteListDetail.Mid = favoriteMeta.UserId;
        favoriteListDetail.MediaCount = favoriteMeta.MediaCount;
        return favoriteListDetail;
    }
}
