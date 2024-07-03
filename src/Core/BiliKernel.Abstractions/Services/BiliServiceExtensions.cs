// Copyright (c) Richasy. All rights reserved.

namespace Richasy.BiliKernel.Services;

/// <summary>
/// Extension methods for <see cref="IBiliService"/>.
/// </summary>
public static class BiliServiceExtensions
{
    /// <summary>
    /// Gets the key used to store the service identifier in the <see cref="IBiliService.Attributes"/> dictionary.
    /// </summary>
    public static string UserIdKey => "UserId";

    /// <summary>
    /// Gets the ser id from <paramref name="service"/>'s <see cref="IBiliService.Attributes"/>.
    /// </summary>
    /// <param name="service">The service from which to get the user id.</param>
    /// <returns>The user id if it was specified in the service's attributes; otherwise, null.</returns>
    public static string? GetUserId(this IBiliService service) => service.GetAttribute(UserIdKey);

    /// <summary>
    /// Gets the specified attribute.
    /// </summary>
    private static string? GetAttribute(this IBiliService service, string key)
    {
        Verify.NotNull(service);
        return service.Attributes?.TryGetValue(key, out var value) == true ? value as string : null;
    }
}
