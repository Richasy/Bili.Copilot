// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels.Views;

/// <summary>
/// 收藏夹页面视图模型.
/// </summary>
public sealed partial class FavoritesPageViewModel : ViewModelBase
{
    private static readonly Lazy<FavoritesPageViewModel> _lazyInstance = new(() => new FavoritesPageViewModel());

    [ObservableProperty]
    private FavoriteType _type;

    [ObservableProperty]
    private bool _isAnimeShown;

    [ObservableProperty]
    private bool _isFilmShown;

    [ObservableProperty]
    private bool _isVideoShown;

    [ObservableProperty]
    private string _title;

    /// <summary>
    /// Initializes a new instance of the <see cref="FavoritesPageViewModel"/> class.
    /// </summary>
    private FavoritesPageViewModel()
    {
        Type = SettingsToolkit.ReadLocalSetting(SettingNames.LastFavoriteType, FavoriteType.Video);
        CheckType();
    }

    /// <summary>
    /// 实例.
    /// </summary>
    public static FavoritesPageViewModel Instance => _lazyInstance.Value;

    /// <summary>
    /// 视频收藏夹.
    /// </summary>
    public VideoFavoriteDetailViewModel Video { get; } = VideoFavoriteDetailViewModel.Instance;

    /// <summary>
    /// 动漫收藏夹.
    /// </summary>
    public PgcFavoriteDetailViewModel Anime { get; } = AnimeFavoriteDetailViewModel.Instance;

    /// <summary>
    /// 影视收藏夹.
    /// </summary>
    public PgcFavoriteDetailViewModel Film { get; } = FilmFavoriteDetailViewModel.Instance;

    private void CheckType()
    {
        Title = Type switch
        {
            FavoriteType.Anime => ResourceToolkit.GetLocalizedString(StringNames.MyFavoriteAnime),
            FavoriteType.Film => ResourceToolkit.GetLocalizedString(StringNames.MyFavoriteFilm),
            _ => ResourceToolkit.GetLocalizedString(StringNames.MyFavorite),
        };

        IsAnimeShown = Type == FavoriteType.Anime;
        IsFilmShown = Type == FavoriteType.Film;
        IsVideoShown = Type == FavoriteType.Video;

        if (IsAnimeShown && !Anime.IsInitialized)
        {
            Anime.InitializeCommand.Execute(default);
        }
        else if (IsFilmShown && !Film.IsInitialized)
        {
            Film.InitializeCommand.Execute(default);
        }
    }

    partial void OnTypeChanged(FavoriteType value)
    {
        CheckType();
        SettingsToolkit.WriteLocalSetting(SettingNames.LastFavoriteType, value);
    }
}
