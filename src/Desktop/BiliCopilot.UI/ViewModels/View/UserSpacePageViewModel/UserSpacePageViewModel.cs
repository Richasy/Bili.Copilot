// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using CommunityToolkit.Mvvm.Input;
using Richasy.BiliKernel.Models.User;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 用户动态页视图模型.
/// </summary>
public sealed partial class UserSpacePageViewModel : ViewModelBase
{
    [RelayCommand]
    private void Initialize(UserProfile profile)
    {
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
}
