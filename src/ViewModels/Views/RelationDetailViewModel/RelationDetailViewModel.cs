// Copyright (c) Bili Copilot. All rights reserved.

using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.User;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 关系用户页面视图模型基类，是粉丝/关注页面的基础.
/// </summary>
public abstract partial class RelationDetailViewModel : InformationFlowViewModel<UserItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RelationDetailViewModel"/> class.
    /// </summary>
    internal RelationDetailViewModel(RelationType type)
    {
        _relationType = type;

        TitleSuffix = type switch
        {
            RelationType.Follows => ResourceToolkit.GetLocalizedString(StringNames.FollowsSuffix),
            _ => ResourceToolkit.GetLocalizedString(StringNames.FansSuffix)
        };
    }

    /// <summary>
    /// 设置用户资料.
    /// </summary>
    /// <param name="profile">用户资料.</param>
    public void SetProfile(UserProfile profile)
    {
        if (Profile?.Id == profile.Id)
        {
            return;
        }

        Profile = profile;
        TryClear(Items);
        BeforeReload();
    }

    /// <inheritdoc/>
    protected override void BeforeReload()
    {
        _isEnd = false;
        IsEmpty = false;
        AccountProvider.Instance.ResetRelationStatus(_relationType);
    }

    /// <inheritdoc/>
    protected override async Task GetDataAsync()
    {
        if (_isEnd)
        {
            return;
        }

        var data = await AccountProvider.Instance.GetUserFansOrFollowsAsync(Profile.Id, _relationType);
        foreach (var item in data.Accounts)
        {
            var userVM = new UserItemViewModel(item);
            Items.Add(userVM);
        }

        _isEnd = Items.Count == data.TotalCount || Items.Count > 200;
        IsEmpty = Items.Count == 0;
    }

    /// <inheritdoc/>
    protected override string FormatException(string errorMsg)
    {
        var prefix = _relationType == RelationType.Follows
            ? ResourceToolkit.GetLocalizedString(StringNames.RequestFollowsFailed)
            : ResourceToolkit.GetLocalizedString(StringNames.RequestFansFailed);
        return $"{prefix}\n{errorMsg}";
    }
}
