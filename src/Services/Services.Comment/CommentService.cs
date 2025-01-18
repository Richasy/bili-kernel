// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili.Comment;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Appearance;
using Richasy.BiliKernel.Models.Comment;
using Richasy.BiliKernel.Services.Comment.Core;

namespace Richasy.BiliKernel.Services.Comment;

/// <summary>
/// 评论服务.
/// </summary>
public sealed class CommentService : ICommentService
{
    private readonly CommentClient _client;

    /// <summary>
    /// 初始化.
    /// </summary>
    public CommentService(
        BiliHttpClient httpClient,
        BiliAuthenticator authenticator)
    {
        _client = new CommentClient(httpClient, authenticator);
    }

    /// <inheritdoc/>
    public Task<CommentView> GetCommentsAsync(string targetId, CommentTargetType type, CommentSortType sort, long offset = 0, CancellationToken cancellationToken = default)
        => _client.GetCommentsAsync(targetId, type, sort, offset, cancellationToken);

    /// <inheritdoc/>
    public Task<CommentView> GetDetailCommentsAsync(string targetId, CommentTargetType type, CommentSortType sort, string rootId, long offset = 0, CancellationToken cancellationToken = default)
        => _client.GetDetailCommentsAsync(targetId, type, sort, rootId, offset, cancellationToken);

    /// <inheritdoc/>
    public Task<IReadOnlyList<EmotePackage>> GetEmotePackagesAsync(CancellationToken cancellationToken = default)
        => _client.GetEmotePackagesAsync(cancellationToken);

    /// <inheritdoc/>
    public Task SendTextCommentAsync(string message, string targetId, CommentTargetType type, string rootId, string parentId, CancellationToken cancellationToken = default)
        => _client.SendTextCommentAsync(message, targetId, type, rootId, parentId, cancellationToken);

    /// <inheritdoc/>
    public Task ToggleLikeAsync(bool isLike, string commentId, string targetId, CommentTargetType type, CancellationToken cancellationToken = default)
        => _client.LiekCommentAsync(isLike, commentId, targetId, type, cancellationToken);
}
