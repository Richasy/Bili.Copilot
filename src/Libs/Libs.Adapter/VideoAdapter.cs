// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Constants;
using Bili.Copilot.Models.BiliBili;
using Bili.Copilot.Models.BiliBili.Player;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Community;
using Bili.Copilot.Models.Constants.Player;
using Bili.Copilot.Models.Data.Community;
using Bili.Copilot.Models.Data.Player;
using Bili.Copilot.Models.Data.User;
using Bili.Copilot.Models.Data.Video;
using Bilibili.App.Card.V1;
using Bilibili.App.Dynamic.V2;
using Bilibili.App.Interface.V1;
using Bilibili.App.Show.V1;
using Bilibili.App.View.V1;
using Humanizer;

namespace Bili.Copilot.Libs.Adapter;

/// <summary>
/// 视频信息适配器.
/// </summary>
public static class VideoAdapter
{
    /// <summary>
    /// 将推荐视频卡片 <see cref="RecommendCard"/> 转换为视频信息.
    /// </summary>
    /// <param name="videoCard">推荐视频卡片.</param>
    /// <returns><see cref="VideoInformation"/>.</returns>
    /// <exception cref="ArgumentException">传入的数据不是预期的视频类型.</exception>
    public static VideoInformation ConvertToVideoInformation(RecommendCard videoCard)
    {
        if (videoCard.CardGoto != ServiceConstants.Av)
        {
            throw new ArgumentException($"推荐卡片的 CardGoTo 属性应该是 {ServiceConstants.Av}，这里是 {videoCard.Goto}，不符合要求，请检查分配条件", nameof(videoCard));
        }

        var communityInfo = CommunityAdapter.ConvertToVideoCommunityInformation(videoCard);
        var publisher = UserAdapter.ConvertToRoleProfile(videoCard.Mask.Avatar);
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(videoCard.Title);
        var id = videoCard.Parameter;
        var duration = (videoCard.PlayerArgs?.Duration).HasValue
                ? videoCard.PlayerArgs.Duration
                : 0;
        var subtitle = videoCard.Description;
        var cover = ImageAdapter.ConvertToVideoCardCover(videoCard.Cover);
        var highlight = TextToolkit.ConvertToTraditionalChineseIfNeeded(videoCard.RecommendReason);

        var identifier = new VideoIdentifier(id, title, duration, cover);
        return new VideoInformation(
            identifier,
            publisher,
            subtitle: subtitle,
            highlight: highlight,
            communityInformation: communityInfo);
    }

    /// <summary>
    /// 将分区视频 <see cref="PartitionVideo"/> 转换为视频信息.
    /// </summary>
    /// <param name="video">分区视频.</param>
    /// <returns><see cref="VideoInformation"/>.</returns>
    /// <remarks>
    /// 分区视频指的是网站按内容类型分的区域下的视频，比如舞蹈区视频、科技区视频等.
    /// </remarks>
    public static VideoInformation ConvertToVideoInformation(PartitionVideo video)
    {
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(video.Title);
        var id = video.Parameter;
        var duration = video.Duration;
        var subtitle = video.Publisher;
        var cover = ImageAdapter.ConvertToVideoCardCover(video.Cover);
        var communityInfo = CommunityAdapter.ConvertToVideoCommunityInformation(video);
        var publishTime = DateTimeOffset.FromUnixTimeSeconds(video.PublishDateTime).ToLocalTime();
        var identifier = new VideoIdentifier(id, title, duration, cover);
        return new VideoInformation(
            identifier,
            null,
            subtitle: subtitle,
            publishTime: publishTime.DateTime,
            communityInformation: communityInfo);
    }

