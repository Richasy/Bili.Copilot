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
            SectionInitialized?.Invoke(this, EventArgs.Empty);
            return;
        }

        IsLoading = true;
        Sections.Add(new PgcFavoriteSectionDetailViewModel(PgcFavoriteType.Anime, _service));
        Sections.Add(new PgcFavoriteSectionDetailViewModel(PgcFavoriteType.Cinema, _service));
        await InitializeVideoFoldersAsync();
        IsLoading = false;
        RestoreSelection();
        SectionInitialized?.Invoke(this, EventArgs.Empty);
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
        else if (vm is VideoFavoriteSectionDetailViewModel videoVM)
        {
            sectionSettingValue = $"video_{videoVM.Data.Id}";
        }
        else if (vm is UgcSeasonFavoriteSectionDetailViewModel ugcVM)
        {
            sectionSettingValue = $"ugc_{ugcVM.Data.Id}";
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
        else if (lastSelectedSection.StartsWith("video"))
        {
            var id = lastSelectedSection.Replace("video_", string.Empty);
            var section = Sections.OfType<VideoFavoriteSectionDetailViewModel>().FirstOrDefault(p => p.Data.Id == id);
            if (section is not null)
            {
                isSelected = true;
                SelectSection(section);
            }
        }
        else if (lastSelectedSection.StartsWith("ugc"))
        {
            var id = lastSelectedSection.Replace("ugc_", string.Empty);
            var section = Sections.OfType<UgcSeasonFavoriteSectionDetailViewModel>().FirstOrDefault(p => p.Data.Id == id);
            if (section is not null)
            {
                isSelected = true;
                SelectSection(section);
            }
        }

        if (!isSelected && CurrentSection is null)
        {
            SelectSection(Sections.First());
        }
    }

    private async Task InitializeVideoFoldersAsync()
    {
        try
        {
            var (groups, defaultGroup) = await _service.GetVideoFavoriteGroupsAsync();
            var defaultGroupVM = new VideoFavoriteSectionDetailViewModel(defaultGroup.Folder, _service);
            defaultGroupVM.InjectFirstPageData(defaultGroup);
            Sections.Add(defaultGroupVM);

            foreach (var group in groups)
            {
                foreach (var folder in group.Folders)
                {
                    var folderVM = folder.IsUgcSeason == true
                        ? (IFavoriteSectionDetailViewModel)new UgcSeasonFavoriteSectionDetailViewModel(folder, _service)
                        : new VideoFavoriteSectionDetailViewModel(folder, _service);
                    Sections.Add(folderVM);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "初始化视频收藏夹失败");
        }
    }
}
