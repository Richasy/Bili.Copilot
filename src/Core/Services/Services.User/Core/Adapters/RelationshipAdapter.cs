// Copyright (c) Richasy. All rights reserved.

using Richasy.BiliKernel.Models.User;

namespace Richasy.BiliKernel.Services.User.Core;

internal static class RelationshipAdapter
{
    public static UserGroup ToUserGroup(this RelatedTag tag)
        => new UserGroup(tag.TagId.ToString(), tag.Name, tag.Count);
}
