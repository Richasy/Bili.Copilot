// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.DependencyInjection;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Resolvers.NativeQRCode;

namespace Richasy.BiliKernel;

/// <summary>
/// 内核构建器扩展.
/// </summary>
public static class KernelBuilderExtensions
{
    /// <summary>
    /// 添加本地二维码解析器.
    /// </summary>
    public static IKernelBuilder AddNativeQRCodeResolver(this IKernelBuilder builder)
    {
        builder.Services.AddSingleton<IQRCodeResolver, NativeQRCodeResolver>();
        return builder;
    }
}
