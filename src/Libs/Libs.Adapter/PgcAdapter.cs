// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Bilibili.App.Dynamic.V2;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Constants;
using Bili.Copilot.Models.BiliBili;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Constants.Player;
using Bili.Copilot.Models.Data.Appearance;
using Bili.Copilot.Models.Data.Pgc;
using Bili.Copilot.Models.Data.Player;
using Bili.Copilot.Models.Data.Video;

namespace Bili.Copilot.Libs.Adapter;

/// <summary>
/// PGC 内容适配器.
/// </summary>
public sealed class PgcAdapter
{
    /// <summary>
    /// 将单集详情 <see cref="PgcEpisodeDetail"/> 转换为单集信息.
    /// </summary>
    /// <param name="episode">单集详情.</param>
    /// <returns><see cref="EpisodeInformation"/>.</returns>
    public static EpisodeInformation ConvertToEpisodeInformation(PgcEpisodeDetail episode)
    {
        var epid = episode.Report.EpisodeId;
        var title = string.IsNullOrEmpty(episode.LongTitle)
            ? episode.ShareTitle
            : episode.LongTitle;
        title = TextToolkit.ConvertToTraditionalChineseIfNeeded(title);
        var subtitle = TextToolkit.ConvertToTraditionalChineseIfNeeded(episode.Subtitle);
        var duration = episode.Duration / 1000;
        var cover = ImageAdapter.ConvertToVideoCardCover(episode.Cover);
        var seasonId = episode.Report.SeasonId;
        var aid = episode.Aid.ToString();
        var cid = episode.PartId.ToString();
        var index = episode.Index;
        var isPv = episode.IsPV == 1;
        var isVip = episode.BadgeText.Contains("会员");
        var publishTime = DateTimeOffset.FromUnixTimeSeconds(episode.PublishTime).ToLocalTime().DateTime;
        var communityInfo = CommunityAdapter.ConvertToVideoCommunityInformation(episode.Stat);
        var seasonType = episode.Report.SeasonType;

        var identifier = new VideoIdentifier(epid, title, duration, cover);
        return new EpisodeInformation(
            identifier,
            seasonId,
            aid,
            subtitle,
            index,
            isVip,
            isPv,
            publishTime,
            communityInfo,
            episode.BadgeText,
            cid,
            seasonType);
    }

    /// <summary>
    /// 将推荐视频卡片 <see cref="RecommendCard"/> 转换为单集信息.
    /// </summary>
    /// <param name="card">推荐卡片.</param>
    /// <returns><see cref="EpisodeInformation"/>.</returns>
    public static EpisodeInformation ConvertToEpisodeInformation(RecommendCard card)
    {
        if (card.CardGoto != ServiceConstants.Bangumi)
        {
            throw new ArgumentException($"推荐卡片的 CardGoTo 属性应该是 {ServiceConstants.Bangumi}，这里是 {card.Goto}，不符合要求，请检查分配条件", nameof(card));
        }

        var epid = card.Parameter;
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(card.Title);
        var subtitle = TextToolkit.ConvertToTraditionalChineseIfNeeded(card.Description);
        var communityInfo = CommunityAdapter.ConvertToVideoCommunityInformation(card);
        var cover = ImageAdapter.ConvertToVideoCardCover(card.Cover);

        var identifier = new VideoIdentifier(epid, title, -1, cover);
        return new EpisodeInformation(identifier, subtitle: subtitle, communityInformation: communityInfo);
    }

    /// <summary>
    /// 将 PGC 模块条目 <see cref="PgcModuleItem"/> 转换为单集信息.
    /// </summary>
    /// <param name="item">PGC 模块条目.</param>
    /// <returns><see cref="EpisodeInformation"/>.</returns>
    public static EpisodeInformation ConvertToEpisodeInformation(PgcModuleItem item)
    {
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(item.Title);
        var epid = item.Aid.ToString();
        var ssid = item.OriginId.ToString();
        var cover = ImageAdapter.ConvertToVideoCardCover(item.Cover);
        var highlight = TextToolkit.ConvertToTraditionalChineseIfNeeded(item.Badge);
        var communityInfo = CommunityAdapter.ConvertToVideoCommunityInformation(item.Stat);
        communityInfo.Id = epid;
        var subtitle = item.Stat?.FollowDisplayText ?? item.DisplayScoreText;
        subtitle = TextToolkit.ConvertToTraditionalChineseIfNeeded(subtitle);

        var identifier = new VideoIdentifier(epid, title, -1, cover);
        return new EpisodeInformation(
            identifier,
            ssid,
            subtitle: subtitle,
            communityInformation: communityInfo,
            highlight: highlight);
    }

