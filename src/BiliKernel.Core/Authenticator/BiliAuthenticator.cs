// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.BiliKernel.Bili;
using Richasy.BiliKernel.Bili.Authorization;
using RichasyKernel;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Richasy.BiliKernel.Authenticator;

/// <summary>
/// 授权器，用于对请求进行授权.
/// </summary>
public sealed partial class BiliAuthenticator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BiliAuthenticator"/> class.
    /// </summary>
    public BiliAuthenticator(
        IBiliCookiesResolver? localCookiesResolver,
        IBiliTokenResolver? localTokenResolver)
    {
        _cookieResolver = localCookiesResolver;
        _tokenResolver = localTokenResolver;
    }

    /// <summary>
    /// 对 REST Api 请求进行授权.
    /// </summary>
    /// <param name="request">请求.</param>
    /// <param name="parameters">参数.</param>
    /// <param name="settings">请求设置.</param>
    public void AuthorizeRestRequest(HttpRequestMessage request, Dictionary<string, string>? parameters = default, BiliAuthorizeExecutionSettings? settings = default)
    {
        Verify.NotNull(request, nameof(request));
        var executionSettings = settings ?? new BiliAuthorizeExecutionSettings();
        if (executionSettings.RequireCookie && _cookieResolver != null)
        {
            var cookies = _cookieResolver.GetCookies();
            request.Headers.Add("Cookie", string.Join("; ", cookies.Select(p => $"{p.Key}={p.Value}")));
        }

        if (request.Method == HttpMethod.Post)
        {
            var queryParameters = AuthorizeRequestParameters(parameters, executionSettings);
            request.Content = new FormUrlEncodedContent(queryParameters);
        }
        else
        {
            var uri = request.RequestUri;
            var query = GenerateAuthorizedQueryString(parameters, executionSettings);
            uri = new UriBuilder(uri) { Query = query }.Uri;
            request.RequestUri = uri;
        }
    }

    /// <summary>
    /// 对 gRPC 请求进行授权.
    /// </summary>
    public void AuthorizeGrpcRequest(HttpRequestMessage request, bool requireToken = false)
    {
        Verify.NotNull(request);
        var token = _tokenResolver?.GetToken();
        if (requireToken && token == null)
        {
            throw new KernelException("需要令牌，但令牌不存在，请重新登录");
        }

        var accessToken = token?.AccessToken ?? string.Empty;
        var grpcConfig = new GrpcConfig(accessToken);
        var userAgent = $"bili-universal/80200100 "
            + $"os/ios model/{GrpcConfig.Model} mobi_app/iphone_i "
            + $"osVer/{GrpcConfig.OSVersion} "
            + $"network/{GrpcConfig.NetworkType} "
            + $"grpc-objc/1.47.0 grpc-c/25.0.0 (ios; cronet_http)";

        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Add("authorization", $"identify_v1 {accessToken}");
            request.Headers.Add("x-bili-mid", token.UserId.ToString());
        }

        request.Headers.Add("user-agent", userAgent);
        request.Headers.Add("x-bili-device-bin", GrpcConfig.GetDeviceBin());
        request.Headers.Add("x-bili-fawkes-req-bin", GrpcConfig.GetFawkesreqBin());
        request.Headers.Add("x-bili-locale-bin", GrpcConfig.GetLocaleBin());
        request.Headers.Add("x-bili-metadata-bin", grpcConfig.GetMetadataBin());
        request.Headers.Add("x-bili-network-bin", GrpcConfig.GetNetworkBin());
        request.Headers.Add("x-bili-restriction-bin", GrpcConfig.GetRestrictionBin());
        request.Headers.Add("grpc-accept-encoding", "identity,deflate,gzip");
        request.Headers.Add("grpc-timeout", "20100m");
        request.Headers.Add("env", GrpcConfig.Envorienment);
        request.Headers.Add("Transfer-Encoding", "chunked");
        request.Headers.Add("TE", "trailers");
        request.Headers.Add("x-bili-aurora-eid", GrpcConfig.GetAuroraEid(token?.UserId ?? 0));
        request.Headers.Add("x-bili-trace-id", GrpcConfig.GetTraceId());
        request.Headers.Add("buvid", GetBuvid());
    }

    /// <summary>
    /// 生成授权后的请求参数.
    /// </summary>
    /// <param name="parameters">基础参数.</param>
    /// <param name="settings">授权设置.</param>
    /// <returns>处理后的参数列表.</returns>
    /// <exception cref="KernelException">令牌获取失败.</exception>
    public Dictionary<string, string> AuthorizeRequestParameters(Dictionary<string, string>? parameters, BiliAuthorizeExecutionSettings? settings = default)
    {
        var queryParameters = parameters ?? new Dictionary<string, string>();
        var executionSettings = settings ?? new BiliAuthorizeExecutionSettings();
        if (executionSettings.NeedCSRF && !queryParameters.ContainsKey("csrf"))
        {
            queryParameters.Add("csrf", GetCsrfToken());
        }

        if (!queryParameters.ContainsKey("build"))
        {
            queryParameters.Add("build", BuildNumber);
        }

        InitializeDeviceParameters(queryParameters, executionSettings.ApiType, executionSettings.OnlyUseAppKey);

        if (!executionSettings.ForceNoToken)
        {
            var token = _tokenResolver?.GetToken();
            if (token == null && executionSettings.RequireToken)
            {
                throw new KernelException("需要令牌，但令牌不存在，请重新登录");
            }

            var accessToken = token?.AccessToken ?? string.Empty;
            if (!string.IsNullOrEmpty(accessToken))
            {
                queryParameters.Add("access_key", accessToken);
            }
        }

        if (executionSettings.NeedRID)
        {
            var rid = GenerateRID(queryParameters);
            queryParameters.Add("w_rid", rid);
        }
        else
        {
            var sign = GenerateSign(queryParameters, executionSettings.ApiType);
            queryParameters.Add("sign", sign);
        }

        return queryParameters;
    }

    /// <summary>
    /// 生成授权后的查询字符串.
    /// </summary>
    /// <param name="parameters">基础参数.</param>
    /// <param name="settings">授权设置.</param>
    /// <returns>字符串.</returns>
    public string GenerateAuthorizedQueryString(Dictionary<string, string>? parameters, BiliAuthorizeExecutionSettings? settings = default)
    {
        var queryParameters = AuthorizeRequestParameters(parameters, settings);
        return GenerateQuery(queryParameters);
    }

    /// <summary>
    /// 初始化 Wbi.
    /// </summary>
    public async Task InitializeWbiAsync(CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(_wbi))
        {
            return;
        }

        using var client = new HttpClient(new HttpClientHandler
        {
            UseCookies = true,
        });
        var request = new HttpRequestMessage(HttpMethod.Get, BiliApis.Passport.WebNav);
        if (_cookieResolver != null)
        {
            request.Headers.Add("Cookie", _cookieResolver.GetCookieString());
        }

        var response = await client.SendAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false);
        var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var result = JsonSerializer.Deserialize(responseText, SourceGenerationContext.Default.BiliDataResponseWebNavResponse);
        var img = result?.Data?.Img?.ImgUrl ?? string.Empty;
        var sub = result?.Data?.Img?.SubUrl ?? string.Empty;
        _img = Path.GetFileNameWithoutExtension(img);
        _sub = Path.GetFileNameWithoutExtension(sub);
        _wbi = GenerateWbi(_img + _sub);

        string GenerateWbi(string key)
        {
            var binding = new List<byte>();
            var rawbiKey = Encoding.UTF8.GetBytes(key);
            foreach (var b in MIXIN_KEY_ENC_TAB)
            {
                binding.Add(rawbiKey[b]);
            }

            var mixinKey = Encoding.UTF8.GetString(binding.ToArray());
            return mixinKey.Substring(0, 32);
        }
    }

    private string GetCsrfToken()
    {
        if (_cookieResolver == null)
        {
            return string.Empty;
        }

        var cookie = _cookieResolver.GetCookieString();
        var csrfToken = string.Empty;
        var match = Regex.Match(cookie, @"bili_jct=(.*?);");
        if (match.Success)
        {
            csrfToken = match.Groups[1].Value;
        }

        return csrfToken;
    }
}
