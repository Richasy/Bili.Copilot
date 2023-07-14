// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 分集交互信息（包括用户投币，点赞，收藏等信息）.
/// </summary>
public class EpisodeInteraction
{
    /// <summary>
    /// 投币数.
    /// </summary>
    [JsonPropertyName("coin_number")]
    public int CoinNumber { get; set; }

    /// <summary>
    /// 是否收藏.
    /// </summary>
    [JsonPropertyName("favorite")]
    public int IsFavorite { get; set; }

    /// <summary>
    /// 是否点赞.
    /// </summary>
    [JsonPropertyName("like")]
    public int IsLike { get; set; }
}

