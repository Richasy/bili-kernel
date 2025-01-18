// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.BiliKernel.Bili.Authorization;

namespace Richasy.BiliKernel.Resolvers.WinUIQRCode;

/// <summary>
/// 适用于 WinUI 的二维码解析器.
/// </summary>
public sealed class WinUIQRCodeResolver(Func<byte[], Task> renderFunc) : IQRCodeResolver
{
    private readonly Func<byte[], Task>? _renderFunc = renderFunc;

    /// <inheritdoc/>
    public Task RenderAsync(byte[] qrImageData)
        => _renderFunc?.Invoke(qrImageData) ?? Task.CompletedTask;
}
