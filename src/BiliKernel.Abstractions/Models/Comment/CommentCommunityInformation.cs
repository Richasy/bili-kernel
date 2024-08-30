// Copyright (c) Richasy. All rights reserved.

using System;

namespace Richasy.BiliKernel.Models.Comment;

/// <summary>
/// 评论社区信息.
/// </summary>
public sealed class CommentCommunityInformation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommentCommunityInformation"/> class.
    /// </summary>
    /// <param name="id">对应的评论 Id.</param>
    /// <param name="likeCount">点赞数.</param>
    /// <param name="childCount">子评论数.</param>
    /// <param name="isLiked">是否已点赞.</param>
    public CommentCommunityInformation(
        string id,
        double likeCount,
        int childCount,
        bool isLiked)
    {
        Id = id;
        LikeCount = likeCount;
        ChildCount = childCount;
        IsLiked = isLiked;
    }

    /// <summary>
    /// 评论 Id.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// 点赞数.
    /// </summary>
    public double LikeCount { get; set; }

    /// <summary>
    /// 子评论数.
    /// </summary>
    public int ChildCount { get; set; }

    /// <summary>
    /// 是否已点赞.
    /// </summary>
    public bool IsLiked { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is CommentCommunityInformation information && Id == information.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
