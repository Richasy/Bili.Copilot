// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.BiliBili;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Community;
using Bili.Copilot.Models.Data.Community;
using Bilibili.App.Archive.V1;
using Bilibili.App.Card.V1;
using Bilibili.App.Dynamic.V2;
using Bilibili.App.Interface.V1;
using Bilibili.App.Show.V1;

namespace Bili.Copilot.Libs.Adapter;

/// <summary>
/// 社区数据适配器.
/// </summary>
public static class CommunityAdapter
{
    /// <summary>
    /// 将分区横幅 <see cref="PartitionBanner"/> 转换为横幅信息.
    /// </summary>
    /// <param name="banner">分区横幅条目.</param>
    /// <returns><see cref="BannerIdentifier"/>.</returns>
    public static BannerIdentifier ConvertToBannerIdentifier(PartitionBanner banner)
    {
        var id = banner.Id.ToString();
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(banner.Title);
        var image = ImageAdapter.ConvertToImage(banner.Image, 600, 180);
        var uri = banner.NavigateUri;
        return new BannerIdentifier(id, title, image, uri);
    }

    /// <summary>
    /// 将直播数据流横幅 <see cref="LiveFeedBanner"/> 转换为横幅信息.
    /// </summary>
    /// <param name="banner">直播数据流横幅条目.</param>
    /// <returns><see cref="BannerIdentifier"/>.</returns>
    public static BannerIdentifier ConvertToBannerIdentifier(LiveFeedBanner banner)
    {
        var id = banner.Id.ToString();
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(banner.Title);
        var image = ImageAdapter.ConvertToImage(banner.Cover, 600, 180);
        var uri = banner.Link;
        return new BannerIdentifier(id, title, image, uri);
    }

    /// <summary>
    /// 将PGC模块条目 <see cref="PgcModuleItem"/> 转换为横幅信息.
    /// </summary>
    /// <param name="item">PGC模块条目.</param>
    /// <returns><see cref="BannerIdentifier"/>.</returns>
    public static BannerIdentifier ConvertToBannerIdentifier(PgcModuleItem item)
    {
        var id = item.OriginId.ToString();
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(item.Title);
        var image = ImageAdapter.ConvertToImage(item.Cover, 600, 320);
        var uri = item.WebLink;
        return new BannerIdentifier(id, title, image, uri);
    }

    /// <summary>
    /// 将分区实例 <see cref="Models.BiliBili.Partition"/> 转换为自定义的分区信息.
    /// </summary>
    /// <param name="partition">分区实例.</param>
    /// <returns><see cref="Models.Data.Community.Partition"/>.</returns>
    public static Models.Data.Community.Partition ConvertToPartition(Models.BiliBili.Partition partition)
    {
        var id = partition.Tid.ToString();
        var name = TextToolkit.ConvertToTraditionalChineseIfNeeded(partition.Name);
        var logo = string.IsNullOrEmpty(partition.Logo)
            ? null
            : ImageAdapter.ConvertToImage(partition.Logo);
        var children = partition.Children?.Select(ConvertToPartition).ToList();
        if (children?.Count > 0)
        {
            children.Insert(0, new Models.Data.Community.Partition(partition.Tid.ToString(), TextToolkit.ConvertToTraditionalChineseIfNeeded("推荐")));
            children.ForEach(p => p.ParentId = id);
        }

        return new Models.Data.Community.Partition(id, name, logo, children);
    }

    /// <summary>
    /// 将直播数据流中的热门区域 <see cref="LiveFeedHotArea"/> 转换为自定义的分区信息.
    /// </summary>
    /// <param name="area">热门区域.</param>
    /// <returns><see cref="Models.Data.Community.Partition"/>.</returns>
    public static Models.Data.Community.Partition ConvertToPartition(LiveFeedHotArea area)
    {
        var id = area.AreaId.ToString();
        var parentId = area.ParentAreaId.ToString();
        var name = TextToolkit.ConvertToTraditionalChineseIfNeeded(area.Title);
        var logo = string.IsNullOrEmpty(area.Cover)
            ? null
            : ImageAdapter.ConvertToImage(area.Cover);

        return new Models.Data.Community.Partition(id, name, logo, parentId: parentId);
    }

