// Copyright (c) Richasy. All rights reserved.

using System;
using System.Linq;
using Bilibili.App.Dynamic.V2;
using Richasy.BiliKernel.Adapters;
using Richasy.BiliKernel.Models.Moment;

namespace Richasy.BiliKernel.Services.Moment.Core;

internal static class UserAdapter
{
    public static MomentProfile ToMomentProfile(this UpListItem item)
    {
        var user = UserAdapterBase.CreateUserProfile(item.Uid, item.Name, item.Face, 48d);
        var liveRoomId = item.Uri.Contains("live.bilibili.com") ? new Uri(item.Uri).Segments.Last() : default;
        return new MomentProfile(user, item.HasUpdate, item.LiveState == LiveState.LiveLive, liveRoomId);
    }
}
