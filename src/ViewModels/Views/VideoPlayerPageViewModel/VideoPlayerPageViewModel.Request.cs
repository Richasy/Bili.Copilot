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
/// 视频播放页面视图模型.
/// </summary>
public sealed partial class VideoPlayerPageViewModel
{
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
        var data = await FavoriteProvider.GetCurrentPlayerFavoriteListAsync(AuthorizeProvider.Instance.CurrentUserId, View.Information.Identifier.Id);
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
    private async Task RequestOnlineCountAsync()
    {
        var text = await PlayerProvider.GetOnlineViewerCountAsync(View.Information.Identifier.Id, CurrentVideoPart.Id);
        _dispatcherQueue.TryEnqueue(() =>
        {
            WatchingCountText = text;
        });
    }

    [RelayCommand]
    private async Task FavoriteVideoAsync()
    {
        var selectedFolders = FavoriteFolders.Where(p => p.IsSelected).Select(p => p.Data.Id).ToList();
        var deselectedFolders = FavoriteFolders.Where(p => !p.IsSelected).Select(p => p.Data.Id).ToList();
        var result = await PlayerProvider.FavoriteAsync(View.Information.Identifier.Id, selectedFolders, deselectedFolders, true);
        if (result == FavoriteResult.Success || result == FavoriteResult.InsufficientAccess)
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
        var isSuccess = await PlayerProvider.CoinAsync(View.Information.Identifier.Id, coinNumber, IsCoinWithLiked);
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
        var isSuccess = await PlayerProvider.LikeAsync(View.Information.Identifier.Id, isLike);
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
        var info = await PlayerProvider.TripleAsync(View.Information.Identifier.Id);
        IsLiked = info.IsLiked;
        IsFavorited = info.IsFavorited;
        IsCoined = info.IsCoined;
        _ = ReloadCommunityInformationCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task ReloadCommunityInformationAsync()
    {
        var data = await PlayerProvider.GetVideoCommunityInformationAsync(View.Information.Identifier.Id);
        View.Information.CommunityInformation = data;
        InitializeCommunityInformation();
    }
}
