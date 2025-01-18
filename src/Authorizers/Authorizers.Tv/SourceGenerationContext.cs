// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.BiliKernel.Authorizers.TV.Core;
using Richasy.BiliKernel.Content;
using Richasy.BiliKernel.Models.Authorization;
using System.Text.Json.Serialization;

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
