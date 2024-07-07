// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Richasy.BiliKernel.Services.Media.Core.Models;

/// <summary>
/// 推荐视频的响应.
/// </summary>
internal class CuratedPlaylistResponse
{
    /// <summary>
    /// 推荐视频列表.
    /// </summary>
    [JsonPropertyName("item")]
    public List<CuratedPlaylistVideo> Items { get; set; }
}

/// <summary>
/// 客户端精选视频.
/// </summary>
internal class CuratedPlaylistVideo
{
    /// <summary>
    /// Avid.
    /// </summary>
    [JsonPropertyName("id")]
    public long AvId { get; set; }

    /// <summary>
    /// Bvid.
    /// </summary>
    [JsonPropertyName("bvid")]
    public string? BvId { get; set; }

    /// <summary>
    /// 分集Id.
    /// </summary>
    [JsonPropertyName("cid")]
    public long? Cid { get; set; }

    /// <summary>
    /// 视频类型.
    /// </summary>
    [JsonPropertyName("goto")]
    public string? Goto { get; set; }

    /// <summary>
    /// 视频链接.
    /// </summary>
    [JsonPropertyName("uri")]
    public string? Link { get; set; }

    /// <summary>
    /// 封面.
    /// </summary>
    [JsonPropertyName("pic")]
    public string? Cover { get; set; }

    /// <summary>
    /// 标题.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// 时长 (秒).
    /// </summary>
    [JsonPropertyName("duration")]
    public int? Duration { get; set; }

    /// <summary>
    /// 发布时间.
    /// </summary>
    [JsonPropertyName("pubdate")]
    public long? PublishTime { get; set; }

    /// <summary>
    /// 发布者.
    /// </summary>
    [JsonPropertyName("owner")]
    public PublisherInfo? Owner { get; set; }

    /// <summary>
    /// 状态.
    /// </summary>
    [JsonPropertyName("stat")]
    public VideoStatusInfo Stat { get; set; }

    /// <summary>
    /// 推荐原因.
    /// </summary>
    [JsonPropertyName("rcmd_reason")]
    public CuratedPlaylistReason RecommendReason { get; set; }
}

/// <summary>
/// 网页推荐原因.
/// </summary>
internal class CuratedPlaylistReason
{
    /// <summary>
    /// 推荐原因.
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; }
}
