using System.Collections.Generic;
using System.Text.Json.Serialization;
using Richasy.BiliKernel.Content;
using Richasy.BiliKernel.Services.User.Core;
using Richasy.BiliKernel.Services.User.Core.Models;

namespace Richasy.BiliKernel.Services.User;


[JsonSerializable(typeof(BiliDataResponse<SendMessageResponse>))]
[JsonSerializable(typeof(BiliDataResponse<ChatSessionListResponse>))]
[JsonSerializable(typeof(BiliDataResponse<List<BiliChatUser>>))]
[JsonSerializable(typeof(BiliDataResponse<LikeMessageResponse>))]
[JsonSerializable(typeof(BiliDataResponse<ReplyMessageResponse>))]
[JsonSerializable(typeof(BiliDataResponse<AtMessageResponse>))]
[JsonSerializable(typeof(BiliDataResponse<FavoriteDetailListResponse>))]
[JsonSerializable(typeof(BiliDataResponse<UgcSeasonDetailResponse>))]
[JsonSerializable(typeof(BiliDataResponse<VideoFavoriteListResponse>))]
[JsonSerializable(typeof(BiliDataResponse<FavoriteListResponse>))]
[JsonSerializable(typeof(BiliResultResponse<PgcFavoriteListResponse>))]
[JsonSerializable(typeof(BiliDataResponse<ArticleFavoriteListResponse>))]
[JsonSerializable(typeof(BiliDataResponse<FavoriteDetailListResponse>))]
[JsonSerializable(typeof(BiliDataResponse<VideoFavoriteGalleryResponse>))]
[JsonSerializable(typeof(BiliDataResponse<ChatMessageResponse>))]
[JsonSerializable(typeof(BiliDataResponse<UnreadMessage>))]
[JsonSerializable(typeof(BiliDataResponse<UserRelationResponse>))]
[JsonSerializable(typeof(BiliDataResponse<List<RelatedUser>>))]
[JsonSerializable(typeof(BiliDataResponse<List<RelatedTag>>))]
[JsonSerializable(typeof(BiliDataResponse<Mine>))]
[JsonSerializable(typeof(BiliDataResponse<MyInfo>))]
[JsonSerializable(typeof(BiliDataResponse<RelatedUserResponse>))]
[JsonSerializable(typeof(BiliDataResponse<ViewLaterSetResponse>))]
[JsonSerializable(typeof(BiliDataResponse<bool>))]
[JsonSerializable(typeof(ContentString))]
internal partial class JsonContext : JsonSerializerContext
{

}

internal class ContentString
{
    [JsonPropertyName("content")]
    public string Content { get; set; }
}
