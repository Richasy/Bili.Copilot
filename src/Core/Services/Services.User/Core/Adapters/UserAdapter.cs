// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Richasy.BiliKernel.Adapters;
using Richasy.BiliKernel.Models.User;

namespace Richasy.BiliKernel.Services.User.Core;

internal static class UserAdapter
{
    public static UserDetailProfile ToUserDetailProfile(this MyInfo info, double avatarSize)
    {
        var user = UserAdapterBase.CreateUserProfile(info.Mid, info.Name, info.Avatar, avatarSize);
        return new UserDetailProfile(user, info.Sign, info.Level, info.VIP?.Status == 1);
    }

    public static UserCommunityInformation ToUserCommunityInformation(this Mine mine)
        => new(mine.Mid.ToString(), mine.FollowCount, mine.FansCount, mine.CoinNumber, dynamicCount: mine.DynamicCount);

    public static UserCommunityInformation ToUserCommunityInformation(this RelatedUser user)
    {
        var relation = user.Attribute switch
        {
            <= 1 => UserRelationStatus.Unfollow,
            2 => UserRelationStatus.Following,
            3 => UserRelationStatus.Friends,
            _ => UserRelationStatus.Unknown,
        };

        return new(user.UserId.ToString(), relation: relation);
    }

    public static UserCard ToUserCard(this RelatedUser user)
    {
        var userProfile = UserAdapterBase.CreateUserProfile(user.UserId, user.Name, user.Avatar, 96d);
        var communityInfo = user.ToUserCommunityInformation();
        var profile = new UserDetailProfile(userProfile, user.Sign, default, user.Vip.Status == 1);
        return new UserCard(profile, communityInfo);
    }

    public static PublisherProfile ToPublisherProfile(this PublisherInfo info)
    {
        var user = UserAdapterBase.CreateUserProfile(info.UserId, info.Publisher, info.PublisherAvatar, 48d);
        return new PublisherProfile(user);
    }

    public static UserProfile ToUserProfile(this BiliChatUser user)
        => UserAdapterBase.CreateUserProfile(user.UserId, user.Publisher, user.PublisherAvatar, 64d);

    public static ChatMessage ToChatMessage(this BiliChatMsg msg, IList<ChatEmoteInfo>? emoteInfos)
    {
        var emoteDict = emoteInfos?.ToDictionary(p => p.Text!, p => p.Url.ToImage());
        var message = new ChatMessage();
        message.SenderId = msg.SenderUid.ToString();
        message.Time = DateTimeOffset.FromUnixTimeMilliseconds(msg.Timestamp);
        message.Key = msg.Key;
        var json = JsonNode.Parse(msg.Content);
        if (msg.Type == 1)
        {
            message.Type = BiliKernel.Models.ChatMessageType.Text;
            message.Content = new BiliKernel.Models.Appearance.EmoteText(json["content"]?.GetValue<string>() ?? string.Empty, emoteDict);
        }
        else if (msg.Type == 2)
        {
            message.Type = BiliKernel.Models.ChatMessageType.Image;
            var url = json["url"]?.GetValue<string>() ?? string.Empty;
            message.Content = new BiliKernel.Models.Appearance.EmoteText(default, emoteDict, [url.ToImage()]);
        }
        else
        {
            message.Type = BiliKernel.Models.ChatMessageType.Unknown;
            message.Content = new BiliKernel.Models.Appearance.EmoteText(string.Empty, emoteDict);
        }

        return message;
    }

    public static ChatMessage ToChatMessage(this SendMessageResponse response)
    {
        var emoteDict = response.EmoteInfos?.ToDictionary(p => p.Text!, p => p.Url.ToImage());
        var message = new ChatMessage();
        message.Key = response.Key;
        var json = JsonNode.Parse(response.Content);
        message.Type = BiliKernel.Models.ChatMessageType.Text;
        var content = json["content"]?.GetValue<string>() ?? string.Empty;
        message.Content = new BiliKernel.Models.Appearance.EmoteText(content, emoteDict);
        return message;
    }

