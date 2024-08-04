// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
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
}
