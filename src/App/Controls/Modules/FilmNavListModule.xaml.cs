// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Appearance;
using Bili.Copilot.ViewModels;
using Bili.Copilot.ViewModels.Items;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 影视导航列表模块.
/// </summary>
public sealed partial class FilmNavListModule : FilmNavListModuleBase
{
    private readonly MovieRecommendDetailViewModel _movieRecommendDetailViewModel;
    private readonly TvRecommendDetailViewModel _tvRecommendDetailViewModel;
    private readonly DocumentaryRecommendDetailViewModel _documentaryRecommendDetailViewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilmNavListModule"/> class.
    /// </summary>
    public FilmNavListModule()
    {
        InitializeComponent();
        ViewModel = FilmPageViewModel.Instance;
        _movieRecommendDetailViewModel = MovieRecommendDetailViewModel.Instance;
        _tvRecommendDetailViewModel = TvRecommendDetailViewModel.Instance;
        _documentaryRecommendDetailViewModel = DocumentaryRecommendDetailViewModel.Instance;

        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
        => FilmTypeSelection.SelectedIndex = (int)ViewModel.CurrentType;

    private void OnFilmTypeSegmentedSelectionChanged(object sender, SelectionChangedEventArgs e)
        => ViewModel.CurrentType = (FilmType)FilmTypeSelection.SelectedIndex;

    private void OnDocumentaryConditionChangedAsync(object sender, SelectionChangedEventArgs e)
    {
        var comboBox = sender as ComboBox;
        if (comboBox.DataContext is IndexFilterItemViewModel source
            && comboBox.SelectedItem is Condition item)
        {
            var index = source.Data.Conditions.ToList().IndexOf(item);
            if (index >= 0 && index != source.SelectedIndex)
            {
                source.SelectedIndex = index;
                _ = _documentaryRecommendDetailViewModel.ReloadCommand.ExecuteAsync(null);
            }
        }
    }

    private void OnTvConditionChangedAsync(object sender, SelectionChangedEventArgs e)
    {
        var comboBox = sender as ComboBox;
        if (comboBox.DataContext is IndexFilterItemViewModel source
            && comboBox.SelectedItem is Condition item)
        {
            var index = source.Data.Conditions.ToList().IndexOf(item);
            if (index >= 0 && index != source.SelectedIndex)
            {
                source.SelectedIndex = index;
                _ = _tvRecommendDetailViewModel.ReloadCommand.ExecuteAsync(null);
            }
        }
    }

    private void OnMovieConditionChangedAsync(object sender, SelectionChangedEventArgs e)
    {
        var comboBox = sender as ComboBox;
        if (comboBox.DataContext is IndexFilterItemViewModel source
            && comboBox.SelectedItem is Condition item)
        {
            var index = source.Data.Conditions.ToList().IndexOf(item);
            if (index >= 0 && index != source.SelectedIndex)
            {
                source.SelectedIndex = index;
                _ = _movieRecommendDetailViewModel.ReloadCommand.ExecuteAsync(null);
            }
        }
    }

    private void OnFavoriteButtonClick(object sender, RoutedEventArgs e)
        => AppViewModel.Instance.ShowFavoritesCommand.Execute(FavoriteType.Film);
}

/// <summary>
/// <see cref="FilmNavListModule"/> 的基类.
/// </summary>
public abstract class FilmNavListModuleBase : ReactiveUserControl<FilmPageViewModel>
{
}
