// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models.Subtitle;

namespace Richasy.BiliKernel.Services.Media.Core;

internal sealed class SubtitleClient
{
    private readonly BiliHttpClient _httpClient;
    private readonly BiliAuthenticator _authenticator;

    public SubtitleClient(
        BiliHttpClient httpClient,
        BiliAuthenticator authenticator)
    {
        _httpClient = httpClient;
        _authenticator = authenticator;
    }

    public async Task<IReadOnlyList<SubtitleMeta>> GetSubtitleMetasAsync(string aid, string cid, CancellationToken cancellationToken)
    {
        var parametes = new Dictionary<string, string>
        {
            { "cid", cid },
            { "aid", aid },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new System.Uri(BiliApis.Video.Subtitle));
        _authenticator.AuthroizeRestRequest(request, parametes, new BiliAuthorizeExecutionSettings { ForceNoToken = true, RequireCookie = true });
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync(response, SourceGenerationContext.Default.BiliDataResponseSubtitleViewResponse).ConfigureAwait(false);
        return responseObj.Data.Subtitle.Subtitles.Select(p => new SubtitleMeta(p.Id.ToString(), p.DisplayLanguage, p.Url)).ToList();
    }

    public async Task<IReadOnlyList<SubtitleInformation>> GetSubtitleDetailAsync(SubtitleMeta meta, CancellationToken cancellationToken)
    {
        var url = meta.Url;
        if (!url.StartsWith("http"))
        {
            url = "https:" + url;
        }

        var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new System.Uri(url));
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync(response, SourceGenerationContext.Default.SubtitleDetailResponse).ConfigureAwait(false);
        return responseObj.Body.Select(p => new SubtitleInformation(p.From, p.To, p.Content)).ToList();
    }
}
