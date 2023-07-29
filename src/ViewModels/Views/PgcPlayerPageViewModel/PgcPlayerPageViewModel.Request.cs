// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// PGC 播放页面视图模型.
/// </summary>
public sealed partial class PgcPlayerPageViewModel
{
    [RelayCommand]
    private async Task ReloadCommunityInformationAsync()
    {
        if (CurrentEpisode != null)
        {
            var data = await PlayerProvider.GetEpisodeInteractionInformationAsync(CurrentEpisode.Identifier.Id);
            IsLiked = data.IsLiked;
            IsFavorited = data.IsFavorited;
            IsCoined = data.IsCoined;
        }
    }

    [RelayCommand]
    private async Task RequestFavoriteFoldersAsync()
    {
        IsFavoriteFoldersError = false;
        FavoriteFoldersErrorText = default;
        if (!IsSignedIn)
        {
            return;
        }

        TryClear(FavoriteFolders);
        var data = await FavoriteProvider.GetCurrentPlayerFavoriteListAsync(AuthorizeProvider.Instance.CurrentUserId, CurrentEpisode.VideoId);
        var selectIds = data.Item2;
        foreach (var item in data.Item1.Items)
        {
            var isSelected = selectIds != null && selectIds.Contains(item.Id);
            var vm = new VideoFavoriteFolderSelectableViewModel(item)
            {
                IsSelected = isSelected,
            };
            FavoriteFolders.Add(vm);
        }
    }

    [RelayCommand]
    private async Task FavoriteEpisodeAsync()
    {
        var selectedFolders = FavoriteFolders.Where(p => p.IsSelected).Select(p => p.Data.Id).ToList();
        var deselectedFolders = FavoriteFolders.Where(p => !p.IsSelected).Select(p => p.Data.Id).ToList();
        var result = await PlayerProvider.FavoriteAsync(CurrentEpisode.Identifier.Id, selectedFolders, deselectedFolders, false);
        if (result is FavoriteResult.Success or FavoriteResult.InsufficientAccess)
        {
            IsFavorited = selectedFolders.Count > 0;
            _ = ReloadCommunityInformationCommand.ExecuteAsync(null);
        }
        else
        {
            AppViewModel.Instance.ShowTip(ResourceToolkit.GetLocalizedString(StringNames.FavoriteFailed), InfoType.Error);
        }
    }

    [RelayCommand]
    private async Task CoinAsync(int coinNumber)
    {
        var isSuccess = await PlayerProvider.CoinAsync(CurrentEpisode.VideoId, coinNumber, IsCoinWithLiked);
        if (isSuccess)
        {
            IsCoined = true;
            if (IsCoinWithLiked)
            {
                IsLiked = true;
            }

            _ = ReloadCommunityInformationCommand.ExecuteAsync(null);
        }
        else
        {
            AppViewModel.Instance.ShowTip(ResourceToolkit.GetLocalizedString(StringNames.CoinFailed), InfoType.Error);
        }
    }

    [RelayCommand]
    private async Task LikeAsync()
    {
        var isLike = !IsLiked;
        var isSuccess = await PlayerProvider.LikeAsync(CurrentEpisode.VideoId, isLike);
        if (isSuccess)
        {
            IsLiked = isLike;
            _ = ReloadCommunityInformationCommand.ExecuteAsync(null);
        }
        else
        {
            AppViewModel.Instance.ShowTip(ResourceToolkit.GetLocalizedString(StringNames.SetFailed), InfoType.Error);
        }
    }

    [RelayCommand]
    private async Task TripleAsync()
    {
        var info = await PlayerProvider.TripleAsync(CurrentEpisode.VideoId);
        IsLiked = info.IsLiked;
        IsFavorited = info.IsFavorited;
        IsCoined = info.IsCoined;
        _ = ReloadCommunityInformationCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task TrackSeasonAsync()
    {
        var isTrack = !IsTracking;
        var isSuccess = await PgcProvider.FollowAsync(View.Information.Identifier.Id, isTrack);
        if (isSuccess)
        {
            IsTracking = isTrack;
        }
        else
        {
            AppViewModel.Instance.ShowTip(ResourceToolkit.GetLocalizedString(StringNames.SetFailed), InfoType.Error);
        }
    }
}
