// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Richasy.BiliKernel.Services.Article.Core;

/// <summary>
/// 文章推荐响应结果.
/// </summary>
internal sealed class ArticleRecommendResponse
{
    /// <summary>
    /// 文章列表.
    /// </summary>
    [JsonPropertyName("articles")]
    public List<Article> Articles { get; set; }

    /// <summary>
    /// 排行榜.
    /// </summary>
    [JsonPropertyName("ranks")]
    public List<Article> Ranks { get; set; }
}

/// <summary>
/// 专栏文章.
/// </summary>
internal sealed class Article
{
    /// <summary>
    /// 文章Id.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; set; }

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

/// <summary>
/// 发布者信息.
/// </summary>
public class PublisherInfo
{
    /// <summary>
    /// 视频发布者的Id.
    /// </summary>
    [JsonPropertyName("mid")]
    public long Mid { get; set; }

    /// <summary>
    /// 视频发布者.
    /// </summary>
    [JsonPropertyName("name")]
    public string Publisher { get; set; }

    /// <summary>
    /// 视频发布者的头像.
    /// </summary>
    [JsonPropertyName("face")]
    public string PublisherAvatar { get; set; }
}

/// <summary>
/// 文章参数.
/// </summary>
public class ArticleStats
{
    /// <summary>
    /// 阅读次数.
    /// </summary>
    [JsonPropertyName("view")]
    public int ViewCount { get; set; }

    /// <summary>
    /// 收藏次数.
    /// </summary>
    [JsonPropertyName("favorite")]
    public int FavoriteCount { get; set; }

    /// <summary>
    /// 点赞次数.
    /// </summary>
    [JsonPropertyName("like")]
    public int LikeCount { get; set; }

    /// <summary>
    /// 点踩次数.
    /// </summary>
    [JsonPropertyName("dislike")]
    public int DislikeCount { get; set; }

    /// <summary>
    /// 评论数.
    /// </summary>
    [JsonPropertyName("reply")]
    public int ReplyCount { get; set; }

    /// <summary>
    /// 分享次数.
    /// </summary>
    [JsonPropertyName("share")]
    public int ShareCount { get; set; }

    /// <summary>
    /// 硬币数.
    /// </summary>
    [JsonPropertyName("coin")]
    public int CoinCount { get; set; }

    /// <summary>
    /// 动态转发次数.
    /// </summary>
    [JsonPropertyName("dynamic")]
    public int DynamicCount { get; set; }
}
