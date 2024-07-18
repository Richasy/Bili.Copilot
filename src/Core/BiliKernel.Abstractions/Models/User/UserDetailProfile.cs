// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;

namespace Richasy.BiliKernel.Models.User;

/// <summary>
/// 账户详细信息.
/// </summary>
/// <remarks>
/// 区别于 <see cref="UserProfile"/>，账户信息包含更多的信息，一般用于显示用户详情时展示完整的用户资料.
/// </remarks>
public sealed class UserDetailProfile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserDetailProfile"/> class.
    /// </summary>
    /// <param name="user">用户资料.</param>
    public UserDetailProfile(UserProfile user) => User = user;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserDetailProfile"/> class.
    /// </summary>
    /// <param name="user">用户资料.</param>
    /// <param name="intro">自我介绍或签名.</param>
    /// <param name="level">等级.</param>
    /// <param name="isVip">是否为高级会员.</param>
    /// <param name="isHardcore">是否为硬核会员.</param>
    public UserDetailProfile(
        UserProfile user,
        string? intro,
        int? level,
        bool? isVip,
        bool? isHardcore = default)
        : this(user)
    {
        Introduce = intro;
        Level = level;
        IsVip = isVip;
        IsHardcore = isHardcore;
    }

    /// <summary>
    /// 用户资料.
    /// </summary>
    public UserProfile User { get; }

    /// <summary>
    /// 个人介绍.
    /// </summary>
    public string? Introduce { get; }

    /// <summary>
    /// 账户等级.
    /// </summary>
    public int? Level { get; }

    /// <summary>
    /// 是否为大会员.
    /// </summary>
    public bool? IsVip { get; }

    /// <summary>
    /// 是否为硬核会员.
    /// </summary>
    public bool? IsHardcore { get; }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is UserDetailProfile profile && EqualityComparer<UserProfile>.Default.Equals(User, profile.User);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(User);
}
