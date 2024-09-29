// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Richasy.BiliKernel.Adapters;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Appearance;
using Richasy.BiliKernel.Models.Media;
using Richasy.BiliKernel.Services.Media.Core.Models;

namespace Richasy.BiliKernel.Services.Media.Core;

internal static class PgcAdapter
{
    private static readonly Regex _episodeRegex = new Regex(@"ep(\d+)");

    public static SeasonInformation ToSeasonInformation(this TimeLineEpisode item)
    {
        var title = item.Title;
        var id = item.SeasonId.ToString();
        var publishTime = DateTimeOffset.FromUnixTimeSeconds(item.PublishTimeStamp).ToLocalTime();
        var tags = item.PublishIndex;
        var cover = item.Cover.ToPgcCover();
        var identifier = new MediaIdentifier(id, title, cover);
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
        var identifier = new MediaIdentifier(id, title, cover);
        var info = new SeasonInformation(identifier);
        info.AddExtensionIfNotNull(SeasonExtensionDataId.IsFinish, item.IsFinish == null ? default : item.IsFinish == 1);
        info.AddExtensionIfNotNull(SeasonExtensionDataId.Highlight, highlight);
        info.AddExtensionIfNotNull(SeasonExtensionDataId.Description, desc);
        info.AddExtensionIfNotNull(SeasonExtensionDataId.Score, string.IsNullOrEmpty(item.Score) ? default : double.Parse(item.Score));
        info.AddExtensionIfNotNull(SeasonExtensionDataId.Subtitle, item.Subtitle);
        info.AddExtensionIfNotNull(SeasonExtensionDataId.EpisodeId, item.FirstEpisode?.EpisodeId);
        return info;
    }

    public static EpisodeInformation ToEpisodeInformation(this PgcEpisodeDetail episode)
    {
        var epid = episode.Report.EpisodeId;
        var title = string.IsNullOrEmpty(episode.LongTitle)
            ? episode.ShareTitle
            : episode.LongTitle;
        var subtitle = episode.Subtitle;
        var duration = episode.Duration / 1000;
        var cover = episode.Cover.ToVideoCover();
        var seasonId = episode.Report.SeasonId;
        var aid = episode.Aid;
        var cid = episode.PartId;
        var index = episode.Index;
        var isPv = episode.BadgeText.Contains("预告");
        var isVip = episode.BadgeText.Contains("会员");
        var publishTime = DateTimeOffset.FromUnixTimeSeconds(episode.PublishTime).ToLocalTime().DateTime;
        var communityInfo = new VideoCommunityInformation(epid, episode.Stat.PlayCount, episode.Stat.DanmakuCount, episode.Stat.LikeCount, default, episode.Stat.CoinCount, episode.Stat.ReplyCount);
        var seasonType = episode.Report.SeasonType;
        var width = episode.Dimension?.width ?? 1920;
        var height = episode.Dimension?.height ?? 1080;
        var ratio = new MediaAspectRatio(width, height);

        var identifier = new MediaIdentifier(epid, title, cover);
        var info = new EpisodeInformation(identifier, duration, index, publishTime, communityInfo);
        info.AddExtensionIfNotNull(EpisodeExtensionDataId.Subtitle, subtitle);
        info.AddExtensionIfNotNull(EpisodeExtensionDataId.SeasonId, seasonId);
        info.AddExtensionIfNotNull(EpisodeExtensionDataId.Aid, aid);
        info.AddExtensionIfNotNull(EpisodeExtensionDataId.Cid, cid);
        info.AddExtensionIfNotNull(EpisodeExtensionDataId.IsPreview, isPv);
        info.AddExtensionIfNotNull(EpisodeExtensionDataId.IsVip, isVip);
        info.AddExtensionIfNotNull(EpisodeExtensionDataId.SeasonType, seasonType);
        info.AddExtensionIfNotNull(EpisodeExtensionDataId.AspectRatio, ratio);
        info.AddExtensionIfNotNull(EpisodeExtensionDataId.RecommendReason, episode.BadgeText);
        return info;
    }

    public static TimelineInformation ToTimelineInformation(this PgcTimeLineItem item)
    {
        var seasons = item.Episodes?.Count > 0
            ? item.Episodes.Select(p => p.ToSeasonInformation()).ToList().AsReadOnly()
            : default;
        var date = DateTimeOffset.FromUnixTimeSeconds(item.DateTimeStamp).ToLocalTime();
        var isToday = date.Date.Equals(DateTimeOffset.Now.Date);
        return new TimelineInformation(item.Date, date.DayOfWeek, item.DateTimeStamp, isToday, seasons);
    }

    public static Filter ToFilter(this PgcIndexFilter filter)
    {
        var conditions = filter.Values.Select(p => new Condition(p.Name, p.Keyword)).ToList();
        return new Filter(filter.Name, filter.Field, conditions);
    }