    /// <summary>
    /// 将动态里的视频内容 <see cref="MdlDynArchive"/> 转换为视频信息.
    /// </summary>
    /// <param name="dynamicVideo">动态里的视频内容.</param>
    /// <returns><see cref="VideoInformation"/>.</returns>
    /// <remarks>
    /// 有些番剧的更新可能也是以 <see cref="MdlDynArchive"/> 类型发布，需要提前根据 <c>IsPGC</c> 属性来判断它是不是视频内容，如果不是，执行该方法会抛出异常.
    /// </remarks>
    /// <exception cref="ArgumentException">传入的数据不是预期的视频类型.</exception>
    public static VideoInformation ConvertToVideoInformation(MdlDynArchive dynamicVideo)
    {
        if (dynamicVideo.IsPGC)
        {
            throw new ArgumentException($"该动态视频是 PGC 内容，不符合要求，请检查分配条件", nameof(dynamicVideo));
        }

        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(dynamicVideo.Title);
        var id = dynamicVideo.Avid.ToString();
        var bvid = dynamicVideo.Bvid;
        var duration = dynamicVideo.Duration;
        var cover = ImageAdapter.ConvertToImage(dynamicVideo.Cover, AppConstants.DynamicCoverWidth, AppConstants.DynamicCoverHeight);
        var communityInfo = CommunityAdapter.ConvertToVideoCommunityInformation(dynamicVideo);
        var identifier = new VideoIdentifier(id, title, duration, cover);
        return new VideoInformation(
            identifier,
            null,
            bvid,
            communityInformation: communityInfo);
    }

    /// <summary>
    /// 将稍后再看里的视频内容 <see cref="ViewLaterVideo"/> 转换为视频信息.
    /// </summary>
    /// <param name="video">稍后再看的视频.</param>
    /// <returns><see cref="VideoInformation"/>.</returns>
    public static VideoInformation ConvertToVideoInformation(ViewLaterVideo video)
    {
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(video.Title);
        var duration = video.Duration;
        var id = video.VideoId.ToString();
        var bvid = video.BvId;
        var description = TextToolkit.ConvertToTraditionalChineseIfNeeded(video.Description);
        var publishTime = DateTimeOffset.FromUnixTimeSeconds(video.PublishDateTime);
        var cover = ImageAdapter.ConvertToVideoCardCover(video.Cover);
        var publisher = UserAdapter.ConvertToRoleProfile(video.Publisher, AvatarSize.Size48);
        var communityInfo = CommunityAdapter.ConvertToVideoCommunityInformation(video.StatusInfo);
        var identifier = new VideoIdentifier(id, title, duration, cover);
        var subtitle = $"{publisher.User.Name} · {TextToolkit.ConvertToTraditionalChineseIfNeeded(publishTime.Humanize(culture: new System.Globalization.CultureInfo("zh-CN")))}";
        return new VideoInformation(
            identifier,
            publisher,
            bvid,
            description: description,
            subtitle: subtitle,
            publishTime: publishTime.DateTime,
            communityInformation: communityInfo);
    }

    /// <summary>
    /// 将排行榜里的视频内容 <see cref="Bilibili.App.Show.V1.Item"/> 转换为视频信息.
    /// </summary>
    /// <param name="rankVideo">排行榜视频.</param>
    /// <returns><see cref="VideoInformation"/>.</returns>
    public static VideoInformation ConvertToVideoInformation(Item rankVideo)
    {
        var id = rankVideo.Param;
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(rankVideo.Title);
        var duration = rankVideo.Duration;
        var publishTime = DateTimeOffset.FromUnixTimeSeconds(rankVideo.PubDate).ToLocalTime();

        var user = UserAdapter.ConvertToUserProfile(rankVideo.Mid, rankVideo.Name, rankVideo.Face, AvatarSize.Size48);
        var publisher = new RoleProfile(user);
        var cover = ImageAdapter.ConvertToVideoCardCover(rankVideo.Cover);
        var communityInfo = CommunityAdapter.ConvertToVideoCommunityInformation(rankVideo);
        var subtitle = $"{user.Name} · {TextToolkit.ConvertToTraditionalChineseIfNeeded(publishTime.Humanize(culture: new System.Globalization.CultureInfo("zh-CN")))}";

        var identifier = new VideoIdentifier(id, title, duration, cover);
        return new VideoInformation(
            identifier,
            publisher,
            subtitle: subtitle,
            publishTime: publishTime.DateTime,
            communityInformation: communityInfo);
    }

