// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

namespace Richasy.BiliKernel.Models.Subtitle;

/// <summary>
/// 字幕元数据信息.
/// </summary>
public sealed class SubtitleMeta
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SubtitleMeta"/> class.
    /// </summary>
    public SubtitleMeta(string id, string name, string url)
    {
        Id = id;
        LanguageName = name;
        Url = url;
        IsAI = url.Contains("ai_subtitle");
    }

    /// <summary>
    /// 标识符.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// 显示的语言名称.
    /// </summary>
    public string LanguageName { get; }

    /// <summary>
    /// 字幕地址.
    /// </summary>
    public string Url { get; }

    /// <summary>
    /// 是否是AI字幕.
    /// </summary>
    public bool IsAI { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is SubtitleMeta information && Id == information.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
