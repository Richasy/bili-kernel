// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.BiliKernel.Content;
using Richasy.BiliKernel.Services.Comment.Core.Models;
using System.Text.Json.Serialization;

namespace Richasy.BiliKernel.Services.Comment;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(EmoteResponse))]
[JsonSerializable(typeof(BiliEmotePackage))]
[JsonSerializable(typeof(BiliEmote))]
[JsonSerializable(typeof(BiliDataResponse<EmoteResponse>))]
internal sealed partial class SourceGenerationContext : JsonSerializerContext
{
}
