// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.DependencyInjection;

namespace Richasy.BiliKernel;

/// <summary>Provides a builder for constructing instances of <see cref="Kernel"/>.</summary>
public interface IKernelBuilder
{
    /// <summary>Gets the collection of services to be built into the <see cref="Kernel"/>.</summary>
    IServiceCollection Services { get; }
}
