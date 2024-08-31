using System.Text.Json.Serialization;
using Richasy.BiliKernel.Content;
using Richasy.BiliKernel.Services.Comment.Core.Models;

namespace Richasy.BiliKernel.Services.Comment;


[JsonSerializable(typeof(BiliDataResponse<EmoteResponse>))]
internal partial class JsonContext : JsonSerializerContext
{

}
