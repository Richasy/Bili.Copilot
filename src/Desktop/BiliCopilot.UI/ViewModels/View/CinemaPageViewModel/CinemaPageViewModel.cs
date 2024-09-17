// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Pages;
using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Core;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Models;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 影院页面视图模型.
/// </summary>
public sealed partial class CinemaPageViewModel : LayoutPageViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CinemaPageViewModel"/> class.
    /// </summary>
    public CinemaPageViewModel(
        IEntertainmentDiscoveryService discoveryService,
        ILogger<CinemaPageViewModel> logger)
    {
        _service = discoveryService;
        _logger = logger;
    }

    /// <inheritdoc/>
    protected override string GetPageKey() => nameof(CinemaPage);

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (Sections?.Count > 0)
        {
            return;
        }

        var list = new List<EntertainmentIndexViewModel>
        {
            new EntertainmentIndexViewModel(PgcSectionType.Movie, _service),
            new EntertainmentIndexViewModel(PgcSectionType.TV, _service),
            new EntertainmentIndexViewModel(PgcSectionType.Documentary, _service),
        };

        Sections = list.AsReadOnly();
        var lastType = SettingsToolkit.ReadLocalSetting(SettingNames.CinemaPageLastSelectedSectionType, PgcSectionType.Movie);
        var section = Sections.FirstOrDefault(p => p.SectionType == lastType);
        await Task.Delay(500);
        SelectSection(section ?? Sections.First());
        SectionInitialized?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void SelectSection(EntertainmentIndexViewModel section)
    {
        if (SelectedSection?.Equals(section) == true)
        {
            return;
        }

        SelectedSection = section;
        SettingsToolkit.WriteLocalSetting(SettingNames.CinemaPageLastSelectedSectionType, section.SectionType);
        section.InitializeCommand.Execute(default);
    }

    [RelayCommand]
    private void ShowMyFavoriteCinema()
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.LastSelectedFavoriteSection, $"pgc_{PgcFavoriteType.Cinema}");
        this.Get<NavigationViewModel>().NavigateToOver(typeof(FavoritesPage));
    }
}
