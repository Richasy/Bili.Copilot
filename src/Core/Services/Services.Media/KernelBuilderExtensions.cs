// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.DependencyInjection;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Services.Media;

namespace Richasy.BiliKernel;

/// <summary>
/// 内核构建器扩展.
/// </summary>
public static class KernelBuilderExtensions
{
    /// <summary>
    /// 添加稍后再看服务.
    /// </summary>
    public static IKernelBuilder AddViewLaterService(this IKernelBuilder builder)
    {
        builder.Services.AddSingleton<IViewLaterService, ViewLaterService>();
        return builder;
    }
}
