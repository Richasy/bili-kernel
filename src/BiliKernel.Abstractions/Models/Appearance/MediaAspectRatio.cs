// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

namespace Richasy.BiliKernel.Models.Appearance;

/// <summary>
/// 媒体宽高比.
/// </summary>
public struct MediaAspectRatio
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediaAspectRatio"/> struct.
    /// </summary>
    public MediaAspectRatio(int width, int height)
    {
        Width = width;
        Height = height;
    }

    /// <summary>
    /// 宽度.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// 高度.
    /// </summary>
    public int Height { get; set; }
}
