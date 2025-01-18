// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

namespace Richasy.BiliKernel.Models.Media;

/// <summary>
/// 视频操作信息.
/// </summary>
public sealed class VideoOperationInformation(
   string id,
   bool isLiked,
   bool isCoined,
   bool isFavorited,
   bool? isFollowed = default)
{

    /// <summary>
    /// 挂载的视频 Id.
    /// </summary>
    public string Id { get; } = id;

    /// <summary>
    /// 是否已点赞.
    /// </summary>
    public bool IsLiked { get; } = isLiked;

    /// <summary>
    /// 是否已投币.
    /// </summary>
    public bool IsCoined { get; } = isCoined;

    /// <summary>
    /// 是否已收藏.
    /// </summary>
    public bool IsFavorited { get; } = isFavorited;

    /// <summary>
    /// 是否已关注.
    /// </summary>
    public bool? IsFollowed { get; } = isFollowed;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is VideoOperationInformation information && Id == information.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
