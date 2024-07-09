// Copyright (c) Richasy. All rights reserved.

using System;
using System.Linq;
using System.Text.RegularExpressions;
using Bilibili.App.Interface.V1;
using Richasy.BiliKernel.Adapters;
using Richasy.BiliKernel.Models.Appearance;
using Richasy.BiliKernel.Models.Media;
using Richasy.BiliKernel.Services.Media.Core.Models;

namespace Richasy.BiliKernel.Services.Media.Core;

internal static class PgcAdapter
{
    private static readonly Regex _episodeRegex = new Regex(@"ep(\d+)");

    public static EpisodeInformation ToEpisodeInformation(this CursorItem item)
    {
        var episode = item.CardOgv;
        var title = item.Title;
        var viewTime = DateTimeOffset.FromUnixTimeSeconds(item.ViewAt).ToLocalTime();
        var seasonId = item.Kid.ToString();
        var episodeId = item.Uri.Contains("ep") ? _episodeRegex.Match(item.Uri).Groups[1].Value : string.Empty;
        var aid = item.Oid.ToString();
        var subtitle = episode.Subtitle;
        var cover = episode.Cover.ToVideoCover();
        var duration = episode.Duration;
        var identifier = new VideoIdentifier(episodeId, title, duration, cover);
        var info = new EpisodeInformation(identifier);
        info.AddExtensionIfNotNull(EpisodeExtensionDataId.Subtitle, subtitle);
        info.AddExtensionIfNotNull(EpisodeExtensionDataId.SeasonId, seasonId);
        info.AddExtensionIfNotNull(EpisodeExtensionDataId.Aid, aid);
        info.AddExtensionIfNotNull(EpisodeExtensionDataId.CollectTime, viewTime);
        info.AddExtensionIfNotNull(EpisodeExtensionDataId.RecommendReason, string.IsNullOrEmpty(episode.Badge) ? default : episode.Badge);
        return info;
    }

    public static SeasonInformation ToSeasonInformation(this TimeLineEpisode item)
    {
        var title = item.Title;
        var id = item.SeasonId.ToString();
        var publishTime = DateTimeOffset.FromUnixTimeSeconds(item.PublishTimeStamp).ToLocalTime();
        var tags = item.PublishIndex;
        var cover = item.Cover.ToPgcCover();
        var identifier = new VideoIdentifier(id, title, default, cover);
        var info = new SeasonInformation(identifier);
        var publishState = item.IsPublished == 1 ? "已更新" : "待发布";
        info.AddExtensionIfNotNull(SeasonExtensionDataId.Subtitle, publishState);
        info.AddExtensionIfNotNull(SeasonExtensionDataId.Tags, tags);
        info.AddExtensionIfNotNull(SeasonExtensionDataId.PublishTime, publishTime);
        info.AddExtensionIfNotNull(SeasonExtensionDataId.EpisodeId, item.EpisodeId);
        return info;
    }

    public static SeasonInformation ToSeasonInformation(this PgcIndexItem item)
    {
        var title = item.Title;
        var id = item.SeasonId.ToString();
        var cover = item.Cover.ToPgcCover();
        var highlight = item.BadgeText;
        var desc = item.AdditionalText;
        var identifier = new VideoIdentifier(id, title, default, cover);
        var info = new SeasonInformation(identifier);
        info.AddExtensionIfNotNull(SeasonExtensionDataId.IsFinish, item.IsFinish == null ? default : item.IsFinish == 1);
        info.AddExtensionIfNotNull(SeasonExtensionDataId.Highlight, highlight);
        info.AddExtensionIfNotNull(SeasonExtensionDataId.Description, desc);
        info.AddExtensionIfNotNull(SeasonExtensionDataId.Score, string.IsNullOrEmpty(item.Score) ? default : double.Parse(item.Score));
        info.AddExtensionIfNotNull(SeasonExtensionDataId.Subtitle, item.Subtitle);
        info.AddExtensionIfNotNull(SeasonExtensionDataId.EpisodeId, item.FirstEpisode?.EpisodeId);
        return info;
    }

    public static TimelineInformation ToTimelineInformation(this PgcTimeLineItem item)
    {
        var seasons = item.Episodes?.Count > 0
            ? item.Episodes.Select(p => p.ToSeasonInformation()).ToList().AsReadOnly()
            : default;
        var date = DateTimeOffset.FromUnixTimeSeconds(item.DateTimeStamp).ToLocalTime();
        var isToday = date.Date.Equals(DateTimeOffset.Now.Date);
        return new TimelineInformation(item.Date, item.DayOfWeek, isToday, seasons);
    }

    public static Filter ToFilter(this PgcIndexFilter filter)
    {
        var conditions = filter.Values.Select(p => new Condition(p.Name, p.Keyword)).ToList();
        return new Filter(filter.Name, filter.Field, conditions);
    }
}
