﻿// Copyright (c) Richasy. All rights reserved.

using Richasy.BiliKernel.Adapters;
using Richasy.BiliKernel.Models.User;

namespace Richasy.BiliKernel.Services.User.Core;

internal static class UserAdapter
{
    public static UserDetailProfile ToUserDetailProfile(this MyInfo info, double avatarSize)
    {
        var user = CreateUserProfile(info.Mid, info.Name, info.Avatar, avatarSize);
        return new UserDetailProfile(user, info.Sign, info.Level, info.VIP?.Status == 1);
    }

    public static PublisherProfile ToPublisherProfile(this PublisherInfo info)
    {
        var user = CreateUserProfile(info.UserId, info.Publisher, info.PublisherAvatar, 48d);
        return new PublisherProfile(user);
    }

    public static UserCommunityInformation ToUserCommunityInformation(this Mine mine)
        => new(mine.Mid.ToString(), mine.FollowCount, mine.FansCount, mine.CoinNumber, dynamicCount: mine.DynamicCount);

    public static UserCommunityInformation ToUserCommunityInformation(this RelatedUser user)
    {
        var relation = user.Attribute switch
        {
            0 => UserRelationStatus.Unfollow,
            2 => UserRelationStatus.Following,
            3 => UserRelationStatus.Friends,
            _ => UserRelationStatus.Unknown,
        };

        return new(user.UserId.ToString(), relation: relation);
    }

    public static UserProfile CreateUserProfile(long userId, string? userName, string? avatarUrl, double size)
    {
        var image = string.IsNullOrEmpty(avatarUrl) ? default : avatarUrl.ToImage(size);
        return new UserProfile(userId.ToString(), userName, image);
    }

    public static UserCard ToUserCard(this RelatedUser user)
    {
        var userProfile = CreateUserProfile(user.UserId, user.Name, user.Avatar, 96d);
        var communityInfo = user.ToUserCommunityInformation();
        var profile = new UserDetailProfile(userProfile, user.Sign, default, user.Vip.Status == 1);
        return new UserCard(profile, communityInfo);
    }
}
