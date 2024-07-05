// Copyright (c) Richasy. All rights reserved.

using System;
using Humanizer;
using Richasy.BiliKernel.Adapters;
using Richasy.BiliKernel.Models.Media;

namespace Richasy.BiliKernel.Services.User.Core;

internal static class VideoAdapter
{
    public static VideoInformation ToVideoInformation(this ViewLaterVideo video)
    {
        var title = video.Title;
        var duration = video.Duration;
        var id = video.VideoId.ToString();
        var bvid = video.BvId;
        var description = video.Description;
        var publishTime = DateTimeOffset.FromUnixTimeSeconds(video.PublishDateTime ?? 0);
        var cover = video.Cover.ToVideoCover();
        var publisher = video.Publisher.ToPublisherProfile();
        var communityInfo = video.StatusInfo.ToVideoCommunityInformation();
        var identifier = new VideoIdentifier(id, title, duration, cover);
        var subtitle = $"{publisher.User.Name} · {publishTime.Humanize()}";
        return new VideoInformation(
            identifier,
            publisher,
            bvid,
            description: description,
            subtitle: subtitle,
            publishTime: publishTime,
            communityInformation: communityInfo);
    }

    public static VideoCommunityInformation ToVideoCommunityInformation(this VideoStatusInfo statusInfo)
        => new VideoCommunityInformation(
            statusInfo.Aid.ToString(),
            statusInfo.PlayCount,
            statusInfo.DanmakuCount,
            statusInfo.LikeCount,
            favoriteCount: statusInfo.FavoriteCount,
            coinCount: statusInfo.CoinCount,
            commentCount: statusInfo.ReplyCount);
}