    /// <summary>
    /// 将 PGC 动态条目 <see cref="MdlDynPGC"/> 转换为单集信息.
    /// </summary>
    /// <param name="pgc">PGC 动态条目.</param>
    /// <returns><see cref="EpisodeInformation"/>.</returns>
    public static EpisodeInformation ConvertToEpisodeInformation(MdlDynPGC pgc)
    {
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(pgc.Title);
        var ssid = pgc.SeasonId.ToString();
        var epid = pgc.Epid.ToString();
        var aid = pgc.Aid.ToString();
        var isPv = pgc.IsPreview;
        var cover = ImageAdapter.ConvertToVideoCardCover(pgc.Cover);
        var duration = pgc.Duration;

        var identifier = new VideoIdentifier(epid, title, duration, cover);
        return new EpisodeInformation(identifier, ssid, aid, isPv: isPv);
    }

    /// <summary>
    /// 将视频动态条目 <see cref="MdlDynArchive"/> 转换为单集信息.
    /// </summary>
    /// <param name="archive">视频动态条目.</param>
    /// <returns><see cref="EpisodeInformation"/>.</returns>
    public static EpisodeInformation ConvertToEpisodeInformation(MdlDynArchive archive)
    {
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(archive.Title);
        var ssid = archive.PgcSeasonId.ToString();
        var epid = archive.EpisodeId.ToString();
        var aid = archive.Avid.ToString();
        var isPv = archive.IsPreview;
        var cover = ImageAdapter.ConvertToVideoCardCover(archive.Cover);
        var duration = archive.Duration;

        var identifier = new VideoIdentifier(epid, title, duration, cover);
        return new EpisodeInformation(identifier, ssid, aid, isPv: isPv);
    }

    /// <summary>
    /// 将 PGC 模块条目 <see cref="PgcModuleItem"/> 转换为剧集信息.
    /// </summary>
    /// <param name="item">PGC 模块条目.</param>
    /// <param name="type">PGC 内容类型.</param>
    /// <returns><see cref="SeasonInformation"/>.</returns>
    public static SeasonInformation ConvertToSeasonInformation(PgcModuleItem item, PgcType type)
    {
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(item.Title);
        var ssid = item.OriginId.ToString();
        var cover = ImageAdapter.ConvertToPgcCover(item.Cover);
        var tags = TextToolkit.ConvertToTraditionalChineseIfNeeded(item.SeasonTags);
        var highlight = TextToolkit.ConvertToTraditionalChineseIfNeeded(item.Badge);
        var communityInfo = CommunityAdapter.ConvertToVideoCommunityInformation(item.Stat);
        communityInfo.Id = ssid;
        var subtitle = TextToolkit.ConvertToTraditionalChineseIfNeeded(item.Description);
        var description = item.Stat?.FollowDisplayText ?? item.DisplayScoreText;
        description = TextToolkit.ConvertToTraditionalChineseIfNeeded(description);
        var isTracking = item.Status?.IsFollow == 1;

        var identifier = new VideoIdentifier(ssid, title, -1, cover);
        return new SeasonInformation(
            identifier,
            subtitle,
            highlight,
            tags,
            type: type,
            description: description,
            communityInformation: communityInfo,
            isTracking: isTracking);
    }

    /// <summary>
    /// 将 PGC 搜索条目 <see cref="PgcSearchItem"/> 转换为剧集信息.
    /// </summary>
    /// <param name="item">PGC 搜索条目.</param>
    /// <returns><see cref="SeasonInformation"/>.</returns>
    public static SeasonInformation ConvertToSeasonInformation(PgcSearchItem item)
    {
        var ssid = item.SeasonId.ToString();
        var title = Regex.Replace(item.Title, "<[^>]+>", string.Empty);
        title = TextToolkit.ConvertToTraditionalChineseIfNeeded(title);
        var subtitle = TextToolkit.ConvertToTraditionalChineseIfNeeded(item.Label);
        var tags = TextToolkit.ConvertToTraditionalChineseIfNeeded(item.SubTitle);
        var cover = ImageAdapter.ConvertToPgcCover(item.Cover);
        var highlight = TextToolkit.ConvertToTraditionalChineseIfNeeded(item.BadgeText);
        var isTracking = item.IsFollow == 1;
        var type = GetPgcTypeFromTypeText(item.SeasonTypeName);
        var description = TextToolkit.ConvertToTraditionalChineseIfNeeded(item.Area);
        var ratingCount = Convert.ToInt32(item.VoteNumber);
        var communityInfo = CommunityAdapter.ConvertToVideoCommunityInformation(item);

        var identifier = new VideoIdentifier(ssid, title, -1, cover);
        return new SeasonInformation(
            identifier,
            subtitle,
            highlight,
            tags,
            ratingCount,
            description: description,
            type: type,
            communityInformation: communityInfo,
            isTracking: isTracking);
    }

