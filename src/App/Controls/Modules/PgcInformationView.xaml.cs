// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// PGC 信息视图.
/// </summary>
public sealed partial class PgcInformationView : PgcInformationViewBase
{
    private bool _isLikeHoldCompleted;
    private bool _isLikeHoldSuspend;

    /// <summary>
    /// Initializes a new instance of the <see cref="PgcInformationView"/> class.
    /// </summary>
    public PgcInformationView() => InitializeComponent();

    private void OnGiveCoinButtonClickAsync(object sender, RoutedEventArgs e)
    {
        var num = int.Parse((sender as FrameworkElement).Tag.ToString());
        ViewModel.CoinCommand.Execute(num);
    }

    private void OnLikeButtonHoldingCompleted(object sender, System.EventArgs e)
    {
        _isLikeHoldCompleted = true;
        ViewModel.TripleCommand.ExecuteAsync(null);
        CoinButton.ShowBubbles();
        FavoriteButton.ShowBubbles();
    }

    private void OnLikeButtonHoldingSuspend(object sender, EventArgs e)
    {
        _isLikeHoldSuspend = true;
    }

    private void OnLikeButtonClick(object sender, RoutedEventArgs e)
    {
        if (_isLikeHoldCompleted || _isLikeHoldSuspend)
        {
            _isLikeHoldCompleted = false;
            _isLikeHoldSuspend = false;
            return;
        }

        ViewModel.LikeCommand.ExecuteAsync(null);
    }

    private void OnCoinButtonClick(object sender, RoutedEventArgs e)
    {
        ViewModel.IsCoined = !ViewModel.IsCoined;
        ViewModel.IsCoined = !ViewModel.IsCoined;

        if (!ViewModel.IsCoined)
        {
            CoinFlyout.ShowAt(CoinButton);
        }
    }

    private void OnFavoriteButtonClickAsync(object sender, RoutedEventArgs e)
    {
        ViewModel.IsFavorited = !ViewModel.IsFavorited;
        ViewModel.IsFavorited = !ViewModel.IsFavorited;

        if (ViewModel.FavoriteFolders.Count == 0)
        {
            ViewModel.RequestFavoriteFoldersCommand.ExecuteAsync(null);
        }

        FavoriteFlyout.ShowAt(FavoriteButton);
    }
}

/// <summary>
/// <see cref="PgcInformationView"/> 的基类.
/// </summary>
public abstract class PgcInformationViewBase : ReactiveUserControl<PgcPlayerPageViewModel>
{
}
