// Copyright (c) Richasy. All rights reserved.

using System.Linq;
using Richasy.BiliKernel.Adapters;
using Richasy.BiliKernel.Models.Media;

namespace Richasy.BiliKernel.Services.User.Core;

internal static class FavoriteAdapter
{
    public static VideoFavoriteFolder ToVideoFavoriteFolder(this FavoriteListDetail detail)
    {
        var id = detail.Id.ToString();
        var title = detail.Title;
        var cover = detail.Cover?.ToImage(160, 120);
        var user = detail.Publisher?.ToPublisherProfile().User;
        var desc = detail.Description;
        var count = detail.MediaCount;

        var folder = new VideoFavoriteFolder(id, title, cover, user, desc, count);
        if (detail.Type == 21)
        {
            folder.IsUgcSeason = true;
            folder.SeasonVideoId = detail.Link.Replace("bilibili://video/", string.Empty);
        }

        return folder;
    }

    public static VideoFavoriteFolder ToVideoFavoriteFolder(this FavoriteMeta meta)
    {
        var id = meta.Id.ToString();
        var title = meta.Title;
        var count = meta.MediaCount;

        return new VideoFavoriteFolder(id, title, default, default, default, count);
    }

    public static VideoFavoriteFolderDetail ToVideoFavoriteFolderDetail(this VideoFavoriteListResponse response)
    {
        var folder = (response.Detail ?? response.Information).ToVideoFavoriteFolder();
        var videos = response.Medias?.Select(p => p.ToVideoInformation()).ToList();
        return new VideoFavoriteFolderDetail(folder, videos, folder.TotalCount);
    }

    public static VideoFavoriteFolderGroup ToVideoFavoriteFolderGroup(this FavoriteFolder folder)
    {
        var folders = folder.MediaList?.List?.Select(p => p.ToVideoFavoriteFolder()).ToList();
        return new VideoFavoriteFolderGroup(folder.Id.ToString(), folder.Name, folder.Id == 1, folders, folder.MediaList?.Count ?? 0);
    }
}
