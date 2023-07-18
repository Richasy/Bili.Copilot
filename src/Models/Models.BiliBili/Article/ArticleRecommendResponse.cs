// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 文章推荐响应结果.
/// </summary>
public class ArticleRecommendResponse
{
    /// <summary>
    /// 横幅.
    /// </summary>
    [JsonPropertyName("banners")]
    public List<PartitionBanner> Banners { get; set; }

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

