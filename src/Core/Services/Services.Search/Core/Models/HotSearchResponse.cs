// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Richasy.BiliKernel.Services.Search.Core;

internal sealed class HotSearchResponse
{
    [JsonPropertyName("trackid")]
    public string TrackId { get; set; }

    [JsonPropertyName("list")]
    public IList<WebHotSearchItem>? List { get; set; }
}

internal sealed class WebHotSearchItem
{
    [JsonPropertyName("position")]
    public int Position { get; set; }

    [JsonPropertyName("keyword")]
    public string? Keyword { get; set; }

    [JsonPropertyName("show_name")]
    public string? ShowName { get; set; }

    [JsonPropertyName("word_type")]
    public int WordType { get; set; }

    [JsonPropertyName("hot_id")]
    public long? HotId { get; set; }

    [JsonPropertyName("resource_id")]
    public long? ResourceId { get; set; }

    [JsonPropertyName("show_live_icon")]
    public bool? ShowLiveIcon { get; set; }

    [JsonPropertyName("is_commercial")]
    public string? IsCommercial { get; set; }

    [JsonPropertyName("icon")]
    public string? Icon { get; set; }
}
