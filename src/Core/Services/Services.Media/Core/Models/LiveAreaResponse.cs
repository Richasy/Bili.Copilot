// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Richasy.BiliKernel.Services.Media.Core;

internal sealed class LiveAreaResponse
{
    [JsonPropertyName("list")]
    public IList<LiveAreaGroup>? List { get; set; }
}

internal sealed class LiveArea
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("link")]
    public string? Link { get; set; }

    [JsonPropertyName("pic")]
    public string? Cover { get; set; }

    [JsonPropertyName("parent_id")]
    public int ParentId { get; set; }

    [JsonPropertyName("parent_name")]
    public string? ParentName { get; set; }

    [JsonPropertyName("area_type")]
    public int AreaType { get; set; }

    [JsonPropertyName("is_new")]
    public bool IsNew { get; set; }
}

internal sealed class LiveAreaGroup
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("parent_area_type")]
    public int ParentAreaType { get; set; }

    [JsonPropertyName("area_list")]
    public IList<LiveArea>? AreaList { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is LiveAreaGroup group && Id == group.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
