// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.DependencyInjection;
using Richasy.BiliKernel.Bili.Article;
using Richasy.BiliKernel.Services.Article;

namespace Richasy.BiliKernel;

/// <summary>
/// 内核构建器扩展.
/// </summary>
public static class KernelBuilderExtensions
{
    /// <summary>
    /// 添加专栏文章服务.
    /// </summary>
    public static IKernelBuilder AddArticleService(this IKernelBuilder builder)
    {
        builder.Services.AddSingleton<IArticleService, ArticleService>();
        return builder;
    }
}
