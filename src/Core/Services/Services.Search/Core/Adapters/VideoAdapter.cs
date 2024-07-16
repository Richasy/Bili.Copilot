// Copyright (c) Richasy. All rights reserved.

using Bilibili.Polymer.App.Search.V1;
using Richasy.BiliKernel.Adapters;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;

namespace Richasy.BiliKernel.Services.Search.Core;

internal static class VideoAdapter
{
    public static VideoInformation ToVideoInformation(this Item item)
    {
        if (item.Av is null)
        {
            throw new KernelException("视频信息不完整");
        }

        var av = item.Av;
        var aid = item.Param;
        var bvid = av.Share.Video.Bvid;
        var cid = av.Share.Video.Cid;

        // 这里的标题可能包含关键字标记，需要去除.
        var title = av.Title.Replace("<em class=\"keyword\">", string.Empty).Replace("</em>", string.Empty);
        var identifier = new MediaIdentifier(aid, av.Title, av.Cover.ToVideoCover());
        var user = UserAdapterBase.CreateUserProfile(av.Mid, av.Author, default, 0d);
        var communityInfo = new VideoCommunityInformation(aid, av.Play, av.Danmaku);
        var duration = av.Duration.ToDurationSeconds();
        var info = new VideoInformation(identifier, new Models.User.PublisherProfile(user), duration, bvid, communityInformation: communityInfo);
        info.AddExtensionIfNotNull(VideoExtensionDataId.Cid, cid);
        info.AddExtensionIfNotNull(VideoExtensionDataId.MediaType, MediaType.Video);
        info.AddExtensionIfNotNull(VideoExtensionDataId.Description, av.Desc);
        info.AddExtensionIfNotNull(VideoExtensionDataId.ShortLink, av.Share.Video.ShortLink);
        return info;
    }
}
