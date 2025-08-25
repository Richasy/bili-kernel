// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

namespace Richasy.BiliKernel.Models.Danmaku;

/// <summary>
/// 弹幕元信息.
/// </summary>
public sealed class DanmakuMeta
{
    /// <summary>
    /// 弹幕分段数.
    /// </summary>
    public int SegmentCount { get; set; }

    /// <summary>
    /// 弹幕总数.
    /// </summary>
    public int Total { get; set; }
}
