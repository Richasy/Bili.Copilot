// Copyright (c) Richasy. All rights reserved.

using System;

namespace Richasy.BiliKernel.Models.Appearance;

/// <summary>
/// 键值对式的条件类型.
/// </summary>
/// <param name="name">名称.</param>
/// <param name="id">标识符.</param>
public sealed class Condition(string name, string id)
{
    /// <summary>
    /// 显示的名称.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// 标识符.
    /// </summary>
    public string Id { get; } = id;

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is Condition condition && Id == condition.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);

    /// <inheritdoc/>
    public override string ToString() => Name;
}
