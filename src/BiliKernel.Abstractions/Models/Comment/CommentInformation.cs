﻿// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.BiliKernel.Models.Appearance;
using Richasy.BiliKernel.Models.User;

namespace Richasy.BiliKernel.Models.Comment;

/// <summary>
/// 评论信息.
/// </summary>
public sealed class CommentInformation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommentInformation"/> class.
    /// </summary>
    /// <param name="id">评论 Id.</param>
    /// <param name="content">评论内容.</param>
    /// <param name="rootId">根评论 Id.</param>
    /// <param name="isTop">是否置顶.</param>
    /// <param name="user">发布者.</param>
    /// <param name="publishTime">发布时间.</param>
    /// <param name="communityInformation">社区信息.</param>
    public CommentInformation(
        string id,
        EmoteText content,
        string rootId,
        bool isTop,
        UserDetailProfile user,
        DateTimeOffset publishTime,
        CommentCommunityInformation? communityInformation = default)
    {
        Id = id;
        Content = content;
        RootId = rootId;
        IsTop = isTop;
        User = user;
        PublishTime = publishTime;
        CommunityInformation = communityInformation;
    }

    /// <summary>
    /// 评论 Id.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// 评论内容.
    /// </summary>
    public EmoteText Content { get; }

    /// <summary>
    /// 根评论 Id.
    /// </summary>
    public string RootId { get; }

    /// <summary>
    /// 是否置顶.
    /// </summary>
    public bool IsTop { get; }

    /// <summary>
    /// 评论发布者.
    /// </summary>
    public UserDetailProfile User { get; }

    /// <summary>
    /// 发布时间.
    /// </summary>
    public DateTimeOffset PublishTime { get; }

    /// <summary>
    /// 社区信息.
    /// </summary>
    public CommentCommunityInformation? CommunityInformation { get; }

    /// <summary>
    /// 评论区类型.
    /// </summary>
    public CommentTargetType CommentType { get; set; }

    /// <summary>
    /// 评论区 Id.
    /// </summary>
    public string CommentId { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is CommentInformation information && Id == information.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