    /// <summary>
    /// 将热门视频内容 <see cref="Bilibili.App.Card.V1.Card"/> 转换为视频信息.
    /// </summary>
    /// <param name="hotVideo">热门视频.</param>
    /// <returns><see cref="VideoInformation"/>.</returns>
    public static VideoInformation ConvertToVideoInformation(Card hotVideo)
    {
        var v5 = hotVideo.SmallCoverV5;
        var card = v5.Base;
        var shareInfo = card.ThreePointV4.SharePlane;
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(card.Title);
        var id = shareInfo.Aid.ToString();
        var bvId = shareInfo.Bvid;
        var subtitle = shareInfo.Author;
        var description = TextToolkit.ConvertToTraditionalChineseIfNeeded(shareInfo.Desc);

        // 对于热门视频来说，它会把观看数和发布时间揉在一起，比如 "13.5万观看 · 21小时前"，
        // 考虑到我们的需求，这里需要把它拆开，让发布时间和作者名一起作为副标题存在，就像推荐视频一样.
        var descSplit = v5.RightDesc2.Split('·');
        if (descSplit.Length > 1)
        {
            var publishTimeText = descSplit[1].Trim();
            subtitle += $" · {TextToolkit.ConvertToTraditionalChineseIfNeeded(publishTimeText)}";
        }

        var duration = NumberToolkit.GetDurationSeconds(v5.CoverRightText1);
        var cover = ImageAdapter.ConvertToVideoCardCover(card.Cover);
        var communityInfo = CommunityAdapter.ConvertToVideoCommunityInformation(hotVideo);
        var highlight = hotVideo.SmallCoverV5.RcmdReasonStyle?.Text ?? string.Empty;
        highlight = TextToolkit.ConvertToTraditionalChineseIfNeeded(highlight);
        var identifier = new VideoIdentifier(id, title, duration, cover);

        return new VideoInformation(
            identifier,
            null,
            bvId,
            description: description,
            subtitle: subtitle,
            highlight: highlight,
            communityInformation: communityInfo);
    }

    /// <summary>
    /// 将关联视频内容 <see cref="Bilibili.App.View.V1.Relate"/> 转换为视频信息.
    /// </summary>
    /// <param name="relatedVideo">关联视频.</param>
    /// <returns><see cref="VideoInformation"/>.</returns>
    public static VideoInformation ConvertToVideoInformation(Relate relatedVideo)
    {
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(relatedVideo.Title);
        var id = relatedVideo.Aid.ToString();
        var duration = relatedVideo.Duration;
        var description = TextToolkit.ConvertToTraditionalChineseIfNeeded(relatedVideo.Desc);
        var publisher = UserAdapter.ConvertToRoleProfile(relatedVideo.Author);
        var cover = ImageAdapter.ConvertToVideoCardCover(relatedVideo.Pic);
        var communityInfo = CommunityAdapter.ConvertToVideoCommunityInformation(relatedVideo.Stat);
        var identifier = new VideoIdentifier(id, title, duration, cover);
        var subtitle = publisher.User.Name;
        subtitle = TextToolkit.ConvertToTraditionalChineseIfNeeded(subtitle);
        return new VideoInformation(
            identifier,
            publisher,
            subtitle: subtitle,
            description: description,
            communityInformation: communityInfo);
    }

    /// <summary>
    /// 将视频搜索结果 <see cref="VideoSearchItem"/> 转换为视频信息.
    /// </summary>
    /// <param name="searchVideo">视频搜索结果.</param>
    /// <returns><see cref="VideoInformation"/>.</returns>
    public static VideoInformation ConvertToVideoInformation(VideoSearchItem searchVideo)
    {
        var title = Regex.Replace(searchVideo.Title, @"<[^<>]+>", string.Empty);
        title = TextToolkit.ConvertToTraditionalChineseIfNeeded(title);
        var id = searchVideo.Parameter;
        var duration = string.IsNullOrEmpty(searchVideo.Duration)
            ? 0
            : NumberToolkit.GetDurationSeconds(searchVideo.Duration);
        var cover = ImageAdapter.ConvertToVideoCardCover(searchVideo.Cover);
        var description = TextToolkit.ConvertToTraditionalChineseIfNeeded(searchVideo.Description);
        var user = UserAdapter.ConvertToUserProfile(searchVideo.UserId, searchVideo.Author, searchVideo.Avatar, AvatarSize.Size48);
        var publisher = new RoleProfile(user);
        var communityInfo = CommunityAdapter.ConvertToVideoCommunityInformation(searchVideo);
        var subtitle = user.Name;

        var identifier = new VideoIdentifier(id, title, duration, cover);
        return new VideoInformation(
            identifier,
            publisher,
            subtitle: subtitle,
            description: description,
            communityInformation: communityInfo);
    }