    /// <summary>
    /// 将直播分区组 <see cref="LiveAreaGroup"/> 转换为自定义的分区信息.
    /// </summary>
    /// <param name="group">直播分区组.</param>
    /// <returns><see cref="Models.Data.Community.Partition"/>.</returns>
    public static Models.Data.Community.Partition ConvertToPartition(LiveAreaGroup group)
    {
        var id = group.Id.ToString();
        var name = TextToolkit.ConvertToTraditionalChineseIfNeeded(group.Name);
        var children = group.AreaList.Select(ConvertToPartition).ToList();

        return new Models.Data.Community.Partition(id, name, children: children);
    }

    /// <summary>
    /// 将直播分区 <see cref="LiveArea"/> 转换为自定义的分区信息.
    /// </summary>
    /// <param name="area">直播分区.</param>
    /// <returns><see cref="Models.Data.Community.Partition"/>.</returns>
    public static Models.Data.Community.Partition ConvertToPartition(LiveArea area)
    {
        var id = area.Id.ToString();
        var parentId = area.ParentId.ToString();
        var name = TextToolkit.ConvertToTraditionalChineseIfNeeded(area.Name);
        var logo = string.IsNullOrEmpty(area.Cover)
            ? null
            : ImageAdapter.ConvertToImage(area.Cover);

        return new Models.Data.Community.Partition(id, name, logo, parentId: parentId);
    }

    /// <summary>
    /// 将PGC标签 <see cref="PgcTab"/> 转换为自定义的分区信息.
    /// </summary>
    /// <param name="tab">PGC标签.</param>
    /// <returns><see cref="Models.Data.Community.Partition"/>.</returns>
    public static Models.Data.Community.Partition ConvertToPartition(PgcTab tab)
        => new(tab.Id.ToString(), TextToolkit.ConvertToTraditionalChineseIfNeeded(tab.Title));

    /// <summary>
    /// 将文章分类 <see cref="LiveAreaGroup"/> 转换为自定义的分区信息.
    /// </summary>
    /// <param name="category">文章分类.</param>
    /// <returns><see cref="Models.Data.Community.Partition"/>.</returns>
    public static Models.Data.Community.Partition ConvertToPartition(ArticleCategory category)
    {
        var id = category.Id.ToString();
        var name = TextToolkit.ConvertToTraditionalChineseIfNeeded(category.Name);
        var children = category.Children?.Any() ?? false
            ? category.Children.Select(ConvertToPartition).ToList()
            : null;
        var parentId = category.ParentId.ToString();

        return new Models.Data.Community.Partition(id, name, children: children, parentId: parentId);
    }

    /// <summary>
    /// 将文章状态 <see cref="ArticleStats"/> 转换为文章社区交互信息.
    /// </summary>
    /// <param name="stats">文章状态.</param>
    /// <param name="articleId">文章Id.</param>
    /// <returns><see cref="ArticleCommunityInformation"/>.</returns>
    public static ArticleCommunityInformation ConvertToArticleCommunityInformation(ArticleStats stats, string articleId)
    {
        return new ArticleCommunityInformation(
            articleId,
            stats.ViewCount,
            stats.FavoriteCount,
            stats.LikeCount,
            stats.ReplyCount,
            stats.ShareCount,
            stats.CoinCount);
    }

    /// <summary>
    /// 将文章搜索结果 <see cref="ArticleSearchItem"/> 转换为文章社区交互信息.
    /// </summary>
    /// <param name="item">文章搜索结果.</param>
    /// <returns><see cref="ArticleCommunityInformation"/>.</returns>
    public static ArticleCommunityInformation ConvertToArticleCommunityInformation(ArticleSearchItem item)
    {
        return new ArticleCommunityInformation(
            item.Id.ToString(),
            item.ViewCount,
            likeCount: item.LikeCount,
            commentCount: item.ReplyCount);
    }

