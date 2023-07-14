// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 直播间分区详情响应结果.
/// </summary>
public class LiveAreaDetailResponse
{
    /// <summary>
    /// 子标签.
    /// </summary>
    [JsonPropertyName("new_tags")]
    public List<LiveAreaDetailTag> Tags { get; set; }

    /// <summary>
    /// 总数.
    /// </summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>
    /// 直播间列表.
    /// </summary>
    [JsonPropertyName("list")]
    public List<LiveFeedRoom> List { get; set; }
}

/// <summary>
/// 直播间详情的子标签.
/// </summary>
public class LiveAreaDetailTag
{
    /// <summary>
    /// 标签Id.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 标签名.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// 排序方式.
    /// </summary>
    [JsonPropertyName("sort_type")]
    public string SortType { get; set; }

    /// <summary>
    /// 类型.
    /// </summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }

    /// <summary>
    /// 排序.
    /// </summary>
    [JsonPropertyName("sort")]
    public int Sort { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is LiveAreaDetailTag tag && Id == tag.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => 2108858624 + Id.GetHashCode();
}

