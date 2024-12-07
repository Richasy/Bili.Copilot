// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Pages;
using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Models;

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
    private void Initialize()
    {
        if (Sections?.Count > 0)
        {
            return;
        }

        Sections = new List<IPgcSectionDetailViewModel>
        {
            new AnimeTimelineViewModel(_service),
            new EntertainmentIndexViewModel(PgcSectionType.Anime, _service),
        };

        var lastType = SettingsToolkit.ReadLocalSetting(SettingNames.AnimePageLastSelectedSectionType, PgcSectionType.Timeline);
        var section = Sections.FirstOrDefault(p => p.SectionType == lastType);
        this.Get<DispatcherQueue>().TryEnqueue(DispatcherQueuePriority.Low, () =>
        {
            SelectSection(section ?? Sections.First());
            SectionInitialized?.Invoke(this, EventArgs.Empty);
        });
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

    [RelayCommand]
    private void ShowMyFavoriteAnime()
    {
        SettingsToolkit.WriteLocalSetting(SettingNames.LastSelectedFavoriteSection, $"pgc_{PgcFavoriteType.Anime}");
        this.Get<NavigationViewModel>().NavigateToOver(typeof(FavoritesPage));
    }
}
