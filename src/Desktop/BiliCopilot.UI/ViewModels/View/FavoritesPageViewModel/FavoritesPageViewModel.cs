// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Models;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 收藏页面视图模型.
/// </summary>
public sealed partial class FavoritesPageViewModel : LayoutPageViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FavoritesPageViewModel"/> class.
    /// </summary>
    public FavoritesPageViewModel(
        IFavoriteService service,
        ILogger<FavoritesPageViewModel> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <inheritdoc/>
    protected override string GetPageKey() => nameof(FavoritesPage);

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (Sections.Count > 0)
        {
            RestoreSelection();
            return;
        }

        IsLoading = true;
        Sections.Add(new PgcFavoriteSectionDetailViewModel(Richasy.BiliKernel.Models.PgcFavoriteType.Anime, _service));
        Sections.Add(new PgcFavoriteSectionDetailViewModel(Richasy.BiliKernel.Models.PgcFavoriteType.Cinema, _service));
        IsLoading = false;
        RestoreSelection();
        SectionInitialized?.Invoke(this, EventArgs.Empty);
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        Sections.Clear();
        await InitializeAsync();
    }

    [RelayCommand]
    private void SelectSection(IFavoriteSectionDetailViewModel vm)
    {
        if (vm is null || vm == CurrentSection)
        {
            return;
        }

        var sectionSettingValue = string.Empty;
        if (vm is PgcFavoriteSectionDetailViewModel pgcVM)
        {
            sectionSettingValue = $"pgc_{pgcVM.Type}";
        }

        SettingsToolkit.WriteLocalSetting(Models.Constants.SettingNames.LastSelectedFavoriteSection, sectionSettingValue);
        CurrentSection = vm;
        CurrentSection.TryFirstLoadCommand.Execute(default);
    }

    private void RestoreSelection()
    {
        var lastSelectedSection = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.LastSelectedFavoriteSection, string.Empty);
        if (string.IsNullOrEmpty(lastSelectedSection))
        {
            SelectSection(Sections.First());
            return;
        }

        var isSelected = false;
        if (lastSelectedSection.StartsWith("pgc"))
        {
            var isSuccess = Enum.TryParse<PgcFavoriteType>(lastSelectedSection.Replace("pgc_", string.Empty), out var type);
            if (isSuccess)
            {
                var section = Sections.OfType<PgcFavoriteSectionDetailViewModel>().FirstOrDefault(p => p.Type == type);
                if (section is not null)
                {
                    isSelected = true;
                    SelectSection(section);
                }
            }
        }

        if (!isSelected && CurrentSection is null)
        {
            SelectSection(Sections.First());
        }
    }
}
