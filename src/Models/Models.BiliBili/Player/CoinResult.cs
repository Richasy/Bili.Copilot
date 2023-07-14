// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 投币结果.
/// </summary>
public class CoinResult
{
    /// <summary>
    /// 是否也点赞.
    /// </summary>
    [JsonPropertyName("like")]
    public bool IsAlsoLike { get; set; }
}

