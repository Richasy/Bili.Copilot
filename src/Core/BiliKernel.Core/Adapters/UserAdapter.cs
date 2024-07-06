// Copyright (c) Richasy. All rights reserved.

using Richasy.BiliKernel.Models.User;

namespace Richasy.BiliKernel.Adapters;

/// <summary>
/// 用户类型适配器.
/// </summary>
public static class UserAdapterBase
{
    /// <summary>
    /// 创建用户资料.
    /// </summary>
    public static UserProfile CreateUserProfile(long userId, string? userName, string? avatarUrl, double size)
    {
        var image = string.IsNullOrEmpty(avatarUrl) ? default : avatarUrl.ToImage(size);
        return new UserProfile(userId.ToString(), userName, image);
    }
}