    /// <summary>
    /// 将个人信息 <see cref="Mine"/> 转换为用户社区交互信息.
    /// </summary>
    /// <param name="mine">个人信息.</param>
    /// <returns><see cref="UserCommunityInformation"/>.</returns>
    public static UserCommunityInformation ConvertToUserCommunityInformation(Mine mine)
        => new(
            mine.Mid.ToString(),
            mine.FollowCount,
            mine.FollowerCount,
            mine.CoinNumber,
            dynamicCount: mine.DynamicCount);

    /// <summary>
    /// 将用户空间信息 <see cref="UserSpaceInformation"/> 转换为用户社区交互信息.
    /// </summary>
    /// <param name="spaceInfo">用户空间信息.</param>
    /// <returns><see cref="UserCommunityInformation"/>.</returns>
    public static UserCommunityInformation ConvertToUserCommunityInformation(UserSpaceInformation spaceInfo)
        => new(
            spaceInfo.UserId,
            spaceInfo.FollowCount,
            spaceInfo.FollowerCount,
            likeCount: spaceInfo.LikeInformation.LikeCount,
            relation: (UserRelationStatus)spaceInfo.Relation.Status);

    /// <summary>
    /// 将个人信息 <see cref="MyInfo"/> 转换为用户社区交互信息.
    /// </summary>
    /// <param name="mine">个人信息.</param>
    /// <returns><see cref="UserCommunityInformation"/>.</returns>
    public static UserCommunityInformation ConvertToUserCommunityInformation(MyInfo mine)
        => new(
            mine.Mid.ToString(),
            coinCount: mine.Coins);

    /// <summary>
    /// 将关系用户 <see cref="RelatedUser"/> 转换为用户社区交互信息.
    /// </summary>
    /// <param name="user">关系用户.</param>
    /// <returns><see cref="UserCommunityInformation"/>.</returns>
    public static UserCommunityInformation ConvertToUserCommunityInformation(RelatedUser user)
    {
        var relation = user.Attribute switch
        {
            0 => UserRelationStatus.Unfollow,
            2 => UserRelationStatus.Following,
            3 => UserRelationStatus.Friends,
            _ => UserRelationStatus.Unknown,
        };
        return new UserCommunityInformation(
            user.Mid.ToString(),
            relation: relation);
    }

    /// <summary>
    /// 将用户搜索结果条目 <see cref="UserSearchItem"/> 转换为用户社区交互信息.
    /// </summary>
    /// <param name="item">用户搜索结果条目.</param>
    /// <returns><see cref="UserCommunityInformation"/>.</returns>
    public static UserCommunityInformation ConvertToUserCommunityInformation(UserSearchItem item)
    {
        return new UserCommunityInformation(
            item.UserId.ToString(),
            -1,
            item.FollowerCount,
            relation: (UserRelationStatus)item.Relation.Status);
    }

    /// <summary>
    /// 将推荐卡片 <see cref="RecommendCard"/> 转换为视频交互信息.
    /// </summary>
    /// <param name="videoCard">推荐卡片信息.</param>
    /// <returns><see cref="VideoCommunityInformation"/>.</returns>
    /// <remarks>
    /// 这里的转换只是将 <see cref="RecommendCard"/> 中关于社区交互的信息提取出来，其它的视频信息交由 <see cref="IVideoAdapter"/> 来处理.
    /// </remarks>
    public static VideoCommunityInformation ConvertToVideoCommunityInformation(RecommendCard videoCard)
    {
        var playCount = NumberToolkit.GetCountNumber(videoCard.PlayCountText, "观看");
        var danmakuCount = -1d;
        var trackCount = -1d;

        if (videoCard.SubStatusText.Contains("弹幕"))
        {
            danmakuCount = NumberToolkit.GetCountNumber(videoCard.SubStatusText, "弹幕");
        }
        else
        {
            var tempText = videoCard.SubStatusText
                .Replace("追剧", string.Empty)
                .Replace("追番", string.Empty);
            trackCount = NumberToolkit.GetCountNumber(tempText);
        }

        return new VideoCommunityInformation(
            videoCard.Parameter,
            playCount: playCount,
            danmakuCount: danmakuCount,
            trackCount: trackCount);
    }

