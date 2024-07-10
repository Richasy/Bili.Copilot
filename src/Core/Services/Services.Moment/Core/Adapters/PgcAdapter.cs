// Copyright (c) Richasy. All rights reserved.

using Bilibili.App.Dynamic.V2;
using Richasy.BiliKernel.Adapters;
using Richasy.BiliKernel.Models.Media;

namespace Richasy.BiliKernel.Services.Moment.Core;

internal static class PgcAdapter
{
    public static EpisodeInformation ToEpisodeInformation(this MdlDynPGC pgc)
    {
        var title = pgc.Title;
        var ssid = pgc.SeasonId;
        var epId = pgc.Epid;
        var aid = pgc.Aid;
        var isPv = pgc.IsPreview;
        var cover = pgc.Cover.ToVideoCover();
        var duration = pgc.Duration;

        var identifier = new VideoIdentifier(epId.ToString(), title, cover);
        var info = new EpisodeInformation(identifier, duration);
        info.AddExtensionIfNotNull(EpisodeExtensionDataId.Aid, aid);
        info.AddExtensionIfNotNull(EpisodeExtensionDataId.SeasonId, ssid);
        info.AddExtensionIfNotNull(EpisodeExtensionDataId.IsPreview, isPv);
        return info;
    }

    public static EpisodeInformation ToEpisodeInformation(this MdlDynArchive archive)
    {
        var title = archive.Title;
        var ssid = archive.PgcSeasonId;
        var epid = archive.EpisodeId;
        var aid = archive.Avid;
        var isPv = archive.IsPreview;
        var cover = archive.Cover.ToVideoCover();
        var duration = archive.Duration;

        var identifier = new VideoIdentifier(epid.ToString(), title, cover);
        var info = new EpisodeInformation(identifier, duration);
        info.AddExtensionIfNotNull(EpisodeExtensionDataId.Aid, aid);
        info.AddExtensionIfNotNull(EpisodeExtensionDataId.SeasonId, ssid);
        info.AddExtensionIfNotNull(EpisodeExtensionDataId.IsPreview, isPv);
        return info;
    }
}
