// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 稍后再看视频.
/// </summary>
public class ViewLaterVideo : VideoBase
{
    /// <summary>
    /// 视频标识符.
    /// </summary>
    [JsonPropertyName("aid")]
    public int VideoId { get; set; }

    /// <summary>
    /// 稿件分P总数.
    /// </summary>
    [JsonPropertyName("videos")]
    public int PartCount { get; set; }

    /// <summary>
    /// 分区Id.
    /// </summary>
    [JsonPropertyName("tid")]
    public int PartitionId { get; set; }

    /// <summary>
    /// 分区名.
    /// </summary>
    [JsonPropertyName("tname")]
    public string PartitionName { get; set; }

    /// <summary>
    /// 转载或原创，1-转载，2-原创.
    /// </summary>
    [JsonPropertyName("copyright")]
    public int Copyright { get; set; }

    /// <summary>
    /// 视频封面.
    /// </summary>
    [JsonPropertyName("pic")]
    public string Cover { get; set; }

    /// <summary>
    /// 稿件创建时间.
    /// </summary>
    [JsonPropertyName("ctime")]
    public int CreateTime { get; set; }

    /// <summary>
    /// 视频描述.
    /// </summary>
    [JsonPropertyName("desc")]
    public string Description { get; set; }

    /// <summary>
    /// 视频状态.
    /// </summary>
    [JsonPropertyName("state")]
    public int State { get; set; }

    /// <summary>
    /// 视频发布者信息.
    /// </summary>
    [JsonPropertyName("owner")]
    public PublisherInfo Publisher { get; set; }

    /// <summary>
    /// 视频参数.
    /// </summary>
    [JsonPropertyName("stat")]
    public VideoStatusInfo StatusInfo { get; set; }

    /// <summary>
    /// 关联动态的文本内容.
    /// </summary>
    [JsonPropertyName("dynamic")]
    public string DynamicText { get; set; }

    /// <summary>
    /// 短链接.
    /// </summary>
    [JsonPropertyName("short_link_v2")]
    public string ShortLink { get; set; }

    /// <summary>
    /// 分P Id.
    /// </summary>
    [JsonPropertyName("cid")]
    public int PartId { get; set; }

    /// <summary>
    /// 播放进度.
    /// </summary>
    [JsonPropertyName("progress")]
    public int Progress { get; set; }

    /// <summary>
    /// 添加时间.
    /// </summary>
    [JsonPropertyName("add_at")]
    public int AddTime { get; set; }

    /// <summary>
    /// 视频BVId.
    /// </summary>
    [JsonPropertyName("bvid")]
    public string BvId { get; set; }

    /// <summary>
    /// 剧集Id.
    /// </summary>
    [JsonPropertyName("season_id")]
    public int SeasonId { get; set; }
}

