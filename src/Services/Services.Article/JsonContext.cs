using System.Collections.Generic;
using System.Text.Json.Serialization;
using Richasy.BiliKernel.Content;
using Richasy.BiliKernel.Services.Article.Core.Models;
using Richasy.BiliKernel.Services.Article.Core;

namespace Richasy.BiliKernel.Services.Article;


[JsonSerializable(typeof(BiliDataResponse<ArticleRecommendResponse>))]
[JsonSerializable(typeof(BiliDataResponse<ArticleViewResponse>))]
[JsonSerializable(typeof(ArticleContentResponse))]
[JsonSerializable(typeof(BiliDataResponse<List<Core.Article>>))]
[JsonSerializable(typeof(OpusParagraph))]
[JsonSerializable(typeof(BiliDataResponse<List<ArticleCategory>>))]
internal partial class JsonContext : JsonSerializerContext
{

}
