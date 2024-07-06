﻿// Copyright (c) Richasy. All rights reserved.

using System;
using System.Text.RegularExpressions;
using Richasy.BiliKernel.Adapters;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;

namespace Richasy.BiliKernel.Services.Media.Core;

internal static class VideoAdapter
{
    private static readonly Regex _episodeRegex = new Regex(@"ep(\d+)");

    public static VideoInformation ToVideoInformation(this ViewLaterVideo video)
    {
        var title = video.Title;
        var duration = video.Duration;
        var id = video.Aid.ToString();
        var bvid = video.Bvid;
        var description = video.Description;
        var publishTime = DateTimeOffset.FromUnixTimeSeconds(video.PublishTime ?? 0).ToLocalTime();
        var cover = video.Cover.ToVideoCover();
        var publisher = video.Owner.ToPublisherProfile();
        var communityInfo = video.Status.ToVideoCommunityInformation();
        var identifier = new VideoIdentifier(id, title, duration, cover);
        var info = new VideoInformation(
            identifier,
            publisher,
            bvid,
            publishTime: publishTime,
            communityInformation: communityInfo);

        info.AddExtensionIfNotNull(VideoExtensionDataId.Description, description);
        info.AddExtensionIfNotNull(VideoExtensionDataId.Cid, video.Cid);
        info.AddExtensionIfNotNull(VideoExtensionDataId.TagId, video.TagId);
        info.AddExtensionIfNotNull(VideoExtensionDataId.TagName, video.TagName);
        info.AddExtensionIfNotNull(VideoExtensionDataId.PublishLocation, video.PublishLocation);
        info.AddExtensionIfNotNull(VideoExtensionDataId.FirstFrame, video.FirstFrame);
        info.AddExtensionIfNotNull(VideoExtensionDataId.ShortLink, video.ShortLink);
        info.AddExtensionIfNotNull(VideoExtensionDataId.Progress, video.Progress);

        if (video.CreateTime is not null)
        {
            info.AddExtensionIfNotNull(VideoExtensionDataId.CreateTime, DateTimeOffset.FromUnixTimeSeconds(video.CreateTime.Value).ToLocalTime());
        }

        if (video.AddAt is not null)
        {
            info.AddExtensionIfNotNull(VideoExtensionDataId.CollectTime, DateTimeOffset.FromUnixTimeSeconds(video.AddAt.Value).ToLocalTime());
        }

        var isPgc = !string.IsNullOrEmpty(video.PgcLabel) && (video.RedirectUrl?.Contains("ep") ?? false);
        if (isPgc)
        {
            var episodeId = _episodeRegex.Match(video.RedirectUrl).Groups[1].Value;
            info.AddExtensionIfNotNull(VideoExtensionDataId.EpisodeId, episodeId);
        }

        info.AddExtensionIfNotNull(VideoExtensionDataId.MediaType, isPgc ? MediaType.Pgc : MediaType.Video);

        return info;
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
