// Copyright (c) Richasy. All rights reserved.

using Bilibili.Polymer.App.Search.V1;
using Richasy.BiliKernel.Models.User;

namespace Richasy.BiliKernel.Services.Search.Core;

internal static class UserAdapter
{
    public static UserProfile ToUserProfile(this Item item)
    {
        var userId = item.Param;
        return default;
    }
}
