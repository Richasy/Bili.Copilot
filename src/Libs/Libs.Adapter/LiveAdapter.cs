// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.BiliBili;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Live;
using Bili.Copilot.Models.Data.Player;
using Bili.Copilot.Models.Data.Video;

namespace Bili.Copilot.Libs.Adapter;

/// <summary>
/// 直播数据适配器.
/// </summary>
public static class LiveAdapter
{
    /// <summary>
    /// 将关注的直播间 <see cref="LiveFeedRoom"/> 转换为直播间信息.
    /// </summary>
    /// <param name="room">关注的直播间.</param>
    /// <returns><see cref="LiveInformation"/>.</returns>
    public static LiveInformation ConvertToLiveInformation(LiveFeedRoom room)
    {
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(room.Title);
        var id = room.RoomId.ToString();
        var viewerCount = room.ViewerCount;
        var user = UserAdapter.ConvertToUserProfile(room.UserId, room.UserName, room.UserAvatar, AvatarSize.Size48);
        var cover = ImageAdapter.ConvertToVideoCardCover(room.Cover);
        var subtitle = TextToolkit.ConvertToTraditionalChineseIfNeeded($"{room.AreaName} · {room.UserName}");

        var identifier = new VideoIdentifier(id, title, -1, cover);
        return new LiveInformation(
            identifier,
            user,
            viewerCount,
            subtitle: subtitle);
    }

    /// <summary>
    /// 从直播间卡片 <see cref="LiveRoomCard"/> 转换为直播间信息.
    /// </summary>
    /// <param name="card">直播间卡片.</param>
    /// <returns><see cref="LiveInformation"/>.</returns>
    public static LiveInformation ConvertToLiveInformation(LiveRoomCard card)
    {
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(card.Title);
        var id = card.RoomId.ToString();
        var viewerCount = NumberToolkit.GetCountNumber(card.CoverRightContent.Text);
        var subtitle = card.CoverLeftContent.Text;
        var cover = ImageAdapter.ConvertToVideoCardCover(card.Cover);
        var identifier = new VideoIdentifier(id, title, -1, cover);
        return new LiveInformation(identifier, default, viewerCount, subtitle: subtitle);
    }

    /// <summary>
    /// 从直播搜索结果 <see cref="LiveSearchItem"/> 转换为直播间信息.
    /// </summary>
    /// <param name="item">直播搜索结果.</param>
    /// <returns><see cref="LiveInformation"/>.</returns>
    public static LiveInformation ConvertToLiveInformation(LiveSearchItem item)
    {
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(item.Title);
        var id = item.RoomId.ToString();
        var viewerCount = item.ViewerCount;
        var cover = ImageAdapter.ConvertToVideoCardCover(item.Cover);
        var subtitle = item.Name;

        var identifier = new VideoIdentifier(id, title, -1, cover);
        return new LiveInformation(identifier, null, viewerCount, subtitle: subtitle);
    }

    /// <summary>
    /// 将直播间详情 <see cref="LiveRoomDetail"/> 转换为直播间播放视图.
    /// </summary>
    /// <param name="detail">直播间详情.</param>
    /// <returns><see cref="LivePlayerView"/>.</returns>
    public static LivePlayerView ConvertToLivePlayerView(LiveRoomDetail detail)
    {
        var roomInfo = detail.RoomInformation;
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(roomInfo.Title);
        var id = roomInfo.RoomId.ToString();
        var description = string.IsNullOrEmpty(roomInfo.Description)
            ? string.Empty
            : Regex.Replace(roomInfo.Description, @"<[^>]*>", string.Empty);

        if (!string.IsNullOrEmpty(description))
        {
            description = WebUtility.HtmlDecode(description).Trim();
        }

        if (string.IsNullOrEmpty(description))
        {
            description = "暂无直播间介绍";
        }

        description = TextToolkit.ConvertToTraditionalChineseIfNeeded(description);

        var viewerCount = roomInfo.ViewerCount;
        var cover = ImageAdapter.ConvertToImage(roomInfo.Cover ?? roomInfo.Keyframe);
        var userInfo = detail.AnchorInformation.UserBasicInformation;
        var userProfile = UserAdapter.ConvertToUserProfile(roomInfo.UserId, userInfo.UserName, userInfo.Avatar, AvatarSize.Size48);
        var partition = $"{roomInfo.ParentAreaName} · {roomInfo.AreaName}";
        var subtitle = DateTimeOffset.FromUnixTimeSeconds(detail.RoomInformation.LiveStartTime).ToLocalTime().ToString("yyyy/MM/dd HH:mm");

        var identifier = new VideoIdentifier(id, title, -1, cover);
        var info = new LiveInformation(identifier, userProfile, viewerCount, subtitle: subtitle, description: description);
        return new LivePlayerView(info, partition);
    }

