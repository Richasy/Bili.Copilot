// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Models.Data.Video;
using Bili.Copilot.ViewModels;
using Bili.Copilot.ViewModels.Views;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 收藏夹导航列表模块.
/// </summary>
public sealed partial class FavoritesNavListModule : FavoritesNavListModuleBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FavoritesNavListModule"/> class.
    /// </summary>
    public FavoritesNavListModule()
    {
        InitializeComponent();
        ViewModel = FavoritesPageViewModel.Instance;
    }

    private void OnVideoFolderClick(object sender, RoutedEventArgs e)
    {
        var vm = (sender as FrameworkElement).DataContext as VideoFavoriteFolderSelectableViewModel;
        ViewModel.Type = Models.Constants.App.FavoriteType.Video;
        ViewModel.Video.SelectFolderCommand.Execute(vm);
    }

    private void OnAnimeItemClick(object sender, RoutedEventArgs e)
    {
        ViewModel.Type = Models.Constants.App.FavoriteType.Anime;
        ViewModel.Video.SelectFolderCommand.Execute(default);
    }

    private void OnFilmItemClick(object sender, RoutedEventArgs e)
    {
        ViewModel.Type = Models.Constants.App.FavoriteType.Film;
        ViewModel.Video.SelectFolderCommand.Execute(default);
    }

    private void OnUgcSeasonClick(object sender, RoutedEventArgs e)
    {
        var data = (sender as FrameworkElement).DataContext as VideoFavoriteFolder;
        ViewModel.Video.PlaySeasonCommand.Execute(data);
    }
}

/// <summary>
/// <see cref="FavoritesNavListModule"/> 的基类.
/// </summary>
public abstract class FavoritesNavListModuleBase : ReactiveUserControl<FavoritesPageViewModel>
{
}
