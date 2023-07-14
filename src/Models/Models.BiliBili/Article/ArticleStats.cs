// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

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

