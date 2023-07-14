// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 横幅定义.
/// </summary>
public class PartitionBanner
{
    /// <summary>
    /// 横幅Id.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 横幅标题.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// 横幅图片地址.
    /// </summary>
    [JsonPropertyName("image")]
    public string Image { get; set; }

    /// <summary>
    /// 哈希特征值.
    /// </summary>
    [JsonPropertyName("hash")]
    public string Hash { get; set; }

    /// <summary>
    /// 导航地址.
    /// </summary>
    [JsonPropertyName("uri")]
    public string NavigateUri { get; set; }

    /// <summary>
    /// 对应资源的Id值.
    /// </summary>
    [JsonPropertyName("resource_id")]
    public int ResourceId { get; set; }

    /// <summary>
    /// 请求Id.
    /// </summary>
    [JsonPropertyName("request_id")]
    public string RequestId { get; set; }

    /// <summary>
    /// 是否为广告.
    /// </summary>
    [JsonPropertyName("is_ad")]
    public bool IsAD { get; set; }

    /// <summary>
    /// 在集合中的索引值.
    /// </summary>
    [JsonPropertyName("index")]
    public int Index { get; set; }
}

