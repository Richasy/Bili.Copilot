// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Models.User;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 用户项视图模型.
/// </summary>
public sealed partial class UserItemViewModel : ViewModelBase<UserCard>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserItemViewModel"/> class.
    /// </summary>
    public UserItemViewModel(UserCard data)
        : base(data)
    {
        Id = data.Profile.User.Id;
        Name = data.Profile.User.Name;
        Avatar = data.Profile.User.Avatar.Uri;
        Description = string.IsNullOrEmpty(data.Profile.Introduce) ? ResourceToolkit.GetLocalizedString(StringNames.NoSelfIntroduce) : data.Profile.Introduce;
        IsFollowed = data.Community.Relation != UserRelationStatus.Unknown && data.Community.Relation != UserRelationStatus.Unfollow;
        IsVip = data.Profile.IsVip ?? false;
        Level = data.Profile.Level;
    }

    [RelayCommand]
    private void ShowUserSpace()
        => this.Get<NavigationViewModel>().NavigateToOver(typeof(UserSpacePage), Data.Profile.User);

    [RelayCommand]
    private void Pin()
    {
        var pinItem = new PinItem(Data.Profile.User.Id, Data.Profile.User.Name, Data.Profile.User.Avatar.Uri.ToString(), PinContentType.User);
        this.Get<PinnerViewModel>().AddItemCommand.Execute(pinItem);
    }

    [RelayCommand]
    private async Task ToggleFollowAsync()
    {
        var relationService = this.Get<IRelationshipService>();
        if (IsFollowed)
        {
            try
            {
                await relationService.UnfollowUserAsync(Id);
                IsFollowed = false;
                this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.Unfollowed), InfoType.Success));
            }
            catch (Exception ex)
            {
                this.Get<ILogger<UserItemViewModel>>().LogError(ex, "取消关注用户时失败");
                this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.FailedToUnfollowUser), InfoType.Error));
            }
        }
        else
        {
            try
            {
                await relationService.FollowUserAsync(Id);
                IsFollowed = true;
                this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.Followed), InfoType.Success));
            }
            catch (Exception ex)
            {
                this.Get<ILogger<UserItemViewModel>>().LogError(ex, "关注用户时失败");
                this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.FailedToFollowUser), InfoType.Error));
            }
        }
    }
}