    /// <summary>
    /// 将 PGC 索引条目 <see cref="PgcIndexItem"/> 转换为剧集信息.
    /// </summary>
    /// <param name="item">PGC 索引条目.</param>
    /// <returns><see cref="SeasonInformation"/>.</returns>
    public static SeasonInformation ConvertToSeasonInformation(PgcIndexItem item)
    {
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(item.Title);
        var ssid = item.SeasonId.ToString();
        var tags = TextToolkit.ConvertToTraditionalChineseIfNeeded(item.OrderText);
        var cover = ImageAdapter.ConvertToPgcCover(item.Cover);
        var highlight = TextToolkit.ConvertToTraditionalChineseIfNeeded(item.BadgeText);
        var description = TextToolkit.ConvertToTraditionalChineseIfNeeded(item.AdditionalText);
        var subtitle = item.IsFinish == 1
            ? "已完结"
            : "连载中";
        subtitle = TextToolkit.ConvertToTraditionalChineseIfNeeded(subtitle);

        var identifier = new VideoIdentifier(ssid, title, -1, cover);
        return new SeasonInformation(
            identifier,
            subtitle,
            highlight,
            tags,
            description: description);
    }

    /// <summary>
    /// 将时间线剧集 <see cref="TimeLineEpisode"/> 转换为剧集信息.
    /// </summary>
    /// <param name="item">时间线剧集.</param>
    /// <returns><see cref="SeasonInformation"/>.</returns>
    public static SeasonInformation ConvertToSeasonInformation(TimeLineEpisode item)
    {
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(item.Title);
        var ssid = item.SeasonId.ToString();
        var publishTime = DateTimeOffset.FromUnixTimeSeconds(item.PublishTimeStamp).ToLocalTime().DateTime;
        var tags = TextToolkit.ConvertToTraditionalChineseIfNeeded(item.PublishIndex);
        var cover = ImageAdapter.ConvertToPgcCover(item.Cover);
        var description = item.IsPublished == 1
            ? "已更新"
            : "待发布";
        description = TextToolkit.ConvertToTraditionalChineseIfNeeded(description);
        var subtitle = publishTime.ToString("MM/dd HH:mm");

        var identifier = new VideoIdentifier(ssid, title, -1, cover);
        return new SeasonInformation(identifier, subtitle, tags: tags, description: description);
    }

    /// <summary>
    /// 将 PGC 播放清单的剧集条目 <see cref="PgcPlayListSeason"/> 转换为剧集信息.
    /// </summary>
    /// <param name="season">PGC 播放清单条目.</param>
    /// <returns><see cref="SeasonInformation"/>.</returns>
    public static SeasonInformation ConvertToSeasonInformation(PgcPlayListSeason season)
    {
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(season.Title);
        var ssid = season.SeasonId.ToString();
        var tags = TextToolkit.ConvertToTraditionalChineseIfNeeded(season.Styles);
        var subtitle = TextToolkit.ConvertToTraditionalChineseIfNeeded(season.Subtitle);
        var description = TextToolkit.ConvertToTraditionalChineseIfNeeded(season.Description);
        var highlight = TextToolkit.ConvertToTraditionalChineseIfNeeded(season.BadgeText);
        var cover = ImageAdapter.ConvertToPgcCover(season.Cover);
        var communityInfo = CommunityAdapter.ConvertToVideoCommunityInformation(season.Stat);
        communityInfo.Id = ssid;
        if (season.Rating != null)
        {
            communityInfo.Score = season.Rating.Score;
        }

        var identifier = new VideoIdentifier(ssid, title, -1, cover);
        return new SeasonInformation(
            identifier,
            subtitle,
            highlight,
            tags,
            description: description,
            communityInformation: communityInfo);
    }

