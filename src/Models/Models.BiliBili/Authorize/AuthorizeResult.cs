// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 授权结果.
/// </summary>
public class AuthorizeResult
{
    /// <summary>
    /// 状态码.
    /// </summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }

    /// <summary>
    /// 错误消息.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; }

    /// <summary>
    /// 导航地址.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; }

    /// <summary>
    /// 授权令牌信息.
    /// </summary>
    [JsonPropertyName("token_info")]
    public TokenInfo TokenInfo { get; set; }

    /// <summary>
    /// Cookie信息.
    /// </summary>
    [JsonPropertyName("cookie_info")]
    public CookieInfo CookieInfo { get; set; }

    /// <summary>
    /// SSO.
    /// </summary>
    [JsonPropertyName("sso")]
    public List<string> SSO { get; set; }
}

