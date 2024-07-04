// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.DependencyInjection;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Resolvers.NativeToken;

namespace Richasy.BiliKernel;

/// <summary>
/// 内核构建器扩展.
/// </summary>
public static class KernelBuilderExtensions
{
    /// <summary>
    /// 添加本地令牌解析器.
    /// </summary>
    public static IKernelBuilder AddNativeTokenResolver(this IKernelBuilder builder)
    {
        builder.Services.AddSingleton<IBiliTokenResolver, NativeBiliTokenResolver>();
        return builder;
    }
}
