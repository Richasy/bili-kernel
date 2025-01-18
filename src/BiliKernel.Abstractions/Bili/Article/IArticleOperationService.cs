// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.BiliKernel.Models.Article;

namespace Richasy.BiliKernel.Bili.Article;

/// <summary>
/// 文章操作服务.
/// </summary>
public interface IArticleOperationService
{
    /// <summary>
    /// 切换点赞状态.
    /// </summary>
    Task ToggleLikeAsync(ArticleIdentifier article, bool isLike, CancellationToken cancellationToken = default);

    /// <summary>
    /// 切换收藏状态.
    /// </summary>
    Task ToggleFavoriteAsync(ArticleIdentifier article, bool isFavorite, CancellationToken cancellationToken = default);
}
