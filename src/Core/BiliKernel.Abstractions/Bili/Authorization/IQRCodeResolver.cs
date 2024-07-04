// Copyright (c) Richasy. All rights reserved.

using System.Threading.Tasks;

namespace Richasy.BiliKernel.Bili.Authorization;

/// <summary>
/// 二维码解析器.
/// </summary>
public interface IQRCodeResolver
{
    /// <summary>
    /// 渲染二维码.
    /// </summary>
    /// <param name="qrImageData">二维码图像信息.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task RenderAsync(byte[] qrImageData);
}
