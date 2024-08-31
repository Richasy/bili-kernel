// Copyright (c) Richasy. All rights reserved.

using System;

namespace Richasy.BiliKernel.Models.Appearance;

/// <summary>
/// 表情.
/// </summary>
public sealed class Emote
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Emote"/> class.
    /// </summary>
    public Emote(
        string id,
        string key,
        BiliImage img)
    {
        Id = id;
        Key = key;
        Image = img;
    }

    /// <summary>
    /// 标识.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// 替代文本.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// 图片信息.
    /// </summary>
    public BiliImage Image { get; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Emote emote && Id == emote.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
