// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Richasy.BiliKernel.Models.Authorization;

/// <summary>
/// Cookies.
/// </summary>
public sealed class BiliCookies
{
    /// <summary>
    /// 名称.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 值.
    /// </summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}

/// <summary>
/// Cookie信息.
/// </summary>
public sealed class BiliCookiesResponse
{
    /// <summary>
    /// Cookie列表.
    /// </summary>
    [JsonPropertyName("cookies")]
    public IList<BiliCookies>? Cookies { get; set; }
}
