// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// PGC收藏详情模块.
/// </summary>
public sealed partial class PgcFavoriteDetailModule : PgcFavoriteDetailModuleBase
{
    /// <summary>
    /// <see cref="FavoriteType"/> 的依赖属性.
    /// </summary>
    public static readonly DependencyProperty FavoriteTypeProperty =
        DependencyProperty.Register(nameof(FavoriteType), typeof(FavoriteType), typeof(PgcFavoriteDetailModule), new PropertyMetadata(default));

    /// <summary>
    /// Initializes a new instance of the <see cref="PgcFavoriteDetailModule"/> class.
    /// </summary>
    public PgcFavoriteDetailModule()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    /// <summary>
    /// 收藏夹类型.
    /// </summary>
    public FavoriteType FavoriteType
    {
        get => (FavoriteType)GetValue(FavoriteTypeProperty);
        set => SetValue(FavoriteTypeProperty, value);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        TitleBlock.Text = FavoriteType switch
        {
            FavoriteType.Anime => ResourceToolkit.GetLocalizedString(StringNames.MyFavoriteAnime),
            FavoriteType.Film => ResourceToolkit.GetLocalizedString(StringNames.MyFavoriteFilm),
            _ => throw new ArgumentOutOfRangeException(nameof(FavoriteType), FavoriteType, "Do not support")
        };

        ViewModel = FavoriteType switch
        {
            FavoriteType.Anime => AnimeFavoriteDetailViewModel.Instance,
            FavoriteType.Film => FilmFavoriteDetailViewModel.Instance,
            _ => throw new ArgumentOutOfRangeException(nameof(FavoriteType), FavoriteType, "Do not support")
        };
    }

    private void OnStatusSelectionChanged(object sender, Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e)
    {
        if (!ViewModel.IsInitialized)
        {
            return;
        }

        ViewModel.SetStatusCommand.Execute(StatusComboBox.SelectedIndex);
    }

    private void OnSeasonViewIncrementalTriggered(object sender, EventArgs e)
        => ViewModel.IncrementalCommand.Execute(default);

    private void OnItemFlyoutOpened(object sender, object e)
    {
        var items = (sender as MenuFlyout).Items.OfType<MenuFlyoutItem>().Take(3);
        foreach (var item in items)
        {
            if (item is MenuFlyoutItem btn)
            {
                var status = int.Parse(btn.Tag.ToString()) - 1;
                item.IsEnabled = status != ViewModel.Status;
            }
        }
    }

    private void OnMarkStatusButtonClick(object sender, RoutedEventArgs e)
    {
        var item = sender as MenuFlyoutItem;
        var context = item.DataContext as SeasonItemViewModel;
        var status = int.Parse(item.Tag.ToString());
        context.ChangeFavoriteStatusCommand.Execute(status);
    }
}

/// <summary>
/// <see cref="PgcFavoriteDetailModule"/> 的基类.
/// </summary>
public abstract class PgcFavoriteDetailModuleBase : ReactiveUserControl<PgcFavoriteDetailViewModel>
{
}
