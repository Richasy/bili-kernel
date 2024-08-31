// Copyright (c) Richasy. All rights reserved.

using System.Text.Json.Serialization;
using Richasy.BiliKernel.Authorizers.TV.Core;
using Richasy.BiliKernel.Content;
using Richasy.BiliKernel.Models.Authorization;

namespace Richasy.BiliKernel;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(TVQRCode))]
[JsonSerializable(typeof(BiliDataResponse<TVQRCode>))]
[JsonSerializable(typeof(BiliResponse))]
[JsonSerializable(typeof(BiliToken))]
[JsonSerializable(typeof(BiliDataResponse<BiliToken>))]
internal sealed partial class SourceGenerationContext : JsonSerializerContext
{
}
