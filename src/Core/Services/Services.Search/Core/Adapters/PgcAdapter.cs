// Copyright (c) Richasy. All rights reserved.

using System;
using System.Linq;
using Bilibili.Polymer.App.Search.V1;
using Richasy.BiliKernel.Adapters;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;

namespace Richasy.BiliKernel.Services.Search.Core;

internal static class PgcAdapter
{
    public static SeasonInformation ToSeasonInformation(this Item item)
    {
        if (item.CardItemCase == Item.CardItemOneofCase.Bangumi)
        {
            return item.GetBangumiSeason();
        }

        return default;
    }

    private static SeasonInformation GetBangumiSeason(this Item item)
    {
        var bangumi = item.Bangumi;
        var title = bangumi.Title.Replace("<em class=\"keyword\">", string.Empty).Replace("</em>", string.Empty);
        var identifier = new MediaIdentifier(bangumi.SeasonId.ToString(), title, bangumi.Cover.ToPgcCover());
        var info = new SeasonInformation(identifier, isTracking: bangumi.IsAtten == 1);
        info.AddExtensionIfNotNull(SeasonExtensionDataId.PgcType, EntertainmentType.Anime);
        info.AddExtensionIfNotNull(SeasonExtensionDataId.Subtitle, bangumi.Label);
        info.AddExtensionIfNotNull(SeasonExtensionDataId.Score, bangumi.Rating);
        info.AddExtensionIfNotNull(SeasonExtensionDataId.PublishTime, DateTimeOffset.FromUnixTimeSeconds(bangumi.Ptime));
        info.AddExtensionIfNotNull(SeasonExtensionDataId.Highlight, bangumi.BadgesV2.FirstOrDefault()?.Text);
        info.AddExtensionIfNotNull(SeasonExtensionDataId.Celebrity, bangumi.Cv);
        info.AddExtensionIfNotNull(SeasonExtensionDataId.EpisodeId, bangumi.Episodes.FirstOrDefault()?.Param);
        return info;
    }
}