    /// <summary>
    /// 将收藏的 PGC 条目 <see cref="FavoritePgcItem"/> 转换为剧集信息.
    /// </summary>
    /// <param name="item">收藏的 PGC 条目.</param>
    /// <returns><see cref="SeasonInformation"/>.</returns>
    public static SeasonInformation ConvertToSeasonInformation(FavoritePgcItem item)
    {
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(item.Title);
        var subtitle = item.NewEpisode?.DisplayText ?? string.Empty;
        subtitle = TextToolkit.ConvertToTraditionalChineseIfNeeded(subtitle);
        var ssid = item.SeasonId.ToString();
        var type = GetPgcTypeFromTypeText(item.SeasonTypeName);
        var cover = ImageAdapter.ConvertToPgcCover(item.Cover);
        var highlight = TextToolkit.ConvertToTraditionalChineseIfNeeded(item.BadgeText);
        var description = TextToolkit.ConvertToTraditionalChineseIfNeeded(item.SeasonTypeName);

        var identifier = new VideoIdentifier(ssid, title, -1, cover);
        return new SeasonInformation(
            identifier,
            subtitle,
            highlight,
            type: type,
            description: description);
    }

    /// <summary>
    /// 将 PGC 展示信息 <see cref="PgcDisplayInformation"/> 封装成视图信息.
    /// </summary>
    /// <param name="display">PGC 展示信息.</param>
    /// <returns><see cref="PgcPlayerView"/>.</returns>
    public static PgcPlayerView ConvertToPgcPlayerView(PgcDisplayInformation display)
    {
        var seasonInfo = GetSeasonInformationFromDisplayInformation(display);
        List<VideoIdentifier> seasons = null;
        List<EpisodeInformation> episodes = null;
        Dictionary<string, IEnumerable<EpisodeInformation>> extras = null;
        PlayedProgress history = null;

        if (display.Modules != null)
        {
            var seasonModule = display.Modules.FirstOrDefault(p => p.Style == ServiceConstants.Season);
            if (seasonModule != null)
            {
                seasons = new List<VideoIdentifier>();
                foreach (var item in seasonModule.Data.Seasons)
                {
                    var cover = ImageAdapter.ConvertToImage(item.Cover, 240, 320);
                    seasons.Add(new VideoIdentifier(item.SeasonId.ToString(), item.Title, -1, cover));
                }
            }

            var episodeModule = display.Modules.FirstOrDefault(p => p.Style == ServiceConstants.Positive);
            if (episodeModule != null)
            {
                episodes = episodeModule.Data.Episodes
                    .Select(p => ConvertToEpisodeInformation(p))
                    .ToList();
            }

            var sectionModules = display.Modules.Where(p => p.Style == ServiceConstants.Section);
            if (sectionModules.Any())
            {
                extras = new Dictionary<string, IEnumerable<EpisodeInformation>>();
                foreach (var section in sectionModules)
                {
                    var title = section.Title;
                    if (extras.ContainsKey(title))
                    {
                        var count = extras.Keys.Count(p => p.StartsWith(title)) + 1;
                        title += count;
                    }

                    if (
                        section.Data?.Episodes?.Any() ?? false)
                    {
                        extras.Add(title, section.Data.Episodes.Select(p => ConvertToEpisodeInformation(p)));
                    }
                }
            }
        }

        if (display.UserStatus?.Progress != null && episodes != null)
        {
            var progress = display.UserStatus.Progress;
            var historyEpid = progress.LastEpisodeId.ToString();
            var historyEp = episodes.Any(p => p.Identifier.Id == historyEpid)
                ? episodes.FirstOrDefault(p => p.Identifier.Id == historyEpid)
                : episodes.FirstOrDefault(p => p.Index.ToString() == progress.LastEpisodeIndex);

            if (historyEp != null)
            {
                var status = progress.LastTime switch
                {
                    -1 => PlayedProgressStatus.Finish,
                    0 => PlayedProgressStatus.NotStarted,
                    _ => PlayedProgressStatus.Playing
                };
                history = new PlayedProgress(progress.LastTime, status, historyEp.Identifier);
            }
        }

        var warning = display.Warning?.Message;

        return new PgcPlayerView(seasonInfo, seasons, episodes, extras, history, warning);
    }