    /// <summary>
    /// 将用户空间内的视频内容 <see cref="UserSpaceVideoItem"/> 转换为视频信息.
    /// </summary>
    /// <param name="spaceVideo">用户空间内的视频内容.</param>
    /// <returns><see cref="VideoInformation"/>.</returns>
    public static VideoInformation ConvertToVideoInformation(UserSpaceVideoItem spaceVideo)
    {
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(spaceVideo.Title);
        var id = spaceVideo.Id;
        var publishDate = DateTimeOffset.FromUnixTimeSeconds(spaceVideo.CreateTime);
        var duration = spaceVideo.Duration;
        var cover = ImageAdapter.ConvertToVideoCardCover(spaceVideo.Cover);
        var communityInfo = CommunityAdapter.ConvertToVideoCommunityInformation(spaceVideo);
        var subtitle = TextToolkit.ConvertToTraditionalChineseIfNeeded(publishDate.DateTime.Humanize(culture: new System.Globalization.CultureInfo("zh-CN")));

        var identifier = new VideoIdentifier(id, title, duration, cover);
        return new VideoInformation(
            identifier,
            null,
            subtitle: subtitle,
            publishTime: publishDate.DateTime,
            communityInformation: communityInfo);
    }

    /// <summary>
    /// 将历史记录里的视频内容 <see cref="CursorItem"/> 转换为视频信息.
    /// </summary>
    /// <param name="historyVideo">历史记录里的视频内容.</param>
    /// <returns><see cref="VideoInformation"/>.</returns>
    public static VideoInformation ConvertToVideoInformation(CursorItem historyVideo)
    {
        if (historyVideo.CardItemCase != CursorItem.CardItemOneofCase.CardUgc)
        {
            throw new ArgumentException($"该历史记录不是视频内容，是 {historyVideo.CardItemCase}, 不符合要求，请检查分配条件", nameof(historyVideo));
        }

        var video = historyVideo.CardUgc;
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(historyVideo.Title);
        var id = historyVideo.Kid.ToString();
        var bvid = video.Bvid;
        var subtitle = $"{video.Name} · {TimeSpan.FromSeconds(video.Progress)}";
        var duration = video.Duration;
        var cover = ImageAdapter.ConvertToVideoCardCover(video.Cover);

        var identifier = new VideoIdentifier(id, title, duration, cover);
        var communityInfo = CommunityAdapter.ConvertToVideoCommunityInformation(video);
        communityInfo.Id = id;
        return new VideoInformation(
            identifier,
            default,
            bvid,
            subtitle: subtitle,
            communityInformation: communityInfo);
    }

    /// <summary>
    /// 将收藏夹内的视频内容 <see cref="FavoriteMedia"/> 转换为视频信息.
    /// </summary>
    /// <param name="video">收藏夹视频.</param>
    /// <returns><see cref="VideoInformation"/>.</returns>
    public static VideoInformation ConvertToVideoInformation(FavoriteMedia video)
    {
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(video.Title);
        var id = video.Id.ToString();
        var publisher = UserAdapter.ConvertToRoleProfile(video.Publisher, AvatarSize.Size48);
        var duration = video.Duration;
        var cover = ImageAdapter.ConvertToVideoCardCover(video.Cover);
        var communityInfo = CommunityAdapter.ConvertToVideoCommunityInformation(video);
        var collectTime = DateTimeOffset.FromUnixTimeSeconds(video.FavoriteTime).ToLocalTime().DateTime;
        var subtitle = TextToolkit.ConvertToTraditionalChineseIfNeeded($"{collectTime.Humanize(culture: new System.Globalization.CultureInfo("zh-CN"))}收藏");

        var identifier = new VideoIdentifier(id, title, duration, cover);
        return new VideoInformation(identifier, publisher, subtitle: subtitle, communityInformation: communityInfo);
    }

    /// <summary>
    /// 将用户空间里的视频搜索结果 <see cref="Arc"/> 转换为视频信息.
    /// </summary>
    /// <param name="video">用户空间里的视频搜索结果.</param>
    /// <returns><see cref="VideoInformation"/>.</returns>
    public static VideoInformation ConvertToVideoInformation(Arc video)
    {
        var archive = video.Archive;
        var title = Regex.Replace(archive.Title, @"<[^<>]+>", string.Empty);
        title = TextToolkit.ConvertToTraditionalChineseIfNeeded(title);
        var id = archive.Aid.ToString();
        var publisher = UserAdapter.ConvertToRoleProfile(archive.Author);
        var duration = archive.Duration;
        var cover = ImageAdapter.ConvertToVideoCardCover(archive.Pic);
        var communityInfo = CommunityAdapter.ConvertToVideoCommunityInformation(archive.Stat);
        var publishTime = DateTimeOffset.FromUnixTimeSeconds(archive.Pubdate).DateTime;
        var description = TextToolkit.ConvertToTraditionalChineseIfNeeded(archive.Desc);
        var subtitle = TextToolkit.ConvertToTraditionalChineseIfNeeded(publishTime.Humanize(culture: new System.Globalization.CultureInfo("zh-CN")));

        var identifier = new VideoIdentifier(id, title, duration, cover);
        return new VideoInformation(
            identifier,
            publisher,
            subtitle: subtitle,
            description: description,
            publishTime: publishTime,
            communityInformation: communityInfo);
    }

