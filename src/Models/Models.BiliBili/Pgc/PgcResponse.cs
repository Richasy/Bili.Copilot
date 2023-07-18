// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 专业产出内容响应结果（包括动漫，电影，电视剧，纪录片等非用户产出内容）.
/// </summary>
public class PgcResponse
{
    /// <summary>
    /// 数据源标识.
    /// </summary>
    [JsonPropertyName("feed")]
    public PgcFeedIdentifier FeedIdentifier { get; set; }

    /// <summary>
    /// 模块.
    /// </summary>
    [JsonPropertyName("modules")]
    public List<PgcModule> Modules { get; set; }

    /// <summary>
    /// 下次请求的指针.
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string NextCursor { get; set; }
}

/// <summary>
/// PGC数据源标识.
/// </summary>
public class PgcFeedIdentifier
{
    /// <summary>
    /// 下属分区Id.
    /// </summary>
    [JsonPropertyName("fall_wid")]
    public List<int> PartitionIds { get; set; }

    /// <summary>
    /// 数据源类型.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }
}