    /// <summary>
    /// 将分区视频 <see cref="PartitionVideo"/> 转换为视频交互信息.
    /// </summary>
    /// <param name="video">分区视频信息.</param>
    /// <returns><see cref="VideoCommunityInformation"/>.</returns>
    public static VideoCommunityInformation ConvertToVideoCommunityInformation(PartitionVideo video)
    {
        return new VideoCommunityInformation(
            video.Parameter,
            video.PlayCount,
            video.DanmakuCount,
            video.LikeCount,
            commentCount: video.ReplyCount,
            favoriteCount: video.FavouriteCount);
    }

    /// <summary>
    /// 将动态视频 <see cref="MdlDynArchive"/> 转换为视频交互信息.
    /// </summary>
    /// <param name="video">动态视频信息.</param>
    /// <returns><see cref="VideoCommunityInformation"/>.</returns>
    public static VideoCommunityInformation ConvertToVideoCommunityInformation(MdlDynArchive video)
    {
        var danmakuCount = NumberToolkit.GetCountNumber(video.CoverLeftText3, "弹幕");
        var dynamicVideoPlayCount = NumberToolkit.GetCountNumber(video.CoverLeftText2, "观看");
        return new VideoCommunityInformation(video.Avid.ToString(), dynamicVideoPlayCount, danmakuCount);
    }

    /// <summary>
    /// 将视频状态信息 <see cref="VideoStatusInfo"/> 转换为视频交互信息.
    /// </summary>
    /// <param name="statusInfo">状态信息.</param>
    /// <returns><see cref="VideoCommunityInformation"/>.</returns>
    public static VideoCommunityInformation ConvertToVideoCommunityInformation(VideoStatusInfo statusInfo)
    {
        return new VideoCommunityInformation(
            statusInfo.Aid.ToString(),
            statusInfo.PlayCount,
            statusInfo.DanmakuCount,
            statusInfo.LikeCount,
            favoriteCount: statusInfo.FavoriteCount,
            coinCount: statusInfo.CoinCount,
            commentCount: statusInfo.ReplyCount);
    }

    /// <summary>
    /// 将排行榜视频 <see cref="Item"/> 转换为视频交互信息.
    /// </summary>
    /// <param name="rankItem">排行榜视频.</param>
    /// <returns><see cref="VideoCommunityInformation"/>.</returns>
    public static VideoCommunityInformation ConvertToVideoCommunityInformation(Item rankItem)
    {
        return new VideoCommunityInformation(
            rankItem.Param,
            rankItem.Play,
            rankItem.Danmaku,
            rankItem.Like,
            rankItem.Pts,
            rankItem.Favourite,
            commentCount: rankItem.Reply);
    }

    /// <summary>
    /// 将热门视频 <see cref="Card"/> 转换为视频交互信息.
    /// </summary>
    /// <param name="hotVideo">热门视频.</param>
    /// <returns><see cref="VideoCommunityInformation"/>.</returns>
    public static VideoCommunityInformation ConvertToVideoCommunityInformation(Card hotVideo)
    {
        var share = hotVideo.SmallCoverV5.Base.ThreePointV4.SharePlane;
        var playCount = NumberToolkit.GetCountNumber(share.PlayNumber, "次");
        return new VideoCommunityInformation(
            share.Aid.ToString(),
            playCount);
    }

