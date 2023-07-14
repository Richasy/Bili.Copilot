// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 哔哩哔哩服务器返回的数据响应结构类型.
/// </summary>
public class ServerResponse
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
    public string Message { get; set; }

    /// <summary>
    /// TTL.
    /// </summary>
    [JsonPropertyName("ttl")]
    public int TTL { get; set; }

    /// <summary>
    /// 是否为网络请求错误，而非服务器传回的错误.
    /// </summary>
    public bool IsHttpError { get; set; }

    /// <summary>
    /// 响应结果是否为成功.
    /// </summary>
    /// <returns>成功或失败.</returns>
    public bool IsSuccess() => Code == 0;
}

/// <summary>
/// 哔哩哔哩服务器返回的数据响应结构类型.
/// </summary>
/// <typeparam name="T"><see cref="Data"/>对应的类型.</typeparam>
public class ServerResponse<T> : ServerResponse
{
    /// <summary>
    /// 响应返回的数据.
    /// </summary>
    [JsonPropertyName("data")]
    public T Data { get; set; }
}

/// <summary>
/// 哔哩哔哩服务器返回的数据响应结构类型.
/// </summary>
/// <typeparam name="T"><see cref="Result"/>对应的类型.</typeparam>
public class ServerResponse2<T> : ServerResponse
{
    /// <summary>
    /// 响应返回的数据.
    /// </summary>
    [JsonPropertyName("result")]
    public T Result { get; set; }
}

