﻿// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System;
using Richasy.BiliKernel.Models.User;

namespace Richasy.BiliKernel.Models.Moment;

/// <summary>
/// 动态 UP 主.
/// </summary>
public sealed class MomentProfile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MomentProfile"/> class.
    /// </summary>
    public MomentProfile(UserProfile profile, bool isUnread, bool isLiving, string? liveRoomId)
    {
        User = profile;
        IsUnread = isUnread;
        IsLiving = isLiving;
        LiveRoomId = liveRoomId;
    }

    /// <summary>
    /// 用户 Id.
    /// </summary>
    public UserProfile User { get; }

    /// <summary>
    /// 是否未读.
    /// </summary>
    public bool IsUnread { get; }

    /// <summary>
    /// 是否正在直播.
    /// </summary>
    public bool IsLiving { get; }

    /// <summary>
    /// 直播间 Id.
    /// </summary>
    public string? LiveRoomId { get; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is MomentProfile uper && EqualityComparer<UserProfile>.Default.Equals(User, uper.User);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(User);
}
