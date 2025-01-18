// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.BiliKernel.Content;
using Richasy.BiliKernel.Services.Search.Core;
using System.Text.Json.Serialization;

namespace Richasy.BiliKernel;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(HotSearchResponse))]
[JsonSerializable(typeof(WebHotSearchItem))]
[JsonSerializable(typeof(SearchRecommendResponse))]
[JsonSerializable(typeof(WebSearchRecommendItem))]
[JsonSerializable(typeof(BiliDataResponse<HotSearchResponse>))]
[JsonSerializable(typeof(BiliDataResponse<SearchRecommendResponse>))]
[JsonSerializable(typeof(BiliDataResponse<SearchPartitionResponse>))]
internal sealed partial class SourceGenerationContext : JsonSerializerContext
{
}
