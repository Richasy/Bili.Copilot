﻿// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Richasy.BiliKernel.Bili.Moment;
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
        IMomentDiscoveryService discoveryService)
    {
        _momentDiscoveryService = discoveryService;
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (Sections?.Count > 0)
        {
            return;
        }

        Sections = new List<IMomentSectionDetailViewModel>
        {
            new VideoMomentSectionDetailViewModel(_momentDiscoveryService),
        };

        SelectSectionCommand.Execute(Sections.First());
        await Task.Delay(200);
        Initialized?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void SelectSection(IMomentSectionDetailViewModel section)
    {
        if (section is null || section == SelectedSection)
        {
            return;
        }

        SelectedSection = section;
        SelectedSection.InitializeCommand.Execute(default);
    }
}