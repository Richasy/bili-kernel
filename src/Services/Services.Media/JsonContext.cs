using System.Collections.Generic;
using System.Text.Json.Serialization;
using Richasy.BiliKernel.Content;
using Richasy.BiliKernel.Services.Media.Core;
using Richasy.BiliKernel.Services.Media.Core.Models;
namespace Richasy.BiliKernel.Services.Media;

[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(LiveRoomInfo))]
[JsonSerializable(typeof(BiliDataResponse<EpisodeInteractionResponse>))]
[JsonSerializable(typeof(BiliDataResponse<PlayerInformation>))]
[JsonSerializable(typeof(BiliDataResponse<LiveRoomDetailResponse>))]
[JsonSerializable(typeof(BiliDataResponse<LiveAppPlayResponse>))]
[JsonSerializable(typeof(BiliDataResponse<PgcDetailResponse>))]
[JsonSerializable(typeof(BiliDataResponse<BuvidResponse>))]
[JsonSerializable(typeof(BiliDataResponse<LiveDanmakuResponse>))]
[JsonSerializable(typeof(BiliDataResponse<PlayerWbiResponse>))]
[JsonSerializable(typeof(BiliDataResponse<ArchiveRelationResponse>))]
[JsonSerializable(typeof(BiliDataResponse<VideoPageResponse>))]
[JsonSerializable(typeof(BiliResultResponse<PlayerInformation>))]
[JsonSerializable(typeof(BiliDataResponse<List<VideoPartition>>))]
[JsonSerializable(typeof(BiliDataResponse<SubPartition>))]
[JsonSerializable(typeof(BiliDataResponse<List<PartitionVideo>>))]
[JsonSerializable(typeof(BiliDataResponse<SubPartition>))]
[JsonSerializable(typeof(BiliDataResponse<CuratedPlaylistResponse>))]
[JsonSerializable(typeof(BiliDataResponse<RecommendVideoResponse>))]
[JsonSerializable(typeof(BiliDataResponse<LiveAreaResponse>))]
[JsonSerializable(typeof(BiliDataResponse<LiveAreaDetailResponse>))]
[JsonSerializable(typeof(BiliDataResponse<LiveFeedResponse>))]
[JsonSerializable(typeof(BiliDataResponse<PgcIndexResultResponse>))]
[JsonSerializable(typeof(BiliDataResponse<PgcIndexConditionResponse>))]
[JsonSerializable(typeof(BiliResultResponse<PgcTimeLineResponse>))]
[JsonSerializable(typeof(SubtitleIndexResponse))]
[JsonSerializable(typeof(SubtitleDetailResponse))]
internal partial class JsonContext : JsonSerializerContext
{

}
