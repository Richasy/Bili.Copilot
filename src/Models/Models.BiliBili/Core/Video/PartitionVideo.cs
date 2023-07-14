// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 视频基类型.
/// </summary>
public class PartitionVideo : VideoBase
{
    /// <summary>
    /// 视频封面图片地址.
    /// </summary>
    [JsonPropertyName("cover")]
    public string Cover { get; set; }

    /// <summary>
    /// 视频播放地址.
    /// </summary>
    [JsonPropertyName("uri")]
    public string Uri { get; set; }

    /// <summary>
    /// 参数，通常指代视频ID.
    /// </summary>
    [JsonPropertyName("param")]
    public string Parameter { get; set; }

    /// <summary>
    /// 视频类型.
    /// </summary>
    [JsonPropertyName("goto")]
    public string Type { get; set; }

    /// <summary>
    /// 视频发布者.
    /// </summary>
    [JsonPropertyName("name")]
    public string Publisher { get; set; }

    /// <summary>
    /// 视频发布者的头像.
    /// </summary>
    [JsonPropertyName("face")]
    public string PublisherAvatar { get; set; }

    /// <summary>
    /// 视频播放数.
    /// </summary>
    [JsonPropertyName("play")]
    public int PlayCount { get; set; }

    /// <summary>
    /// 弹幕数.
    /// </summary>
    [JsonPropertyName("danmaku")]
    public int DanmakuCount { get; set; }

    /// <summary>
    /// 视频评论数.
    /// </summary>
    [JsonPropertyName("reply")]
    public int ReplyCount { get; set; }

    /// <summary>
    /// 视频收藏数.
    /// </summary>
    [JsonPropertyName("favourite")]
    public int FavouriteCount { get; set; }

    /// <summary>
    /// 所属分区ID.
    /// </summary>
    [JsonPropertyName("rid")]
    public int PartitionId { get; set; }

    /// <summary>
    /// 所属分区名称.
    /// </summary>
    [JsonPropertyName("rname")]
    public string PartitionName { get; set; }

    /// <summary>
    /// 点赞数.
    /// </summary>
    [JsonPropertyName("like")]
    public int LikeCount { get; set; }
}

