// Copyright (c) Bili Copilot. All rights reserved.

using System.Threading.Tasks;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 影视圈页面视图模型.
/// </summary>
public sealed partial class FilmPageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FilmPageViewModel"/> class.
    /// </summary>
    private FilmPageViewModel()
    {
        CurrentType = SettingsToolkit.ReadLocalSetting(SettingNames.LastFilmType, FilmType.Movie);
        CheckModuleStateAsync();

        AttachIsRunningToAsyncCommand(p => IsReloading = p, ReloadCommand, InitializeCommand);
    }

    [RelayCommand]
    private async Task ReloadAsync()
    {
        if (IsMovieShown)
        {
            await MovieRecommendDetailViewModel.Instance.ReloadCommand.ExecuteAsync(default);
        }
        else if (IsTvShown)
        {
            await TvRecommendDetailViewModel.Instance.ReloadCommand.ExecuteAsync(default);
        }
        else if (IsDocumentaryShown)
        {
            await DocumentaryRecommendDetailViewModel.Instance.ReloadCommand.ExecuteAsync(default);
        }
        else if (IsFavoriteShown)
        {
            await FilmFavoriteDetailViewModel.Instance.ReloadCommand.ExecuteAsync(default);
        }
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        await InitializeCurrentModuleAsync();
        _isInitialized = true;
    }

    private async Task InitializeCurrentModuleAsync()
    {
        if (IsMovieShown)
        {
            await MovieRecommendDetailViewModel.Instance.InitializeCommand.ExecuteAsync(default);
        }
        else if (IsTvShown)
        {
            await TvRecommendDetailViewModel.Instance.InitializeCommand.ExecuteAsync(default);
        }
        else if (IsDocumentaryShown)
        {
            await DocumentaryRecommendDetailViewModel.Instance.InitializeCommand.ExecuteAsync(default);
        }
        else if (IsFavoriteShown)
        {
            await FilmFavoriteDetailViewModel.Instance.InitializeCommand.ExecuteAsync(default);
        }
    }

    private async void CheckModuleStateAsync()
    {
        IsMovieShown = CurrentType == FilmType.Movie;
        IsTvShown = CurrentType == FilmType.Tv;
        IsDocumentaryShown = CurrentType == FilmType.Documentary;
        IsFavoriteShown = CurrentType == FilmType.Favorite;

        Title = CurrentType switch
        {
            FilmType.Movie => ResourceToolkit.GetLocalizedString(StringNames.Movie),
            FilmType.Tv => ResourceToolkit.GetLocalizedString(StringNames.TV),
            FilmType.Documentary => ResourceToolkit.GetLocalizedString(StringNames.Documentary),
            FilmType.Favorite => ResourceToolkit.GetLocalizedString(StringNames.MyFavoriteFilm),
            _ => string.Empty,
        };

        await InitializeCurrentModuleAsync();
    }

    partial void OnCurrentTypeChanged(FilmType value)
    {
        CheckModuleStateAsync();
        SettingsToolkit.WriteLocalSetting(SettingNames.LastFilmType, value);
    }
}
