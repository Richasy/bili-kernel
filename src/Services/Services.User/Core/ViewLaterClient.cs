// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;

namespace Richasy.BiliKernel.Services.User.Core;

internal sealed class ViewLaterClient
{
    private readonly BiliHttpClient _httpClient;
    private readonly IAuthenticationService _authenticationService;
    private readonly BiliAuthenticator _authenticator;

    public ViewLaterClient(
        BiliHttpClient httpClient,
        IAuthenticationService authenticationService,
        BiliAuthenticator basicAuthenticator)
    {
        _httpClient = httpClient;
        _authenticationService = authenticationService;
        _authenticator = basicAuthenticator;
    }

    public async Task<(IReadOnlyList<VideoInformation>? Videos, int Count)> GetMyViewLaterAsync(int page = 0, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "pn", page.ToString() },
            { "ps", "40" },
        };

        var responseObj = await GetAsync(BiliApis.Account.ViewLaterList, SourceGenerationContext.Default.BiliDataResponseViewLaterSetResponse, parameters, cancellationToken).ConfigureAwait(false);
        var videos = responseObj.Data?.List?.Select(p => p.ToVideoInformation()).ToList().AsReadOnly();
        return (videos, responseObj.Data.Count);
    }

    public async Task AddAsync(string aid, CancellationToken cancellationToken = default)
    {
        await _authenticationService.EnsureTokenAsync(cancellationToken).ConfigureAwait(false);
        var parameters = new Dictionary<string, string>
        {
            { "aid", aid },
            { "toview_version", "v2" },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Post, new Uri(BiliApis.Account.ViewLaterAdd));
        _authenticator.AuthroizeRestRequest(request, parameters);
        await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task RemoveAsync(string[] aids, CancellationToken cancellationToken = default)
    {
        await _authenticationService.EnsureTokenAsync(cancellationToken).ConfigureAwait(false);
        var parameters = new Dictionary<string, string>
        {
            { "aid", string.Join(",", aids) },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Post, new Uri(BiliApis.Account.ViewLaterDelete));
        _authenticator.AuthroizeRestRequest(request, parameters);
        await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task CleanAsync(ViewLaterCleanType cleanType, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "clean_type",  ((int)cleanType).ToString() },
        };

        await _authenticationService.EnsureTokenAsync(cancellationToken).ConfigureAwait(false);
        var request = BiliHttpClient.CreateRequest(HttpMethod.Post, new Uri(BiliApis.Account.ViewLaterClear));
        _authenticator.AuthroizeRestRequest(request, parameters);
        await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    private async Task<T> GetAsync<T>(string url, JsonTypeInfo<T> converter, Dictionary<string, string>? paramters = default, CancellationToken cancellationToken = default)
    {
        await _authenticationService.EnsureTokenAsync(cancellationToken).ConfigureAwait(false);
        var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new Uri(url));
        _authenticator.AuthroizeRestRequest(request, paramters);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        return await BiliHttpClient.ParseAsync<T>(response, converter).ConfigureAwait(false);
    }
}
