// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Linq;
using System.Text.RegularExpressions;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.BiliBili;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.User;
using Bilibili.App.Archive.V1;
using Bilibili.App.View.V1;
using Bilibili.Main.Community.Reply.V1;

namespace Bili.Copilot.Libs.Adapter;

/// <summary>
/// 用户资料适配器，将来自 BiliBili 的用户数据转换为 <see cref="UserProfile"/> , <see cref="RoleProfile"/> 或 <see cref="AccountInformation"/>.
/// </summary>
public sealed class UserAdapter
{
    /// <summary>
    /// 将个人信息 <see cref="MyInfo"/> 转换为 <see cref="AccountInformation"/>.
    /// </summary>
    /// <param name="myInfo">我的资料.</param>
    /// <param name="avatarSize">头像尺寸.</param>
    /// <returns><see cref="AccountInformation"/>.</returns>
    public static AccountInformation ConvertToAccountInformation(MyInfo myInfo, AvatarSize avatarSize)
    {
        var user = ConvertToUserProfile(myInfo.Mid, myInfo.Name, myInfo.Avatar, avatarSize);
        var communityInfo = CommunityAdapter.ConvertToUserCommunityInformation(myInfo);
        return new AccountInformation(
            user,
            myInfo.Sign,
            myInfo.Level,
            myInfo.VIP.Status == 1,
            communityInfo);
    }

    /// <summary>
    /// 将个人信息 <see cref="Mine"/> 转换为 <see cref="AccountInformation"/>.
    /// </summary>
    /// <param name="myInfo">我的资料.</param>
    /// <param name="avatarSize">头像尺寸.</param>
    /// <returns><see cref="AccountInformation"/>.</returns>
    public static AccountInformation ConvertToAccountInformation(Mine myInfo, AvatarSize avatarSize)
    {
        var user = ConvertToUserProfile(myInfo.Mid, myInfo.Name, myInfo.Avatar, avatarSize);
        var communityInfo = CommunityAdapter.ConvertToUserCommunityInformation(myInfo);
        return new AccountInformation(
            user,
            string.Empty,
            myInfo.Level,
            false,
            communityInfo);
    }

    /// <summary>
    /// 将用户空间资料 <see cref="UserSpaceInformation"/> 转换为 <see cref="AccountInformation"/>.
    /// </summary>
    /// <param name="spaceInfo">用户空间资料.</param>
    /// <param name="avatarSize">头像尺寸.</param>
    /// <returns><see cref="AccountInformation"/>.</returns>
    public static AccountInformation ConvertToAccountInformation(UserSpaceInformation spaceInfo, AvatarSize avatarSize)
    {
        var user = ConvertToUserProfile(Convert.ToInt64(spaceInfo.UserId), spaceInfo.UserName, spaceInfo.Avatar, avatarSize);
        var communityInfo = CommunityAdapter.ConvertToUserCommunityInformation(spaceInfo);
        return new AccountInformation(
            user,
            spaceInfo.Sign,
            spaceInfo.LevelInformation.CurrentLevel,
            spaceInfo.Vip.Status == 1,
            communityInfo);
    }

    /// <summary>
    /// 将关系用户信息 <see cref="RelatedUser"/> 转换为 <see cref="AccountInformation"/>.
    /// </summary>
    /// <param name="user">关系用户信息.</param>
    /// <param name="avatarSize">头像大小.</param>
    /// <returns><see cref="AccountInformation"/>.</returns>
    public static AccountInformation ConvertToAccountInformation(RelatedUser user, AvatarSize avatarSize = AvatarSize.Size64)
    {
        var profile = ConvertToUserProfile(user.Mid, user.Name, user.Avatar, avatarSize);
        var communityInfo = CommunityAdapter.ConvertToUserCommunityInformation(user);
        return new AccountInformation(
            profile,
            user.Sign,
            -1,
            user.Vip.Status == 1,
            communityInfo);
    }

    /// <summary>
    /// 将用户搜索条目 <see cref="UserSearchItem"/> 转换为 <see cref="AccountInformation"/>.
    /// </summary>
    /// <param name="item">用户搜索条目.</param>
    /// <param name="avatarSize">头像尺寸.</param>
    /// <returns><see cref="AccountInformation"/>.</returns>
    public static AccountInformation ConvertToAccountInformation(UserSearchItem item, AvatarSize avatarSize = AvatarSize.Size64)
    {
        var profile = ConvertToUserProfile(item.UserId, Regex.Replace(item.Title, "<[^>]+>", string.Empty), item.Cover, avatarSize);
        var communityInfo = CommunityAdapter.ConvertToUserCommunityInformation(item);
        return new AccountInformation(
            profile,
            item.Sign,
            item.Level,
            item.Vip.Status == 1,
            communityInfo);
    }

