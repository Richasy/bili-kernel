// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Bilibili.Community.Service.Dm.V1;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Danmaku;

namespace Richasy.BiliKernel.Services.Media.Core;

internal sealed class DanmakuClient
{
    private readonly BiliHttpClient _httpClient;
    private readonly BiliAuthenticator _authenticator;
    private readonly IBiliTokenResolver _tokenResolver;

    public DanmakuClient(
        BiliHttpClient httpClient,
        BiliAuthenticator authenticator,
        IBiliTokenResolver tokenResolver)
    {
        _httpClient = httpClient;
        _authenticator = authenticator;
        _tokenResolver = tokenResolver;
    }

    public async Task<DanmakuMeta> GetDanmakuMetaAsync(string aid, string cid, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(aid) || string.IsNullOrEmpty(cid))
        {
            throw new ArgumentNullException(nameof(aid), "aid 或者 cid 不能为空");
        }
        var queryParameters = new Dictionary<string, string>
        {
            { "type", "1" },
            { "oid", cid },
            { "pid", aid },
        };

        DmWebViewReply? reply = default;
        var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new Uri(BiliApis.Video.DanmakuMetaData));
        _authenticator.AuthorizeRestRequest(request, queryParameters, new BiliAuthorizeExecutionSettings
        {
            RequireCookie = true,
            ApiType = BiliApiType.Web,
        });
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        reply = await BiliHttpClient.ParseAsync(response, DmWebViewReply.Parser).ConfigureAwait(false);

        return new DanmakuMeta { SegmentCount = Convert.ToInt32(reply.DmSge?.Total ?? 0), Total = Convert.ToInt32(reply.Count) };
    }

    public async Task<IReadOnlyList<DanmakuInformation>> GetSegmentDanmakusAsync(string aid, string cid, int segmentIndex, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(aid) || string.IsNullOrEmpty(cid))
        {
            throw new ArgumentNullException(nameof(aid), "aid 或者 cid 不能为空");
        }

        DmSegMobileReply? reply = default;
        var retryCount = 0;

        do
        {
            try
            {
                var queryParameters = new Dictionary<string, string>
                {
                    { "type", "1" },
                    { "oid", cid },
                    { "pid", aid },
                    { "segment_index", segmentIndex.ToString() },
                };
                var request = BiliHttpClient.CreateRequest(HttpMethod.Get, new Uri(BiliApis.Video.SegmentDanmaku));
                _authenticator.AuthorizeRestRequest(request, queryParameters, new BiliAuthorizeExecutionSettings
                {
                    RequireCookie = true,
                    ApiType = BiliApiType.Web,
                });
                var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
                reply = await BiliHttpClient.ParseAsync(response, DmSegMobileReply.Parser).ConfigureAwait(false);
            }
            catch (Exception)
            {
                retryCount++;
                if (retryCount > 2)
                {
                    throw;
                }
            }
        }
        while (reply == null);

        return reply.Elems.Select(p => new DanmakuInformation(
            p.Id.ToString(),
            p.Content,
            p.Mode,
            p.Progress / 1000.0,
            (uint)p.Color,
            p.Fontsize)).ToList();
    }

    public async Task SendVideoDanmakuAsync(string content, string aid, string cid, int progress, string color, bool isStandardSize = true, DanmakuLocation location = DanmakuLocation.Scroll, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "aid", aid },
            { "type", "1" },
            { "oid", cid },
            { "msg", content },
            { "progress", (progress * 1000).ToString() },
            { "color", color },
            { "fontsize", isStandardSize ? "25" : "18" },
            { "mode", ((int)location).ToString() },
            { "rnd", DateTimeOffset.Now.ToLocalTime().ToUnixTimeMilliseconds().ToString() },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Post, new Uri(BiliApis.Video.SendDanmaku));
        _authenticator.AuthorizeRestRequest(request, parameters);
        await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task SendLiveDanmakuAsync(string message, string roomId, string color, bool isStandardSize = true, DanmakuLocation location = DanmakuLocation.Scroll, CancellationToken cancellationToken = default)
    {
        var mid = _tokenResolver.GetToken().UserId.ToString();
        var parameters = new Dictionary<string, string>
        {
            { "cid", roomId },
            { "mid", mid },
            { "color", color },
            { "fontsize", isStandardSize ? "25" : "18" },
            { "msg", message },
            { "rnd", DateTimeOffset.Now.ToLocalTime().ToUnixTimeMilliseconds().ToString() },
            { "mode", ((int)location).ToString() },
            { "pool", "0" },
            { "type", "json" },
            { "playtime", "0.0" },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Post, new Uri(BiliApis.Live.SendMessage));
        _authenticator.AuthorizeRestRequest(request, parameters);
        await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
