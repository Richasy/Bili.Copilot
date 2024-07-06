// Copyright (c) Richasy. All rights reserved.

namespace Richasy.BiliKernel.Models.User;

/// <summary>
/// 用户之间的关系状态.
/// </summary>
public enum UserRelationStatus
{
    /// <summary>
    /// 未知状态.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// 你未关注 TA.
    /// </summary>
    Unfollow = 1,

    /// <summary>
    /// 你正在关注 TA.
    /// </summary>
    Following = 2,

    /// <summary>
    /// 你被 TA 关注.
    /// </summary>
    BeFollowed = 3,

    /// <summary>
    /// 你和 TA 互相关注，也许是好友.
    /// </summary>
    Friends = 4,

    /// <summary>
    /// 你特别关注了 TA.
    /// </summary>
    SpeciallyFollowed = 100,

    /// <summary>
    /// 你已将 TA 拉黑.
    /// </summary>
    Blocked = 128,
}
