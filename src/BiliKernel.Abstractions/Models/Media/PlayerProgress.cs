// Copyright (c) Richasy. All rights reserved.

namespace Richasy.BiliKernel.Models.Media;

/// <summary>
/// 播放进度.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PlayerProgress"/> class.
/// </remarks>
public sealed class PlayerProgress(
    double progress,
    PlayerProgressStatus status,
    string cid)
{
    /// <summary>
    /// 进度（秒）.
    /// </summary>
    public double Progress { get; } = progress;

    /// <summary>
    /// 状态.
    /// </summary>
    public PlayerProgressStatus Status { get; } = status;

    /// <summary>
    /// 标识.
    /// </summary>
    public string Cid { get; } = cid;
}
