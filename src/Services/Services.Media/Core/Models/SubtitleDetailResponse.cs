// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Richasy.BiliKernel.Services.Media.Core.Models;

/// <summary>
/// 字幕详情响应结果.
/// </summary>
internal sealed class SubtitleDetailResponse
{
    /// <summary>
    /// 字幕列表.
    /// </summary>
    [JsonPropertyName("body")]
    public List<SubtitleItem> Body { get; set; }
}

/// <summary>
/// 字幕条目.
/// </summary>
internal sealed class SubtitleItem
{
    /// <summary>
    /// 开始时间.
    /// </summary>
    [JsonPropertyName("from")]
    public double From { get; set; }

    /// <summary>
    /// 结束时间.
    /// </summary>
    [JsonPropertyName("to")]
    public double To { get; set; }

    /// <summary>
    /// 字幕内容.
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; }
}
