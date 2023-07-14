// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 搜索推荐条目.
/// </summary>
public class SearchSquareItem
{
    /// <summary>
    /// 类型.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>
    /// 标题.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// 响应代码.
    /// </summary>
    [JsonPropertyName("data")]
    public SearchSquareData Data { get; set; }
}

/// <summary>
/// 搜索推荐的具体条目数据.
/// </summary>
public class SearchSquareData
{
    /// <summary>
    /// 响应代码.
    /// </summary>
    [JsonPropertyName("trackid")]
    public string TrackId { get; set; }

    /// <summary>
    /// 搜索建议条目.
    /// </summary>
    [JsonPropertyName("list")]
    public List<SearchRecommendItem> List { get; set; }

    /// <summary>
    /// 具体显示标题.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }
}

/// <summary>
/// 搜索建议条目.
/// </summary>
public class SearchRecommendItem
{
    /// <summary>
    /// 搜索关键词.
    /// </summary>
    [JsonPropertyName("keyword")]
    public string Keyword { get; set; }

    /// <summary>
    /// 显示名称.
    /// </summary>
    [JsonPropertyName("show_name")]
    public string DisplayName { get; set; }

    /// <summary>
    /// 要显示的图标.
    /// </summary>
    [JsonPropertyName("icon")]
    public string Icon { get; set; }

    /// <summary>
    /// 排序.
    /// </summary>
    [JsonPropertyName("position")]
    public int Position { get; set; }

    /// <summary>
    /// 标题（在搜索推荐中有效）.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>
    /// 建议类型（在搜索推荐中有效，通常为guess）.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>
    /// 建议的Id（在搜索推荐中有效，通常为标签Id）.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; set; }
}

