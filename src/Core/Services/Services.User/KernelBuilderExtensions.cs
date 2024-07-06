// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.DependencyInjection;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Services.User;

namespace Richasy.BiliKernel;

/// <summary>
/// 内核构建器扩展.
/// </summary>
public static class KernelBuilderExtensions
{
    /// <summary>
    /// 添加个人资料服务，获取已登录用户的基础信息.
    /// </summary>
    public static IKernelBuilder AddMyProfileService(this IKernelBuilder builder)
    {
        builder.Services.AddSingleton<IMyProfileService, MyProfileService>();
        return builder;
    }

    /// <summary>
    /// 添加用户关系服务，处理诸如关注、粉丝等关系操作.
    /// </summary>
    public static IKernelBuilder AddRelationshipService(this IKernelBuilder builder)
    {
        builder.Services.AddSingleton<IRelationshipService, RelationshipService>();
        return builder;
    }
}
