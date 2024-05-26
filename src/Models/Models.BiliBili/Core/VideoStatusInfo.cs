// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 视频状态信息.
/// </summary>
public class VideoStatusInfo
{
    /// <summary>
    /// 视频的Aid.
    /// </summary>
    [JsonPropertyName("aid")]
    public long Aid { get; set; }

    /// <summary>
    /// 视频播放数.
    /// </summary>
    [JsonPropertyName("view")]
    public long PlayCount { get; set; }

    /// <summary>
    /// 弹幕数.
    /// </summary>
    [JsonPropertyName("danmaku")]
    public long DanmakuCount { get; set; }

    /// <summary>
    /// 视频评论数.
    /// </summary>
    [JsonPropertyName("reply")]
    public long ReplyCount { get; set; }

    /// <summary>
    /// 视频收藏数.
    /// </summary>
    [JsonPropertyName("favorite")]
    public long FavoriteCount { get; set; }

    /// <summary>
    /// 投币数.
    /// </summary>
    [JsonPropertyName("coin")]
    public long CoinCount { get; set; }

    /// <summary>
    /// 分享数.
    /// </summary>
    [JsonPropertyName("share")]
    public long ShareCount { get; set; }

    /// <summary>
    /// 当前排名.
    /// </summary>
    [JsonPropertyName("now_rank")]
    public int CurrentRanking { get; set; }

    /// <summary>
    /// 历史最高排名.
    /// </summary>
    [JsonPropertyName("his_rank")]
    public int HistoryRanking { get; set; }

    /// <summary>
    /// 点赞数.
    /// </summary>
    [JsonPropertyName("like")]
    public long LikeCount { get; set; }

    /// <summary>
    /// 点踩数.
    /// </summary>
    [JsonPropertyName("dislike")]
    public long DislikeCount { get; set; }
}

