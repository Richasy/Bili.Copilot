// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Richasy.BiliKernel.Services.Media.Core;

/// <summary>
/// 子分区类型定义.
/// </summary>
internal class SubPartition
{
    /// <summary>
    /// 推荐视频列表.
    /// </summary>
    [JsonPropertyName("recommend")]
    public List<PartitionVideo> RecommendVideos { get; set; }

    /// <summary>
    /// 新的视频列表.
    /// </summary>
    [JsonPropertyName("new")]
    public List<PartitionVideo> NewVideos { get; set; }

    /// <summary>
    /// 向上刷新的标识符.
    /// </summary>
    [JsonPropertyName("ctop")]
    public long TopOffsetId { get; set; }

    /// <summary>
    /// 向下刷新的标识符.
    /// </summary>
    [JsonPropertyName("cbottom")]
    public long BottomOffsetId { get; set; }
}

/// <summary>
/// 常规子分区.
/// </summary>
internal sealed class SubPartitionDefault : SubPartition
{
    /// <summary>
    /// 高频标签.
    /// </summary>
    [JsonPropertyName("top_tag")]
    public List<PartitionTag> TopTags { get; set; }
}

internal sealed class PartitionTag
{
    [JsonPropertyName("tid")]
    public int TagId { get; set; }

    [JsonPropertyName("tname")]
    public string TagName { get; set; }

    [JsonPropertyName("rid")]
    public int SubPartitionId { get; set; }

    [JsonPropertyName("rname")]
    public string SubPartitionName { get; set; }

    [JsonPropertyName("reid")]
    public int PartitionId { get; set; }

    [JsonPropertyName("rename")]
    public string PartitionName { get; set; }
}
