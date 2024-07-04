// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.DependencyInjection;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Resolvers.NativeCookies;

namespace Richasy.BiliKernel;

/// <summary>
/// 内核构建器扩展.
/// </summary>
public static class KernelBuilderExtensions
{
    /// <summary>
    /// 添加本地 Cookie 解析器.
    /// </summary>
    public static IKernelBuilder AddNativeCookiesResolver(this IKernelBuilder builder)
    {
        builder.Services.AddSingleton<IBiliCookiesResolver, NativeBiliCookiesResolver>();
        return builder;
    }
}
