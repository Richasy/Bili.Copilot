// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.Data.User;

/// <summary>
/// 动态 UP 主.
/// </summary>
public sealed class DynamicUper
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicUper"/> class.
    /// </summary>
    public DynamicUper(UserProfile profile, bool isUnread)
    {
        User = profile;
        IsUnread = isUnread;
    }

    /// <summary>
    /// 用户 Id.
    /// </summary>
    public UserProfile User { get; }

    /// <summary>
    /// 是否未读.
    /// </summary>
    public bool IsUnread { get; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is DynamicUper uper && EqualityComparer<UserProfile>.Default.Equals(User, uper.User);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(User);
}
