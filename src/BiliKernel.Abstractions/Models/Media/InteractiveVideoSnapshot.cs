// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

namespace Richasy.BiliKernel.Models.Media;

/// <summary>
/// 互动视频记录点.
/// </summary>
public sealed class InteractiveVideoSnapshot(
    string version,
    string? partId = default,
    string? nodeId = default)
{

    /// <summary>
    /// 版本.
    /// </summary>
    public string GraphVersion { get; } = version;

    /// <summary>
    /// 分集 ID.
    /// </summary>
    public string? PartId { get; } = partId;

    /// <summary>
    /// 节点 ID.
    /// </summary>
    public string? NodeId { get; } = nodeId;
}
