// Copyright (c) Richasy. All rights reserved.

using System.Text.Json.Serialization;

namespace Richasy.BiliKernel.Content;

/// <summary>
/// BiliBili 响应结构.
/// </summary>
public class BiliResponse
{
    /// <summary>
    /// 响应代码.
    /// </summary>
    [JsonPropertyName("code")]
    public int Code { get; set; }

    /// <summary>
    /// 响应消息.
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    /// <summary>
    /// TTL.
    /// </summary>
    [JsonPropertyName("ttl")]
    public int TTL { get; set; }

    /// <summary>
    /// 响应是否表示成功.
    /// </summary>
    /// <returns>结果.</returns>
    public bool IsSuccess() => Code == 0;
}

/// <summary>
/// 哔哩哔哩数据响应结构.
/// </summary>
/// <typeparam name="T">数据类型.</typeparam>
public sealed class BiliDataResponse<T> : BiliResponse
{
    /// <summary>
    /// 数据.
    /// </summary>
    [JsonPropertyName("data")]
    public T? Data { get; set; }
}

/// <summary>
/// 哔哩哔哩结果响应结构.
/// </summary>
/// <typeparam name="T">结果类型.</typeparam>
public sealed class BiliResultResponse<T> : BiliResponse
{
    /// <summary>
    /// 结果.
    /// </summary>
    [JsonPropertyName("result")]
    public T? Result { get; set; }
}
