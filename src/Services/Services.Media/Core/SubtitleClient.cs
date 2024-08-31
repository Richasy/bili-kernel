// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
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
            { "id", $"cid:{cid}" },
            { "aid", aid },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new System.Uri(BiliApis.Video.Subtitle));
        _authenticator.AuthroizeRestRequest(request, parametes);
        var text = await _httpClient.GetStringAsync(request, cancellationToken).ConfigureAwait(false);
        if (string.IsNullOrEmpty(text) || !text.Contains("subtitle"))
        {
            throw new KernelException("获取字幕元数据失败");
        }

        var json = Regex.Match(text, @"<subtitle>(.*?)</subtitle>").Groups[1].Value;
        var index = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.SubtitleIndexResponse);
        return index.Subtitles.Select(p => new SubtitleMeta(p.Id.ToString(), p.DisplayLanguage, p.Url)).ToList();
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
