// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// Cookies.
/// </summary>
public class Cookies
{
    /// <summary>
    /// 名称.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// 值.
    /// </summary>
    [JsonPropertyName("value")]
    public string Value { get; set; }

    /// <summary>
    /// 仅限http.
    /// </summary>
    [JsonPropertyName("http_only")]
    public int HttpOnly { get; set; }

    /// <summary>
    /// 过期时间.
    /// </summary>
    [JsonPropertyName("expires")]
    public int Expires { get; set; }

    /// <summary>
    /// 安全.
    /// </summary>
    [JsonPropertyName("secure")]
    public int Secure { get; set; }
}

/// <summary>
/// Cookie信息.
/// </summary>
public class CookieInfo
{
    /// <summary>
    /// Cookie列表.
    /// </summary>
    [JsonPropertyName("cookies")]
    public List<Cookies> Cookies { get; set; }

    /// <summary>
    /// 域名.
    /// </summary>
    [JsonPropertyName("domains")]
    public List<string> Domains { get; set; }
}

