// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 视频搜索结果条目.
/// </summary>
public class VideoSearchItem : SearchItemBase
{
    /// <summary>
    /// 播放数.
    /// </summary>
    [JsonPropertyName("play")]
    public int PlayCount { get; set; }

    /// <summary>
    /// 弹幕数.
    /// </summary>
    [JsonPropertyName("danmaku")]
    public int DanmakuCount { get; set; }

    /// <summary>
    /// 作者.
    /// </summary>
    [JsonPropertyName("author")]
    public string Author { get; set; }

    /// <summary>
    /// 说明文本.
    /// </summary>
    [JsonPropertyName("desc")]
    public string Description { get; set; }

    /// <summary>
    /// 时长.
    /// </summary>
    [JsonPropertyName("duration")]
    public string Duration { get; set; }

    /// <summary>
    /// 用户Id.
    /// </summary>
    [JsonPropertyName("mid")]
    public long UserId { get; set; }

    /// <summary>
    /// 头像.
    /// </summary>
    [JsonPropertyName("face")]
    public string Avatar { get; set; }
}

