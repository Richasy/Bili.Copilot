// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// �����������������ص�������Ӧ�ṹ����.
/// </summary>
public class ServerResponse
{
    /// <summary>
    /// ��Ӧ����.
    /// </summary>
    [JsonPropertyName("code")]
    public int Code { get; set; }

    /// <summary>
    /// ��Ӧ��Ϣ.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; }

    /// <summary>
    /// TTL.
    /// </summary>
    [JsonPropertyName("ttl")]
    public int TTL { get; set; }

    /// <summary>
    /// �Ƿ�Ϊ����������󣬶��Ƿ��������صĴ���.
    /// </summary>
    public bool IsHttpError { get; set; }

    /// <summary>
    /// ��Ӧ����Ƿ�Ϊ�ɹ�.
    /// </summary>
    /// <returns>�ɹ���ʧ��.</returns>
    public bool IsSuccess() => Code == 0;
}

/// <summary>
/// �����������������ص�������Ӧ�ṹ����.
/// </summary>
/// <typeparam name="T"><see cref="Data"/>��Ӧ������.</typeparam>
public class ServerResponse<T> : ServerResponse
{
    /// <summary>
    /// ��Ӧ���ص�����.
    /// </summary>
    [JsonPropertyName("data")]
    public T Data { get; set; }
}

/// <summary>
/// �����������������ص�������Ӧ�ṹ����.
/// </summary>
/// <typeparam name="T"><see cref="Result"/>��Ӧ������.</typeparam>
public class ServerResponse2<T> : ServerResponse
{
    /// <summary>
    /// ��Ӧ���ص�����.
    /// </summary>
    [JsonPropertyName("result")]
    public T Result { get; set; }
}

