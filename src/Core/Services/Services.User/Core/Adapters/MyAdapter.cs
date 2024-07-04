// Copyright (c) Richasy. All rights reserved.

using Richasy.BiliKernel.Adapters;
using Richasy.BiliKernel.Models.User;

namespace Richasy.BiliKernel.Services.User.Core;

internal static class MyAdapter
{
    public static UserDetailProfile ToUserDetailProfile(this MyInfo info, double avatarSize)
    {
        var user = CreateUserProfile(info.Mid, info.Name, info.Avatar, avatarSize);
        return new UserDetailProfile(user, info.Sign, info.Level, info.VIP?.Status == 1);
    }

    public static UserProfile CreateUserProfile(long userId, string? userName, string? avatarUrl, double size)
    {
        var image = string.IsNullOrEmpty(avatarUrl) ? default : avatarUrl.ConvertToImage(size);
        return new UserProfile(userId.ToString(), userName, image);
    }
}
