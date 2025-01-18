// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.BiliKernel.Content;
using Richasy.BiliKernel.Services.Article.Core;
using Richasy.BiliKernel.Services.Article.Core.Models;
using System.Text.Json.Serialization;

namespace Richasy.BiliKernel;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(ArticleCategory))]
[JsonSerializable(typeof(ArticleContentResponse))]
[JsonSerializable(typeof(Readinfo))]
[JsonSerializable(typeof(Category))]
[JsonSerializable(typeof(Author))]
[JsonSerializable(typeof(Official_Verify))]
[JsonSerializable(typeof(Vip))]
[JsonSerializable(typeof(Media))]
[JsonSerializable(typeof(Topic_Info))]
[JsonSerializable(typeof(Opus))]
[JsonSerializable(typeof(OpusContent))]
[JsonSerializable(typeof(OpusParagraph))]
[JsonSerializable(typeof(OpusParagraphFormat))]
[JsonSerializable(typeof(OpusLine))]
[JsonSerializable(typeof(OpusLinePic))]
[JsonSerializable(typeof(OpusText))]
[JsonSerializable(typeof(OpusNode))]
[JsonSerializable(typeof(OpusWord))]
[JsonSerializable(typeof(OpusTextStyle))]
[JsonSerializable(typeof(OpusPic))]
[JsonSerializable(typeof(OpusPicture))]
[JsonSerializable(typeof(OpusPubInfo))]
[JsonSerializable(typeof(ArticleTag))]
[JsonSerializable(typeof(ArticleRecommendResponse))]
[JsonSerializable(typeof(Article))]
[JsonSerializable(typeof(PublisherInfo))]
[JsonSerializable(typeof(ArticleStats))]
[JsonSerializable(typeof(ArticleViewResponse))]
[JsonSerializable(typeof(Stats))]
[JsonSerializable(typeof(BiliDataResponse<List<ArticleCategory>>))]
[JsonSerializable(typeof(BiliDataResponse<List<Article>>))]
[JsonSerializable(typeof(BiliDataResponse<ArticleRecommendResponse>))]
[JsonSerializable(typeof(BiliDataResponse<ArticleViewResponse>))]
internal sealed partial class SourceGenerationContext : JsonSerializerContext
{
}
