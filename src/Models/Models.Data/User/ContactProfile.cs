// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.Data.User;

/// <summary>
/// 联系人信息.
/// </summary>
public sealed class ContactProfile
{
    /// <summary>
    /// 用户信息.
    /// </summary>
    public UserProfile User { get; set; }

    /// <summary>
    /// 用户等级.
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// 是否为大会员.
    /// </summary>
    public bool IsVip { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is ContactProfile profile && EqualityComparer<UserProfile>.Default.Equals(User, profile.User);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(User);
}
