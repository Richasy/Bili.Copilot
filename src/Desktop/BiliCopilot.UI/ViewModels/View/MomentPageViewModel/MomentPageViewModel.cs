// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Richasy.BiliKernel.Bili.Moment;
using Richasy.BiliKernel.Models.Moment;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 动态页面视图模型.
/// </summary>
public sealed partial class MomentPageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MomentPageViewModel"/> class.
    /// </summary>
    public MomentPageViewModel(
        IMomentDiscoveryService discoveryService,
        CommentMainViewModel comment)
    {
        _momentDiscoveryService = discoveryService;
        CommentModule = comment;
    }

    [RelayCommand]
    private void Initialize()
    {
        if (Sections?.Count > 0)
        {
            return;
        }

        Sections = new List<IMomentSectionDetailViewModel>
        {
            new VideoMomentSectionDetailViewModel(_momentDiscoveryService),
            new ComprehensiveMomentSectionDetailViewModel(_momentDiscoveryService),
        };

        var isLastVideoSection = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.LastMomentSectionIsVideo, true);
        var section = isLastVideoSection ? Sections.First() : Sections.Last();
        SelectSectionCommand.Execute(section);
        this.Get<DispatcherQueue>().TryEnqueue(DispatcherQueuePriority.Low, () => Initialized?.Invoke(this, EventArgs.Empty));
    }

    [RelayCommand]
    private void SelectSection(IMomentSectionDetailViewModel section)
    {
        if (section is null || section == SelectedSection)
        {
            return;
        }

        SelectedSection = section;
        var isVideo = section is VideoMomentSectionDetailViewModel;
        SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.LastMomentSectionIsVideo, isVideo);
        SelectedSection.InitializeCommand.Execute(default);
    }

    [RelayCommand]
    private void ShowComment(MomentInformation data)
    {
        IsCommentsOpened = true;
        if (CommentModule.Id == data.CommentId)
        {
            return;
        }

        CommentModule.Initialize(data.CommentId, data.CommentType!.Value, Richasy.BiliKernel.Models.CommentSortType.Hot);
        CommentModule.RefreshCommand.Execute(default);
    }
}
