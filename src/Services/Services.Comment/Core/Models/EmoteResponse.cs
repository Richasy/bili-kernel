// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Richasy.BiliKernel.Services.Comment.Core.Models;

/// <summary>
/// 表情包响应.
/// </summary>
internal sealed class EmoteResponse
{
    /// <summary>
    /// 表情包集合.
    /// </summary>
    [JsonPropertyName("packages")]
    public IList<BiliEmotePackage> Packages { get; set; }
}

/// <summary>
/// 表情包.
/// </summary>
internal sealed class BiliEmotePackage
{
    /// <summary>
    /// 标识符.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 对应文本.
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>
    /// 图标地址.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// 表情集合.
    /// </summary>
    [JsonPropertyName("emote")]
    public IList<BiliEmote>? Emotes { get; set; }
}

/// <summary>
/// 表情.
/// </summary>
internal sealed class BiliEmote
{
    /// <summary>
    /// 标识符.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 表情包标识符.
    /// </summary>
    [JsonPropertyName("package_id")]
    public int PackageId { get; set; }

    /// <summary>
    /// 文本.
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>
    /// 图标地址.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
