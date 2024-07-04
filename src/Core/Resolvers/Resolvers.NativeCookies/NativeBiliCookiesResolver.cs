// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Richasy.BiliKernel.Bili.Authorization;

namespace Richasy.BiliKernel.Resolvers.NativeCookies;

/// <summary>
/// 本地 B站 Cookie 解析器.
/// </summary>
public sealed class NativeBiliCookiesResolver : IBiliCookiesResolver
{
    private const string CookieFileName = "cookie.json";
    private Dictionary<string, string>? _cacheCookies;

    /// <inheritdoc/>
    public Dictionary<string, string> GetCookies()
    {
        if (_cacheCookies?.Count > 0)
        {
            return _cacheCookies;
        }

        if (File.Exists(CookieFileName))
        {
            var json = File.ReadAllText(CookieFileName);
            _cacheCookies = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        }

        return _cacheCookies ?? [];
    }

    /// <inheritdoc/>
    public string GetCookieString()
    {
        var cookies = GetCookies();
        var cookieList = cookies.Select(item => $"{item.Key}={item.Value}");
        return string.Join("; ", cookieList);
    }

    /// <inheritdoc/>
    public void SaveCookies(Dictionary<string, string> cookies)
    {
        _cacheCookies = cookies;
        File.WriteAllText(CookieFileName, JsonSerializer.Serialize(cookies));
    }

    /// <inheritdoc/>
    public void RemoveCookies()
    {
        if (!File.Exists(CookieFileName))
        {
            return;
        }

        _cacheCookies = null;
        File.Delete(CookieFileName);
    }
}
