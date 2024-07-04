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
    /// 添加用户资料服务.
    /// </summary>
    public static IKernelBuilder AddUserProfileService(this IKernelBuilder builder)
    {
        builder.Services.AddSingleton<IUserProfileService, UserProfileService>();
        return builder;
    }
}
