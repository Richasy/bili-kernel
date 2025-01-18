// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

namespace Richasy.BiliKernel.Models.Media;

/// <summary>
/// 直播线路信息.
/// </summary>
public sealed class LiveLineInformation(
    string name,
    int quality,
    IList<int> acceptQualities,
    IList<LivePlayUrl> urls)
{
    /// <summary>
    /// 解码名.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// 清晰度标识.
    /// </summary>
    public int Quality { get; } = quality;

    /// <summary>
    /// 支持的清晰度标识.
    /// </summary>
    public IList<int> AcceptQualities { get; } = acceptQualities;

    /// <summary>
    /// 播放地址列表.
    /// </summary>
    public IList<LivePlayUrl> Urls { get; } = urls;
}
