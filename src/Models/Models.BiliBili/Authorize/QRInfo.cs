// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 二维码信息.
/// </summary>
public class QRInfo
{
    /// <summary>
    /// 获取二维码数据的地址.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; }

    /// <summary>
    /// 验证码.
    /// </summary>
    [JsonPropertyName("auth_code")]
    public string AuthCode { get; set; }
}

