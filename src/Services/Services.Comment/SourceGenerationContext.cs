// Copyright (c) Richasy. All rights reserved.

using System.Text.Json.Serialization;
using Richasy.BiliKernel.Content;
using Richasy.BiliKernel.Services.Comment.Core.Models;

namespace Richasy.BiliKernel.Services.Comment;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(EmoteResponse))]
[JsonSerializable(typeof(BiliEmotePackage))]
[JsonSerializable(typeof(BiliEmote))]
[JsonSerializable(typeof(BiliDataResponse<EmoteResponse>))]
internal sealed partial class SourceGenerationContext : JsonSerializerContext
{
}
