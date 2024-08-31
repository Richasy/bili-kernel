// Copyright (c) Richasy. All rights reserved.

using System.Text.Json.Serialization;
using Richasy.BiliKernel.Content;
using static Richasy.BiliKernel.Authenticator.BiliAuthenticator;

namespace Richasy.BiliKernel;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(WebNavResponse))]
[JsonSerializable(typeof(BiliDataResponse<WebNavResponse>))]
internal sealed partial class SourceGenerationContext : JsonSerializerContext
{
}
