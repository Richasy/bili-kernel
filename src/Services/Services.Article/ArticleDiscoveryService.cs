﻿// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili.Article;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Article;
using Richasy.BiliKernel.Services.Article.Core;
using RichasyKernel;

namespace Richasy.BiliKernel.Services.Article;

/// <summary>
/// 专栏服务.
/// </summary>
public sealed class ArticleDiscoveryService : IArticleDiscoveryService
{
    private readonly ArticleDiscoveryClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArticleDiscoveryService"/> class.
    /// </summary>
    public ArticleDiscoveryService(
        BiliHttpClient httpClient,
        BiliAuthenticator authenticator,
        IBiliTokenResolver tokenResolver)
    {
        _client = new ArticleDiscoveryClient(httpClient, authenticator, tokenResolver);
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<Partition>> GetPartitionsAsync(CancellationToken cancellationToken = default)
        => _client.GetPartitionsAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<ArticleDetail?> GetArticleContentAsync(ArticleIdentifier identifier, CancellationToken cancellationToken = default)
        => _client.GetArticleContentAsync(identifier, cancellationToken);

    /// <inheritdoc/>
    public async Task<(IReadOnlyList<ArticleInformation> RecommendArticles, int NextPageNumber)> GetPartitionArticlesAsync(Partition partition, ArticleSortType sortType, int pageNumber = 0, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 0)
        {
            throw new KernelException("页码不能小于0");
        }

        pageNumber++;
        var (articles, _) = await _client.GetPartitionArticlesAsync(partition, sortType, pageNumber, cancellationToken).ConfigureAwait(false);
        return (articles, pageNumber);
    }

    /// <inheritdoc/>
    public async Task<(IReadOnlyList<ArticleInformation> RecommendArticles, IReadOnlyList<ArticleInformation>? TopArticles, int NextPageNumber)> GetRecommendArticlesAsync(int pageNumber = 0, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 0)
        {
            throw new KernelException("页码不能小于0");
        }

        pageNumber++;
        var (recommendArticles, topArticles) = await _client.GetPartitionArticlesAsync(default, default, pageNumber, cancellationToken).ConfigureAwait(false);
        return (recommendArticles, topArticles, pageNumber);
    }

    /// <inheritdoc/>
    public Task<Dictionary<int, string>> GetHotCategoriesAsync(CancellationToken cancellationToken = default)
        => _client.GetHotCategoriesAsync(cancellationToken);

    /// <inheritdoc/>
    public Task<IReadOnlyList<ArticleInformation>> GetHotArticlesAsync(int categoryId, CancellationToken cancellationToken = default)
        => _client.GetHotArticlesAsync(categoryId, cancellationToken);
}
