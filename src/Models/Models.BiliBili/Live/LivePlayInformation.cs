// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 直播播放信息.
/// </summary>
public class LivePlayInformation
{
    /// <summary>
    /// 当前播放清晰度.
    /// </summary>
    [JsonPropertyName("current_quality")]
    public int CurrentQuality { get; set; }

    /// <summary>
    /// 当前播放清晰度（网络标识）.
    /// </summary>
    [JsonPropertyName("current_qn")]
    public int CurrentQuality2 { get; set; }

    /// <summary>
    /// 支持的播放清晰度.
    /// </summary>
    [JsonPropertyName("accept_quality")]
    public List<string> AcceptQuality { get; set; }

    /// <summary>
    /// 清晰度列表.
    /// </summary>
    [JsonPropertyName("quality_description")]
    public List<LiveQualityDescription> QualityDescriptions { get; set; }

    /// <summary>
    /// 播放地址列表.
    /// </summary>
    [JsonPropertyName("durl")]
    public List<LivePlayLine> PlayLines { get; set; }
}

/// <summary>
/// 直播播放地址响应结果.
/// </summary>
public class LivePlayUrlResponse
{
    /// <summary>
    /// 当前播放清晰度.
    /// </summary>
    [JsonPropertyName("play_url")]
    public LivePlayInformation Information { get; set; }
}

/// <summary>
/// 直播播放地址.
/// </summary>
public class LivePlayLine
{
    /// <summary>
    /// 播放地址列表.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; }

    /// <summary>
    /// 未知.
    /// </summary>
    [JsonPropertyName("length")]
    public int Length { get; set; }

    /// <summary>
    /// 排序.
    /// </summary>
    [JsonPropertyName("order")]
    public int Order { get; set; }

    /// <summary>
    /// 流类型.
    /// </summary>
    [JsonPropertyName("stream_type")]
    public int StreamType { get; set; }

    /// <summary>
    /// P2P类型.
    /// </summary>
    [JsonPropertyName("p2p_type")]
    public int P2PType { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is LivePlayLine line && Url == line.Url;

    /// <inheritdoc/>
    public override int GetHashCode() => -1915121810 + EqualityComparer<string>.Default.GetHashCode(Url);
}

