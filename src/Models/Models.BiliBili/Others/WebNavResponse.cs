// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili.Others;

/// <summary>
/// 网页导航响应.
/// </summary>
/// <remarks>
/// https://github.com/SocialSisterYi/bilibili-API-collect/blob/master/docs/misc/sign/wbi.md.
/// </remarks>
public class WebNavResponse
{
    /// <summary>
    /// 未知.
    /// </summary>
    [JsonPropertyName("wbi_img")]
    public WbiImage Img { get; set; }
}

/// <summary>
/// Wbi image.
/// </summary>
public class WbiImage
{
    /// <summary>
    /// 未知.
    /// </summary>
    [JsonPropertyName("img_url")]
    public string ImgUrl { get; set; }

    /// <summary>
    /// 未知.
    /// </summary>
    [JsonPropertyName("sub_url")]
    public string SubUrl { get; set; }
}
