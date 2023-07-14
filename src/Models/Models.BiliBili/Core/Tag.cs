// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 标签类型.
/// </summary>
public class Tag
{
    /// <summary>
    /// 标签ID.
    /// </summary>
    [JsonPropertyName("tid")]
    public int TagId { get; set; }

    /// <summary>
    /// 标签名.
    /// </summary>
    [JsonPropertyName("tname")]
    public string TagName { get; set; }

    /// <summary>
    /// 所属子分区ID.
    /// </summary>
    [JsonPropertyName("rid")]
    public int SubPartitionId { get; set; }

    /// <summary>
    /// 所属子分区名称.
    /// </summary>
    [JsonPropertyName("rname")]
    public string SubPartitionName { get; set; }

    /// <summary>
    /// 所属主分区ID.
    /// </summary>
    [JsonPropertyName("reid")]
    public int PartitionId { get; set; }

    /// <summary>
    /// 所属主分区名称.
    /// </summary>
    [JsonPropertyName("rename")]
    public string PartitionName { get; set; }
}

