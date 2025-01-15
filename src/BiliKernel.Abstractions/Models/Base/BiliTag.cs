// Copyright (c) Richasy. All rights reserved.

using System;

namespace Richasy.BiliKernel.Models.Base;

/// <summary>
/// 哔哩标签.
/// </summary>
public sealed class BiliTag
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BiliTag"/> class.
    /// </summary>
    public BiliTag(string id, string name)
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// 标识.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// 名称.
    /// </summary>
    public string Name { get; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is BiliTag tag && Id == tag.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
