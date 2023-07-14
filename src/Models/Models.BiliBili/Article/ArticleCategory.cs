// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;

namespace Bili.Copilot.Models.BiliBili;

/// <summary>
/// 专栏分区.
/// </summary>
public class ArticleCategory
{
    /// <summary>
    /// 分区Id.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 父分区Id.
    /// </summary>
    [JsonPropertyName("parent_id")]
    public int ParentId { get; set; }

    /// <summary>
    /// 分区名.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// 子分区列表.
    /// </summary>
    [JsonPropertyName("children")]
    public List<ArticleCategory> Children { get; set; }
}

