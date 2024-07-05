// Copyright (c) Richasy. All rights reserved.

using System;
using System.Text.Json.Serialization;

namespace Richasy.BiliKernel.Services.User.Core;

/// <summary>
/// 关注分组.
/// </summary>
internal sealed class RelatedTag
{
    /// <summary>
    /// 分组Id.
    /// </summary>
    [JsonPropertyName("tagid")]
    public int TagId { get; set; }

    /// <summary>
    /// 名称.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 总数.
    /// </summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>
    /// 说明.
    /// </summary>
    [JsonPropertyName("tip")]
    public string? Tip { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is RelatedTag tag && TagId == tag.TagId;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(TagId);
}
