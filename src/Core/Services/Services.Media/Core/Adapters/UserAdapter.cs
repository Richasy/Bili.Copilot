// Copyright (c) Richasy. All rights reserved.

using Richasy.BiliKernel.Adapters;
using Richasy.BiliKernel.Models.User;

namespace Richasy.BiliKernel.Services.Media.Core;

internal static class UserAdapter
{
    public static PublisherProfile ToPublisherProfile(this PublisherInfo info)
    {
        var user = UserAdapterBase.CreateUserProfile(info.UserId, info.Publisher, info.PublisherAvatar, 48d);
        return new PublisherProfile(user);
    }
}