    public static SeasonInformation ToSeasonInformation(this PgcDetailResponse display)
    {
        var ssid = display.SeasonId.ToString();
        var title = display.Title;
        var cover = display.Cover.ToImage();
        var subtitle = display.Subtitle;
        var description = display.Evaluate;
        var highlight = display.BadgeText;
        var progress = display.PublishInformation.DisplayProgress;
        var publishDate = display.PublishInformation.DisplayPublishTime;
        var originName = display.OriginName;
        var alias = display.Alias;
        var isTracking = display.UserStatus.IsFollow == 1;
        var ratingCount = display.Rating != null
            ? display.Rating.Count
            : -1;
        var labors = new Dictionary<string, string>();
        if (!string.IsNullOrEmpty(display.Actor?.Information))
        {
            labors.Add(display.Actor.Title, display.Actor.Information);
        }

        if (!string.IsNullOrEmpty(display.Staff?.Information))
        {
            labors.Add(display.Staff.Title, display.Staff.Information);
        }

        var celebrities = display.Celebrity?.Select(p => p.ToPublisherProfile());
        var stat = display.InformationStat;
        var communityInfo = new VideoCommunityInformation(ssid, stat.PlayCount, stat.DanmakuCount, stat.LikeCount, stat.FavoriteCount, stat.CoinCount, stat.ReplyCount, stat.ShareCount, stat.SeriesFavoriteCount);
        var score = display.Rating?.Score;

        var type = GetPgcTypeFromTypeText(display.TypeName);

        var identifier = new MediaIdentifier(ssid, title, cover);
        var info = new SeasonInformation(identifier, communityInfo, isTracking);
        info.AddExtensionIfNotNull(SeasonExtensionDataId.Subtitle, subtitle);
        info.AddExtensionIfNotNull(SeasonExtensionDataId.Description, description);
        info.AddExtensionIfNotNull(SeasonExtensionDataId.Highlight, highlight);
        info.AddExtensionIfNotNull(SeasonExtensionDataId.PublishTime, publishDate);
        info.AddExtensionIfNotNull(SeasonExtensionDataId.OriginName, originName);
        info.AddExtensionIfNotNull(SeasonExtensionDataId.Alias, alias);
        info.AddExtensionIfNotNull(SeasonExtensionDataId.RatingCount, ratingCount);
        info.AddExtensionIfNotNull(SeasonExtensionDataId.LaborSections, labors);
        info.AddExtensionIfNotNull(SeasonExtensionDataId.Celebrity, celebrities);
        info.AddExtensionIfNotNull(SeasonExtensionDataId.Score, score);
        info.AddExtensionIfNotNull(SeasonExtensionDataId.PgcType, type);
        return info;
    }

    public static PgcPlayerView ToPgcPlayerView(this PgcDetailResponse display)
    {
        var seasonInfo = display.ToSeasonInformation();
        List<SeasonInformation> seasons = null;
        List<EpisodeInformation> episodes = null;
        Dictionary<string, IList<EpisodeInformation>> extras = null;
        PlayerProgress history = null;

        if (display.Modules != null)
        {
            var seasonModule = display.Modules.FirstOrDefault(p => p.Style == "season");
            if (seasonModule != null)
            {
                seasons = [];
                foreach (var item in seasonModule.Data.Seasons)
                {
                    var cover = item.Cover.ToPgcCover();
                    var identifier = new MediaIdentifier(item.SeasonId.ToString(), item.Title, cover);
                    var info = new SeasonInformation(identifier);
                    info.AddExtensionIfNotNull(SeasonExtensionDataId.Highlight, item.Badge);
                    seasons.Add(info);
                }
            }

            var episodeModule = display.Modules.FirstOrDefault(p => p.Style == "positive");
            if (episodeModule != null)
            {
                episodes = episodeModule.Data.Episodes
                    .Select(p => p.ToEpisodeInformation())
                    .ToList();
            }

            var sectionModules = display.Modules.Where(p => p.Style == "section");
            if (sectionModules.Any())
            {
                extras = new Dictionary<string, IList<EpisodeInformation>>();
                foreach (var section in sectionModules)
                {
                    var title = section.Title;
                    if (extras.ContainsKey(title))
                    {
                        var count = extras.Keys.Count(p => p.StartsWith(title)) + 1;
                        title += count;
                    }

                    if (section.Data?.Episodes?.Any() ?? false)
                    {
                        extras.Add(title, section.Data.Episodes.Select(p => p.ToEpisodeInformation()).ToList());
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
                    -1 => PlayerProgressStatus.Finish,
                    0 => PlayerProgressStatus.NotStarted,
                    _ => PlayerProgressStatus.Playing
                };
                history = new PlayerProgress(progress.LastTime, status, historyEp.Identifier.Id);
            }
        }

        var warning = display.Warning?.Message;
        return new PgcPlayerView(seasonInfo, seasons, episodes, extras, history, warning);
    }

    private static EntertainmentType GetPgcTypeFromTypeText(string typeText)
    {
        return typeText switch
        {
            "电影" => EntertainmentType.Movie,
            "电视剧" => EntertainmentType.TV,
            "纪录片" => EntertainmentType.Documentary,
            _ => EntertainmentType.Anime,
        };
    }
}