    /// <summary>
    /// 将视频详情 <see cref="ViewReply"/> 转换为视频信息.
    /// </summary>
    /// <param name="videoDetail">视频详情.</param>
    /// <returns><see cref="VideoPlayerView"/>.</returns>
    public static VideoPlayerView ConvertToVideoView(ViewReply videoDetail)
    {
        var videoInfo = GetVideoInformationFromViewReply(videoDetail);
        var subVideos = GetSubVideosFromViewReply(videoDetail);
        var interaction = GetInteractionRecordFromViewReply(videoDetail);
        var publisherCommunity = GetPublisherCommunityFromViewReply(videoDetail);
        var operation = GetOperationInformationFromViewReply(videoDetail);
        var relatedVideos = GetRelatedVideosFromViewReply(videoDetail);
        var sections = GetVideoSectionsFromViewReply(videoDetail);
        var history = GetHistoryFromViewReply(videoDetail);

        if (history != null)
        {
            if (subVideos.Count() > 0)
            {
                var historyVideo = subVideos.FirstOrDefault(p => p.Id.Equals(history.Identifier.Id));
                history.Identifier = historyVideo;
            }
            else if (sections.Count() > 0)
            {
                history.Identifier = sections
                    .SelectMany(p => p.Videos)
                    .FirstOrDefault(p => p.AlternateId == history.Identifier.Id)
                    .Identifier;
            }
        }

        var tags = videoDetail.Tag.Select(p => new Models.Data.Community.Tag(p.Id.ToString(), p.Name.TrimStart('#'), p.Uri));

        return new VideoPlayerView(
            videoInfo,
            publisherCommunity,
            subVideos,
            sections,
            relatedVideos,
            history,
            operation,
            interaction,
            tags);
    }

    /// <summary>
    /// 将视频页详情 <see cref="ViewReply"/> 转换为视频信息.
    /// </summary>
    /// <param name="response">视频详情.</param>
    /// <returns><see cref="VideoPlayerView"/>.</returns>
    public static VideoPlayerView ConvertToVideoView(VideoPageResponse response)
    {
        var videoInfo = GetVideoInformationFromVideoPageResponse(response);
        var subVideos = GetSubVideosFromVideoPageResponse(response);
        return new VideoPlayerView(videoInfo, default, subVideos, default, default, default, default, default, default);
    }

    /// <summary>
    /// 将稍后再看响应 <see cref="ViewLaterResponse"/> 转换为视频集.
    /// </summary>
    /// <param name="response">稍后在看响应结果.</param>
    /// <returns><see cref="VideoSet"/>.</returns>
    public static VideoSet ConvertToVideoSet(ViewLaterResponse response)
    {
        var count = response.Count;
        var items = response.List == null
            ? new List<VideoInformation>()
            : response.List.Select(ConvertToVideoInformation).ToList();
        return new VideoSet(items, count);
    }

    /// <summary>
    /// 将用户空间视频集 <see cref="UserSpaceVideoSet"/> 转换为视频集.
    /// </summary>
    /// <param name="set">稍后在看响应结果.</param>
    /// <returns><see cref="VideoSet"/>.</returns>
    public static VideoSet ConvertToVideoSet(UserSpaceVideoSet set)
    {
        var count = set?.Count ?? 0;
        var items = set?.List == null
            ? new List<VideoInformation>()
            : set.List.Select(ConvertToVideoInformation).ToList();
        return new VideoSet(items, count);
    }

