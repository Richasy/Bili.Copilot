// Copyright (c) Richasy. All rights reserved.

using System;

namespace Richasy.BiliKernel.Models.Search;

/// <summary>
/// 搜索分区.
/// </summary>
public sealed class SearchPartition
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchPartition"/> class.
    /// </summary>
    public SearchPartition(
        int id,
        string title,
        int? totalPageCount = default,
        int? totalItemCount = default)
    {
        Id = id;
        Title = title;
        TotalPageCount = totalPageCount;
        TotalItemCount = totalItemCount;
    }

    /// <summary>
    /// 标识符.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// 标题.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// 总共分页数.
    /// </summary>
    public int? TotalPageCount { get; }

    /// <summary>
    /// 总共条目数.
    /// </summary>
    public int? TotalItemCount { get; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is SearchPartition partition && Id == partition.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
