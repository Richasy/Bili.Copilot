// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 文章搜索条目.
/// </summary>
public class ArticleSearchItem : SearchItemBase
{
    /// <summary>
    /// 用户名.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// 描述.
    /// </summary>
    [JsonPropertyName("desc")]
    public string Description { get; set; }

    /// <summary>
    /// 用户名.
    /// </summary>
    [JsonPropertyName("mid")]
    public int UserId { get; set; }

    /// <summary>
    /// 文章Id.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 图片链接.
    /// </summary>
    [JsonPropertyName("image_urls")]
    public List<string> CoverUrls { get; set; }

    /// <summary>
    /// 阅读次数.
    /// </summary>
    [JsonPropertyName("view")]
    public int ViewCount { get; set; }

    /// <summary>
    /// 点赞次数.
    /// </summary>
    [JsonPropertyName("like")]
    public int LikeCount { get; set; }

    /// <summary>
    /// 评论数.
    /// </summary>
    [JsonPropertyName("reply")]
    public int ReplyCount { get; set; }

    /// <summary>
    /// 徽章文本.
    /// </summary>
    [JsonPropertyName("badge")]
    public string BadgeText { get; set; }
}

