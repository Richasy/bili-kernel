﻿// Copyright (c) Richasy. All rights reserved.

using System.Text.Json.Serialization;
using Richasy.BiliKernel.Content;
using Richasy.BiliKernel.Services.Search.Core;

namespace Richasy.BiliKernel;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(HotSearchResponse))]
[JsonSerializable(typeof(WebHotSearchItem))]
[JsonSerializable(typeof(SearchRecommendResponse))]
[JsonSerializable(typeof(WebSearchRecommendItem))]
[JsonSerializable(typeof(BiliDataResponse<HotSearchResponse>))]
[JsonSerializable(typeof(BiliDataResponse<SearchRecommendResponse>))]
internal sealed partial class SourceGenerationContext : JsonSerializerContext
{
}
