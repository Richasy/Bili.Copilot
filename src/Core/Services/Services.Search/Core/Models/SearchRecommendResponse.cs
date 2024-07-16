// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Richasy.BiliKernel.Services.Search.Core;


internal sealed class SearchRecommendResponse
{
    [JsonPropertyName("trackid")]
    public string Trackid { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("pages")]
    public int Count { get; set; }

    [JsonPropertyName("list")]
    public IList<WebSearchRecommendItem> List { get; set; }
}

internal sealed class WebSearchRecommendItem
{
    [JsonPropertyName("position")]
    public int Position { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("param")]
    public string Param { get; set; }

    [JsonPropertyName("recommend_reason")]
    public string RecommendReason { get; set; }

    [JsonPropertyName("author_perfix")]
    public string AuthorPrefix { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("keyword")]
    public string Keyword { get; set; }
}

