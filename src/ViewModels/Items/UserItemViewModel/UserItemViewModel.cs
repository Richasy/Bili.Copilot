﻿// Copyright (c) Bili Copilot. All rights reserved.

using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Authorize;
using Bili.Copilot.Models.Constants.Community;
using Bili.Copilot.Models.Data.Local;
using Bili.Copilot.Models.Data.User;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 用户条目视图模型.
/// </summary>
public sealed partial class UserItemViewModel : SelectableViewModel<UserProfile>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserItemViewModel"/> class.
    /// </summary>
    /// <param name="information">账户信息.</param>
    public UserItemViewModel(AccountInformation information)
        : base(information.User)
    {
        Introduce = string.IsNullOrEmpty(information.Introduce)
            ? ResourceToolkit.GetLocalizedString(StringNames.NoSelfIntroduce)
            : information.Introduce;
        IsVip = information.IsVip;
        Level = information.Level;
        if (information.CommunityInformation != null)
        {
            var communityInfo = information.CommunityInformation;
            Relation = information.CommunityInformation.Relation;
            FollowCountText = NumberToolkit.GetCountText(communityInfo.FollowCount);
            FansCountText = NumberToolkit.GetCountText(communityInfo.FansCount);
            CoinCountText = NumberToolkit.GetCountText(communityInfo.CoinCount);
            LikeCountText = NumberToolkit.GetCountText(communityInfo.LikeCount);
        }

        IsRelationButtonShown = Relation != UserRelationStatus.Unknown;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserItemViewModel"/> class.
    /// </summary>
    /// <param name="profile">用户资料.</param>
    public UserItemViewModel(RoleProfile profile)
        : base(profile?.User)
    {
        if (profile != null)
        {
            Role = profile.Role;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserItemViewModel"/> class.
    /// </summary>
    public UserItemViewModel(ContactProfile profile)
        : base(profile?.User)
    {
        IsVip = profile.IsVip;
        Level = profile.Level;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserItemViewModel"/> class.
    /// </summary>
    /// <param name="profile">用户资料.</param>
    public UserItemViewModel(UserProfile profile)
        : base(profile)
    {
    }

    [RelayCommand]
    private async Task ToggleRelationAsync()
    {
        if (AuthorizeProvider.Instance.State != AuthorizeState.SignedIn)
        {
            AppViewModel.Instance.ShowTip(ResourceToolkit.GetLocalizedString(StringNames.NeedLoginFirst), InfoType.Warning);
            return;
        }

        bool? isFollow = null;
        if (Relation is UserRelationStatus.Unfollow or UserRelationStatus.BeFollowed)
        {
            // 未关注该用户.
            isFollow = true;
        }
        else if (Relation is UserRelationStatus.Following or UserRelationStatus.Friends)
        {
            isFollow = false;
        }

        var result = await AccountProvider.ModifyUserRelationAsync(Data.Id, isFollow.Value);
        if (result)
        {
            var relation = await AccountProvider.GetRelationAsync(Data.Id);
            Relation = relation;
        }
    }

    [RelayCommand]
    private async Task InitializeRelationAsync()
    {
        if (AuthorizeProvider.Instance.State != AuthorizeState.SignedIn)
        {
            IsRelationButtonShown = false;
            return;
        }

        var relation = await AccountProvider.GetRelationAsync(Data.Id);
        Relation = relation;
        IsRelationButtonShown = Relation != UserRelationStatus.Unknown;
    }

    [RelayCommand]
    private void ShowDetail()
        => AppViewModel.Instance.ShowUserDetailCommand.Execute(Data);

    [RelayCommand]
    private void Fix()
    {
        FixModuleViewModel.Instance.AddFixedItemCommand.Execute(new FixedItem(
            Data.Avatar.Uri,
            Data.Name,
            Data.Id,
            FixedType.User));
    }
}
