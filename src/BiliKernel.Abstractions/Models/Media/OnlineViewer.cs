// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

namespace Richasy.BiliKernel.Models.Media;

/// <summary>
/// 在线观看人数.
/// </summary>
public readonly struct OnlineViewer(
    int count,
    string text)
{

    /// <summary>
    /// 总数.
    /// </summary>
    public int Count { get; } = count;

    /// <summary>
    /// 文本.
    /// </summary>
    public string Text { get; } = text;
}
