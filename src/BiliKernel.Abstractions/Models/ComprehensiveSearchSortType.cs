// Copyright (c) Richasy. All rights reserved.

namespace Richasy.BiliKernel.Models;

/// <summary>
/// 综合搜索排序方式.
/// </summary>
public enum ComprehensiveSearchSortType
{
    /// <summary>
    /// 默认.
    /// </summary>
    Default = 0,

    /// <summary>
    /// 播放次数.
    /// </summary>
    Play = 1,

    /// <summary>
    /// 按时间顺序.
    /// </summary>
    Newest = 2,

    /// <summary>
    /// 弹幕.
    /// </summary>
    Danmaku = 3,
}
