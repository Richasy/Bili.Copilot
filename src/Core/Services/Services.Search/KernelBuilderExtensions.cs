// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.DependencyInjection;
using Richasy.BiliKernel.Bili.Search;
using Richasy.BiliKernel.Services.Search;

namespace Richasy.BiliKernel;

/// <summary>
/// 内核构建器扩展.
/// </summary>
public static class KernelBuilderExtensions
{
    /// <summary>
    /// 添加搜索服务.
    /// </summary>
    public static IKernelBuilder AddSearchService(this IKernelBuilder builder)
    {
        builder.Services.AddSingleton<ISearchService, SearchService>();
        return builder;
    }
}