    /// <summary>
    /// 将用户信息 <see cref="Member"/> 转换为 <see cref="AccountInformation"/>.
    /// </summary>
    /// <param name="member">用户信息.</param>
    /// <param name="avatarSize">头像尺寸.</param>
    /// <returns><see cref="AccountInformation"/>.</returns>
    public static AccountInformation ConvertToAccountInformation(Member member, AvatarSize avatarSize = AvatarSize.Size64)
    {
        var profile = ConvertToUserProfile(member.Mid, member.Name, member.Face, avatarSize);
        return new AccountInformation(profile, default, Convert.ToInt32(member.Level), default);
    }

    /// <summary>
    /// 将 <see cref="PublisherInfo"/> 转换为发布者资料.
    /// </summary>
    /// <param name="publisher">BiliBili的视频发布者信息.</param>
    /// <param name="avatarSize">头像尺寸.</param>
    /// <returns><see cref="RoleProfile"/>.</returns>
    public static RoleProfile ConvertToRoleProfile(PublisherInfo publisher, AvatarSize avatarSize)
    {
        var user = ConvertToUserProfile(publisher.Mid, publisher.Publisher, publisher.PublisherAvatar, avatarSize);
        return new RoleProfile(user);
    }

    /// <summary>
    /// 将视频合作者信息 <see cref="Staff"/> 转换为发布者资料.
    /// </summary>
    /// <param name="staff">视频合作者信息.</param>
    /// <param name="avatarSize">头像尺寸.</param>
    /// <returns><see cref="RoleProfile"/>.</returns>
    public static RoleProfile ConvertToRoleProfile(Staff staff, AvatarSize avatarSize)
    {
        var user = ConvertToUserProfile(staff.Mid, staff.Name, staff.Face, avatarSize);
        return new RoleProfile(
            user,
            TextToolkit.ConvertToTraditionalChineseIfNeeded(staff.Title));
    }

    /// <summary>
    /// 将推荐卡片的头像信息 <see cref="RecommendAvatar"/> 转换为角色资料.
    /// </summary>
    /// <param name="avatar">推荐卡片的头像信息.</param>
    /// <param name="avatarSize">头像尺寸.</param>
    /// <returns><see cref="RoleProfile"/>.</returns>
    public static RoleProfile ConvertToRoleProfile(RecommendAvatar avatar, AvatarSize avatarSize = AvatarSize.Size48)
    {
        var user = ConvertToUserProfile(avatar.UserId, avatar.UserName, avatar.Cover, avatarSize);
        return new RoleProfile(user);
    }

    /// <summary>
    /// 将作者信息 <see cref="Author"/> 转换为发布者资料.
    /// </summary>
    /// <param name="author">作者信息.</param>
    /// <param name="avatarSize">头像大小.</param>
    /// <returns><see cref="RoleProfile"/>.</returns>
    public static RoleProfile ConvertToRoleProfile(Author author, AvatarSize avatarSize = AvatarSize.Size32)
    {
        var user = ConvertToUserProfile(author.Mid, author.Name, author.Face, avatarSize);
        return new RoleProfile(user);
    }

    /// <summary>
    /// 将明星信息 <see cref="PgcCelebrity"/> 转换为角色资料.
    /// </summary>
    /// <param name="celebrity">明星信息.</param>
    /// <param name="avatarSize">头像大小.</param>
    /// <returns><see cref="RoleProfile"/>.</returns>
    public static RoleProfile ConvertToRoleProfile(PgcCelebrity celebrity, AvatarSize avatarSize = AvatarSize.Size96)
    {
        var user = ConvertToUserProfile(celebrity.Id, celebrity.Name, celebrity.Avatar, avatarSize);
        return new RoleProfile(
            user,
            TextToolkit.ConvertToTraditionalChineseIfNeeded(celebrity.ShortDescription));
    }

    /// <summary>
    /// 将数据整合为用户资料.
    /// </summary>
    /// <param name="userId">用户Id.</param>
    /// <param name="userName">用户名.</param>
    /// <param name="avatar">封面.</param>
    /// <param name="avatarSize">头像尺寸.</param>
    /// <returns><see cref="UserProfile"/>.</returns>
    public static UserProfile ConvertToUserProfile(long userId, string userName, string avatar, AvatarSize avatarSize)
    {
        var size = int.Parse(avatarSize.ToString().Replace("Size", string.Empty));
        var image = string.IsNullOrEmpty(avatar)
            ? default
            : ImageAdapter.ConvertToImage(avatar, size, size);
        var profile = new UserProfile(userId.ToString(), userName, image);
        return profile;
    }

    /// <summary>
    /// 将关系用户响应结果 <see cref="UserRelationResponse"/> 转换为 <see cref="RelationView"/>.
    /// </summary>
    /// <param name="response">关系用户响应结果.</param>
    /// <returns><see cref="RelationView"/>.</returns>
    public static RelationView ConvertToRelationView(RelatedUserResponse response)
    {
        var count = response.TotalCount;
        var accounts = response.UserList.Select(p => ConvertToAccountInformation(p)).ToList();
        return new RelationView(accounts, count);
    }
}
