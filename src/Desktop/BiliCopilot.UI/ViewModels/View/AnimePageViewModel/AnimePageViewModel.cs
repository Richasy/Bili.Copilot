// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Pages;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 动漫页面视图模型.
/// </summary>
public sealed partial class AnimePageViewModel : LayoutPageViewModelBase
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
    }

    /// <inheritdoc/>
    protected override string GetPageKey() => nameof(AnimePage);

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (Sections?.Count > 0)
        {
            return;
        }

        var list = new List<IPgcSectionDetailViewModel>
        {
            new AnimeTimelineViewModel(_service),
            new EntertainmentIndexViewModel(PgcSectionType.Anime, _service),
        };

        Sections = list.AsReadOnly();
        var lastType = SettingsToolkit.ReadLocalSetting(SettingNames.AnimePageLastSelectedSectionType, PgcSectionType.Timeline);
        var section = Sections.FirstOrDefault(p => p.SectionType == lastType);
        await Task.Delay(500);
        SelectSection(section ?? Sections.First());
        SectionInitialized?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void SelectSection(IPgcSectionDetailViewModel section)
    {
        if (SelectedSection?.Equals(section) == true)
        {
            return;
        }

        SelectedSection = section;
        SettingsToolkit.WriteLocalSetting(SettingNames.AnimePageLastSelectedSectionType, section.SectionType);
        section.InitializeCommand.Execute(default);
    }
}
