// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

namespace Richasy.BiliKernel.Models.Media;

/// <summary>
/// Dash 播放媒体信息.
/// </summary>
public sealed class DashMediaInformation(
    IList<DashSegmentInformation>? videoSegments,
    IList<DashSegmentInformation>? audioSegments,
    IList<PlayerFormatInformation>? formats,
    int duration = 0)
{
    /// <summary>
    /// 视频时长.
    /// </summary>
    public int Duration { get; } = duration;

    /// <summary>
    /// 不同清晰度的视频列表.
    /// </summary>
    public IList<DashSegmentInformation>? Videos { get; } = videoSegments;

    /// <summary>
    /// 不同码率的音频列表.
    /// </summary>
    public IList<DashSegmentInformation>? Audios { get; } = audioSegments;

    /// <summary>
    /// 格式列表.
    /// </summary>
    public IList<PlayerFormatInformation>? Formats { get; } = formats;
}
