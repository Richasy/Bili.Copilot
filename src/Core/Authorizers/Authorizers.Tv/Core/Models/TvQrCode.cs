// Copyright (c) Richasy. All rights reserved.

using System.Text.Json.Serialization;

namespace Richasy.BiliKernel.Authorizers.TV.Core;

/// <summary>
/// 电视端二维码信息.
/// </summary>
public sealed class TVQRCode
{
    /// <summary>
    /// 获取二维码数据的地址.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// 验证码.
    /// </summary>
    [JsonPropertyName("auth_code")]
    public string? AuthCode { get; set; }
}
