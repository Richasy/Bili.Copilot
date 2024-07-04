// Copyright (c) Richasy. All rights reserved.

using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Richasy.BiliKernel.Bili.Authorization;

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
        // Step 1: Save the QR code image to the file system.
        File.WriteAllBytes(QrCodeFileName, qrImageData);

        // Step 2: Open the QR code image with the default image viewer.
        return Task.Run(() => Process.Start(new ProcessStartInfo(QrCodeFileName) { UseShellExecute = true }));
    }
}
