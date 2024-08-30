// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;

namespace Richasy.BiliKernel.Models.Media;

/// <summary>
/// 直播播放器视图.
/// </summary>
public sealed class LivePlayerView(
    LiveInformation info,
    LiveTag tag)
{
    /// <summary>
    /// 直播间信息.
    /// </summary>
    public LiveInformation Information { get; } = info;

    /// <summary>
    /// 直播间所属分区及分类.
    /// </summary>
    public LiveTag Tag { get; } = tag;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is LivePlayerView view && EqualityComparer<LiveInformation>.Default.Equals(Information, view.Information);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Information);
}