    /// <summary>
    /// 将 PGC 模块 <see cref="PgcModule"/> 转换为播放列表.
    /// </summary>
    /// <param name="module">PGC 模块.</param>
    /// <returns><see cref="PgcPlaylist"/>.</returns>
    public static PgcPlaylist ConvertToPgcPlaylist(PgcModule module)
    {
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(module.Title);
        var id = string.Empty;
        if (module.Headers?.Count > 0)
        {
            var header = module.Headers.First();
            if (header.Url.Contains("/sl"))
            {
                var uri = new Uri(header.Url);
                id = uri.Segments.Where(p => p.Contains("sl"))
                    .Select(p => p.Replace("sl", string.Empty))
                    .FirstOrDefault();
            }
        }

        var seasons = module.Items.Select(p => ConvertToSeasonInformation(p, PgcType.Bangumi | PgcType.Domestic));
        return new PgcPlaylist(title, id, seasons: seasons);
    }

    /// <summary>
    /// 将 PGC 播放列表响应 <see cref="PgcPlayListResponse"/> 转换为播放列表.
    /// </summary>
    /// <param name="response">PGC 播放列表响应.</param>
    /// <returns><see cref="PgcPlaylist"/>.</returns>
    public static PgcPlaylist ConvertToPgcPlaylist(PgcPlayListResponse response)
    {
        var id = response.Id.ToString();
        var subtitle = $"{response.Total} · {response.Description}";
        subtitle = TextToolkit.ConvertToTraditionalChineseIfNeeded(subtitle);
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(response.Title);
        var seasons = response.Seasons.Select(p => ConvertToSeasonInformation(p));
        return new PgcPlaylist(title, id, subtitle, seasons);
    }

    /// <summary>
    /// 将 PGC 页面响应 <see cref="PgcResponse"/> 转换为 PGC 页面视图信息.
    /// </summary>
    /// <param name="response">PGC 页面响应.</param>
    /// <returns><see cref="PgcPageView"/>.</returns>
    public static PgcPageView ConvertToPgcPageView(PgcResponse response)
    {
        var banners = response.Modules.Where(p => p.Style.Contains("banner"))
            .SelectMany(p => p.Items)
            .Select(p => CommunityAdapter.ConvertToBannerIdentifier(p));

        var originRanks = response.Modules.Where(p => p.Style.Contains("rank"))
            .SelectMany(p => p.Items);

        var ranks = new Dictionary<string, IEnumerable<EpisodeInformation>>();
        foreach (var item in originRanks)
        {
            var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(item.Title);
            var subRanks = item.Cards.Take(3).Select(p => ConvertToEpisodeInformation(p)).ToList();
            ranks.Add(title, subRanks);
        }

        var partitionId = response.FeedIdentifier == null
            ? string.Empty
            : response.FeedIdentifier.PartitionIds.First().ToString();

        var playLists = response.Modules.Where(p => p.Style == "v_card" || p.Style.Contains("feed"))
            .Select(p => ConvertToPgcPlaylist(p))
            .ToList();

        var seasons = response.Modules.Where(p => p.Style == "common_feed")
            .SelectMany(p => p.Items)
            .Select(p => ConvertToSeasonInformation(p, PgcType.Documentary | PgcType.Movie | PgcType.TV))
            .ToList();

        return new PgcPageView(banners, ranks, playLists, seasons, partitionId);
    }

    /// <summary>
    /// 将 PGC 索引条件响应 <see cref="PgcIndexConditionResponse"/> 转换为筛选条件列表.
    /// </summary>
    /// <param name="response">PGC 索引条件响应.</param>
    /// <returns>筛选条件列表.</returns>
    public static IEnumerable<Filter> ConvertToFilters(PgcIndexConditionResponse response)
    {
        var filters = response.FilterList
            .Select(p => GetFilterFromIndexFilter(p))
            .ToList();
        if (response.OrderList?.Count > 0)
        {
            var name = TextToolkit.ConvertToTraditionalChineseIfNeeded("排序方式");
            var id = "order";
            var conditions = response.OrderList.Select(p => new Condition(TextToolkit.ConvertToTraditionalChineseIfNeeded(p.Name), p.Field)).ToList();
            filters.Insert(0, new Filter(name, id, conditions));
        }

        return filters;
    }

    /// <summary>
    /// 将 PGC 时间线响应结果 <see cref="PgcTimeLineResponse"/> 转换为时间线视图.
    /// </summary>
    /// <param name="response">时间线响应结果.</param>
    /// <returns><see cref="TimelineView"/>.</returns>
    public static TimelineView ConvertToTimelineView(PgcTimeLineResponse response)
    {
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(response.Title);
        var desc = TextToolkit.ConvertToTraditionalChineseIfNeeded(response.Subtitle);
        var timelines = new List<TimelineInformation>();
        foreach (var item in response.Data)
        {
            var seasons = item.Episodes?.Count > 0
                ? item.Episodes.Select(p => ConvertToSeasonInformation(p)).ToList()
                : null;
            var info = new TimelineInformation(item.Date, ConvertDayOfWeek(item.DayOfWeek), item.IsToday == 1, seasons);
            timelines.Add(info);
        }

        return new TimelineView(title, desc, timelines);
    }

