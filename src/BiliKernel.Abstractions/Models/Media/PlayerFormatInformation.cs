// Copyright (c) Richasy. All rights reserved.

using System;

namespace Richasy.BiliKernel.Models.Media;

/// <summary>
/// 播放器格式信息.
/// </summary>
public sealed class PlayerFormatInformation(int quality, string description, bool needVip, bool needLogin)
{
    /// <summary>
    /// 视频清晰度.
    /// </summary>
    public int Quality { get; } = quality;

    /// <summary>
    /// 清晰度描述.
    /// </summary>
    public string Description { get; } = description;

    /// <summary>
    /// 是否需要登录或大会员才能观看.
    /// </summary>
    public bool NeedVip { get; } = needVip;

    /// <summary>
    /// 是否需要登录才能观看.
    /// </summary>
    public bool NeedLogin { get; } = needLogin;

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is PlayerFormatInformation information && Quality == information.Quality;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Quality);
}