    /// <summary>
    /// 将视频搜索结果 <see cref="SearchArchiveReply"/> 转换为视频集.
    /// </summary>
    /// <param name="reply">稍后在看响应结果.</param>
    /// <returns><see cref="VideoSet"/>.</returns>
    public static VideoSet ConvertToVideoSet(SearchArchiveReply reply)
    {
        var count = Convert.ToInt32(reply.Total);
        var items = reply.Archives == null
            ? new List<VideoInformation>()
            : reply.Archives.Select(ConvertToVideoInformation).ToList();
        return new VideoSet(items, count);
    }

    /// <summary>
    /// 将历史记录响应结果 <see cref="CursorV2Reply"/> 转换为历史记录视图.
    /// </summary>
    /// <param name="reply">历史记录响应结果.</param>
    /// <returns><see cref="VideoHistoryView"/>.</returns>
    public static VideoHistoryView ConvertToVideoHistoryView(CursorV2Reply reply)
    {
        var isFinished = !reply.HasMore;
        var items = reply.Items.Where(p => p.CardItemCase == CursorItem.CardItemOneofCase.CardUgc).Select(ConvertToVideoInformation).ToList();
        return new VideoHistoryView(items, isFinished);
    }

    private static VideoInformation GetVideoInformationFromEpisode(Episode episode)
    {
        var id = episode.Aid.ToString();
        var cid = episode.Cid.ToString();
        var title = Regex.Replace(episode.Title, "<[^>]+>", string.Empty);
        var duration = episode.Page.Duration;
        var publisher = UserAdapter.ConvertToRoleProfile(episode.Author);
        var communityInfo = CommunityAdapter.ConvertToVideoCommunityInformation(episode.Stat);
        var cover = ImageAdapter.ConvertToVideoCardCover(episode.Cover);
        var subtitle = episode.CoverRightText;

        var identifier = new VideoIdentifier(id, title, duration, cover);
        return new VideoInformation(identifier, publisher, cid, subtitle: subtitle, communityInformation: communityInfo);
    }

    private static VideoInformation GetVideoInformationFromVideoPageResponse(VideoPageResponse response)
    {
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(response.Title);
        var id = response.Aid.ToString();
        var bvid = response.Bvid;
        var duration = response.Duration;
        var cover = ImageAdapter.ConvertToImage(response.Cover);
        var publisher = UserAdapter.ConvertToRoleProfile(response.Owner, AvatarSize.Size32);
        var desc = TextToolkit.ConvertToTraditionalChineseIfNeeded(response.Description);
        var publishTime = DateTimeOffset.FromUnixTimeSeconds(response.PublishDateTime).DateTime;
        var communityInfo = CommunityAdapter.ConvertToVideoCommunityInformation(response.Stat);

        var identifier = new VideoIdentifier(id, title, duration, cover);
        return new VideoInformation(identifier, publisher, bvid, desc, publishTime: publishTime, communityInformation: communityInfo);
    }

    private static VideoInformation GetVideoInformationFromViewReply(ViewReply videoDetail)
    {
        var arc = videoDetail.Arc;
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(arc.Title);
        var id = arc.Aid.ToString();
        var bvid = videoDetail.Bvid;
        var duration = arc.Duration;
        var cover = ImageAdapter.ConvertToImage(arc.Pic);
        var collaborators = videoDetail.Staff.Count > 0
            ? videoDetail.Staff.Select(p => UserAdapter.ConvertToRoleProfile(p, AvatarSize.Size32))
            : null;
        var publisher = videoDetail.Staff.Count > 0
            ? null
            : UserAdapter.ConvertToRoleProfile(arc.Author, AvatarSize.Size32);
        var description = TextToolkit.ConvertToTraditionalChineseIfNeeded(arc.Desc);
        var publishTime = DateTimeOffset.FromUnixTimeSeconds(arc.Pubdate).DateTime;
        var communityInfo = CommunityAdapter.ConvertToVideoCommunityInformation(arc.Stat);
        var isOriginal = videoDetail.Arc.Copyright == 1;

        var identifier = new VideoIdentifier(id, title, duration, cover);
        return new VideoInformation(
            identifier,
            publisher,
            bvid,
            description,
            publishTime: publishTime,
            collaborators: collaborators,
            communityInformation: communityInfo,
            isOriginal: isOriginal);
    }

    private static IEnumerable<VideoIdentifier> GetSubVideosFromVideoPageResponse(VideoPageResponse response)
    {
        var subVideos = new List<VideoIdentifier>();
        foreach (var page in response.SubVideos.OrderBy(p => p.Page))
        {
            var cid = page.Cid.ToString();
            var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(page.Title);
            var duration = page.Duration;
            var identifier = new VideoIdentifier(cid, title, duration, null);
            subVideos.Add(identifier);
        }

        return subVideos;
    }

