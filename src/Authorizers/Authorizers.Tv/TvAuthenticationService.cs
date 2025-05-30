﻿// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using QRCoder;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Authorizers.TV.Core;
using Richasy.BiliKernel.Bili;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Http;

namespace Richasy.BiliKernel.Authorizers.TV;

/// <summary>
/// 电视端的认证服务.
/// </summary>
public sealed class TVAuthenticationService : IAuthenticationService
{
    private readonly TVAuthorizeClient _client;
    private readonly IQRCodeResolver _qrCodeResolver;

    /// <summary>
    /// 初始化 <see cref="TVAuthenticationService"/> 类的新实例.
    /// </summary>
    public TVAuthenticationService(
        BiliHttpClient biliHttpClient,
        IQRCodeResolver qrCodeResolver,
        IBiliCookiesResolver localCookiesResolver,
        IBiliTokenResolver localTokenResolver,
        BiliAuthenticator basicAuthenticator)
    {
        _qrCodeResolver = qrCodeResolver;
        _client = new TVAuthorizeClient(
            httpClient: biliHttpClient,
            localCookiesResolver: localCookiesResolver,
            localTokenResolver: localTokenResolver,
            basicAuthenticator: basicAuthenticator);
    }

    /// <inheritdoc/>
    public Task EnsureTokenAsync(CancellationToken cancellationToken = default)
        => _client.EnsureTokenValidAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task SignInAsync(AuthorizeExecutionSettings? settings = default, CancellationToken cancellationToken = default)
    {
        if (settings?.Cookies is not null)
        {
            await _client.SignInWithCookiesAsync(settings.Cookies, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            // Step 1: 获取二维码并渲染.
            var qrCode = await _client.GetQRCodeAsync(cancellationToken).ConfigureAwait(false);
            var qrCodeGenerator = new QRCodeGenerator();
            var data = qrCodeGenerator.CreateQrCode(qrCode.Url, QRCodeGenerator.ECCLevel.Q);
            var code = new PngByteQRCode(data);
            var image = code.GetGraphic(20);
            var ms = new System.IO.MemoryStream();
            await ms.WriteAsync(image, 0, image.Length).ConfigureAwait(false);
            ms.Position = 0;
            await _qrCodeResolver.RenderAsync(ms.ToArray()).ConfigureAwait(false);

            // Step 2: 轮询扫码状态.
            await _client.WaitQRCodeScanAsync(qrCode, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public Task SignOutAsync(CancellationToken cancellationToken = default)
    {
        _client.SignOut();
        return Task.CompletedTask;
    }
}
