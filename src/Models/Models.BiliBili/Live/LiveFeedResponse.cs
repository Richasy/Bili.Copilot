// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 直播源列表响应类型.
/// </summary>
public class LiveFeedResponse
{
    /// <summary>
    /// 直播源卡片列表.
    /// </summary>
    [JsonPropertyName("card_list")]
    public List<LiveFeedCard> CardList { get; set; }
}

