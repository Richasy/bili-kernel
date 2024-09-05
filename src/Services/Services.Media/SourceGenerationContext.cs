﻿// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using Richasy.BiliKernel.Content;
using Richasy.BiliKernel.Services.Media.Core;
using Richasy.BiliKernel.Services.Media.Core.Models;

namespace Richasy.BiliKernel;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(ArchiveRelationResponse))]
[JsonSerializable(typeof(BuvidResponse))]
[JsonSerializable(typeof(CuratedPlaylistResponse))]
[JsonSerializable(typeof(CuratedPlaylistVideo))]
[JsonSerializable(typeof(CuratedPlaylistReason))]
[JsonSerializable(typeof(EpisodeInteractionResponse))]
[JsonSerializable(typeof(LiveAppPlayResponse))]
[JsonSerializable(typeof(LiveAppPlayData))]
[JsonSerializable(typeof(LiveAppPlayUrlInfo))]
[JsonSerializable(typeof(LiveAppQualityDescription))]
[JsonSerializable(typeof(LiveAppPlayStream))]
[JsonSerializable(typeof(LiveAppPlayFormat))]
[JsonSerializable(typeof(LiveAppPlayUrl))]
[JsonSerializable(typeof(LiveAppPlayCodec))]
[JsonSerializable(typeof(LiveAreaDetailResponse))]
[JsonSerializable(typeof(LiveAreaDetailTag))]
[JsonSerializable(typeof(LiveAreaResponse))]
[JsonSerializable(typeof(LiveArea))]
[JsonSerializable(typeof(LiveAreaGroup))]
[JsonSerializable(typeof(LiveDanmakuResponse))]
[JsonSerializable(typeof(WebLiveDanmakuHost))]
[JsonSerializable(typeof(LiveFeedResponse))]
[JsonSerializable(typeof(LiveFeedCard))]
[JsonSerializable(typeof(LiveFeedCardData))]
[JsonSerializable(typeof(LiveFeedRoom))]
[JsonSerializable(typeof(LiveRoomCard))]
[JsonSerializable(typeof(LiveRoomCard.LiveCoverContent))]
[JsonSerializable(typeof(LiveRoomCard.LiveFeedback))]
[JsonSerializable(typeof(LiveRoomCard.LiveFeedback.LiveFeedbackReason))]
[JsonSerializable(typeof(LiveFeedFollowUserList))]
[JsonSerializable(typeof(LiveRoomBase))]
[JsonSerializable(typeof(LiveQualityDescription))]
[JsonSerializable(typeof(LiveRoomDetailResponse))]
[JsonSerializable(typeof(LiveRoomInformation))]
[JsonSerializable(typeof(LiveAnchorInformation))]
[JsonSerializable(typeof(LiveUserBasicInformation))]
[JsonSerializable(typeof(LiveLevelInformation))]
[JsonSerializable(typeof(LiveRelationInformation))]
[JsonSerializable(typeof(LiveMedalInformation))]
[JsonSerializable(typeof(PartitionVideo))]
[JsonSerializable(typeof(PgcDetailResponse))]
[JsonSerializable(typeof(PgcPlayerDialog))]
[JsonSerializable(typeof(PgcPublishInformation))]
[JsonSerializable(typeof(PgcEpisodeDetail))]
[JsonSerializable(typeof(PgcSeries))]
[JsonSerializable(typeof(PgcUserStatus))]
[JsonSerializable(typeof(PgcStaff))]
[JsonSerializable(typeof(PgcInformationStat))]
[JsonSerializable(typeof(PgcEpisodeStat))]
[JsonSerializable(typeof(PgcProgress))]
[JsonSerializable(typeof(PgcActivityTab))]
[JsonSerializable(typeof(PgcRating))]
[JsonSerializable(typeof(PgcArea))]
[JsonSerializable(typeof(PgcDetailModule))]
[JsonSerializable(typeof(PgcDetailModuleData))]
[JsonSerializable(typeof(PgcSeason))]
[JsonSerializable(typeof(PgcIndex))]
[JsonSerializable(typeof(PgcCelebrity))]
[JsonSerializable(typeof(PgcModuleReport))]
[JsonSerializable(typeof(PgcIndexConditionResponse))]
[JsonSerializable(typeof(PgcIndexFilter))]
[JsonSerializable(typeof(PgcIndexFilterValue))]
[JsonSerializable(typeof(PgcIndexOrder))]
[JsonSerializable(typeof(PgcIndexResultResponse))]
[JsonSerializable(typeof(PgcIndexItem))]
[JsonSerializable(typeof(PgcIndexFirstEpisode))]
[JsonSerializable(typeof(PgcTimeLineResponse))]
[JsonSerializable(typeof(PgcTimeLineItem))]
[JsonSerializable(typeof(TimeLineEpisode))]
[JsonSerializable(typeof(PlayerInformation))]
[JsonSerializable(typeof(DashVideo))]
[JsonSerializable(typeof(DashItem))]
[JsonSerializable(typeof(SegmentBase))]
[JsonSerializable(typeof(VideoFormat))]
[JsonSerializable(typeof(FlvItem))]
[JsonSerializable(typeof(PlayerWbiResponse))]
[JsonSerializable(typeof(PublisherInfo))]
[JsonSerializable(typeof(RecommendVideoResponse))]
[JsonSerializable(typeof(RecommendCard))]
[JsonSerializable(typeof(VideoArgs))]
[JsonSerializable(typeof(PlayerArgs))]
[JsonSerializable(typeof(OverflowFlyoutItem))]
[JsonSerializable(typeof(OverflowReason))]
[JsonSerializable(typeof(SubPartition))]
[JsonSerializable(typeof(PartitionTag))]
[JsonSerializable(typeof(SubtitleDetailResponse))]
[JsonSerializable(typeof(SubtitleItem))]
[JsonSerializable(typeof(SubtitleIndexResponse))]
[JsonSerializable(typeof(SubtitleIndexItem))]
[JsonSerializable(typeof(VideoPageResponse))]
[JsonSerializable(typeof(VideoPageVideo))]
[JsonSerializable(typeof(VideoPageVideo.Rights))]
[JsonSerializable(typeof(Dimension))]
[JsonSerializable(typeof(VideoPagePart))]
[JsonSerializable(typeof(VideoPageAuthor))]
[JsonSerializable(typeof(VideoPageAuthorDetail))]
[JsonSerializable(typeof(VideoPageAuthorDetail.Level_Info))]
[JsonSerializable(typeof(Vip))]
[JsonSerializable(typeof(VideoPageTag))]
[JsonSerializable(typeof(VideoPageRelated))]
[JsonSerializable(typeof(UgcSeason))]
[JsonSerializable(typeof(VideoPageSection))]
[JsonSerializable(typeof(VideoEpisode))]
[JsonSerializable(typeof(VideoEpisodeArc))]
[JsonSerializable(typeof(VideoPartition))]
[JsonSerializable(typeof(VideoStatusInfo))]
[JsonSerializable(typeof(EnterRoomMessage))]
[JsonSerializable(typeof(BiliDataResponse<LiveAreaResponse>))]
[JsonSerializable(typeof(BiliDataResponse<LiveAreaDetailResponse>))]
[JsonSerializable(typeof(BiliDataResponse<LiveFeedResponse>))]
[JsonSerializable(typeof(BiliResultResponse<PgcTimeLineResponse>))]
[JsonSerializable(typeof(BiliDataResponse<PgcIndexConditionResponse>))]
[JsonSerializable(typeof(BiliDataResponse<PgcIndexResultResponse>))]
[JsonSerializable(typeof(BiliDataResponse<PlayerInformation>))]
[JsonSerializable(typeof(BiliDataResponse<LiveRoomDetailResponse>))]
[JsonSerializable(typeof(BiliDataResponse<LiveAppPlayResponse>))]
[JsonSerializable(typeof(BiliDataResponse<PgcDetailResponse>))]
[JsonSerializable(typeof(BiliResultResponse<PlayerInformation>))]
[JsonSerializable(typeof(BiliDataResponse<EpisodeInteractionResponse>))]
[JsonSerializable(typeof(BiliDataResponse<BuvidResponse>))]
[JsonSerializable(typeof(BiliDataResponse<LiveDanmakuResponse>))]
[JsonSerializable(typeof(BiliDataResponse<PlayerWbiResponse>))]
[JsonSerializable(typeof(BiliDataResponse<ArchiveRelationResponse>))]
[JsonSerializable(typeof(BiliDataResponse<VideoPageResponse>))]
[JsonSerializable(typeof(BiliDataResponse<List<VideoPartition>>))]
[JsonSerializable(typeof(BiliDataResponse<SubPartition>))]
[JsonSerializable(typeof(BiliDataResponse<List<PartitionVideo>>))]
[JsonSerializable(typeof(BiliDataResponse<CuratedPlaylistResponse>))]
[JsonSerializable(typeof(BiliDataResponse<RecommendVideoResponse>))]
internal sealed partial class SourceGenerationContext : JsonSerializerContext
{
}