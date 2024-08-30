// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;

namespace Richasy.BiliKernel.Models.Media;

/// <summary>
/// 直播媒体信息.
/// </summary>
public sealed class LiveMediaInformation(
    string id,
    IList<PlayerFormatInformation> formats,
    IList<LiveLineInformation> lines)
{
    /// <summary>
    /// 直播间 Id.
    /// </summary>
    public string Id { get; } = id;

    /// <summary>
    /// 格式列表.
    /// </summary>
    public IList<PlayerFormatInformation> Formats { get; } = formats;

    /// <summary>
    /// 播放线路列表.
    /// </summary>
    public IList<LiveLineInformation> Lines { get; } = lines;
}
