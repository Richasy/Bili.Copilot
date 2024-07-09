// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Richasy.BiliKernel.Services.Article.Core;

/// <summary>
/// 专栏分区.
/// </summary>
internal sealed class ArticleCategory
{
    /// <summary>
    /// 分区Id.
    /// </summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>
    /// 父分区Id.
    /// </summary>
    [JsonPropertyName("parent_id")]
    public long? ParentId { get; set; }

    /// <summary>
    /// 分区名.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 子分区列表.
    /// </summary>
    [JsonPropertyName("children")]
    public IList<ArticleCategory>? Children { get; set; }
}
