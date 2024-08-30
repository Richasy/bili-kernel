// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;

namespace Richasy.BiliKernel.Models.Media;

/// <summary>
/// 视频合集.
/// </summary>
public sealed class VideoSeason(
    string id,
    string title,
    IList<VideoInformation> videos)
{

    /// <summary>
    /// 合集 ID.
    /// </summary>
    public string Id { get; } = id;

    /// <summary>
    /// 标题.
    /// </summary>
    public string Title { get; } = title;

    /// <summary>
    /// 视频列表.
    /// </summary>
    public IList<VideoInformation> Videos { get; } = videos;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VideoSeason season && Id == season.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
