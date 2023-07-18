// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 专栏文章.
/// </summary>
public class Article
{
    /// <summary>
    /// 文章Id.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 所属标签.
    /// </summary>
    [JsonPropertyName("category")]
    public ArticleCategory Category { get; set; }

    /// <summary>
    /// 关联标签列表.
    /// </summary>
    [JsonPropertyName("categories")]
    public List<ArticleCategory> RelatedCategories { get; set; }

    /// <summary>
    /// 标题.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// 提要.
    /// </summary>
    [JsonPropertyName("summary")]
    public string Summary { get; set; }

    /// <summary>
    /// 作者.
    /// </summary>
    [JsonPropertyName("author")]
    public PublisherInfo Publisher { get; set; }

    /// <summary>
    /// 发布时间.
    /// </summary>
    [JsonPropertyName("publish_time")]
    public long PublishTime { get; set; }

    /// <summary>
    /// 创建时间.
    /// </summary>
    [JsonPropertyName("ctime")]
    public long CreateTime { get; set; }

    /// <summary>
    /// 文章状态参数.
    /// </summary>
    [JsonPropertyName("stats")]
    public ArticleStats Stats { get; set; }

    /// <summary>
    /// 字数.
    /// </summary>
    [JsonPropertyName("words")]
    public int WordCount { get; set; }

    /// <summary>
    /// 文章封面列表.
    /// </summary>
    [JsonPropertyName("origin_image_urls")]
    public List<string> CoverUrls { get; set; }

    /// <summary>
    /// 是否已点赞.
    /// </summary>
    [JsonPropertyName("is_like")]
    public bool IsLike { get; set; }

    /// <summary>
    /// 是否为原创，0-非原创，1-原创.
    /// </summary>
    [JsonPropertyName("original")]
    public int IsOriginal { get; set; }
}

