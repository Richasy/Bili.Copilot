// Copyright (c) Richasy. All rights reserved.

using System.Text.Json.Serialization;

namespace Richasy.BiliKernel.Models.Authorization;

/// <summary>
/// 令牌信息.
/// </summary>
public class BiliToken
{
    /// <summary>
    /// 用户Id.
    /// </summary>
    [JsonPropertyName("mid")]
    public long UserId { get; set; }

    /// <summary>
    /// 访问令牌.
    /// </summary>
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    /// <summary>
    /// 刷新令牌.
    /// </summary>
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }

    /// <summary>
    /// 过期时间.
    /// </summary>
    [JsonPropertyName("expires_in")]
    public long ExpiresIn { get; set; }

    /// <summary>
    /// Cookies.
    /// </summary>
    [JsonPropertyName("cookie_info")]
    public BiliCookiesResponse? CookieInfo { get; set; }
}
