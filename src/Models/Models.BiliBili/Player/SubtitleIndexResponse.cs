// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 字幕索引响应结果.
/// </summary>
public class SubtitleIndexResponse
{
    /// <summary>
    /// 支持提交.
    /// </summary>
    [JsonPropertyName("allow_submit")]
    public bool AllowSubmit { get; set; }

    /// <summary>
    /// 字幕索引列表.
    /// </summary>
    [JsonPropertyName("subtitles")]
    public List<SubtitleIndexItem> Subtitles { get; set; }
}

/// <summary>
/// 字幕索引条目.
/// </summary>
public class SubtitleIndexItem
{
    /// <summary>
    /// 字幕Id.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; set; }

    /// <summary>
    /// 语言代码.
    /// </summary>
    [JsonPropertyName("lan")]
    public string Language { get; set; }

    /// <summary>
    /// 显示语言.
    /// </summary>
    [JsonPropertyName("lan_doc")]
    public string DisplayLanguage { get; set; }

    /// <summary>
    /// 字幕地址.
    /// </summary>
    [JsonPropertyName("subtitle_url")]
    public string Url { get; set; }
}

