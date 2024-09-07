// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Models.User;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 用户动态页视图模型.
/// </summary>
public sealed partial class UserSpacePageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserSpacePageViewModel"/> class.
    /// </summary>
    public UserSpacePageViewModel(
        CommentMainViewModel comment,
        IRelationshipService relationshipService,
        ILogger<UserSpacePageViewModel> logger)
    {
        CommentModule = comment;
        _relationshipService = relationshipService;
        _logger = logger;
    }

    [RelayCommand]
    private async Task InitializeAsync(UserProfile profile)
    {
        _profile = profile;
        UserName = profile.Name;
        if (Sections is null)
        {
            Sections = new List<UserMomentDetailViewModel>
            {
                this.Get<UserMomentDetailViewModel>(),
                this.Get<UserMomentDetailViewModel>(),
            };

            Sections.ElementAt(0).SetIsVideo(true);
            Sections.ElementAt(1).SetIsVideo(false);
        }

        foreach (var item in Sections)
        {
            item.SetShowCommentAction(ShowComment);
            item.ResetCommand.Execute(profile);
        }

        if (SelectedSection is null)
        {
            var isLastVideoSection = SettingsToolkit.ReadLocalSetting(SettingNames.LastUserSpaceSectionIsVideo, true);
            var section = isLastVideoSection ? Sections.First() : Sections.Last();
            SelectSection(section);
        }
        else
        {
            SelectedSection.InitializeCommand.Execute(default);
        }

        await InitializeRelationAsync();

        Initialized?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private async Task RefreshAsync()
        => await SelectedSection?.RefreshCommand.ExecuteAsync(default);

    [RelayCommand]
    private void SelectSection(UserMomentDetailViewModel section)
    {
        if (section is null || section == SelectedSection)
        {
            return;
        }

        SelectedSection = section;
        SettingsToolkit.WriteLocalSetting(SettingNames.LastUserSpaceSectionIsVideo, SelectedSection.IsVideo());
        SelectedSection.InitializeCommand.Execute(default);
    }

    [RelayCommand]
    private async Task ToggleFollowAsync()
    {
        if (IsFollowed)
        {
            try
            {
                await _relationshipService.UnfollowUserAsync(_profile.Id);
                IsFollowed = false;
                this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.Unfollowed), InfoType.Success));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取消关注用户时失败");
                this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.FailedToUnfollowUser), InfoType.Error));
            }
        }
        else
        {
            try
            {
                await _relationshipService.FollowUserAsync(_profile.Id);
                IsFollowed = true;
                this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.Followed), InfoType.Success));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "关注用户时失败");
                this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.FailedToFollowUser), InfoType.Error));
            }
        }
    }

    private async Task InitializeRelationAsync()
    {
        try
        {
            var state = await _relationshipService.GetRelationshipAsync(_profile.Id);
            IsFollowed = state != UserRelationStatus.Unknown && state != UserRelationStatus.Unfollow;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取用户关系时失败");
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.FailedToGetUserRelation), InfoType.Error));
        }
    }

    private void ShowComment(MomentItemViewModel data)
    {
        var moment = data.Data;
        if (CommentModule.Id == moment.CommentId)
        {
            return;
        }

        IsCommentsOpened = true;
        CommentModule.Initialize(moment.CommentId, moment.CommentType!.Value, Richasy.BiliKernel.Models.CommentSortType.Hot);
        CommentModule.RefreshCommand.Execute(default);
    }
}
