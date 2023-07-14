// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 在线观看人数.
/// </summary>
public class OnlineViewerCount
{
    /// <summary>
    /// 显示文本.
    /// </summary>
    [JsonPropertyName("total_text")]
    public string DisplayText { get; set; }
}

/// <summary>
/// 在线观看人数响应.
/// </summary>
public class OnlineViewerResponse
{
    /// <summary>
    /// 数据.
    /// </summary>
    [JsonPropertyName("online")]
    public OnlineViewerCount Data { get; set; }
}