    /// <summary>
    /// 将PGC收藏内容响应 <see cref="PgcFavoriteListResponse"/> 转换为剧集集合.
    /// </summary>
    /// <param name="response">PGC收藏内容响应.</param>
    /// <returns><see cref="SeasonSet"/>.</returns>
    public static SeasonSet ConvertToSeasonSet(PgcFavoriteListResponse response)
    {
        var total = response.Total;
        var items = response.FollowList != null
            ? response.FollowList.Select(p => ConvertToSeasonInformation(p))
            : new List<SeasonInformation>();
        return new SeasonSet(items, total);
    }

    private static SeasonInformation GetSeasonInformationFromDisplayInformation(PgcDisplayInformation display)
    {
        var ssid = display.SeasonId.ToString();
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(display.Title);
        var cover = ImageAdapter.ConvertToPgcCover(display.Cover);
        var subtitle = TextToolkit.ConvertToTraditionalChineseIfNeeded(display.Subtitle);
        var description = TextToolkit.ConvertToTraditionalChineseIfNeeded(display.Evaluate);
        var highlight = TextToolkit.ConvertToTraditionalChineseIfNeeded(display.BadgeText);
        var progress = display.PublishInformation.DisplayProgress;
        var publishDate = display.PublishInformation.DisplayPublishTime;
        var originName = display.OriginName;
        var alias = display.Alias;
        var tags = $"{display.TypeDescription}\n" +
            $"{display.PublishInformation.DisplayReleaseDate}\n" +
            $"{display.PublishInformation.DisplayProgress}";
        tags = TextToolkit.ConvertToTraditionalChineseIfNeeded(tags);
        var isTracking = display.UserStatus.IsFollow == 1;
        var ratingCount = display.Rating != null
            ? display.Rating.Count
            : -1;
        var labors = new Dictionary<string, string>();
        if (!string.IsNullOrEmpty(display.Actor?.Information))
        {
            labors.Add(
                TextToolkit.ConvertToTraditionalChineseIfNeeded(display.Actor.Title),
                TextToolkit.ConvertToTraditionalChineseIfNeeded(display.Actor.Information));
        }

        if (!string.IsNullOrEmpty(display.Staff?.Information))
        {
            labors.Add(
                TextToolkit.ConvertToTraditionalChineseIfNeeded(display.Staff.Title),
                TextToolkit.ConvertToTraditionalChineseIfNeeded(display.Staff.Information));
        }

        var celebrities = display.Celebrity?.Select(p => UserAdapter.ConvertToRoleProfile(p));
        var communityInfo = CommunityAdapter.ConvertToVideoCommunityInformation(display.InformationStat);
        communityInfo.Id = ssid;
        if (display.Rating != null)
        {
            communityInfo.Score = display.Rating.Score;
        }

        var type = GetPgcTypeFromTypeText(display.TypeName);

        var identifier = new VideoIdentifier(ssid, title, -1, cover);
        return new SeasonInformation(
            identifier,
            subtitle,
            highlight,
            tags,
            ratingCount,
            originName,
            alias,
            publishDate,
            progress,
            description,
            type,
            labors,
            communityInfo,
            celebrities,
            isTracking);
    }

    private static PgcType GetPgcTypeFromTypeText(string typeText)
    {
        return typeText switch
        {
            "国创" => PgcType.Domestic,
            "电影" => PgcType.Movie,
            "电视剧" => PgcType.TV,
            "纪录片" => PgcType.Documentary,
            _ => PgcType.Bangumi,
        };
    }

    private static Filter GetFilterFromIndexFilter(PgcIndexFilter filter)
    {
        var conditions = filter.Values.Select(p => new Condition(p.Name, p.Keyword));
        return new Filter(filter.Name, filter.Field, conditions);
    }

    private static string ConvertDayOfWeek(int day)
    {
        var dayOfWeek = day switch
        {
            1 => "一",
            2 => "二",
            3 => "三",
            4 => "四",
            5 => "五",
            6 => "六",
            7 => "日",
            _ => "-",
        };

        return TextToolkit.ConvertToTraditionalChineseIfNeeded($"周{dayOfWeek}");
    }
}
