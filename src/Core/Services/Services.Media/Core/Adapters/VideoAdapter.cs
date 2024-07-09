// Copyright (c) Richasy. All rights reserved.

using System;
using System.Linq;
using System.Text.RegularExpressions;
using Bilibili.App.Interface.V1;
using Richasy.BiliKernel.Adapters;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;
using Richasy.BiliKernel.Services.Media.Core.Models;

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
        var identifier = new VideoIdentifier(id, title, cover);
        var info = new VideoInformation(
            identifier,
            publisher,
            duration,
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

    public static VideoInformation ToVideoInformation(this CursorItem cursorItem)
    {
        var video = cursorItem.CardUgc;
        var viewTime = DateTimeOffset.FromUnixTimeSeconds(cursorItem.ViewAt).ToLocalTime();
        var title = cursorItem.Title;
        var aid = cursorItem.Kid.ToString();
        var bvid = video.Bvid;
        var owner = new PublisherInfo { Publisher = video.Name, UserId = video.Mid };
        var cover = video.Cover.ToVideoCover();
        var identifier = new VideoIdentifier(aid, title, cover);
        var communityInfo = new VideoCommunityInformation(cursorItem.Kid.ToString(), video.View);

        var info = new VideoInformation(
            identifier,
            owner.ToPublisherProfile(),
            video.Duration,
            bvid,
            communityInformation: communityInfo);

        info.AddExtensionIfNotNull(VideoExtensionDataId.CollectTime, viewTime);
        info.AddExtensionIfNotNull(VideoExtensionDataId.Progress, Convert.ToInt32(video.Progress));
        info.AddExtensionIfNotNull(VideoExtensionDataId.MediaType, MediaType.Video);
        info.AddExtensionIfNotNull(VideoExtensionDataId.Cid, video.Cid);

        return info;
    }

    public static VideoInformation ToVideoInformation(this CuratedPlaylistVideo video)
    {
        var identifier = new VideoIdentifier(video.AvId.ToString(), video.Title, video.Cover.ToVideoCover());
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
        var identifier = new VideoIdentifier(card.Args.Aid.ToString(), card.Title, card.Cover.ToVideoCover());
        var publisher = new PublisherInfo { Publisher = card.Args.UpName, UserId = card.Args.UpId ?? 0 };
        var communityInfo = new VideoCommunityInformation(card.Args.Aid.ToString(), GetCountNumber(card.PlayCountText), GetCountNumber(card.DanmakuCountText));
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
        var duration = GetDurationSeconds(v5.CoverRightText1);
        var identifier = new VideoIdentifier(id, title, cover);

        var playCount = GetCountNumber(baseCard.ThreePointV4.SharePlane.PlayNumber, "次");
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

        var identifier = new VideoIdentifier(id, title, cover);
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
        var identifier = new VideoIdentifier(video.Parameter, video.Title, video.Cover.ToVideoCover());
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

    /// <summary>
    /// 将数字简写文本中转换为大略的次数.
    /// </summary>
    /// <param name="text">数字简写文本.</param>
    /// <param name="removeText">需要先在简写文本中移除的内容.</param>
    /// <returns>一个大概的次数，比如 <c>3.2万播放</c>，最终会返回 <c>32000</c>.</returns>
    internal static double GetCountNumber(string text, string removeText = "")
    {
        if (!string.IsNullOrEmpty(removeText))
        {
            text = text.Replace(removeText, string.Empty).Trim();
        }

        // 对于目前的B站来说，汉字单位只有 `万` 和 `亿` 两种.
        if (text.EndsWith("万"))
        {
            var num = Convert.ToDouble(text.Replace("万", string.Empty));
            return num * 10000;
        }
        else if (text.EndsWith("亿"))
        {
            var num = Convert.ToDouble(text.Replace("亿", string.Empty));
            return num * 100000000;
        }

        return double.TryParse(text, out var number) ? number : -1;
    }

    internal static int GetDurationSeconds(string durationText)
    {
        var colonCount = durationText.Count(p => p == ':');
        var hourStr = string.Empty;
        if (colonCount == 1)
        {
            durationText = "00:" + durationText;
        }
        else if (colonCount == 2)
        {
            var sp = durationText.Split(':');
            durationText = string.Join(":", "00", sp[1], sp[2]);
            hourStr = sp[0];
        }

        var ts = TimeSpan.Parse(durationText);
        if (!string.IsNullOrEmpty(hourStr))
        {
            ts += TimeSpan.FromHours(Convert.ToInt32(hourStr));
        }

        return Convert.ToInt32(ts.TotalSeconds);
    }
}
