// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

namespace Richasy.BiliKernel.Models.Appearance;

/// <summary>
/// 表情包.
/// </summary>
public sealed class EmotePackage
{
    /// <summary>
    /// 名称.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 图标.
    /// </summary>
    public BiliImage Icon { get; set; }

    /// <summary>
    /// 表情列表.
    /// </summary>
    public List<Emote> Images { get; set; }
}
