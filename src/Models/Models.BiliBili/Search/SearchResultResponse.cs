// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 搜索结果响应.
/// </summary>
public class SearchResultResponse
{
    /// <summary>
    /// 追踪Id.
    /// </summary>
    [JsonPropertyName("trackid")]
    public string TrackId { get; set; }

    /// <summary>
    /// 页码.
    /// </summary>
    [JsonPropertyName("pages")]
    public int PageNumber { get; set; }

    /// <summary>
    /// 总数.
    /// </summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }

    /// <summary>
    /// 搜索关键词.
    /// </summary>
    [JsonPropertyName("keyword")]
    public string Keyword { get; set; }
}

/// <summary>
/// 综合搜索结果.
/// </summary>
public class ComprehensiveSearchResultResponse : SearchResultResponse
{
    /// <summary>
    /// 子模块列表.
    /// </summary>
    [JsonPropertyName("nav")]
    public List<SearchSubModule> SubModuleList { get; set; }

    /// <summary>
    /// 条目列表.
    /// </summary>
    [JsonPropertyName("item")]
    public List<VideoSearchItem> ItemList { get; set; }
}

/// <summary>
/// 常规子模块搜索结果.
/// </summary>
/// <typeparam name="T">内容类型.</typeparam>
public class SubModuleSearchResultResponse<T> : SearchResultResponse
{
    /// <summary>
    /// 条目列表.
    /// </summary>
    [JsonPropertyName("items")]
    public List<T> ItemList { get; set; }
}

/// <summary>
/// 直播搜索结果.
/// </summary>
public class LiveSearchResultResponse : SearchResultResponse
{
    /// <summary>
    /// 直播间结果.
    /// </summary>
    [JsonPropertyName("live_room")]
    public LiveRoomSearchResult RoomResult { get; set; }
}

/// <summary>
/// 搜索子模块.
/// </summary>
public class SearchSubModule
{
    /// <summary>
    /// 显示标题.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// 搜索结果总数.
    /// </summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }

    /// <summary>
    /// 分页数.
    /// </summary>
    [JsonPropertyName("pages")]
    public int PageCount { get; set; }

    /// <summary>
    /// 类型.
    /// </summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }
}

/// <summary>
/// 直播间搜索结果.
/// </summary>
public class LiveRoomSearchResult
{
    /// <summary>
    /// 条目列表.
    /// </summary>
    [JsonPropertyName("items")]
    public List<LiveSearchItem> Items { get; set; }
}

