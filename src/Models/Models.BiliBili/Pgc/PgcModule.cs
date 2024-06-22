// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 动漫及影视模块.
/// </summary>
public class PgcModule
{
    /// <summary>
    /// 模块子项列表.
    /// </summary>
    [JsonPropertyName("items")]
    public List<PgcModuleItem> Items { get; set; }

    /// <summary>
    /// 模块Id.
    /// </summary>
    [JsonPropertyName("module_id")]
    public int Id { get; set; }

    /// <summary>
    /// 模块样式. banner, function, v_card, topic.
    /// </summary>
    [JsonPropertyName("style")]
    public string Style { get; set; }

    /// <summary>
    /// 模块标题.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// 模块类型.
    /// </summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }

    /// <summary>
    /// 模块头列表.
    /// </summary>
    [JsonPropertyName("headers")]
    public List<PgcModuleHeader> Headers { get; set; }
}

/// <summary>
/// PGC内容头.
/// </summary>
public class PgcModuleHeader
{
    /// <summary>
    /// 标题.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// 导航地址.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; }
}

/// <summary>
/// 动漫及影视模块条目.
/// </summary>
public class PgcModuleItem
{
    /// <summary>
    /// 分集Id.
    /// </summary>
    [JsonPropertyName("aid")]
    public long Aid { get; set; }

    /// <summary>
    /// 徽章内容.
    /// </summary>
    [JsonPropertyName("badge")]
    public string Badge { get; set; }

    /// <summary>
    /// 网页链接.
    /// </summary>
    [JsonPropertyName("blink")]
    public string WebLink { get; set; }

    /// <summary>
    /// 不明.
    /// </summary>
    [JsonPropertyName("cid")]
    public long Cid { get; set; }

    /// <summary>
    /// 封面.
    /// </summary>
    [JsonPropertyName("cover")]
    public string Cover { get; set; }

    /// <summary>
    /// 描述内容.
    /// </summary>
    [JsonPropertyName("desc")]
    public string Description { get; set; }

    /// <summary>
    /// 最新章节.
    /// </summary>
    [JsonPropertyName("new_ep")]
    public PgcEpisode NewEpisode { get; set; }

    /// <summary>
    /// 所属动漫或影视剧的Id.
    /// </summary>
    [JsonPropertyName("oid")]
    public int OriginId { get; set; }

    /// <summary>
    /// 剧集的季Id.
    /// </summary>
    [JsonPropertyName("season_id")]
    public int SeasonId { get; set; }

    /// <summary>
    /// 剧集的标签.
    /// </summary>
    [JsonPropertyName("season_styles")]
    public string SeasonTags { get; set; }

    /// <summary>
    /// PGC用户交互参数.
    /// </summary>
    [JsonPropertyName("stat")]
    public PgcItemStat Stat { get; set; }

    /// <summary>
    /// 标题.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// 卡片类型.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>
    /// 动漫状态.
    /// </summary>
    [JsonPropertyName("status")]
    public PgcItemStatus Status { get; set; }

    /// <summary>
    /// 徽章内容.
    /// </summary>
    [JsonPropertyName("cards")]
    public List<PgcModuleItem> Cards { get; set; }

    /// <summary>
    /// 显示的综合评分文本.
    /// </summary>
    [JsonPropertyName("pts")]
    public string DisplayScoreText { get; set; }
}

/// <summary>
/// 剧集信息.
/// </summary>
public class PgcEpisode
{
    /// <summary>
    /// 剧集封面.
    /// </summary>
    [JsonPropertyName("cover")]
    public string Cover { get; set; }

    /// <summary>
    /// 剧集Id.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 显示内容.
    /// </summary>
    [JsonPropertyName("index_show")]
    public string DisplayText { get; set; }
}

/// <summary>
/// 内容参数.
/// </summary>
public class PgcItemStat
{
    /// <summary>
    /// 弹幕数.
    /// </summary>
    [JsonPropertyName("danmaku")]
    public int DanmakuCount { get; set; }

    /// <summary>
    /// 关注数.
    /// </summary>
    [JsonPropertyName("follow")]
    public int FollowCount { get; set; }

    /// <summary>
    /// 关注的显示文本.
    /// </summary>
    [JsonPropertyName("follow_view")]
    public string FollowDisplayText { get; set; }

    /// <summary>
    /// 观看次数.
    /// </summary>
    [JsonPropertyName("view")]
    public int ViewCount { get; set; }
}

/// <summary>
/// 内容状态（关于我是否关注或点赞）.
/// </summary>
public class PgcItemStatus
{
    /// <summary>
    /// 是否已关注，0-未关注，1-已关注.
    /// </summary>
    [JsonPropertyName("follow")]
    public int IsFollow { get; set; }

    /// <summary>
    /// 是否已点赞，0-未点赞，1-已点赞.
    /// </summary>
    [JsonPropertyName("like")]
    public int IsLike { get; set; }
}

