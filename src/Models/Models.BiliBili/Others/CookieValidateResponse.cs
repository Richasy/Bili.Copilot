// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili.Others;

/// <summary>
/// Cookie 验证响应.
/// </summary>
public class CookieValidateResponse
{
    /// <summary>
    /// 是否需要刷新 Cookie.
    /// </summary>
    [JsonPropertyName("refresh")]
    public bool NeedRefresh { get; set; }

    /// <summary>
    /// 时间戳.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }
}

/// <summary>
/// Cookie 刷新响应.
/// </summary>
public class CookieRefreshResponse
{
    /// <summary>
    /// 状态.
    /// </summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }

    /// <summary>
    /// 消息.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; }

    /// <summary>
    /// Token.
    /// </summary>
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }
}