    /// <summary>
    /// 将视频状态数据 <see cref="Stat"/> 转换为视频交互信息.
    /// </summary>
    /// <param name="videoStat">视频状态数据.</param>
    /// <returns><see cref="VideoCommunityInformation"/>.</returns>
    public static VideoCommunityInformation ConvertToVideoCommunityInformation(Stat videoStat)
    {
        return new VideoCommunityInformation(
            videoStat.Aid.ToString(),
            videoStat.View,
            videoStat.Danmaku,
            videoStat.Like,
            favoriteCount: videoStat.Fav,
            coinCount: videoStat.Coin,
            commentCount: videoStat.Reply,
            shareCount: videoStat.Share);
    }

    /// <summary>
    /// 将视频搜索条目 <see cref="VideoSearchItem"/> 转换为视频交互信息.
    /// </summary>
    /// <param name="searchVideo">搜索视频条目.</param>
    /// <returns><see cref="VideoCommunityInformation"/>.</returns>
    public static VideoCommunityInformation ConvertToVideoCommunityInformation(VideoSearchItem searchVideo)
    {
        return new VideoCommunityInformation(
            searchVideo.Parameter,
            searchVideo.PlayCount,
            searchVideo.DanmakuCount);
    }

    /// <summary>
    /// 将用户空间视频条目 <see cref="UserSpaceVideoItem"/> 转换为视频交互信息.
    /// </summary>
    /// <param name="video">用户空间视频条目.</param>
    /// <returns><see cref="VideoCommunityInformation"/>.</returns>
    public static VideoCommunityInformation ConvertToVideoCommunityInformation(UserSpaceVideoItem video)
    {
        return new VideoCommunityInformation(
            video.Id,
            video.PlayCount,
            video.DanmakuCount);
    }

    /// <summary>
    /// 将收藏夹视频条目 <see cref="FavoriteMedia"/> 转换为视频交互信息.
    /// </summary>
    /// <param name="video">收藏夹视频条目.</param>
    /// <returns><see cref="VideoCommunityInformation"/>.</returns>
    public static VideoCommunityInformation ConvertToVideoCommunityInformation(FavoriteMedia video)
    {
        return new VideoCommunityInformation(
            video.Id.ToString(),
            video.Stat.PlayCount,
            video.Stat.DanmakuCount,
            favoriteCount: video.Stat.FavoriteCount);
    }

    /// <summary>
    /// 将剧集单集社区信息 <see cref="PgcEpisodeStat"/> 转换为视频交互信息.
    /// </summary>
    /// <param name="stat">剧集单集社区信息.</param>
    /// <returns><see cref="VideoCommunityInformation"/>.</returns>
    public static VideoCommunityInformation ConvertToVideoCommunityInformation(PgcEpisodeStat stat)
    {
        return new VideoCommunityInformation(
            default,
            stat.PlayCount,
            stat.DanmakuCount,
            stat.LikeCount,
            coinCount: stat.CoinCount,
            commentCount: stat.ReplyCount);
    }

    /// <summary>
    /// 将剧集社区信息 <see cref="PgcInformationStat"/> 转换为视频交互信息.
    /// </summary>
    /// <param name="stat">剧集社区信息.</param>
    /// <returns><see cref="VideoCommunityInformation"/>.</returns>
    public static VideoCommunityInformation ConvertToVideoCommunityInformation(PgcInformationStat stat)
    {
        _ = NumberToolkit.GetCountNumber(stat.FollowerDisplayText);
        return new VideoCommunityInformation(
            default,
            stat.PlayCount,
            stat.DanmakuCount,
            stat.LikeCount,
            -1,
            stat.FavoriteCount,
            stat.CoinCount,
            stat.ReplyCount,
            stat.ShareCount);
    }

