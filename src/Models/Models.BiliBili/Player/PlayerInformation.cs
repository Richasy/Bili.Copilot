// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// Dash播放信息.
/// </summary>
public class PlayerInformation
{
    /// <summary>
    /// 视频清晰度.
    /// </summary>
    [JsonPropertyName("quality")]
    public int Quality { get; set; }

    /// <summary>
    /// 当前视频格式.
    /// </summary>
    [JsonPropertyName("format")]
    public string Format { get; set; }

    /// <summary>
    /// 视频时长.
    /// </summary>
    [JsonPropertyName("timelength")]
    public int Duration { get; set; }

    /// <summary>
    /// 支持的清晰度说明文本列表.
    /// </summary>
    [JsonPropertyName("accept_description")]
    public List<string> AcceptDescription { get; set; }

    /// <summary>
    /// 支持的清晰度列表.
    /// </summary>
    [JsonPropertyName("accept_quality")]
    public List<int> AcceptQualities { get; set; }

    /// <summary>
    /// 视频编码Id.
    /// </summary>
    [JsonPropertyName("video_codecid")]
    public int CodecId { get; set; }

    /// <summary>
    /// Dash视频播放信息.
    /// </summary>
    [JsonPropertyName("dash")]
    public DashVideo VideoInformation { get; set; }

    /// <summary>
    /// Flv视频播放信息.
    /// </summary>
    [JsonPropertyName("durl")]
    public List<FlvItem> FlvInformation { get; set; }

    /// <summary>
    /// 支持的视频格式列表.
    /// </summary>
    [JsonPropertyName("support_formats")]
    public List<VideoFormat> SupportFormats { get; set; }
}

/// <summary>
/// Dash视频信息.
/// </summary>
public class DashVideo
{
    /// <summary>
    /// 视频时长.
    /// </summary>
    [JsonPropertyName("duration")]
    public int Duration { get; set; }

    /// <summary>
    /// 最低缓冲时间.
    /// </summary>
    [JsonPropertyName("minBufferTime")]
    public float MinBufferTime { get; set; }

    /// <summary>
    /// 不同清晰度的视频列表.
    /// </summary>
    [JsonPropertyName("video")]
    public List<DashItem> Video { get; set; }

    /// <summary>
    /// 不同码率的音频列表.
    /// </summary>
    [JsonPropertyName("audio")]
    public List<DashItem> Audio { get; set; }
}

/// <summary>
/// Dash条目.
/// </summary>
public class DashItem
{
    /// <summary>
    /// Dash Id.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 基链接.
    /// </summary>
    [JsonPropertyName("base_url")]
    public string BaseUrl { get; set; }

    /// <summary>
    /// 备份链接.
    /// </summary>
    [JsonPropertyName("backup_url")]
    public List<string> BackupUrl { get; set; }

    /// <summary>
    /// 媒体要求的带宽.
    /// </summary>
    [JsonPropertyName("bandwidth")]
    public int BandWidth { get; set; }

    /// <summary>
    /// 媒体格式.
    /// </summary>
    [JsonPropertyName("mime_type")]
    public string MimeType { get; set; }

    /// <summary>
    /// 媒体编码.
    /// </summary>
    [JsonPropertyName("codecs")]
    public string Codecs { get; set; }

    /// <summary>
    /// 媒体宽度.
    /// </summary>
    [JsonPropertyName("width")]
    public int Width { get; set; }

    /// <summary>
    /// 媒体高度.
    /// </summary>
    [JsonPropertyName("height")]
    public int Height { get; set; }

    /// <summary>
    /// 帧率.
    /// </summary>
    [JsonPropertyName("frame_rate")]
    public string FrameRate { get; set; }

    /// <summary>
    /// 播放比例.
    /// </summary>
    [JsonPropertyName("sar")]
    public string Scale { get; set; }

    /// <summary>
    /// 分段的基础信息.
    /// </summary>
    [JsonPropertyName("segment_base")]
    public SegmentBase SegmentBase { get; set; }

    /// <summary>
    /// 编码Id.
    /// </summary>
    [JsonPropertyName("codecid")]
    public int CodecId { get; set; }
}

/// <summary>
/// 分段基础信息.
/// </summary>
public class SegmentBase
{
    /// <summary>
    /// 起始位置.
    /// </summary>
    [JsonPropertyName("initialization")]
    public string Initialization { get; set; }

    /// <summary>
    /// 索引范围.
    /// </summary>
    [JsonPropertyName("index_range")]
    public string IndexRange { get; set; }
}

/// <summary>
/// 支持的格式.
/// </summary>
public class VideoFormat
{
    /// <summary>
    /// 清晰度标识.
    /// </summary>
    [JsonPropertyName("quality")]
    public int Quality { get; set; }

    /// <summary>
    /// 格式.
    /// </summary>
    [JsonPropertyName("format")]
    public string Format { get; set; }

    /// <summary>
    /// 显示的说明文本.
    /// </summary>
    [JsonPropertyName("new_description")]
    public string Description { get; set; }

    /// <summary>
    /// 上标.
    /// </summary>
    [JsonPropertyName("superscript")]
    public string Superscript { get; set; }
}

/// <summary>
/// FLV视频条目.
/// </summary>
public class FlvItem
{
    /// <summary>
    /// 序号.
    /// </summary>
    [JsonPropertyName("order")]
    public int Order { get; set; }

    /// <summary>
    /// 时长.
    /// </summary>
    [JsonPropertyName("length")]
    public int Length { get; set; }

    /// <summary>
    /// 大小.
    /// </summary>
    [JsonPropertyName("size")]
    public int Size { get; set; }

    /// <summary>
    /// 播放地址.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; }

    /// <summary>
    /// 备用地址列表.
    /// </summary>
    [JsonPropertyName("backup_url")]
    public List<string> BackupUrls { get; set; }
}

