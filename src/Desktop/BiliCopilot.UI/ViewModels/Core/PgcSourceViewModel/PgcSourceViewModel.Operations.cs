// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUIKernel.Share.Base;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;
using Windows.System;
using WinUIEx;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// PGC 源视图模型.
/// </summary>
public sealed partial class PgcSourceViewModel
{
    [RelayCommand]
    private async Task ToggleLikeAsync()
    {
        try
        {
            var state = !IsLiked;
            var aid = _episode.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.Aid).ToString();
            await _service.ToggleVideoLikeAsync(aid, state);
            IsLiked = state;
            ReloadEpisodeOperationCommand.Execute(default);
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
            var aid = _episode.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.Aid).ToString();
            await _service.CoinVideoAsync(aid, count, IsCoinAlsoLike);
            IsCoined = true;
            if (IsCoinAlsoLike && !IsLiked)
            {
                IsLiked = true;
            }

            ReloadEpisodeOperationCommand.Execute(default);
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
            var aid = _episode.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.Aid).ToString();
            await _service.TripleVideoAsync(aid);
            IsLiked = true;
            IsCoined = true;
            IsFavorited = true;
            ReloadEpisodeOperationCommand.Execute(default);
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
            var aid = _episode.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.Aid).ToString();
            var (folders, ids) = await _favoriteService.GetPlayingVideoFavoriteFoldersAsync(aid);
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
            var aid = _episode.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.Aid).ToString();
            await _service.FavoriteVideoAsync(aid, selectedFolders, unselectedFolders);
            IsFavorited = selectedFolders.Count != 0;
            FavoriteFolders = default;
            ReloadEpisodeOperationCommand.Execute(default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试收藏视频时失败.");
        }
    }

    [RelayCommand]
    private void CopyEpisodeUrl()
    {
        var url = GetEpisodeUrl();
        var dp = new DataPackage();
        dp.SetText(url);
        dp.SetWebLink(new Uri(url));
        Clipboard.SetContent(dp);
        this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.Copied), InfoType.Success));
    }

    [RelayCommand]
    private void CopySeasonUrl()
    {
        var url = GetSeasonUrl();
        var dp = new DataPackage();
        dp.SetText(url);
        dp.SetWebLink(new Uri(url));
        Clipboard.SetContent(dp);
        this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.Copied), InfoType.Success));
    }

    [RelayCommand]
    private void ShareEpisodeUrl()
    {
        var handle = this.Get<AppViewModel>().ActivatedWindow.GetWindowHandle();
        var transferManager = DataTransferManagerInterop.GetForWindow(handle);
        transferManager.DataRequested += OnTransferDataRequested;
        DataTransferManagerInterop.ShowShareUIForWindow(handle);

        void OnTransferDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var url = GetEpisodeUrl();
            var dp = new DataPackage();
            dp.SetText(url);
            dp.SetWebLink(new Uri(url));
            dp.Properties.Title = EpisodeTitle;
            dp.Properties.Description = Description;
            dp.Properties.Thumbnail = RandomAccessStreamReference.CreateFromUri(_view.Information.Identifier.Cover.Uri);
            args.Request.Data = dp;
            sender.DataRequested -= OnTransferDataRequested;
        }
    }

    [RelayCommand]
    private async Task OpenInBrowserAsync()
    {
        var url = GetEpisodeUrl();
        await Launcher.LaunchUriAsync(new Uri(url)).AsTask();
    }

    [RelayCommand]
    private void PlayPrevEpisode()
    {
        var prevPart = FindPrevEpisode();
        if (prevPart is null)
        {
            return;
        }

        if (prevPart is EpisodeInformation part)
        {
            ChangeEpisode(part);
        }
        else if (prevPart is VideoInformation video)
        {
            this.Get<AppViewModel>().OpenVideo(new VideoSnapshot(video));
        }
    }

    [RelayCommand]
    private void PlayNextEpisode()
    {
        var nextPart = FindNextEpisode();
        if (nextPart is null)
        {
            return;
        }

        if (nextPart is EpisodeInformation part)
        {
            ChangeEpisode(part);
        }
        else if (nextPart is VideoInformation video)
        {
            this.Get<AppViewModel>().OpenVideo(new VideoSnapshot(video));
        }
    }

    [RelayCommand]
    private async Task ReloadEpisodeOperationAsync()
    {
        if (_episode is null)
        {
            return;
        }

        try
        {
            var operation = await _service.GetEpisodeOperationInformationAsync(_episode.Identifier.Id);
            IsLiked = operation.IsLiked;
            IsCoined = operation.IsCoined;
            IsFavorited = operation.IsFavorited;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试获取剧集操作信息时失败.");
        }
    }

    [RelayCommand]
    private async Task ToggleFollowAsync()
    {
        try
        {
            var state = !IsFollow;
            if (state)
            {
                await _discoveryService.FollowAsync(_view.Information.Identifier.Id);
            }
            else
            {
                await _discoveryService.UnfollowAsync(_view.Information.Identifier.Id);
            }

            IsFollow = state;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"尝试追番/追剧失败 ({!IsFollow})");
        }
    }

    private void ChangeEpisode(EpisodeInformation episode)
    {
        _initialProgress = 0;
        var isFirstSet = _episode is null;
        _episode = episode;
        if (isFirstSet)
        {
            LoadInitialProgress();
        }

        if (_injectedProgress is not null)
        {
            _initialProgress = _injectedProgress.Value;
            _injectedProgress = null;
        }

        EpisodeId = episode.Identifier.Id;
        EpisodeTitle = SeasonTitle + " - " + episode.Identifier.Title;
        var aid = episode.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.Aid).ToString();
        var cid = episode.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.Cid).ToString();
        CommentSection.Initialize(aid, Richasy.BiliKernel.Models.CommentTargetType.Video, Richasy.BiliKernel.Models.CommentSortType.Hot);
        Subtitle?.ResetData(aid, cid);
        ReloadEpisodeOperationCommand.Execute(default);
        InitializeDashMediaCommand.Execute(episode);
        Subtitle.InitializeCommand.Execute(default);
        InitializeEpisodeNavigation();
    }

    [RelayCommand]
    private void Pin()
    {
        var pinItem = new PinItem($"ss_{_view.Information.Identifier.Id}", _view.Information.Identifier.Title, _view.Information.Identifier.Cover.Uri.ToString(), PinContentType.Pgc);
        this.Get<PinnerViewModel>().AddItemCommand.Execute(pinItem);
    }

    private void PlayerProgressChanged(int progress, int duration)
    {
        if (progress < duration && progress > 1 && (Danmaku?.IsEmpty() ?? false))
        {
            var aid = _episode.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.Aid).ToString();
            var cid = _episode.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.Cid).ToString();
            Danmaku.ResetData(aid, cid);
            Danmaku.LoadDanmakusCommand.Execute(duration);
        }

        Danmaku?.UpdatePosition(progress);
        Subtitle?.UpdatePosition(progress);
    }

    private string GetSeasonUrl()
        => $"https://www.bilibili.com/bangumi/play/ss{_view.Information.Identifier.Id}";

    private string GetEpisodeUrl()
        => $"https://www.bilibili.com/bangumi/play/ep{_episode.Identifier.Id}";
}
