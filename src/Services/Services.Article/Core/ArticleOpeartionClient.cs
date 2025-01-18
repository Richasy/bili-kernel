// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili;
using Richasy.BiliKernel.Http;

namespace Richasy.BiliKernel.Services.Article.Core;

internal sealed class ArticleOpeartionClient
{
    private readonly BiliHttpClient _httpClient;
    private readonly BiliAuthenticator _authenticator;

    public ArticleOpeartionClient(
        BiliHttpClient httpClient,
        BiliAuthenticator authenticator)
    {
        _httpClient = httpClient;
        _authenticator = authenticator;
    }

    public async Task ToggleLikeAsync(string articleId, bool isLike, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "id", articleId.ToString() },
            { "type", isLike ? "1" : "2" }
        };
        var request = BiliHttpClient.CreateRequest(HttpMethod.Post, new Uri(BiliApis.Article.LikeArticle));
        _authenticator.AuthorizeRestRequest(request, parameters, new BiliAuthorizeExecutionSettings { NeedCSRF = true });
        await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task ToggleFavoriteAsync(string articleId, bool isFavorite, CancellationToken cancellationToken)
    {
        var parameters = new Dictionary<string, string>
        {
            { "id", articleId.ToString() },
        };
        var url = isFavorite ? BiliApis.Article.AddFavorite : BiliApis.Article.DeleteFavorite;
        var request = BiliHttpClient.CreateRequest(HttpMethod.Post, new Uri(url));
        _authenticator.AuthorizeRestRequest(request, parameters, new BiliAuthorizeExecutionSettings { NeedCSRF = true });
        await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
