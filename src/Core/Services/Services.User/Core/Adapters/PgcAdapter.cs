// Copyright (c) Richasy. All rights reserved.

using System;
using System.Text.RegularExpressions;
using Bilibili.App.Interface.V1;
using Richasy.BiliKernel.Adapters;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;

namespace Richasy.BiliKernel.Services.User.Core;

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
        var identifier = new MediaIdentifier(episodeId, title, cover);
        var info = new EpisodeInformation(identifier, duration);
        info.AddExtensionIfNotNull(EpisodeExtensionDataId.Subtitle, subtitle);
        info.AddExtensionIfNotNull(EpisodeExtensionDataId.SeasonId, seasonId);
        info.AddExtensionIfNotNull(EpisodeExtensionDataId.Aid, aid);
        info.AddExtensionIfNotNull(EpisodeExtensionDataId.CollectTime, viewTime);
        info.AddExtensionIfNotNull(EpisodeExtensionDataId.RecommendReason, string.IsNullOrEmpty(episode.Badge) ? default : episode.Badge);
        return info;
    }

    public static SeasonInformation ToSeasonInformation(this FavoritePgcItem item)
    {
        var title = item.Title;
        var subtitle = item.NewEpisode?.DisplayText;
        var ssid = item.SeasonId.ToString();
        var type = item.SeasonTypeName switch
        {
            "电影" => EntertainmentType.Movie,
            "电视剧" => EntertainmentType.TV,
            "纪录片" => EntertainmentType.Documentary,
            _ => EntertainmentType.Anime,
        };
        var identifier = new MediaIdentifier(ssid, title, item.Cover.ToPgcCover());
        var info = new SeasonInformation(identifier);
        info.AddExtensionIfNotNull(SeasonExtensionDataId.Subtitle, subtitle);
        info.AddExtensionIfNotNull(SeasonExtensionDataId.PgcType, type);
        info.AddExtensionIfNotNull(SeasonExtensionDataId.Highlight, item.BadgeText);
        info.AddExtensionIfNotNull(SeasonExtensionDataId.EpisodeId, item.NewEpisode?.Id);
        return info;
    }
}
