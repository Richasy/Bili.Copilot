// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 子分区类型定义.
/// </summary>
public class SubPartition
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
/// 子分区的推荐模块.
/// </summary>
public class SubPartitionRecommend : SubPartition
{
    /// <summary>
    /// 横幅.
    /// </summary>
    [JsonPropertyName("banner")]
    public RecommendBanner Banner { get; set; }

    /// <summary>
    /// 推荐列表下的横幅定义.
    /// </summary>
    public class RecommendBanner
    {
        /// <summary>
        /// 顶层横幅.
        /// </summary>
        [JsonPropertyName("top")]
        public List<PartitionBanner> TopBanners { get; set; }
    }
}

/// <summary>
/// 常规子分区.
/// </summary>
public class SubPartitionDefault : SubPartition
{
    /// <summary>
    /// 高频标签.
    /// </summary>
    [JsonPropertyName("top_tag")]
    public List<Tag> TopTags { get; set; }
}

