// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.DependencyInjection;
using Richasy.BiliKernel.Bili.Moment;
using Richasy.BiliKernel.Services.Moment;

namespace Richasy.BiliKernel;

/// <summary>
/// 内核构建器扩展.
/// </summary>
public static class KernelBuilderExtensions
{
    /// <summary>
    /// 添加动态发现服务.
    /// </summary>
    public static IKernelBuilder AddMomentService(this IKernelBuilder builder)
    {
        builder.Services.AddSingleton<IMomentDiscoveryService, MomentDiscoveryService>();
        return builder;
    }
}