    /// <summary>
    /// 将 PGC 条目社区信息 <see cref="PgcItemStat"/> 转换为视频交互信息.
    /// </summary>
    /// <param name="stat">PGC 条目社区信息.</param>
    /// <returns><see cref="VideoCommunityInformation"/>.</returns>
    public static VideoCommunityInformation ConvertToVideoCommunityInformation(PgcItemStat stat)
    {
        return new VideoCommunityInformation(
            default,
            stat.ViewCount,
            stat.DanmakuCount,
            trackCount: stat.FollowCount);
    }

    /// <summary>
    /// 将 PGC 搜索条目 <see cref="PgcSearchItem"/> 转换为视频交互信息.
    /// </summary>
    /// <param name="item">PGC 搜索条目.</param>
    /// <returns><see cref="VideoCommunityInformation"/>.</returns>
    public static VideoCommunityInformation ConvertToVideoCommunityInformation(PgcSearchItem item)
    {
        return new VideoCommunityInformation(
            item.SeasonId.ToString(),
            score: item.Rating);
    }

    /// <summary>
    /// 将 PGC 播放列表条目社区信息 <see cref="PgcPlayListItemStat"/> 转换为视频交互信息.
    /// </summary>
    /// <param name="stat">PGC 搜索条目.</param>
    /// <returns><see cref="VideoCommunityInformation"/>.</returns>
    public static VideoCommunityInformation ConvertToVideoCommunityInformation(PgcPlayListItemStat stat)
    {
        return new VideoCommunityInformation(
            default,
            stat.PlayCount,
            stat.DanmakuCount,
            favoriteCount: stat.FavoriteCount);
    }

    /// <summary>
    /// 将 UGC 视频卡片 <see cref="PgcPlayListItemStat"/> 转换为视频交互信息.
    /// </summary>
    /// <param name="ugc">UGC 条目信息.</param>
    /// <returns><see cref="VideoCommunityInformation"/>.</returns>
    public static VideoCommunityInformation ConvertToVideoCommunityInformation(CardUGC ugc)
        => new(default, ugc.View);

    /// <summary>
    /// 将视频状态信息 <see cref="VideoStatusInfo"/> 转换为视频交互信息.
    /// </summary>
    /// <param name="info">视频状态信息.</param>
    /// <returns><see cref="VideoCommunityInformation"/>.</returns>
    public static VideoCommunityInformation CovnertToVideoCommunityInformation(VideoStatusInfo info)
    {
        return new VideoCommunityInformation(
            info.Aid.ToString(),
            info.PlayCount,
            info.DanmakuCount,
            info.LikeCount,
            favoriteCount: info.FavoriteCount,
            coinCount: info.CoinCount,
            commentCount: info.ReplyCount,
            shareCount: info.ShareCount);
    }

    /// <summary>
    /// 将未读消息 <see cref="UnreadMessage"/> 转换为未读信息.
    /// </summary>
    /// <param name="message">未读消息.</param>
    /// <returns><see cref="UnreadInformation"/>.</returns>
    public static UnreadInformation ConvertToUnreadInformation(UnreadMessage message)
        => new(message.At, message.Reply, message.Like);

    /// <summary>
    /// 将点赞消息条目 <see cref="LikeMessageItem"/> 转换为消息信息.
    /// </summary>
    /// <param name="messageItem">消息条目.</param>
    /// <returns><see cref="MessageInformation"/>.</returns>
    public static MessageInformation ConvertToMessageInformation(LikeMessageItem messageItem)
    {
        var isMultiple = messageItem.Users.Count > 1;
        var firstUser = messageItem.Users[0];
        var userName = firstUser.UserName;
        var avatar = ImageAdapter.ConvertToImage(firstUser.Avatar, 48, 48);
        string message;
        if (isMultiple)
        {
            var secondUser = messageItem.Users[1];
            message = string.Format(
                    ResourceToolkit.GetLocalizedString(StringNames.LikeMessageMultipleDescription),
                    userName,
                    secondUser.UserName,
                    messageItem.Count,
                    messageItem.Item.Business);
        }
        else
        {
            message = string.Format(
                    ResourceToolkit.GetLocalizedString(StringNames.LikeMessageSingleDescription),
                    userName,
                    messageItem.Item.Business);
        }

        var publishTime = DateTimeOffset.FromUnixTimeSeconds(messageItem.LikeTime).DateTime;
        var id = messageItem.Id.ToString();
        var sourceContent = string.IsNullOrEmpty(messageItem.Item.Title)
            ? messageItem.Item.Description
            : messageItem.Item.Title;
        sourceContent = TextToolkit.ConvertToTraditionalChineseIfNeeded(sourceContent);
        var sourceId = messageItem.Item.Uri;

        return new MessageInformation(
            id,
            MessageType.Like,
            avatar,
            string.Empty,
            isMultiple,
            publishTime,
            string.Empty,
            message,
            sourceContent,
            sourceId);
    }

