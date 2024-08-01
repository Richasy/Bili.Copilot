// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 动漫页面视图模型.
/// </summary>
public sealed partial class AnimePageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnimePageViewModel"/> class.
    /// </summary>
    public AnimePageViewModel(
        IEntertainmentDiscoveryService discoveryService,
        ILogger<AnimePageViewModel> logger)
    {
        _service = discoveryService;
        _logger = logger;
        NavColumnWidth = SettingsToolkit.ReadLocalSetting(SettingNames.AnimePageNavColumnWidth, 240d);
        IsNavColumnManualHide = SettingsToolkit.ReadLocalSetting(SettingNames.IsAnimePageNavColumnManualHide, false);
    }

    [RelayCommand]
    private void Initialize()
    {
        if (Sections?.Count > 0)
        {
            return;
        }

        var list = new List<IAnimeSectionDetailViewModel>
        {
            new AnimeTimelineViewModel(_service),
            new AnimeSectionDetailViewModel(AnimeSectionType.Bangumi, _service),
            new AnimeSectionDetailViewModel(AnimeSectionType.Domestic, _service),
        };

        Sections = list.AsReadOnly();
        var lastType = SettingsToolkit.ReadLocalSetting(SettingNames.AnimePageLastSelectedSectionType, AnimeSectionType.Timeline);
        var section = Sections.FirstOrDefault(p => p.SectionType == lastType);
        SelectSection(section ?? Sections.First());
        SectionInitialized?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void SelectSection(IAnimeSectionDetailViewModel section)
    {
        if (SelectedSection?.Equals(section) == true)
        {
            return;
        }

        SelectedSection = section;
        SettingsToolkit.WriteLocalSetting(SettingNames.AnimePageLastSelectedSectionType, section.SectionType);
        section.InitializeCommand.Execute(default);
    }

    partial void OnNavColumnWidthChanged(double value)
    {
        if (value > 0)
        {
            SettingsToolkit.WriteLocalSetting(SettingNames.AnimePageNavColumnWidth, value);
        }
    }

    partial void OnIsNavColumnManualHideChanged(bool value)
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.IsAnimePageNavColumnManualHide, value);
        NavColumnWidth = value ? 0 : SettingsToolkit.ReadLocalSetting(SettingNames.AnimePageNavColumnWidth, 240d);
    }
}
