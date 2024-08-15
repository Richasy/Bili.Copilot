// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 视频播放器页面视图模型.
/// </summary>
public sealed partial class VideoPlayerPageViewModel
{
    [RelayCommand]
    private async Task ToggleFollowAsync()
    {
        try
        {
            if (IsFollow)
            {
                await _relationshipService.UnfollowUserAsync(_view.Information.Publisher.User.Id);
                IsFollow = false;
            }
            else
            {
                await _relationshipService.FollowUserAsync(_view.Information.Publisher.User.Id);
                IsFollow = true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试关注/取消关注 UP 主时失败.");
        }
    }

    [RelayCommand]
    private async Task ToggleLikeAsync()
    {
        try
        {
            var state = !IsLiked;
            await _service.ToggleVideoLikeAsync(_view.Information.Identifier.Id, state);
            IsLiked = state;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试点赞/取消点赞视频时失败.");
        }
    }

    [RelayCommand]
    private async Task CoinAsync(int count = 1)
    {
        try
        {
            await _service.CoinVideoAsync(_view.Information.Identifier.Id, count, IsCoinAlsoLike);
            IsCoined = true;
            if (IsCoinAlsoLike && !IsLiked)
            {
                IsLiked = true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试投币/取消投币视频时失败.");
        }
    }

    [RelayCommand]
    private async Task TripleAsync()
    {
        try
        {
            await _service.TripleVideoAsync(_view.Information.Identifier.Id);
            IsLiked = true;
            IsCoined = true;
            IsFavorited = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试一键三连视频时失败.");
        }
    }

    [RelayCommand]
    private async Task InitializeFavoritesAsync()
    {
        if (FavoriteFolders is not null)
        {
            return;
        }

        try
        {
            var (folders, ids) = await _favoriteService.GetPlayingVideoFavoriteFoldersAsync(_view.Information);
            FavoriteFolders = folders.Select(p => new PlayerFavoriteFolderViewModel(p, ids.Contains(p.Id))).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试获取收藏夹列表时失败.");
        }
    }

    [RelayCommand]
    private async Task FavoriteAsync()
    {
        if (FavoriteFolders is null)
        {
            return;
        }

        var selectedFolders = FavoriteFolders?.Where(p => p.IsSelected).Select(p => p.Data.Id).ToList();
        var unselectedFolders = FavoriteFolders?.Where(p => !p.IsSelected).Select(p => p.Data.Id).ToList();
        try
        {
            await _service.FavoriteVideoAsync(_view.Information.Identifier.Id, selectedFolders, unselectedFolders);
            IsFavorited = selectedFolders.Any();
            FavoriteFolders = default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试收藏视频时失败.");
        }
    }

    [RelayCommand]
    private void CopyVideoUrl()
    {
        var url = $"https://www.bilibili.com/video/av{_view.Information.Identifier.Id}";
        var dp = new DataPackage();
        dp.SetText(url);
        dp.SetWebLink(new Uri(url));
        Clipboard.SetContent(dp);
    }

    [RelayCommand]
    private async Task OpenInBroswerAsync()
    {
        var url = $"https://www.bilibili.com/video/av{_view.Information.Identifier.Id}";
        await Launcher.LaunchUriAsync(new Uri(url)).AsTask();
    }
}
