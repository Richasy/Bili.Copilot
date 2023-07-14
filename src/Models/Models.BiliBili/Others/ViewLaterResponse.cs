// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 稍后再看响应结果.
/// </summary>
public class ViewLaterResponse
{
    /// <summary>
    /// 视频列表.
    /// </summary>
    [JsonPropertyName("list")]
    public List<ViewLaterVideo> List { get; set; }

    /// <summary>
    /// 稍后再看视频数.
    /// </summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }
}

