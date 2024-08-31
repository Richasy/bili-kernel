// Copyright (c) Richasy. All rights reserved.

using System.Text.Json.Serialization;
using Richasy.BiliKernel.Models.Authorization;

namespace Richasy.BiliKernel.Resolvers.WinUIToken;

[JsonSerializable(typeof(BiliToken))]
internal partial class JsonContext : JsonSerializerContext
{
}
