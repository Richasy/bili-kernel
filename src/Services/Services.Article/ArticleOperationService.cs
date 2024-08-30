// Copyright (c) Richasy. All rights reserved.

using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili.Article;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models.Article;
using Richasy.BiliKernel.Services.Article.Core;

namespace Richasy.BiliKernel.Services.Article;

/// <summary>
/// 文章操作服务.
/// </summary>
public sealed class ArticleOperationService : IArticleOperationService
{
    private readonly ArticleOpeartionClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArticleOperationService"/> class.
    /// </summary>
    public ArticleOperationService(
        BiliHttpClient httpClient,
        BiliAuthenticator authenticator)
        => _client = new ArticleOpeartionClient(httpClient, authenticator);

    /// <inheritdoc/>
    public Task ToggleFavoriteAsync(ArticleIdentifier article, bool isFavorite, CancellationToken cancellationToken = default)
        => _client.ToggleFavoriteAsync(article.Id, isFavorite, cancellationToken);

    /// <inheritdoc/>
    public Task ToggleLikeAsync(ArticleIdentifier article, bool isLike, CancellationToken cancellationToken = default)
        => _client.ToggleLikeAsync(article.Id, isLike, cancellationToken);
}
