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

    /// <inheritdoc/>
    public Dictionary<string, string> GetCookies()
    {
        if (File.Exists(CookieFileName))
        {
            var json = File.ReadAllText(CookieFileName);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        }

        return [];
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
        => File.WriteAllText(CookieFileName, JsonSerializer.Serialize(cookies));

    /// <inheritdoc/>
    public void RemoveCookies()
    {
        if (!File.Exists(CookieFileName))
        {
            return;
        }

        File.Delete(CookieFileName);
    }
}
