// Copyright (c) Richasy. All rights reserved.

using Richasy.BiliKernel.Models.User;

namespace Richasy.BiliKernel.Services.User.Core;

internal static class RelationshipAdapter
{
    public static UserGroup ToUserGroup(this RelatedTag tag)
        => new UserGroup(tag.TagId.ToString(), tag.Name, tag.Count);

    public static UserRelationStatus ToUserRelationStatus(this UserRelationResponse response)
    {
        return response.IsSpecialFollow is 1
            ? UserRelationStatus.SpeciallyFollowed
            : response.Type switch
                {
                    2 => UserRelationStatus.Following,
                    6 => UserRelationStatus.Friends,
                    0 => UserRelationStatus.Unfollow,
                    128 => UserRelationStatus.Blocked,
                    _ => UserRelationStatus.Unknown,
                };
    }
}
