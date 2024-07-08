// Copyright (c) Richasy. All rights reserved.

using System;
using System.Text.Json.Serialization;

namespace Richasy.BiliKernel.Services.Media.Core;

internal sealed class PartitionVideo
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("pubdate")]
    public long? PublishDateTime { get; set; }

    [JsonPropertyName("duration")]
    public int? Duration { get; set; }

    [JsonPropertyName("cover")]
    public string? Cover { get; set; }

    [JsonPropertyName("uri")]
    public string? Uri { get; set; }

    [JsonPropertyName("param")]
    public string? Parameter { get; set; }

    [JsonPropertyName("goto")]
    public string? Type { get; set; }

    [JsonPropertyName("name")]
    public string? Publisher { get; set; }

    [JsonPropertyName("face")]
    public string? PublisherAvatar { get; set; }

    [JsonPropertyName("play")]
    public int? PlayCount { get; set; }

    [JsonPropertyName("danmaku")]
    public int? DanmakuCount { get; set; }

    [JsonPropertyName("reply")]
    public int? ReplyCount { get; set; }

    [JsonPropertyName("favourite")]
    public int? FavouriteCount { get; set; }

    [JsonPropertyName("rid")]
    public int? PartitionId { get; set; }

    [JsonPropertyName("rname")]
    public string? PartitionName { get; set; }

    [JsonPropertyName("like")]
    public int? LikeCount { get; set; }

    public override bool Equals(object? obj) => obj is PartitionVideo video && Parameter == video.Parameter;
    public override int GetHashCode() => HashCode.Combine(Parameter);
}
