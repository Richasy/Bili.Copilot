// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 登录时请求appThird响应信息.
/// </summary>
public class LoginAppThird
{
    /// <summary>
    /// api域.
    /// </summary>
    [JsonPropertyName("api_host")]
    public string ApiHost { get; set; }

    /// <summary>
    /// 是否登录.
    /// </summary>
    [JsonPropertyName("has_login")]
    public int HasLogin { get; set; }

    /// <summary>
    /// 是否直接登录.
    /// </summary>
    [JsonPropertyName("direct_login")]
    public int DirectLogin { get; set; }

    /// <summary>
    /// 确认链接.
    /// </summary>
    [JsonPropertyName("confirm_uri")]
    public string ConfirmUri { get; set; }
}

