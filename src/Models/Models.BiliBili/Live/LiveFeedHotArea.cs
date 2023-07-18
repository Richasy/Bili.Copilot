// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 直播源热门分区.
/// </summary>
public class LiveFeedHotArea : LiveFeedExtraCardBase
{
    /// <summary>
    /// 分区Id.
    /// </summary>
    [JsonPropertyName("area_v2_id")]
    public long AreaId { get; set; }

    /// <summary>
    /// 父分区Id.
    /// </summary>
    [JsonPropertyName("area_v2_parent_id")]
    public long ParentAreaId { get; set; }

    /// <summary>
    /// 标签类型.
    /// </summary>
    [JsonPropertyName("tag_type")]
    public long TagType { get; set; }
}

/// <summary>
/// 直播源热门分区列表.
/// </summary>
public class LiveFeedHotAreaList
{
    /// <summary>
    /// 列表数据.
    /// </summary>
    [JsonPropertyName("list")]
    public List<LiveFeedHotArea> List { get; set; }
}

