// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

namespace Richasy.BiliKernel.Models.Media;

/// <summary>
/// 视频分集.
/// </summary>
public sealed class VideoPart(
    MediaIdentifier identifier,
    int index,
    int duration)
{

    /// <summary>
    /// 标识.
    /// </summary>
    public MediaIdentifier Identifier { get; } = identifier;

    /// <summary>
    /// 索引.
    /// </summary>
    public int Index { get; } = index;

    /// <summary>
    /// 时长.
    /// </summary>
    public int Duration { get; } = duration;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VideoPart part && EqualityComparer<MediaIdentifier>.Default.Equals(Identifier, part.Identifier);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Identifier);
}
