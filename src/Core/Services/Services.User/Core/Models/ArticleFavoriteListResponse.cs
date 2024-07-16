// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Richasy.BiliKernel.Services.User.Core;

/// <summary>
/// 文章收藏夹响应结果.
/// </summary>
internal sealed class ArticleFavoriteListResponse
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
    public IList<FavoriteArticleItem>? Items { get; set; }
}

/// <summary>
/// 收藏的文章.
/// </summary>
internal sealed class FavoriteArticleItem
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
    public string? Title { get; set; }

    /// <summary>
    /// 横幅图片.
    /// </summary>
    [JsonPropertyName("banner_url")]
    public string? Banner { get; set; }

    /// <summary>
    /// 发布者名称.
    /// </summary>
    [JsonPropertyName("name")]
    public string? PublisherName { get; set; }

    /// <summary>
    /// 图片链接.
    /// </summary>
    [JsonPropertyName("image_urls")]
    public IList<string>? Images { get; set; }

    /// <summary>
    /// 提要.
    /// </summary>
    [JsonPropertyName("summary")]
    public string? Summary { get; set; }

    /// <summary>
    /// 收藏时间.
    /// </summary>
    [JsonPropertyName("favorite_time")]
    public long CollectTime { get; set; }

    /// <summary>
    /// 网址.
    /// </summary>
    [JsonPropertyName("uri")]
    public string? Url { get; set; }

    /// <summary>
    /// 发布者名称.
    /// </summary>
    [JsonPropertyName("up_mid")]
    public long PublisherId { get; set; }

    /// <summary>
    /// 徽章文本.
    /// </summary>
    [JsonPropertyName("badge")]
    public string? BadgeText { get; set; }
}
