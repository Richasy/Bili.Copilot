// Copyright (c) Richasy. All rights reserved.

using System;

namespace Richasy.BiliKernel.Models.User;

/// <summary>
/// 用户分组.
/// </summary>
public sealed class UserGroup
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserGroup"/> class.
    /// </summary>
    /// <param name="id">组标识.</param>
    /// <param name="name">组名.</param>
    /// <param name="totalCount">人数.</param>
    public UserGroup(string? id, string? name, int totalCount)
    {
        Id = id;
        Name = name;
        TotalCount = totalCount;
    }

    /// <summary>
    /// 组Id.
    /// </summary>
    public string? Id { get; }

    /// <summary>
    /// 组名.
    /// </summary>
    public string? Name { get; }

    /// <summary>
    /// 分组下的条目数.
    /// </summary>
    public int TotalCount { get; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is UserGroup group && Id == group.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