    public static ChatSession ToChatSession(this BiliChatSession session, UserProfile user)
    {
        var s = new ChatSession();
        s.User = user;
        if (!string.IsNullOrEmpty(session.LastMessage.Content))
        {
            var json = JsonNode.Parse(session.LastMessage.Content);
            var content = json["content"]?.GetValue<string>() ?? string.Empty;
            s.LastMessage = content;
        }

        if (string.IsNullOrEmpty(s.LastMessage))
        {
            s.LastMessage = "不支持的消息类型";
        }

        s.Time = DateTimeOffset.FromUnixTimeMilliseconds(session.LastMessage.Timestamp);
        s.UnreadCount = session.UnreadCount;
        s.DoNotDisturb = session.DoNotDisturb == 1;
        s.IsFollow = session.IsFollow == 1;
        return s;
    }

    public static IReadOnlyList<NotifyMessage> ToNotifyMessages(this LikeMessageResponse response)
    {
        var items = new List<NotifyMessage>();
        if (response.Latest is not null)
        {
            items = items.Concat(response.Latest.Items.Select(p => p.ToNotifyMessage())).ToList();
        }

        if (response.Total is not null)
        {
            items = items.Concat(response.Total.Items.Select(p => p.ToNotifyMessage())).ToList();
        }

        return items;
    }

    public static IReadOnlyList<NotifyMessage> ToNotifyMessages(this AtMessageResponse response)
    {
        var items = new List<NotifyMessage>();
        if (response.Items is not null)
        {
            items = items.Concat(response.Items.Select(p => p.ToNotifyMessage())).ToList();
        }

        return items;
    }

    public static IReadOnlyList<NotifyMessage> ToNotifyMessages(this ReplyMessageResponse response)
    {
        var items = new List<NotifyMessage>();
        if (response.Items is not null)
        {
            items = items.Concat(response.Items.Select(p => p.ToNotifyMessage())).ToList();
        }

        return items;
    }

    public static NotifyMessage ToNotifyMessage(this LikeMessageItem item)
    {
        var users = item.Users.Select(p => UserAdapterBase.CreateUserProfile(p.UserId, p.UserName, p.Avatar, 48d)).ToList();
        var publishTime = DateTimeOffset.FromUnixTimeMilliseconds(item.LikeTime).ToLocalTime();
        var sourceContent = string.IsNullOrEmpty(item.Item.Title)
            ? item.Item.Description
            : item.Item.Title;
        return new NotifyMessage(item.Id.ToString(), NotifyMessageType.Like, users, publishTime, item.Item.Business, default, sourceContent, item.Item.Uri);
    }

    public static NotifyMessage ToNotifyMessage(this AtMessageItem item)
    {
        var users = new List<UserProfile> { UserAdapterBase.CreateUserProfile(item.User.UserId, item.User.UserName, item.User.Avatar, 48d) };
        var message = item.Item.SourceContent;
        var publishTime = DateTimeOffset.FromUnixTimeMilliseconds(item.AtTime).ToLocalTime();
        var sourceContent = item.Item.Title;
        return new NotifyMessage(item.Id.ToString(), NotifyMessageType.At, users, publishTime, item.Item.Business, message, sourceContent, item.Item.Uri);
    }

    public static NotifyMessage ToNotifyMessage(this ReplyMessageItem item)
    {
        var users = new List<UserProfile> { UserAdapterBase.CreateUserProfile(item.User.UserId, item.User.UserName, item.User.Avatar, 48d) };
        var message = item.Item.SourceContent;
        var publishTime = DateTimeOffset.FromUnixTimeMilliseconds(item.ReplyTime).ToLocalTime();
        var sourceContent = string.IsNullOrEmpty(item.Item.Title) ? item.Item.Description : item.Item.Title;
        return new NotifyMessage(item.Id.ToString(), NotifyMessageType.Reply, users, publishTime, item.Item.Business, message, sourceContent, item.Item.Uri);
    }
}
