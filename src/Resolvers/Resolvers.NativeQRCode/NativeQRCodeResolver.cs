// Copyright (c) Richasy. All rights reserved.
// Licensed under the MIT License.

using Richasy.BiliKernel.Bili.Authorization;
using System.Diagnostics;

namespace Richasy.BiliKernel.Resolvers.NativeQRCode;

/// <summary>
/// 本地二维码解析器.
/// </summary>
public sealed class NativeQRCodeResolver : IQRCodeResolver
{
    private const string QrCodeFileName = "qrcode.png";

    /// <inheritdoc/>
    public Task RenderAsync(byte[] qrImageData)
    {
        return Task.Run(() =>
        {
            // Step 1: Save the QR code image to the file system.
            File.WriteAllBytes(QrCodeFileName, qrImageData);
            // Step 2: Open the QR code image with the default image viewer.
            return Process.Start(new ProcessStartInfo(QrCodeFileName) { UseShellExecute = true });
        });
    }
}
