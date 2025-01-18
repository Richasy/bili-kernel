// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.BiliKernel.Content;
using System.Text.Json.Serialization;
using static Richasy.BiliKernel.Authenticator.BiliAuthenticator;

namespace Richasy.BiliKernel;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(WebNavResponse))]
[JsonSerializable(typeof(BiliDataResponse<WebNavResponse>))]
internal sealed partial class SourceGenerationContext : JsonSerializerContext
{
}
