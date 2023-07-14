// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 直播分区响应结果.
/// </summary>
public class LiveAreaResponse
{
    /// <summary>
    /// 列表.
    /// </summary>
    [JsonPropertyName("list")]
    public List<LiveAreaGroup> List { get; set; }
}

