// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 直播播放信息.
/// </summary>
public class LiveAppPlayInformation
{
    /// <summary>
    /// 直播间Id.
    /// </summary>
    [JsonPropertyName("room_id")]
    public int RoomId { get; set; }

    /// <summary>
    /// 用户Id.
    /// </summary>
    [JsonPropertyName("uid")]
    public long UserId { get; set; }

    /// <summary>
    /// 直播状态，1表示正在直播.
    /// </summary>
    [JsonPropertyName("live_status")]
    public int LiveStatus { get; set; }

    /// <summary>
    /// 直播时间.
    /// </summary>
    [JsonPropertyName("live_time")]
    public long LiveTime { get; set; }

    /// <summary>
    /// 播放信息.
    /// </summary>
    [JsonPropertyName("playurl_info")]
    public LiveAppPlayUrlInfo PlayUrlInfo { get; set; }
}

/// <summary>
/// 直播播放地址信息.
/// </summary>
public class LiveAppPlayUrlInfo
{
    /// <summary>
    /// 播放信息.
    /// </summary>
    [JsonPropertyName("playurl")]
    public LiveAppPlayData PlayUrl { get; set; }
}

/// <summary>
/// 直播播放链接.
/// </summary>
public class LiveAppPlayData
{
    /// <summary>
    /// 直播间Id.
    /// </summary>
    [JsonPropertyName("cid")]
    public int Cid { get; set; }

    /// <summary>
    /// 清晰度列表.
    /// </summary>
    [JsonPropertyName("g_qn_desc")]
    public List<LiveAppQualityDescription> Descriptions { get; set; }

    /// <summary>
    /// 播放流.
    /// </summary>
    [JsonPropertyName("stream")]
    public List<LiveAppPlayStream> StreamList { get; set; }
}

/// <summary>
/// 清晰度描述.
/// </summary>
public class LiveAppQualityDescription
{
    /// <summary>
    /// 清晰度标识.
    /// </summary>
    [JsonPropertyName("qn")]
    public int Quality { get; set; }

    /// <summary>
    /// 描述.
    /// </summary>
    [JsonPropertyName("desc")]
    public string Description { get; set; }

    /// <summary>
    /// HDR 标识.
    /// </summary>
    [JsonPropertyName("hdr_desc")]
    public string HDRSign { get; set; }
}

/// <summary>
/// 直播播放流.
/// </summary>
public class LiveAppPlayStream
{
    /// <summary>
    /// 协议名称.
    /// </summary>
    [JsonPropertyName("protocol_name")]
    public string ProtocolName { get; set; }

    /// <summary>
    /// 格式列表.
    /// </summary>
    [JsonPropertyName("format")]
    public List<LiveAppPlayFormat> FormatList { get; set; }
}

/// <summary>
/// 直播播放格式.
/// </summary>
public class LiveAppPlayFormat
{
    /// <summary>
    /// 格式名称.
    /// </summary>
    [JsonPropertyName("format_name")]
    public string FormatName { get; set; }

    /// <summary>
    /// 分区Id.
    /// </summary>
    [JsonPropertyName("codec")]
    public List<LiveAppPlayCodec> CodecList { get; set; }
}

/// <summary>
/// 直播播放解码信息.
/// </summary>
public class LiveAppPlayCodec
{
    /// <summary>
    /// 解码名.
    /// </summary>
    [JsonPropertyName("codec_name")]
    public string CodecName { get; set; }

    /// <summary>
    /// 当前清晰度标识.
    /// </summary>
    [JsonPropertyName("current_qn")]
    public int CurrentQuality { get; set; }

    /// <summary>
    /// 支持的清晰度.
    /// </summary>
    [JsonPropertyName("accept_qn")]
    public List<int> AcceptQualities { get; set; }

    /// <summary>
    /// 基础链接.
    /// </summary>
    [JsonPropertyName("base_url")]
    public string BaseUrl { get; set; }

    /// <summary>
    /// 播放地址列表.
    /// </summary>
    [JsonPropertyName("url_info")]
    public List<LiveAppPlayUrl> Urls { get; set; }

    /// <summary>
    /// 杜比类型，0-关闭, 1-开启.
    /// </summary>
    [JsonPropertyName("dolby_type")]
    public int DolbyType { get; set; }
}

/// <summary>
/// 直播播放地址拼接信息.
/// </summary>
public class LiveAppPlayUrl
{
    /// <summary>
    /// 域名.
    /// </summary>
    [JsonPropertyName("host")]
    public string Host { get; set; }

    /// <summary>
    /// 后缀.
    /// </summary>
    [JsonPropertyName("extra")]
    public string Extra { get; set; }

    /// <summary>
    /// 流的有效时间，通常为1个小时.
    /// </summary>
    [JsonPropertyName("stream_ttl")]
    public int StreamTTL { get; set; }
}

