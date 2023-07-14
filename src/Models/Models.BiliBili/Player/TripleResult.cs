// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 一键三连结果.
/// </summary>
public class TripleResult
{
    /// <summary>
    /// 是否点赞.
    /// </summary>
    [JsonPropertyName("like")]
    public bool IsLike { get; set; }

    /// <summary>
    /// 是否投币.
    /// </summary>
    [JsonPropertyName("coin")]
    public bool IsCoin { get; set; }

    /// <summary>
    /// 是否收藏.
    /// </summary>
    [JsonPropertyName("fav")]
    public bool IsFavorite { get; set; }

    /// <summary>
    /// 投币个数.
    /// </summary>
    [JsonPropertyName("multiply")]
    public int CoinNumber { get; set; }
}

