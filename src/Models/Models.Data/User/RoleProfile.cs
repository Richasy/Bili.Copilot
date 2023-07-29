﻿// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.Data.User;

/// <summary>
/// 角色信息，可以是视频发布者，也可以是参演人员.
/// </summary>
/// <remarks>
/// 该类是 <see cref="UserProfile"/> 的包装，增加了 <see cref="Role"/> 属性表示在视频中的职能.
/// </remarks>
public sealed class RoleProfile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RoleProfile"/> class.
    /// </summary>
    public RoleProfile()
        => Role = "Publisher";

    /// <summary>
    /// Initializes a new instance of the <see cref="RoleProfile"/> class.
    /// </summary>
    /// <param name="user">用户信息.</param>
    public RoleProfile(UserProfile user)
        : this() => User = user;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoleProfile"/> class.
    /// </summary>
    /// <param name="user">用户信息.</param>
    /// <param name="role">角色.</param>
    public RoleProfile(UserProfile user, string role)
        : this(user) => Role = role;

    /// <summary>
    /// 用户信息.
    /// </summary>
    public UserProfile User { get; set; }

    /// <summary>
    /// 视频中所扮演的角色.
    /// </summary>
    /// <remarks>
    /// 通常来说，对于视频发布者独立制作的视频，该属性默认为 Publisher.
    /// 但是当该视频为多人合作发布或为 PGC 内容时，不同的制作者在视频制作期间担任的角色不同，这里的属性可以用以区分.
    /// </remarks>
    public string Role { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is RoleProfile profile && EqualityComparer<UserProfile>.Default.Equals(User, profile.User);

    /// <inheritdoc/>
    public override int GetHashCode() => User.GetHashCode();
}
