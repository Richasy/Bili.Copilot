// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.ViewModels.Views;

namespace Bili.Copilot.App.Pages;

/// <summary>
/// 收藏夹页面.
/// </summary>
public sealed partial class FavoritesPage : FavoritesPageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FavoritesPage"/> class.
    /// </summary>
    public FavoritesPage()
    {
        InitializeComponent();
        ViewModel = FavoritesPageViewModel.Instance;
    }

    /// <inheritdoc/>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is FavoriteType type)
        {
            ViewModel.Type = type;
        }
    }

    /// <inheritdoc/>
    protected override void OnPageLoaded()
        => ViewModel.Video.InitializeCommand.Execute(default);
}

/// <summary>
/// <see cref="FavoritesPage"/> 的基类.
/// </summary>
public abstract class FavoritesPageBase : PageBase<FavoritesPageViewModel>
{
}
