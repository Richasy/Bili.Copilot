// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 字幕详情响应结果.
/// </summary>
public class SubtitleDetailResponse
{
    /// <summary>
    /// 字幕列表.
    /// </summary>
    [JsonPropertyName("body")]
    public List<SubtitleItem> Body { get; set; }
}

/// <summary>
/// 字幕条目.
/// </summary>
public class SubtitleItem
{
    /// <summary>
    /// 开始时间.
    /// </summary>
    [JsonPropertyName("from")]
    public double From { get; set; }

    /// <summary>
    /// 结束时间.
    /// </summary>
    [JsonPropertyName("to")]
    public double To { get; set; }

    /// <summary>
    /// 字幕内容.
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; }
}

