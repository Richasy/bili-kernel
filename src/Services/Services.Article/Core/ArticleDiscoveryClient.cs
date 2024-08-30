// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Content;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Article;
using Richasy.BiliKernel.Services.Article.Core.Models;

namespace Richasy.BiliKernel.Services.Article.Core;

internal sealed class ArticleDiscoveryClient
{
    private readonly BiliHttpClient _httpClient;
    private readonly BiliAuthenticator _authenticator;
    private readonly IBiliTokenResolver _tokenResolver;

    public ArticleDiscoveryClient(
        BiliHttpClient httpClient,
        BiliAuthenticator authenticator,
        IBiliTokenResolver tokenResolver)
    {
        _httpClient = httpClient;
        _authenticator = authenticator;
        _tokenResolver = tokenResolver;
    }

    public async Task<Dictionary<int, string>> GetHotCategoriesAsync(CancellationToken cancellationToken)
    {
        var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new Uri(BiliApis.Article.HotCategories));
        _authenticator.AuthroizeRestRequest(request, settings: new BiliAuthorizeExecutionSettings { RequireToken = false });
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync<BiliDataResponse<List<ArticleCategory>>>(response).ConfigureAwait(false);
        return responseObj.Data is null || responseObj.Data.Count == 0
            ? throw new KernelException("热门专栏分区数据为空")
            : responseObj.Data.ToDictionary(p => Convert.ToInt32(p.Id), p => p.Name!);
    }

    public async Task<IReadOnlyList<Partition>> GetPartitionsAsync(CancellationToken cancellationToken)
    {
        var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new Uri(BiliApis.Article.Categories));
        _authenticator.AuthroizeRestRequest(request, settings: new BiliAuthorizeExecutionSettings { RequireToken = false });
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync<BiliDataResponse<List<ArticleCategory>>>(response).ConfigureAwait(false);
        return responseObj.Data is null || responseObj.Data.Count == 0
            ? throw new KernelException("专栏分区数据为空")
            : (IReadOnlyList<Partition>)responseObj.Data.Select(p => p.ToPartition()).ToList().AsReadOnly();
    }

    public async Task<IReadOnlyList<ArticleInformation>> GetHotArticlesAsync(int categoryId, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "cid", categoryId.ToString() },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new Uri(BiliApis.Article.HotArticles));
        _authenticator.AuthroizeRestRequest(request, parameters, new BiliAuthorizeExecutionSettings { RequireToken = false });
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync<BiliDataResponse<List<Article>>>(response).ConfigureAwait(false);
        return responseObj.Data is null || responseObj.Data.Count == 0
            ? throw new KernelException("热门专栏文章数据为空")
            : responseObj.Data.Select(p => p.ToArticleInformation()).ToList().AsReadOnly();
    }

    public async Task<(IReadOnlyList<ArticleInformation>, IReadOnlyList<ArticleInformation>?)> GetPartitionArticlesAsync(Partition? partition, ArticleSortType? sort, int pageNumber, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "cid", partition?.Id ?? "0" },
            { "pn", pageNumber.ToString() },
            { "ps", "20" },
        };

        if (partition != null && sort != null)
        {
            parameters.Add("sort", ((int)sort).ToString());
        }

        var localToken = _tokenResolver.GetToken();
        if (localToken is not null)
        {
            parameters.Add("mid", localToken.UserId.ToString());
        }

        var uri = partition is null ? BiliApis.Article.Recommend : BiliApis.Article.ArticleList;
        var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new Uri(uri));
        _authenticator.AuthroizeRestRequest(request, parameters, new BiliAuthorizeExecutionSettings { RequireToken = false });
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        if (partition is null)
        {
            var responseObj = await BiliHttpClient.ParseAsync<BiliDataResponse<ArticleRecommendResponse>>(response).ConfigureAwait(false);
            var recommendArticles = responseObj.Data.Articles.Select(p => p.ToArticleInformation()).ToList().AsReadOnly();
            var topArticles = responseObj.Data.Ranks?.Select(p => p.ToArticleInformation()).ToList().AsReadOnly();
            return recommendArticles.Count == 0
                ? throw new KernelException("推荐专栏数据为空")
                : ((IReadOnlyList<ArticleInformation>, IReadOnlyList<ArticleInformation>?))(recommendArticles, topArticles);
        }
        else
        {
            var responseObj = await BiliHttpClient.ParseAsync<BiliDataResponse<List<Article>>>(response).ConfigureAwait(false);
            var articles = responseObj.Data.Select(p => p.ToArticleInformation()).ToList().AsReadOnly();
            return articles.Count == 0
                ? throw new KernelException("没有获取到有效的专栏文章列表，请稍后再试")
                : ((IReadOnlyList<ArticleInformation>, IReadOnlyList<ArticleInformation>?))(articles, null);
        }
    }

    public async Task<ArticleDetail?> GetArticleContentAsync(ArticleIdentifier article, CancellationToken cancellationToken)
    {
        var sourceJson = await GetArticleInitialJsonAsync(article, cancellationToken).ConfigureAwait(false);
        var articleContent = JsonSerializer.Deserialize<ArticleContentResponse>(sourceJson)
            ?? throw new KernelException("无法解析该文章的内容.");
        var articleView = await GetArticleMetadataAsync(article, cancellationToken).ConfigureAwait(false);
        return articleContent.ToArticleDetail(article, articleView);
    }

    private async Task<string> GetArticleInitialJsonAsync(ArticleIdentifier article, CancellationToken cancellationToken)
    {
        var url = BiliApis.Article.ArticleContent + $"?id={article.Id}";
        var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new Uri(url));
        var html = await _httpClient.GetStringAsync(request, cancellationToken).ConfigureAwait(false);
        const string pattern = @"window\.__INITIAL_STATE__\s*=\s*(\{.*?\});";
        var match = Regex.Match(html, pattern, RegexOptions.Singleline);
        return match.Success ? match.Groups[1].Value : null;
    }

    private async Task<ArticleViewResponse> GetArticleMetadataAsync(ArticleIdentifier article, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "id", article.Id },
        };
        var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new Uri(BiliApis.Article.ArticleViewInfo));
        _authenticator.AuthroizeRestRequest(request, parameters);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync<BiliDataResponse<ArticleViewResponse>>(response).ConfigureAwait(false);
        return responseObj.Data ?? throw new KernelException("无法获取文章元数据");
    }
}
