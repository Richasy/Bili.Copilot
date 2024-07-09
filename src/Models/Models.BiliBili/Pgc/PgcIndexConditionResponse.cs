// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// PGC����ɸѡ������Ӧ.
/// </summary>
public class PgcIndexConditionResponse
{
    /// <summary>
    /// ɸѡ����.
    /// </summary>
    [JsonPropertyName("filter")]
    public List<PgcIndexFilter> FilterList { get; set; }

    /// <summary>
    /// ����ʽ.
    /// </summary>
    [JsonPropertyName("order")]
    public List<PgcIndexOrder> OrderList { get; set; }
}

/// <summary>
/// PGC����ɸѡ����.
/// </summary>
public class PgcIndexFilter
{
    /// <summary>
    /// ɸѡ�ؼ���.
    /// </summary>
    [JsonPropertyName("field")]
    public string Field { get; set; }

    /// <summary>
    /// ɸѡ��Ŀ��.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// ��ѡֵ.
    /// </summary>
    [JsonPropertyName("values")]
    public List<PgcIndexFilterValue> Values { get; set; }
}

/// <summary>
/// PGC����ɸѡ������ѡֵ.
/// </summary>
public class PgcIndexFilterValue
{
    /// <summary>
    /// �ؼ���.
    /// </summary>
    [JsonPropertyName("keyword")]
    public string Keyword { get; set; }

    /// <summary>
    /// ��ʾ����.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }
}

/// <summary>
/// PGC������������.
/// </summary>
public class PgcIndexOrder
{
    /// <summary>
    /// ����ؼ���.
    /// </summary>
    [JsonPropertyName("field")]
    public string Field { get; set; }

    /// <summary>
    /// ������.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }
}

