// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Appearance;
using Richasy.BiliKernel.Models.Comment;

namespace Richasy.BiliKernel.Bili.Comment;

/// <summary>
/// 评论服务.
/// </summary>
public interface ICommentService
{
    /// <summary>
    /// 获取表情包列表.
    /// </summary>
    Task<IReadOnlyList<EmotePackage>> GetEmotePackagesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 切换点赞状态.
    /// </summary>
    Task ToggleLikeAsync(bool isLike, string commentId, string targetId, CommentTargetType type, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取评论列表.
    /// </summary>
    Task<CommentView> GetCommentsAsync(
        string targetId,
        CommentTargetType type,
        CommentSortType sort,
        long offset = 0,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取评论详情.
    /// </summary>
    Task<CommentView> GetDetailCommentsAsync(
        string targetId,
        CommentTargetType type,
        CommentSortType sort,
        string rootId,
        long offset = 0,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送文本评论.
    /// </summary>
    Task SendTextCommentAsync(
        string message,
        string targetId,
        CommentTargetType type,
        string rootId,
        string parentId,
        CancellationToken cancellationToken = default);
}
