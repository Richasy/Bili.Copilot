// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 文章收藏夹响应结果.
/// </summary>
public class ArticleFavoriteListResponse
{
    /// <summary>
    /// 总条目数.
    /// </summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>
    /// 文章列表.
    /// </summary>
    [JsonPropertyName("items")]
    public List<FavoriteArticleItem> Items { get; set; }
}

/// <summary>
/// 收藏的文章.
/// </summary>
public class FavoriteArticleItem
{
    /// <summary>
    /// 文章Id.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; set; }

    /// <summary>
    /// 标题.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// 横幅图片.
    /// </summary>
    [JsonPropertyName("banner_url")]
    public string Banner { get; set; }

    /// <summary>
    /// 发布者名称.
    /// </summary>
    [JsonPropertyName("name")]
    public string PublisherName { get; set; }

    /// <summary>
    /// 图片链接.
    /// </summary>
    [JsonPropertyName("image_urls")]
    public List<string> Images { get; set; }

    /// <summary>
    /// 提要.
    /// </summary>
    [JsonPropertyName("summary")]
    public string Summary { get; set; }

    /// <summary>
    /// 收藏时间.
    /// </summary>
    [JsonPropertyName("favorite_time")]
    public int CollectTime { get; set; }

    /// <summary>
    /// 网址.
    /// </summary>
    [JsonPropertyName("uri")]
    public string Url { get; set; }

    /// <summary>
    /// 发布者名称.
    /// </summary>
    [JsonPropertyName("up_mid")]
    public int PublisherId { get; set; }

    /// <summary>
    /// 徽章文本.
    /// </summary>
    [JsonPropertyName("badge")]
    public string BadgeText { get; set; }
}

