// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 视频基类型.
/// </summary>
public class VideoBase
{
    /// <summary>
    /// 视频标题.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// 视频发布时间 (Unix时间戳).
    /// </summary>
    [JsonPropertyName("pubdate")]
    public long PublishDateTime { get; set; }

    /// <summary>
    /// 视频时长 (秒).
    /// </summary>
    [JsonPropertyName("duration")]
    public int Duration { get; set; }
}

