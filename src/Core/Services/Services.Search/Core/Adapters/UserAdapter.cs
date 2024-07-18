// Copyright (c) Richasy. All rights reserved.

using System;
using Bilibili.Polymer.App.Search.V1;
using Richasy.BiliKernel.Adapters;
using Richasy.BiliKernel.Models.User;

namespace Richasy.BiliKernel.Services.Search.Core;

internal static class UserAdapter
{
    public static UserCard ToUserCard(this Item item)
    {
        var user = item.Author;
        if (user is null)
        {
            return default;
        }

        var profile = UserAdapterBase.CreateUserProfile(Convert.ToInt64(item.Param), user.Title, user.Cover, 96);
        var relation = user.Relation.Status switch
        {
            <= 1 => UserRelationStatus.Unfollow,
            2 => UserRelationStatus.Following,
            3 => UserRelationStatus.Friends,
            _ => UserRelationStatus.Unknown,
        };
        var detailProfile = new UserDetailProfile(profile, user.Sign, user.Level, user.Vip.Status == 1, user.IsSeniorMember == 1);
        var communityInfo = new UserCommunityInformation(item.Param, user.Attentions, user.Fans, relation: relation);
        return new UserCard(detailProfile, communityInfo);
    }
}
