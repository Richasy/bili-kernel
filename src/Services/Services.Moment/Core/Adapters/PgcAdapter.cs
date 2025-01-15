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
        var cover = pgc.Cover.ToVideoCover();
        var duration = pgc.Duration;
        var playCount = pgc.CoverLeftText2.ToCountNumber("观看");
        var danmakuCount = pgc.CoverLeftText3.ToCountNumber("弹幕");

        var identifier = new MediaIdentifier(epId.ToString(), title, cover);
        var community = new VideoCommunityInformation(epId.ToString(), playCount, danmakuCount);
        var info = new EpisodeInformation(identifier, duration, communityInformation: community);
        info.AddExtensionIfNotNull(EpisodeExtensionDataId.Aid, aid);
        info.AddExtensionIfNotNull(EpisodeExtensionDataId.SeasonId, ssid);
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

        var identifier = new MediaIdentifier(epid.ToString(), title, cover);
        var info = new EpisodeInformation(identifier, duration);
        info.AddExtensionIfNotNull(EpisodeExtensionDataId.Aid, aid);
        info.AddExtensionIfNotNull(EpisodeExtensionDataId.SeasonId, ssid);
        info.AddExtensionIfNotNull(EpisodeExtensionDataId.IsPreview, isPv);
        return info;
    }
}