    private static IEnumerable<VideoIdentifier> GetSubVideosFromViewReply(ViewReply videoDetail)
    {
        var subVideos = new List<VideoIdentifier>();
        foreach (var page in videoDetail.Pages.OrderBy(p => p.Page.Page_))
        {
            var cid = page.Page.Cid.ToString();
            var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(page.Page.Part);
            var duration = page.Page.Duration;
            var identifier = new VideoIdentifier(cid, title, duration, null);
            subVideos.Add(identifier);
        }

        return subVideos;
    }

    private static InteractionVideoRecord GetInteractionRecordFromViewReply(ViewReply videoDetail)
    {
        var interaction = videoDetail.Interaction;
        if (interaction == null)
        {
            return null;
        }

        var partId = interaction.HistoryNode != null
                ? interaction.HistoryNode.Cid.ToString()
                : videoDetail.Pages.First().Page.Cid.ToString();
        var nodeId = interaction.HistoryNode != null
                ? interaction.HistoryNode.NodeId.ToString()
                : string.Empty;
        var graphVersion = interaction.GraphVersion.ToString();

        return new InteractionVideoRecord(graphVersion, partId, nodeId);
    }

    private static UserCommunityInformation GetPublisherCommunityFromViewReply(ViewReply videoDetail)
    {
        var relation = UserRelationStatus.Unfollow;
        if (videoDetail.ReqUser.Attention == 1)
        {
            relation = videoDetail.ReqUser.GuestAttention == 1
                ? UserRelationStatus.Friends
                : UserRelationStatus.Following;
        }
        else if (videoDetail.ReqUser.GuestAttention == 1)
        {
            relation = UserRelationStatus.BeFollowed;
        }

        return new UserCommunityInformation()
        {
            Id = videoDetail.Arc.Author.Mid.ToString(),
            Relation = relation,
        };
    }

    private static VideoOpeartionInformation GetOperationInformationFromViewReply(ViewReply videoDetail)
    {
        var reqUser = videoDetail.ReqUser;
        var isLiked = reqUser.Like == 1;
        var isCoined = reqUser.Coin == 1;
        var isFavorited = reqUser.Favorite == 1;
        return new VideoOpeartionInformation(
            videoDetail.Arc.Aid.ToString(),
            isLiked,
            isCoined,
            isFavorited,
            false);
    }

    private static IEnumerable<VideoInformation> GetRelatedVideosFromViewReply(ViewReply videoDetail)
    {
        // 将非视频内容过滤掉.
        var relates = videoDetail.Relates.Where(p => p.Goto.Equals(ServiceConstants.Av, StringComparison.OrdinalIgnoreCase));
        var relatedVideos = relates.Select(ConvertToVideoInformation);
        return relatedVideos;
    }

    private static IEnumerable<VideoSeason> GetVideoSectionsFromViewReply(ViewReply videoDetail)
    {
        if (videoDetail.UgcSeason == null || videoDetail.UgcSeason.Sections?.Count == 0)
        {
            return null;
        }

        var sections = new List<VideoSeason>();
        foreach (var item in videoDetail.UgcSeason.Sections)
        {
            var id = item.Id.ToString();
            var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(item.Title);
            var videos = item.Episodes.Select(GetVideoInformationFromEpisode);
            var section = new VideoSeason(id, title, videos);
            sections.Add(section);
        }

        return sections;
    }

    private static PlayedProgress GetHistoryFromViewReply(ViewReply videoDetail)
    {
        // 当历史记录为空，或者当前视频为交互视频时（交互视频按照节点记录历史），返回 null.
        if (videoDetail.History == null || videoDetail.Interaction != null)
        {
            return null;
        }

        var history = videoDetail.History;
        var historyStatus = history.Progress switch
        {
            0 => PlayedProgressStatus.NotStarted,
            -1 => PlayedProgressStatus.Finish,
            _ => PlayedProgressStatus.Playing
        };

        var progress = historyStatus == PlayedProgressStatus.Playing
            ? history.Progress
            : 0;

        var id = history.Cid.ToString();
        var identifier = new VideoIdentifier(id, default, default, default);
        return new PlayedProgress(progress, historyStatus, identifier);
    }
}
