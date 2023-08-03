// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Community;
using Bili.Copilot.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 视频信息视图.
/// </summary>
public sealed partial class VideoInformationView : VideoInformationViewBase
{
    private bool _isLikeHoldCompleted;
    private bool _isLikeHoldSuspend;

    /// <summary>
    /// Initializes a new instance of the <see cref="VideoInformationView"/> class.
    /// </summary>
    public VideoInformationView() => InitializeComponent();

    private void OnGiveCoinButtonClick(object sender, RoutedEventArgs e)
    {
        var num = int.Parse((sender as FrameworkElement).Tag.ToString());
        _ = ViewModel.CoinCommand.ExecuteAsync(num);
        CoinFlyout.Hide();
    }

    private void OnTagButtonClick(object sender, RoutedEventArgs e)
    {
        var data = (sender as FrameworkElement).DataContext as Tag;
        ViewModel.SearchTagCommand.Execute(data);
    }

    private void OnLikeButtonHoldingCompleted(object sender, EventArgs e)
    {
        _isLikeHoldCompleted = true;
        _ = ViewModel.TripleCommand.ExecuteAsync(null);
        CoinButton.ShowBubbles();
        FavoriteButton.ShowBubbles();
    }

    private void OnLikeButtonHoldingSuspend(object sender, EventArgs e)
        => _isLikeHoldSuspend = true;

    private void OnLikeButtonClick(object sender, RoutedEventArgs e)
    {
        if (_isLikeHoldCompleted || _isLikeHoldSuspend)
        {
            _isLikeHoldCompleted = false;
            _isLikeHoldSuspend = false;
            return;
        }

        _ = ViewModel.LikeCommand.ExecuteAsync(null);
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

    private void OnFavoriteButtonClick(object sender, RoutedEventArgs e)
    {
        ViewModel.IsFavorited = !ViewModel.IsFavorited;
        ViewModel.IsFavorited = !ViewModel.IsFavorited;

        if (ViewModel.FavoriteFolders.Count == 0)
        {
            _ = ViewModel.RequestFavoriteFoldersCommand.ExecuteAsync(null);
        }

        FavoriteFlyout.ShowAt(FavoriteButton);
    }

    private void OnOnlyAudioToggledAsync(object sender, RoutedEventArgs e)
    {
        var control = sender as ToggleSwitch;
        var isAudioOnly = control.IsOn;
        if (ViewModel.PlayerDetail.IsAudioOnly != isAudioOnly)
        {
            ViewModel.PlayerDetail.ChangeAudioOnlyCommand.Execute(isAudioOnly);
        }
    }
}

/// <summary>
/// <see cref="VideoInformationView"/> 的基类.
/// </summary>
public abstract class VideoInformationViewBase : ReactiveUserControl<VideoPlayerPageViewModel>
{
}