    /// <summary>
    /// 将直播首页数据流信息 <see cref="LiveFeedResponse"/> 转换为直播流视图.
    /// </summary>
    /// <param name="response">直播首页数据流信息.</param>
    /// <returns><see cref="LiveFeedView"/>.</returns>
    public static LiveFeedView ConvertToLiveFeedView(LiveFeedResponse response)
    {
        var recommendRooms = response.CardList.Where(p => p.CardType.Contains("small_card"))
            .Where(p => p.CardData?.LiveCard != null)
            .Select(p => ConvertToLiveInformation(p.CardData.LiveCard))
            .ToList();
        var followRooms = response.CardList.Where(p => p.CardType.Contains("idol"))
            .SelectMany(p => p.CardData?.FollowList?.List)
            .Select(p => ConvertToLiveInformation(p))
            .ToList();
        var banners = response.CardList.Where(p => p.CardType.Contains("banner"))
            .SelectMany(p => p.CardData?.Banners?.List)
            .Select(p => CommunityAdapter.ConvertToBannerIdentifier(p))
            .ToList();
        var partitions = response.CardList.Where(p => p.CardType.Contains("area"))
            .SelectMany(p => p.CardData?.HotAreas?.List)
            .Where(p => p.Id != 0)
            .Select(p => CommunityAdapter.ConvertToPartition(p))
            .ToList();

        return new LiveFeedView(banners, partitions, followRooms, recommendRooms);
    }

    /// <summary>
    /// 将直播分区详情信息 <see cref="LiveAreaDetailResponse"/> 转换为直播分区详情视图.
    /// </summary>
    /// <param name="response">直播分区详情视图.</param>
    /// <returns><see cref="LivePartitionView"/>.</returns>
    public static LivePartitionView ConvertToLivePartitionView(LiveAreaDetailResponse response)
    {
        var lives = response.List.Select(p => ConvertToLiveInformation(p)).ToList();
        var tags = response.Tags?.Count > 0
            ? response.Tags.Select(p => new LiveTag(p.Id.ToString(), p.Name, p.SortType)).ToList()
            : new List<LiveTag>() { new LiveTag(string.Empty, TextToolkit.ConvertToTraditionalChineseIfNeeded("全部"), string.Empty) };
        return new LivePartitionView(response.Count, lives, tags);
    }

    /// <summary>
    /// 将直播播放信息 <see cref="LiveAppPlayInformation"/> 转换为直播媒体信息.
    /// </summary>
    /// <param name="information">直播播放信息.</param>
    /// <returns><see cref="LiveMediaInformation"/>.</returns>
    public static LiveMediaInformation ConvertToLiveMediaInformation(LiveAppPlayInformation information)
    {
        var id = information.RoomId.ToString();
        var playInfo = information.PlayUrlInfo.PlayUrl;
        var formats = new List<FormatInformation>();
        foreach (var item in playInfo.Descriptions)
        {
            var desc = TextToolkit.ConvertToTraditionalChineseIfNeeded(item.Description);
            formats.Add(new FormatInformation(item.Quality, desc, false));
        }

        var lines = new List<LivePlaylineInformation>();
        foreach (var stream in playInfo.StreamList)
        {
            foreach (var format in stream.FormatList)
            {
                foreach (var codec in format.CodecList)
                {
                    var name = codec.CodecName;
                    var urls = codec.Urls.Select(p => new LivePlayUrl(stream.ProtocolName, p.Host, codec.BaseUrl, p.Extra));
                    lines.Add(new LivePlaylineInformation(name, codec.CurrentQuality, codec.AcceptQualities, urls));
                }
            }
        }

        return new LiveMediaInformation(id, formats, lines);
    }
}
