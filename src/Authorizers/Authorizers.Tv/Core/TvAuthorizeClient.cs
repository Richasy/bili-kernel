﻿// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models.Authorization;
using RichasyKernel;
using System.Text.Json;

namespace Richasy.BiliKernel.Authorizers.TV.Core;

internal sealed class TVAuthorizeClient
{
    private readonly string _localId;
    private readonly BiliHttpClient _httpClient;
    private readonly IBiliCookiesResolver _localCookiesResolver;
    private readonly IBiliTokenResolver _localTokenResolver;
    private readonly BiliAuthenticator _basicAuthenticator;
    private bool _isScanCheckInvoking;

    public TVAuthorizeClient(
        BiliHttpClient httpClient,
        IBiliCookiesResolver localCookiesResolver,
        IBiliTokenResolver localTokenResolver,
        BiliAuthenticator basicAuthenticator)
    {
        _localId = Guid.NewGuid().ToString("N");
        _httpClient = httpClient;
        _localCookiesResolver = localCookiesResolver;
        _localTokenResolver = localTokenResolver;
        _basicAuthenticator = basicAuthenticator;
    }

    public async Task EnsureTokenValidAsync(CancellationToken cancellationToken = default)
    {
        var localToken = _localTokenResolver.GetToken();
        BasicTokenCheck(localToken);
        var expiredTime = DateTimeOffset.FromUnixTimeSeconds(localToken.ExpiresIn);
        if (DateTimeOffset.Now < expiredTime)
        {
            return;
        }

        var newToken = await RefreshTokenAsync(cancellationToken).ConfigureAwait(false);
        if (newToken != null)
        {
            SaveToken(newToken);
            return;
        }

        throw new KernelException("令牌已过期，请重新登录");
    }

    public async Task<TVQRCode> GetQRCodeAsync(CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "local_id", _localId },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Post, new Uri(BiliApis.Passport.QRCode));
        _basicAuthenticator.AuthorizeRestRequest(request, parameters, new BiliAuthorizeExecutionSettings() { ForceNoToken = true });
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync(response, SourceGenerationContext.Default.BiliDataResponseTVQRCode).ConfigureAwait(false);
        return responseObj.Data;
    }

    public async Task<BiliToken?> WaitQRCodeScanAsync(TVQRCode code, CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (_isScanCheckInvoking)
            {
                await Task.Delay(500, cancellationToken).ConfigureAwait(false);
                continue;
            }

            _isScanCheckInvoking = true;
            try
            {
                var token = await GetTokenIfCodeScannedAsync(code.AuthCode, cancellationToken).ConfigureAwait(false);
                TrySaveCookiesFromToken(token);
                SaveToken(token);
                return token;
            }
            catch (KernelException ke)
            {
                if (ke.InnerException is TaskCanceledException)
                {
                    break;
                }

                if (ke.InnerException is not null && ke.InnerException.Message.Contains("Code"))
                {
                    var error = JsonSerializer.Deserialize(ke.InnerException.Message, SourceGenerationContext.Default.BiliResponse);
                    if (error != null)
                    {
                        var qrStatus = error.Code is 86090 or 86039
                            ? QRCodeStatus.NotConfirm
                            : error.Code is 86038 or -3 ? QRCodeStatus.Expired : QRCodeStatus.Failed;

                        if (qrStatus == QRCodeStatus.NotConfirm)
                        {
                            await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
                        }
                        else
                        {
                            throw new Exception(qrStatus == QRCodeStatus.Expired ? "二维码已过期，请刷新二维码" : "二维码扫描失败", ke.InnerException);
                        }
                    }
                }
            }
            finally
            {
                _isScanCheckInvoking = false;
            }
        }

        return default;
    }

    public void SignOut()
    {
        _localTokenResolver.RemoveToken();
        _localCookiesResolver.RemoveCookies();
    }

    public async Task SignInWithCookiesAsync(IDictionary<string, string> cookies, CancellationToken cancellationToken)
    {
        _localCookiesResolver.SaveCookies(cookies);
        var qrCode = await GetQRCodeAsync(cancellationToken).ConfigureAwait(false) ?? throw new KernelException("获取二维码失败");
        var parameters = new Dictionary<string, string>
        {
            { "auth_code", qrCode.AuthCode },
        };
        var request = BiliHttpClient.CreateRequest(HttpMethod.Post, new Uri(BiliApis.Passport.QRCodeConfirm));
        _basicAuthenticator.AuthorizeRestRequest(request, parameters, new BiliAuthorizeExecutionSettings() { ForceNoToken = true, NeedCSRF = true, RequireCookie = true });
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        _isScanCheckInvoking = false;
        await WaitQRCodeScanAsync(qrCode, cancellationToken).ConfigureAwait(false);
    }

    private static void BasicTokenCheck(BiliToken? localToken)
    {
        if (localToken == null)
        {
            throw new InvalidOperationException("没有本地缓存的令牌信息，请重新登录");
        }

        if (string.IsNullOrEmpty(localToken.AccessToken)
            || string.IsNullOrEmpty(localToken.RefreshToken)
            || localToken.ExpiresIn < 1)
        {
            throw new KernelException("令牌信息不完整，请重新登录");
        }
    }

    private async Task<BiliToken> RefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        var localToken = _localTokenResolver.GetToken();
        BasicTokenCheck(localToken);

        var paramters = new Dictionary<string, string>
        {
            { "access_token", localToken.AccessToken },
            { "refresh_token", localToken.RefreshToken },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Post, new Uri(BiliApis.Passport.RefreshToken));
        _basicAuthenticator.AuthorizeRestRequest(request, paramters);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync(response, SourceGenerationContext.Default.BiliDataResponseBiliToken).ConfigureAwait(false);
        var token = responseObj.Data;
        TrySaveCookiesFromToken(token);

        return token;
    }

    private async Task<BiliToken> GetTokenIfCodeScannedAsync(string authCode, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "auth_code", authCode },
            { "local_id", _localId },
            { "guid", Guid.NewGuid().ToString() },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Post, new Uri(BiliApis.Passport.QRCodeCheck));
        _basicAuthenticator.AuthorizeRestRequest(request, parameters, new BiliAuthorizeExecutionSettings() { ForceNoToken = true });
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync(response, SourceGenerationContext.Default.BiliDataResponseBiliToken).ConfigureAwait(false);
        var token = responseObj?.Data;
        TrySaveCookiesFromToken(token);
        return token;
    }

    private void TrySaveCookiesFromToken(BiliToken? token)
    {
        if (token?.CookieInfo is null)
        {
            return;
        }

        var cookies = token.CookieInfo.Cookies.ToDictionary(cookie => cookie.Name!, cookie => cookie.Value ?? string.Empty);
        _localCookiesResolver.SaveCookies(cookies);
    }

    private void SaveToken(BiliToken token)
    {
        token.ExpiresIn = DateTimeOffset.Now.ToLocalTime().ToUnixTimeSeconds() + token.ExpiresIn;
        _localTokenResolver.SaveToken(token);
    }
}