    /// <summary>
    /// 将提及消息条目 <see cref="AtMessageItem"/> 转换为消息信息.
    /// </summary>
    /// <param name="messageItem">消息条目.</param>
    /// <returns><see cref="MessageInformation"/>.</returns>
    public static MessageInformation ConvertToMessageInformation(AtMessageItem messageItem)
    {
        var user = messageItem.User;
        var userName = user.UserName;
        var avatar = ImageAdapter.ConvertToImage(user.Avatar, 48, 48);
        var subtitle = string.Format(
            ResourceToolkit.GetLocalizedString(StringNames.AtMessageTypeDescription),
            messageItem.Item.Business);
        var message = TextToolkit.ConvertToTraditionalChineseIfNeeded(messageItem.Item.SourceContent);
        var sourceContent = string.IsNullOrEmpty(messageItem.Item.Title)
            ? ResourceToolkit.GetLocalizedString(StringNames.NoSpecificData)
            : messageItem.Item.Title;
        sourceContent = TextToolkit.ConvertToTraditionalChineseIfNeeded(sourceContent);
        var publishTime = DateTimeOffset.FromUnixTimeSeconds(messageItem.AtTime).DateTime;
        var id = messageItem.Id.ToString();
        var sourceId = messageItem.Item.Uri;

        return new MessageInformation(
            id,
            MessageType.At,
            avatar,
            userName,
            false,
            publishTime,
            subtitle,
            message,
            sourceContent,
            sourceId);
    }

    /// <summary>
    /// 将回复消息条目 <see cref="ReplyMessageItem"/> 转换为消息信息.
    /// </summary>
    /// <param name="messageItem">消息条目.</param>
    /// <returns><see cref="MessageInformation"/>.</returns>
    public static MessageInformation ConvertToMessageInformation(ReplyMessageItem messageItem)
    {
        var user = messageItem.User;
        var userName = user.UserName;
        var avatar = ImageAdapter.ConvertToImage(user.Avatar, 48, 48);
        var isMultiple = messageItem.IsMultiple == 1;
        var subtitle = string.Format(
            ResourceToolkit.GetLocalizedString(StringNames.ReplyMessageTypeDescription),
            messageItem.Item.Business,
            messageItem.Counts);
        var message = TextToolkit.ConvertToTraditionalChineseIfNeeded(messageItem.Item.SourceContent);
        var sourceContent = string.IsNullOrEmpty(messageItem.Item.Title)
            ? messageItem.Item.Description
            : messageItem.Item.Title;
        sourceContent = TextToolkit.ConvertToTraditionalChineseIfNeeded(sourceContent);
        var publishTime = DateTimeOffset.FromUnixTimeSeconds(messageItem.ReplyTime).DateTime;
        var id = messageItem.Id.ToString();
        var sourceId = messageItem.Item.Uri.ToString();
        var properties = new Dictionary<string, string>()
        {
            { "type", messageItem.Item.BusinessId.ToString() },
        };

        return new MessageInformation(
            id,
            MessageType.Reply,
            avatar,
            userName,
            isMultiple,
            publishTime,
            subtitle,
            message,
            sourceContent,
            sourceId,
            properties);
    }

