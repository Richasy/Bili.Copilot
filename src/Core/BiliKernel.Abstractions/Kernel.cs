// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace Richasy.BiliKernel;

/// <summary>
/// Provides state for use throughout a Bili Kernel workload.
/// </summary>
/// <remarks>
/// An instance of <see cref="Kernel"/> is passed through to every function invocation and service call
/// throughout the system, providing to each the ability to access shared state and services.
/// </remarks>
public sealed class Kernel
{
    /// <summary>Key used by <see cref="IKernelBuilder"/> to store type information into the service provider.</summary>
    internal const string KernelServiceTypeToKeyMappings = nameof(KernelServiceTypeToKeyMappings);

    /// <summary>Dictionary containing ambient data stored in the kernel, lazily-initialized on first access.</summary>
    private Dictionary<string, object?>? _data;
    /// <summary><see cref="CultureInfo"/> to be used by any operations that need access to the culture, a format provider, etc.</summary>
    private CultureInfo _culture = CultureInfo.InvariantCulture;

    /// <summary>
    /// Initializes a new instance of <see cref="Kernel"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceProvider"/> used to query for services available through the kernel.</param>
    /// <remarks>
    /// The KernelBuilder class provides a fluent API for constructing a <see cref="Kernel"/> instance.
    /// </remarks>
    public Kernel(
        IServiceProvider? services = null) =>
        // Store the provided services, or an empty singleton if there aren't any.
        Services = services ?? EmptyServiceProvider.Instance;

    /// <summary>Creates a builder for constructing <see cref="Kernel"/> instances.</summary>
    /// <returns>A new <see cref="IKernelBuilder"/> instance.</returns>
    public static IKernelBuilder CreateBuilder() => new KernelBuilder();

    /// <summary>
    /// Clone the <see cref="Kernel"/> object to create a new instance that may be mutated without affecting the current instance.
    /// </summary>
    /// <remarks>
    /// The current instance is unmodified by this operation. The new <see cref="Kernel"/> will be initialized with:
    /// <list type="bullet">
    /// <item>
    /// The same <see cref="IServiceProvider"/> reference as is returned by the current instance's <see cref="Kernel.Services"/>.
    /// </item>
    /// <item>
    /// All of the delegates registered with each event. Delegates are immutable (every time an additional delegate is added or removed, a new one is created),
    /// so changes to the new instance's event delegates will not affect the current instance's event delegates, and vice versa.
    /// </item>
    /// <item>
    /// A new <see cref="IDictionary{TKey, TValue}"/> containing all of the key/value pairs from the current instance's <see cref="Kernel.Data"/> dictionary.
    /// Any changes made to the new instance's dictionary will not affect the current instance's dictionary, and vice versa.
    /// </item>
    /// <item>The same <see cref="CultureInfo"/> reference as is returned by the current instance's <see cref="Kernel.Culture"/>.</item>
    /// </list>
    /// </remarks>
    public Kernel Clone() =>
        new(Services)
        {
            _data = _data is { Count: > 0 } ? new Dictionary<string, object?>(_data) : null,
            _culture = _culture,
        };

    /// <summary>
    /// Gets the service provider used to query for services available through the kernel.
    /// </summary>
    public IServiceProvider Services { get; }

    /// <summary>
    /// Gets the culture currently associated with this <see cref="Kernel"/>.
    /// </summary>
    /// <remarks>
    /// The culture defaults to <see cref="CultureInfo.InvariantCulture"/> if not explicitly set.
    /// It may be set to another culture, such as <see cref="CultureInfo.CurrentCulture"/>,
    /// and any functions invoked within the context can consult this property for use in
    /// operations like formatting and parsing.
    /// </remarks>
    [AllowNull]
    public CultureInfo Culture
    {
        get => _culture;
        set => _culture = value ?? CultureInfo.InvariantCulture;
    }

    /// <summary>
    /// Gets the <see cref="ILoggerFactory"/> associated with this <see cref="Kernel"/>.
    /// </summary>
    /// <remarks>
    /// This returns any <see cref="ILoggerFactory"/> in <see cref="Services"/>. If there is
    /// none, it returns an <see cref="ILoggerFactory"/> that won't perform any logging.
    /// </remarks>
    public ILoggerFactory LoggerFactory =>
        Services.GetService<ILoggerFactory>() ??
        NullLoggerFactory.Instance;

    /// <summary>
    /// Gets a dictionary for ambient data associated with the kernel.
    /// </summary>
    /// <remarks>
    /// This may be used to flow arbitrary data in and out of operations performed with this kernel instance.
    /// </remarks>
    public IDictionary<string, object?> Data =>
        _data ??
        Interlocked.CompareExchange(ref _data, [], null) ??
        _data;

    #region GetServices
    /// <summary>Gets a required service from the <see cref="Services"/> provider.</summary>
    /// <typeparam name="T">Specifies the type of the service to get.</typeparam>
    /// <param name="serviceKey">An object that specifies the key of the service to get.</param>
    /// <returns>The found service instance.</returns>
    /// <exception cref="KernelException">A service of the specified type and name could not be found.</exception>
    public T GetRequiredService<T>(object? serviceKey = null) where T : class
    {
        T? service = null;

        if (serviceKey is not null)
        {
            if (Services is IKeyedServiceProvider)
            {
                // We were given a service ID, so we need to use the keyed service lookup.
                service = Services.GetKeyedService<T>(serviceKey);
            }
        }
        else
        {
            // No ID was given. We first want to use non-keyed lookup, in order to match against
            // a service registered without an ID. If we can't find one, then we try to match with
            // a service registered with an ID. In both cases, if there were multiple, this will match
            // with whichever was registered last.
            service = Services.GetService<T>();
            if (service is null && Services is IKeyedServiceProvider)
            {
                service = GetAllServices<T>().LastOrDefault();
            }
        }

        // If we couldn't find the service, throw an exception.
        if (service is null)
        {
            var message =
                serviceKey is null ? $"Service of type '{typeof(T)}' not registered." :
                Services is not IKeyedServiceProvider ? $"Key '{serviceKey}' specified but service provider '{Services}' is not a {nameof(IKeyedServiceProvider)}." :
                $"Service of type '{typeof(T)}' and key '{serviceKey}' not registered.";

            throw new KernelException(message);
        }

        // Return the found service.
        return service;
    }

    /// <summary>Gets all services of the specified type.</summary>
    /// <typeparam name="T">Specifies the type of the services to retrieve.</typeparam>
    /// <returns>An enumerable of all instances of the specified service that are registered.</returns>
    /// <remarks>There is no guaranteed ordering on the results.</remarks>
    public IEnumerable<T> GetAllServices<T>() where T : class
    {
        if (Services is IKeyedServiceProvider)
        {
            // M.E.DI doesn't support querying for a service without a key, and it also doesn't
            // support AnyKey currently: https://github.com/dotnet/runtime/issues/91466
            // As a workaround, KernelBuilder injects a service containing the type-to-all-keys
            // mapping. We can query for that service and and then use it to try to get a service.
            if (Services.GetKeyedService<Dictionary<Type, HashSet<object?>>>(KernelServiceTypeToKeyMappings) is { } typeToKeyMappings)
            {
                return typeToKeyMappings.TryGetValue(typeof(T), out var keys)
                    ? keys.SelectMany(Services.GetKeyedServices<T>)
                    : (IEnumerable<T>)([]);
            }
        }

        return Services.GetServices<T>();
    }

    #endregion

    #region Private
    private static bool IsNotEmpty<T>(IEnumerable<T> enumerable) =>
        enumerable is not ICollection<T> collection || collection.Count != 0;
    #endregion
}
