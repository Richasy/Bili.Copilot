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

    /// <summary>
    /// 分享数据.
    /// </summary>
    [JsonPropertyName("share")]
    public ShareData Share { get; set; }

    /// <summary>
    /// 分享数据.
    /// </summary>
    public class ShareData
    {
        /// <summary>
        /// 类型.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// 视频数据.
        /// </summary>
        [JsonPropertyName("video")]
        public Video Video { get; set; }
    }

    /// <summary>
    /// 视频基本数据.
    /// </summary>
    public class Video
    {
        /// <summary>
        /// BV Id.
        /// </summary>
        [JsonPropertyName("bvid")]
        public string BvId { get; set; }

        /// <summary>
        /// 分P Id.
        /// </summary>
        [JsonPropertyName("cid")]
        public int Cid { get; set; }

        /// <summary>
        /// 短链接.
        /// </summary>
        [JsonPropertyName("short_link")]
        public string ShortLink { get; set; }
    }
}

