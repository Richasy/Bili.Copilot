// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 首页推荐信息.
/// </summary>
public class HomeRecommendInfo
{
    /// <summary>
    /// 返回的推荐卡片信息列表.
    /// </summary>
    [JsonPropertyName("items")]
    public List<RecommendCard> Items { get; set; }
}

