// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;

namespace Richasy.BiliKernel.Models.Appearance;

/// <summary>
/// 条件筛选，可用于生成一个选择器.
/// </summary>
/// <param name="name">显示名称.</param>
/// <param name="id">标识符.</param>
/// <param name="conditions">条件列表.</param>
public sealed class Filter(string name, string id, IList<Condition> conditions)
{
    /// <summary>
    /// 筛选条件的显示名称.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// 条件标识符.
    /// </summary>
    public string Id { get; } = id;

    /// <summary>
    /// 备选值列表.
    /// </summary>
    public IList<Condition> Conditions { get; } = conditions;

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is Filter filter && Id == filter.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
