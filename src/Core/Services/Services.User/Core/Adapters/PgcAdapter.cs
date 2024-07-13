// Copyright (c) Richasy. All rights reserved.

using System;
using System.Text.RegularExpressions;
using Bilibili.App.Interface.V1;
using Richasy.BiliKernel.Adapters;
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
}
