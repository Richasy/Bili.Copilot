// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Richasy.BiliKernel.Authenticator;

namespace Richasy.BiliKernel;

/// <summary>
/// 内核扩展.
/// </summary>
public static class KernelExtensions
{
    /// <summary>
    /// 构建内核.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public static Kernel Build(this IKernelBuilder builder)
    {
        Verify.NotNull(builder, nameof(builder));

        if (builder is KernelBuilder kb && !kb.AllowBuild)
        {
            throw new InvalidOperationException("The builder has been built.");
        }

        var serviceProvider = EmptyServiceProvider.Instance;
        if (builder.Services is { Count: > 0 } services)
        {
            Dictionary<Type, HashSet<object?>> typeToKeyMappings = [];
            foreach (var serviceDescriptor in services)
            {
                if (!typeToKeyMappings.TryGetValue(serviceDescriptor.ServiceType, out var keys))
                {
                    typeToKeyMappings[serviceDescriptor.ServiceType] = keys = [];
                }

                keys.Add(serviceDescriptor.ServiceKey);
            }
            services.AddKeyedSingleton(Kernel.KernelServiceTypeToKeyMappings, typeToKeyMappings);
            serviceProvider = services.BuildServiceProvider();
        }

        return serviceProvider.GetService(typeof(BasicAuthenticator)) == null
            ? throw new InvalidOperationException("The BasicAuthenticator service is required.")
            : new Kernel(serviceProvider);
    }
}
