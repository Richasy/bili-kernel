// Copyright (c) Richasy. All rights reserved.

using System.Text.Json.Serialization;

namespace Richasy.BiliKernel.Services.Media.Core.Models;

/// <summary>
/// BUVID 响应.
/// </summary>
public class BuvidResponse
{
    /// <summary>
    /// B3 文本.
    /// </summary>
    [JsonPropertyName("b_3")]
    public string B3 { get; set; }

    /// <summary>
    /// B4 文本.
    /// </summary>
    [JsonPropertyName("b_4")]
    public string B4 { get; set; }
}
