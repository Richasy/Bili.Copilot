// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// PGC索引筛选条件响应.
/// </summary>
public class PgcIndexConditionResponse
{
    /// <summary>
    /// 筛选条件.
    /// </summary>
    [JsonPropertyName("filter")]
    public List<PgcIndexFilter> FilterList { get; set; }

    /// <summary>
    /// 排序方式.
    /// </summary>
    [JsonPropertyName("order")]
    public List<PgcIndexOrder> OrderList { get; set; }
}

/// <summary>
/// PGC索引筛选条件.
/// </summary>
public class PgcIndexFilter
{
    /// <summary>
    /// 筛选关键词.
    /// </summary>
    [JsonPropertyName("field")]
    public string Field { get; set; }

    /// <summary>
    /// 筛选条目名.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// 可选值.
    /// </summary>
    [JsonPropertyName("values")]
    public List<PgcIndexFilterValue> Values { get; set; }
}

/// <summary>
/// PGC索引筛选条件可选值.
/// </summary>
public class PgcIndexFilterValue
{
    /// <summary>
    /// 关键词.
    /// </summary>
    [JsonPropertyName("keyword")]
    public string Keyword { get; set; }

    /// <summary>
    /// 显示名称.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }
}

/// <summary>
/// PGC索引排序条件.
/// </summary>
public class PgcIndexOrder
{
    /// <summary>
    /// 排序关键词.
    /// </summary>
    [JsonPropertyName("field")]
    public string Field { get; set; }

    /// <summary>
    /// 排序名.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }
}

