// Copyright (c) Richasy. All rights reserved.

using System.Linq;
using Richasy.BiliKernel.Adapters;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;

namespace Richasy.BiliKernel.Services.Media.Core;

internal static class LiveAdapter
{
    public static LiveInformation ToLiveInformation(this LiveRoomCard card)
    {
        var title = card.Title;
        var roomId = card.RoomId.ToString();
        var user = UserAdapterBase.CreateUserProfile(card.UserId ?? 0, card.UpName, default, 0d);
        var viewerCount = VideoAdapter.GetCountNumber(card.CoverRightContent.Text);
        var subtitle = card.CoverLeftContent.Text;
        var cover = card.Cover.ToVideoCover();
        var identifier = new VideoIdentifier(roomId, title, cover);
        var info = new LiveInformation(identifier, user, default);
        info.AddExtensionIfNotNull(LiveExtensionDataId.ViewerCount, viewerCount);
        info.AddExtensionIfNotNull(LiveExtensionDataId.Subtitle, subtitle);
        return info;
    }

    public static LiveInformation ToLiveInformation(this LiveFeedRoom room)
    {
        var title = room.Title;
        var roomId = room.RoomId.ToString();
        var viewerCount = room.ViewerCount;
        var user = UserAdapterBase.CreateUserProfile(room.UserId ?? 0, room.UserName, room.UserAvatar, 48d);
        var cover = room.Cover.ToVideoCover();
        var subtitle = $"{room.AreaName} · {room.UserName}";
        var identifier = new VideoIdentifier(roomId, title, cover);
        var info = new LiveInformation(identifier, user);
        info.AddExtensionIfNotNull(LiveExtensionDataId.ViewerCount, viewerCount);
        info.AddExtensionIfNotNull(LiveExtensionDataId.Subtitle, subtitle);
        return info;
    }

    public static Partition ToPartition(this LiveAreaGroup group)
    {
        var id = group.Id.ToString();
        var name = group.Name;
        var children = group.AreaList.Select(p => p.ToPartition()).ToList();
        return new Partition(id, name, children: children);
    }

    public static Partition ToPartition(this LiveArea area)
    {
        var id = area.Id.ToString();
        var name = area.Name;
        var logo = string.IsNullOrEmpty(area.Cover) ? default : area.Cover.ToImage();
        return new Partition(id, name, logo, parentId: area.ParentId.ToString());
    }

    public static LiveTag ToLiveTag(this LiveAreaDetailTag tag)
    {
        var id = tag.Id.ToString();
        var name = tag.Name;
        var sortType = tag.SortType;
        return new LiveTag(id, name, sortType);
    }
}
