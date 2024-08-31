using System.Text.Json.Serialization;
using Richasy.BiliKernel.Content;
using Richasy.BiliKernel.Services.Search.Core;

namespace Richasy.BiliKernel.Services.Search;


[JsonSerializable(typeof(BiliDataResponse<SearchRecommendResponse>))]
[JsonSerializable(typeof(BiliDataResponse<HotSearchResponse>))]
internal partial class JsonContext : JsonSerializerContext
{

}
