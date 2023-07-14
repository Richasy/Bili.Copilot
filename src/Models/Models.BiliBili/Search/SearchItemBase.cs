// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 搜索条目基类.
/// </summary>
public class SearchItemBase
{
    /// <summary>
    /// 追踪Id.
    /// </summary>
    [JsonPropertyName("trackid")]
    public string TrackId { get; set; }

    /// <summary>
    /// 链接类型. bgm_media指动漫番剧内容，app_user指用户，video指视频.
    /// </summary>
    [JsonPropertyName("linktype")]
    public string LinkType { get; set; }

    /// <summary>
    /// 排序.
    /// </summary>
    [JsonPropertyName("position")]
    public int Position { get; set; }

    /// <summary>
    /// 标题.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// 封面.
    /// </summary>
    [JsonPropertyName("cover")]
    public string Cover { get; set; }

    /// <summary>
    /// 导航链接.
    /// </summary>
    [JsonPropertyName("uri")]
    public string Uri { get; set; }

    /// <summary>
    /// 参数，通常指Id.
    /// </summary>
    [JsonPropertyName("param")]
    public string Parameter { get; set; }

    /// <summary>
    /// 目标指向类型.
    /// </summary>
    [JsonPropertyName("goto")]
    public string Goto { get; set; }
}

