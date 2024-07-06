// Copyright (c) Richasy. All rights reserved.

using System;
using Bilibili.App.Interface.V1;
using Richasy.BiliKernel.Adapters;
using Richasy.BiliKernel.Models.Media;
using Richasy.BiliKernel.Models.User;

namespace Richasy.BiliKernel.Services.Media.Core;

internal static class LiveAdapter
{
    public static LiveInformation ToLiveInformation(this CursorItem item)
    {
        var live = item.CardLive;
        var title = item.Title;
        var viewTime = DateTimeOffset.FromUnixTimeSeconds(item.ViewAt).ToLocalTime();
        var roomId = item.Kid.ToString();
        var cover = live.Cover.ToVideoCover();
        var identifier = new VideoIdentifier(roomId, title, default, cover);
        var user = UserAdapterBase.CreateUserProfile(live.Mid, live.Name, default, 0d);
        var relation = live.Relation.Status switch
        {
            1 => UserRelationStatus.Unfollow,
            2 => UserRelationStatus.Following,
            3 => UserRelationStatus.BeFollowed,
            4 => UserRelationStatus.Friends,
            _ => UserRelationStatus.Unknown,
        };
        var info = new LiveInformation(identifier, user, relation);
        info.AddExtensionIfNotNull(LiveExtensionDataId.CollectTime, viewTime);
        info.AddExtensionIfNotNull(LiveExtensionDataId.TagName, live.Tag);
        info.AddExtensionIfNotNull(LiveExtensionDataId.IsLiving, live.Ststus != 0);
        return info;
    }
}