    /// <summary>
    /// 将点赞消息响应 <see cref="LikeMessageResponse"/> 转换为消息信息.
    /// </summary>
    /// <param name="messageResponse">消息响应.</param>
    /// <returns><see cref="MessageView"/>.</returns>
    public static MessageView ConvertToMessageView(LikeMessageResponse messageResponse)
    {
        var cursor = messageResponse.Total.Cursor;
        var items = new List<MessageInformation>();
        if (messageResponse.Latest != null)
        {
            items = items
                .Concat(messageResponse.Latest.Items.Select(ConvertToMessageInformation))
                .ToList();
        }

        if (messageResponse.Total != null)
        {
            items = items
                .Concat(messageResponse.Total.Items.Select(ConvertToMessageInformation))
                .ToList();
        }

        return new MessageView(items, cursor.IsEnd);
    }

    /// <summary>
    /// 将提及消息响应 <see cref="AtMessageResponse"/> 转换为消息信息.
    /// </summary>
    /// <param name="messageResponse">消息响应.</param>
    /// <returns><see cref="MessageView"/>.</returns>
    public static MessageView ConvertToMessageView(AtMessageResponse messageResponse)
    {
        var cursor = messageResponse.Cursor;
        var items = messageResponse.Items.Select(ConvertToMessageInformation).ToList();
        return new MessageView(items, cursor.IsEnd);
    }

    /// <summary>
    /// 将回复消息响应 <see cref="ReplyMessageResponse"/> 转换为消息信息.
    /// </summary>
    /// <param name="messageResponse">消息响应.</param>
    /// <returns><see cref="MessageView"/>.</returns>
    public static MessageView ConvertToMessageView(ReplyMessageResponse messageResponse)
    {
        var cursor = messageResponse.Cursor;
        var items = messageResponse.Items.Select(ConvertToMessageInformation).ToList();
        return new MessageView(items, cursor.IsEnd);
    }

    /// <summary>
    /// 将关联标签 <see cref="RelatedTag"/> 转换为关注分组.
    /// </summary>
    /// <param name="tag">关联标签.</param>
    /// <returns><see cref="FollowGroup"/>.</returns>
    public static FollowGroup ConvertToFollowGroup(RelatedTag tag)
        => new(tag.TagId.ToString(), TextToolkit.ConvertToTraditionalChineseIfNeeded(tag.Name), tag.Count);

    /// <summary>
    /// 将动态状态数据 <see cref="ModuleStat"/> 转换为动态社区信息.
    /// </summary>
    /// <param name="stat">动态状态数据.</param>
    /// <param name="dynamicId">动态 Id.</param>
    /// <returns><see cref="DynamicCommunityInformation"/>.</returns>
    public static DynamicCommunityInformation ConvertToDynamicCommunityInformation(ModuleStat stat, string dynamicId)
        => new(dynamicId, stat.Like, stat.Reply, stat.LikeInfo.IsLike);

    /// <summary>
    /// 将一键三连的结果 <see cref="TripleResult"/> 转换为一键三连信息.
    /// </summary>
    /// <param name="result">一键三连的结果.</param>
    /// <param name="id">视频 Id.</param>
    /// <returns><see cref="TripleInformation"/>.</returns>
    public static TripleInformation ConvertToTripleInformation(TripleResult result, string id)
        => new(id, result.IsLike, result.IsCoin, result.IsFavorite);

    /// <summary>
    /// 将单集交互响应 <see cref="EpisodeInteraction"/> 转换为单集交互信息.
    /// </summary>
    /// <param name="interaction">单集交互响应.</param>
    /// <returns><see cref="EpisodeInteractionInformation"/>.</returns>
    public static EpisodeInteractionInformation ConvertToEpisodeInteractionInformation(EpisodeInteraction interaction)
        => new(interaction.IsLike == 1, interaction.CoinNumber > 0, interaction.IsFavorite == 1);
}
