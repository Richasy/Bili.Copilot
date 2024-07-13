// Copyright (c) Richasy. All rights reserved.

using System;
using System.Linq;
using Richasy.BiliKernel.Adapters;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;
using Richasy.BiliKernel.Services.Media.Core.Models;

namespace Richasy.BiliKernel.Services.Media.Core;

internal static class VideoAdapter
{
    public static VideoInformation ToVideoInformation(this CuratedPlaylistVideo video)
    {
        var identifier = new MediaIdentifier(video.AvId.ToString(), video.Title, video.Cover.ToVideoCover());
        var publisher = video.Owner.ToPublisherProfile();
        var communityInfo = video.Stat.ToVideoCommunityInformation();
        var publishTime = DateTimeOffset.FromUnixTimeSeconds(video.PublishTime ?? 0).ToLocalTime();
        var info = new VideoInformation(
            identifier,
            publisher,
            video.Duration,
            video.BvId,
            publishTime,
            communityInformation: communityInfo);
        info.AddExtensionIfNotNull(VideoExtensionDataId.Cid, video.Cid);
        info.AddExtensionIfNotNull(VideoExtensionDataId.MediaType, MediaType.Video);
        info.AddExtensionIfNotNull(VideoExtensionDataId.RecommendReason, video.RecommendReason?.Content);

        return info;
    }

    public static VideoInformation ToVideoInformation(this RecommendCard card)
    {
        var identifier = new MediaIdentifier(card.Args.Aid.ToString(), card.Title, card.Cover.ToVideoCover());
        var publisher = new PublisherInfo { Publisher = card.Args.UpName, UserId = card.Args.UpId ?? 0 };
        var communityInfo = new VideoCommunityInformation(card.Args.Aid.ToString(), card.PlayCountText.ToCountNumber(), card.DanmakuCountText.ToCountNumber());
        var info = new VideoInformation(
            identifier,
            publisher.ToPublisherProfile(),
            card.PlayerArgs?.Duration,
            communityInformation: communityInfo);

        var flyoutItems = card.OverflowFlyout.Where(p => p.Reasons != null).Select(p => new VideoOverflowFlyoutItem
        {
            Id = p.Type,
            Title = p.Title,
            Options = p.Reasons.ToDictionary(r => r.Id.ToString(), r => r.Name ?? string.Empty),
        }).ToList();

        info.AddExtensionIfNotNull(VideoExtensionDataId.Cid, card.PlayerArgs?.Cid);
        info.AddExtensionIfNotNull(VideoExtensionDataId.MediaType, MediaType.Video);
        info.AddExtensionIfNotNull(VideoExtensionDataId.RecommendReason, card.RecommendReason);
        info.AddExtensionIfNotNull(VideoExtensionDataId.TagId, card.Args?.TagId);
        info.AddExtensionIfNotNull(VideoExtensionDataId.TagName, card.Args?.TagName);
        info.AddExtensionIfNotNull(VideoExtensionDataId.OverflowFlyout, new VideoOverflowFlyout { Items = flyoutItems });

        return info;
    }

    public static VideoInformation ToVideoInformation(this Bilibili.App.Card.V1.Card card)
    {
        var v5 = card.SmallCoverV5;
        var baseCard = v5.Base;
        var shareInfo = baseCard.ThreePointV4.SharePlane;
        var title = baseCard.Title;
        var id = shareInfo.Aid.ToString();
        var bvId = shareInfo.Bvid;
        var publisherInfo = new PublisherInfo { Publisher = shareInfo.Author, UserId = shareInfo.AuthorId };
        var description = shareInfo.Desc;

        var cover = baseCard.Cover.ToVideoCover();
        var highlight = v5.RcmdReasonStyle?.Text ?? string.Empty;
        var duration = v5.CoverRightText1.ToDurationSeconds();
        var identifier = new MediaIdentifier(id, title, cover);

        var playCount = baseCard.ThreePointV4.SharePlane.PlayNumber.ToCountNumber("次");
        var communityInfo = new VideoCommunityInformation(id, playCount);

        var info = new VideoInformation(
            identifier,
            publisherInfo.ToPublisherProfile(),
            duration,
            bvId,
            communityInformation: communityInfo);
        info.AddExtensionIfNotNull(VideoExtensionDataId.Description, description);
        info.AddExtensionIfNotNull(VideoExtensionDataId.MediaType, MediaType.Video);
        info.AddExtensionIfNotNull(VideoExtensionDataId.RecommendReason, highlight);
        info.AddExtensionIfNotNull(VideoExtensionDataId.Cid, shareInfo.FirstCid);

        return info;
    }

    public static VideoInformation ToVideoInformation(this Bilibili.App.Show.V1.Item item)
    {
        var id = item.Param;
        var title = item.Title;
        var duration = item.Duration;
        var publishTime = DateTimeOffset.FromUnixTimeSeconds(item.PubDate).ToLocalTime();

        var user = new PublisherInfo { UserId = item.Mid, Publisher = item.Name, PublisherAvatar = item.Face }.ToPublisherProfile();
        var cover = item.Cover.ToVideoCover();
        var communityInfo = new VideoCommunityInformation(
            item.Param,
            item.Play,
            item.Danmaku,
            item.Like,
            item.Pts,
            item.Favourite,
            commentCount: item.Reply);

        var identifier = new MediaIdentifier(id, title, cover);
        var info = new VideoInformation(
            identifier,
            user,
            duration,
            publishTime: publishTime,
            communityInformation: communityInfo);
        info.AddExtensionIfNotNull(VideoExtensionDataId.Cid, item.Cid);
        info.AddExtensionIfNotNull(VideoExtensionDataId.MediaType, MediaType.Video);
        return info;
    }

    public static VideoInformation ToVideoInformation(this PartitionVideo video)
    {
        var identifier = new MediaIdentifier(video.Parameter, video.Title, video.Cover.ToVideoCover());
        var communityInfo = new VideoCommunityInformation(video.Parameter, video.PlayCount, video.DanmakuCount, video.LikeCount, favoriteCount: video.FavouriteCount, commentCount: video.ReplyCount);
        var publishTime = DateTimeOffset.FromUnixTimeSeconds(video.PublishDateTime ?? 0).ToLocalTime();
        var info = new VideoInformation(
            identifier,
            default,
            video.Duration,
            publishTime: publishTime,
            communityInformation: communityInfo);
        info.AddExtensionIfNotNull(VideoExtensionDataId.Subtitle, video.PartitionName);
        info.AddExtensionIfNotNull(VideoExtensionDataId.MediaType, MediaType.Video);
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

    public static Partition ToPartition(this VideoPartition partition)
    {
        var id = partition.Tid.ToString();
        var name = partition.Name;
        var logo = string.IsNullOrEmpty(partition.Logo) || !partition.Logo.StartsWith("http")
            ? null
            : partition.Logo.ToImage();
        var children = partition.Children?.Select(p=>p.ToPartition()).ToList();
        if (children?.Count > 0)
        {
            children.ForEach(p => p.ParentId = id);
        }

        return new Partition(id, name, logo, children);
    }
}
