// Copyright (c) Richasy. All rights reserved.

using System;

namespace Richasy.BiliKernel.Models.Media;

/// <summary>
/// 直播分类标签.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="LiveTag"/> class.
/// </remarks>
/// <param name="id">标识符.</param>
/// <param name="name">名称.</param>
/// <param name="sortType">排序方式.</param>
public sealed class LiveTag(string id, string name, string sortType)
{
    /// <summary>
    /// 标签Id.
    /// </summary>
    public string Id { get; } = id;

    /// <summary>
    /// 标签名.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// 排序方式.
    /// </summary>
    public string SortType { get; } = sortType;

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is LiveTag tag && Id == tag.Id && SortType == tag.SortType;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id, SortType);
}
