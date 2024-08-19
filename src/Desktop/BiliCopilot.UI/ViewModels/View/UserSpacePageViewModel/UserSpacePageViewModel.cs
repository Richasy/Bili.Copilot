// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
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
    public UserSpacePageViewModel(CommentMainViewModel comment)
    {
        CommentModule = comment;
    }

    [RelayCommand]
    private void Initialize(UserProfile profile)
    {
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
            SelectSection(Sections.First());
        }
        else
        {
            SelectedSection.InitializeCommand.Execute(default);
        }

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
        SelectedSection.InitializeCommand.Execute(default);
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
